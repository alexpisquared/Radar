using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Reflection;

namespace EventLogLib;

public static class EventLogHelper // 2022-09 excrept from C:\g\TimeTracking\N50\TimeTracking50\TimeTracker\AsLink\EvLogMngr.cs
{
  public static TimeSpan CurrentSessionDuration() // lengthy operation: > 100 ms.
  {
    var lastUp = DateTime.FromOADate(Math.Max(
        GetDaysLastBootTime(DateTime.Today).ToOADate(), Math.Max(
        GetDaysLastWakeBoot(DateTime.Today).ToOADate(),
        GetDaysLastSsDnTime(DateTime.Today).ToOADate())));

    return GetDaysLastSsUpTime(DateTime.Today) > lastUp ? TimeSpan.Zero : DateTime.Now - lastUp;
  }

  public static DateTime GetDaysLastSsDnTime(DateTime hr00ofTheDate)
  {
    var hr24ofTheDate = hr00ofTheDate.AddDays(1);
    var rv = hr00ofTheDate;
    try
    {
      using (var reader = new EventLogReader(new EventLogQuery(_aavLogName, PathType.LogName, qryScrSvr(_ssrDn, hr00ofTheDate, hr24ofTheDate))))
      {
        for (var er = (EventLogRecord?)reader.read(); null != er && er.TimeCreated.HasValue; er = (EventLogRecord?)reader.read())
          if (rv < er.TimeCreated.Value)
            rv = er.TimeCreated.Value;
      }
    }
    catch (Exception ex) { Trace.WriteLine(ex.Message, MethodInfo.GetCurrentMethod()?.ToString()); }

    return rv;
  }
  public static DateTime GetDaysLastBootTime(DateTime hr00ofTheDate, bool ignoreReboots = true)
  {
    var hr24ofTheDate = hr00ofTheDate.AddDays(1);
    var rv = hr00ofTheDate;
    try
    {
      using (var reader = new EventLogReader(new EventLogQuery("System", PathType.LogName, qryBootUpsOnly(hr00ofTheDate, hr24ofTheDate))))
      {
        for (var er = (EventLogRecord?)reader.read(); null != er && er.TimeCreated.HasValue; er = (EventLogRecord?)reader.read())
        {
          if (!ignoreReboots)
          {
            var bootUpTime = er.TimeCreated.Value;
            using (var reader2 = new EventLogReader(new EventLogQuery("System", PathType.LogName, BootDnWithin5min(bootUpTime))))
            {
              if ((EventLogRecord?)reader2.read() != null) // this is a reboot - ignore it since it is not a session start.
                continue;
            }
          }

          if (rv < er.TimeCreated.Value)
            rv = er.TimeCreated.Value;
        }
      }
    }
    catch (Exception ex) { Trace.WriteLine(ex.Message, MethodInfo.GetCurrentMethod()?.ToString()); }

    return rv;
  }

  public static DateTime GetDaysLastSsUpTime(DateTime hr00ofTheDate)
  {
    //sep 2019: if (hr00ofTheDate == DateTime.Today) return DateTime.Now;

    var hr24ofTheDate = hr00ofTheDate.AddDays(1);
    var rv = hr00ofTheDate;

    try
    {
      using (var reader = new EventLogReader(new EventLogQuery(_aavLogName, PathType.LogName, qryScrSvr(_ssrUp, hr00ofTheDate, hr24ofTheDate))))
      {
        for (var er = (EventLogRecord?)reader.read(); null != er && er.TimeCreated.HasValue; er = (EventLogRecord?)reader.read())
          if (rv < er.TimeCreated.Value)
            rv = er.TimeCreated.Value;
      }
    }
    catch (Exception ex) { Trace.WriteLine(ex.Message, MethodInfo.GetCurrentMethod()?.ToString()); }

    return rv.AddSeconds(-Ssto); // actually - earlier.
  }
  static DateTime GetDaysLastWakeBoot(DateTime hr00ofTheDate)
  {
    var hr24ofTheDate = hr00ofTheDate.AddDays(1);
    var rv = hr00ofTheDate;
    var qry = qryBootAndWakeUps(hr00ofTheDate, hr24ofTheDate);
    try
    {
      using (var reader = new EventLogReader(new EventLogQuery("System", PathType.LogName, qry)))
      {
        for (var er = (EventLogRecord?)reader.read(); null != er && er.TimeCreated.HasValue; er = (EventLogRecord?)reader.read())
          if (rv < er.TimeCreated.Value)
            rv = er.TimeCreated.Value;
      }
    }
    catch (Exception ex) { Trace.WriteLine(ex.Message, MethodInfo.GetCurrentMethod()?.ToString()); throw; }

    return rv;
  }


  static int _ssto = -1; public static int Ssto // ScreenSaveTimeOut 
  {
    get
    {
      if (_ssto < 0)
      {
        try
        {
          const string key = "ScreenSaveTimeOut";
          if (_ssto == -1)
          {
            if (!int.TryParse(Registry.GetValue(@"HKEY_CURRENT_USER\Control Panel\Desktop", key, 299)?.ToString(), out _ssto) || _ssto == 299)
              if (!int.TryParse(Registry.GetValue(@"HKEY_CURRENT_USER\Software\Policies\Microsoft\Windows\Control Panel\Desktop", key, 298)?.ToString(), out _ssto))
                _ssto = 300;

            ////https://docs.microsoft.com/en-us/dotnet/api/microsoft.win32.registrykey.getvalue?view=netframework-4.8
            //var registryKey = Registry.CurrentUser.CreateSubKey(@"Software\Policies\Microsoft\Windows\Control Panel\Desktop"); // UnauthorizedAccessException: Access to the registry key 'HKEY_CURRENT_USER\Software\Policies\Microsoft\Windows\Control Panel\Desktop' is denied.
            //Console.WriteLine("Unexpanded: \"{0}\"", registryKey.GetValue(key, "No Value", RegistryValueOptions.DoNotExpandEnvironmentNames));
            //Console.WriteLine("  Expanded: \"{0}\"", registryKey.GetValue(key));
          }
        }
        catch (Exception ex) { Trace.WriteLine(ex.Message, MethodInfo.GetCurrentMethod()?.ToString()); _ssto = 299; }
      }

      return _ssto;
    }
  }

  static EventRecord? read(this EventLogReader reader) { try { return reader.ReadEvent(); } catch (Exception ex) { Trace.WriteLine(ex.Message, MethodInfo.GetCurrentMethod()?.ToString()); throw;  return null; } }

  static string qryScrSvr(int upOrDn, DateTime a, DateTime b) => $@"<QueryList><Query Id='0' Path='{_aavLogName}'><Select Path='{_aavLogName}'>*[System[Provider[@Name='{_aavSource}'] and (Level=4 or Level=0) and ( EventID={upOrDn} and TimeCreated[@SystemTime&gt;='{a.ToUniversalTime():o}'] and TimeCreated[@SystemTime&lt;='{b.ToUniversalTime():o}'] )]]</Select></Query></QueryList>";
  static string qryBootUpsOnly(DateTime a, DateTime b) => $@"<QueryList><Query Id='0' Path='System'><Select Path='System'>*[System[Provider[@Name='Microsoft-Windows-Kernel-General'] and (Level=4 or Level=0) and (EventID={_bootUp_12}) and TimeCreated[@SystemTime&gt;='{a.ToUniversalTime():o}'] and TimeCreated[@SystemTime&lt;='{b.ToUniversalTime():o}']]]</Select></Query></QueryList>";
  static string BootDnWithin5min(DateTime bootUpTime, int min = -5) => $@"<QueryList><Query Id='0' Path='System'><Select Path='System'>*[System[Provider[@Name='Microsoft-Windows-Kernel-General'] and (Level=4 or Level=0) and (EventID={_bootDn_13}) and TimeCreated[@SystemTime&gt;='{bootUpTime.AddMinutes(min).ToUniversalTime():o}'] and TimeCreated[@SystemTime&lt;='{bootUpTime.ToUniversalTime():o}']]]</Select></Query></QueryList>";

  const int _ssrUp = 7101, _ssrDn = 7102, _bootUp_12 = 12, _bootDn_13 = 13, _syTime_01 = 1; // when waking from hibernation: 12 is nowhere to be seen, 1 is there.
  const string _app = "Application", _sec = "Security", _sys = "System", _aavSource = "AavSource", _aavLogName = "AavNewLog";

  static string qryBootAndWakeUps(DateTime a, DateTime b) =>
//Both wake and boot up:           Kernel-General 12 - up   	OR      Power-TroubleShooter 1 
$@"<QueryList><Query Id='0' Path='System'><Select Path='System'>*[System[ (
(Provider[@Name='Microsoft-Windows-Kernel-General'] and (EventID={_bootUp_12} or EventID={_syTime_01})) or 
(Provider[@Name='Microsoft-Windows-Power-Troubleshooter'] and EventID={_syTime_01}) )  
and TimeCreated[@SystemTime&gt;='{a.ToUniversalTime():o}'] and TimeCreated[@SystemTime&lt;='{b.ToUniversalTime():o}'] ]] </Select></Query></QueryList>";//   <QueryList><Query Id='0' Path='System'><Select Path='System'>*[System[ Provider[@Name='Microsoft-Windows-Kernel-General'] and (Level=4 or Level=0) and (EventID={_bootUp}) and TimeCreated[@SystemTime&gt;='{a.ToUniversalTime():o}'] and TimeCreated[@SystemTime&lt;='{b.ToUniversalTime():o}'] ]]</Select></Query></QueryList>";
}
