﻿using Microsoft.Win32;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Reflection;
using System.Runtime.Versioning;

namespace EventLogLib;

[SupportedOSPlatform("windows")]
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
  public static TimeSpan GetIdleGapsTotal(DateTime hr00ofTheDate)
  {
    var sw = Stopwatch.StartNew();
    DateTime now = DateTime.Now, t1 = DateTime.MinValue, t2 = DateTime.MinValue;
    TimeSpan ttlSsDnTime = TimeSpan.FromTicks(0), ttlSsUpTime = TimeSpan.FromTicks(0), ttlSsto = TimeSpan.FromTicks(0);
    var hr24ofTheDate = hr00ofTheDate.AddDays(1);
    var ssto = TimeSpan.FromSeconds(Ssto + _graceEvLogAndLockPeriodSec);
    var apl1hr = $@"<QueryList><Query Id='0' Path='{_aavLogName}'><Select Path='{_aavLogName}'>*[System[Provider[@Name='{_aavSource}'] and (Level=4 or Level=0) and ( (EventID &gt;= {_ssrUp} and EventID &lt;= {_ssrDn}) ) and TimeCreated[@SystemTime&gt;='{hr00ofTheDate.ToUniversalTime():o}']]]</Select></Query></QueryList>";

    Debug.Write($"   ScrSvr Up  -  ScrSvr Dn         dt          Ttl Up  +    Ttl Dn  =     TTL    ==>  ttlSsUp  +   ttlSsDn  =    +++");

    using (var reader = new EventLogReader(new EventLogQuery(_aavLogName, PathType.LogName, apl1hr)))
    {
      for (var er = (EventLogRecord?)reader.ReadEventSafe(); null != er && er.TimeCreated.HasValue; er = (EventLogRecord?)reader.ReadEventSafe())
      {
        if (er.TimeCreated.Value > hr24ofTheDate)
        {
          if (t1 > t2) // if last event was up - add this range to uptime
            ttlSsUpTime += (hr24ofTheDate - t1);
          else
            ttlSsDnTime += (hr24ofTheDate - t2);

          break;
        }

        if (er.Id == _ssrUp)
        {
          ttlSsto += ssto;
          t1 = er.TimeCreated.Value;
          var timeBetweenEvents = t2 == DateTime.MinValue ? t1 - hr00ofTheDate : t1 - t2;
          ttlSsDnTime += timeBetweenEvents;
          Debug.Write($"\r\n {er.TimeCreated:ddd HH:mm:ss} -              :  {timeBetweenEvents,8:h\\:mm\\:ss}  ");
        }
        else if (er.Id == _ssrDn)
        {
          t2 = er.TimeCreated.Value;
          var timeBetweenEvents = t1 == DateTime.MinValue ? t2 - hr00ofTheDate : t2 - t1;
          ttlSsUpTime += timeBetweenEvents;
          Debug.Write($"\r\n              - {er.TimeCreated:ddd HH:mm:ss} :  {timeBetweenEvents,8:h\\:mm\\:ss}  ");
        }

        Debug.Write($"   {ttlSsUpTime,8:h\\:mm\\:ss}  +  {ttlSsDnTime,8:h\\:mm\\:ss}  =  {(ttlSsUpTime + ttlSsDnTime),8:h\\:mm\\:ss}");
    Debug.Write($"  ==> {ttlSsUpTime,8:h\\:mm\\:ss}  +  {ttlSsto,8:h\\:mm\\:ss}  =  {ttlSsUpTime + ttlSsto,8:h\\:mm\\:ss}     (ss dn: {ttlSsDnTime:h\\:mm\\:ss})\t  {sw.ElapsedMilliseconds,5}ms ");
      }
    }

    if (now < hr24ofTheDate) // if this is today 
    {
      if (t1 > t2) // if last event was up - add this range to uptime
        ttlSsUpTime += now - t1;
      else
        ttlSsDnTime += now - t2;
    }

    Debug.Write($"  ==> {ttlSsUpTime,8:h\\:mm\\:ss}  +  {ttlSsto,8:h\\:mm\\:ss}  =  {ttlSsUpTime + ttlSsto,8:h\\:mm\\:ss}     (ss dn: {ttlSsDnTime:h\\:mm\\:ss})\t  {sw.ElapsedMilliseconds,5}ms \n");

    return ttlSsUpTime + ttlSsto;
  }
  public static TimeSpan GetTotalPowerUpTimeForTheDay(DateTime hr00ofTheDate) // from first power on till last power off (or now, if today) + IGNORING all idle gaps   (Sep 2022)
  {
    var sw = Stopwatch.StartNew();
    DateTime now = DateTime.Now, prevWaken = DateTime.MinValue;
    var ttlwd = TimeSpan.FromTicks(0);
    var hr24ofTheDate = hr00ofTheDate.AddDays(1);
    var sleeps = $@"<QueryList><Query Id='0' Path='System'><Select Path='System'>*[System[Provider[@Name='Microsoft-Windows-Power-Troubleshooter'] and TimeCreated[@SystemTime &gt;= '{hr00ofTheDate.ToUniversalTime():o}' ]]]</Select></Query></QueryList>";

    using (var reader = new EventLogReader(new EventLogQuery("System", PathType.LogName, sleeps)))
    {
      for (var er = (EventLogRecord?)reader.ReadEventSafe(); null != er; er = (EventLogRecord?)reader.ReadEventSafe())
      {
        if (er == null || er?.Properties == null || er?.Properties?[0] == null || !(er?.Properties?[0]?.Value is DateTime) || er.Properties[1] == null || !(er.Properties[1].Value is DateTime)) throw new Exception("Not a datetime !!! (AP)");

        var sleepAt = (DateTime)er.Properties[0].Value;
        var wakenAt = (DateTime)er.Properties[1].Value;

        if (prevWaken == DateTime.MinValue && sleepAt > hr00ofTheDate) //ie: worked past midnight
        {
          ttlwd += sleepAt - GetDays1rstBootTime(hr00ofTheDate);
        }

        //if (wakenTime > hr00ofTheDate)
        {
          if (prevWaken != DateTime.MinValue)
            ttlwd += (sleepAt - prevWaken);

          prevWaken = wakenAt;
        }

        //77 Debug.Write($"\n {er.TimeCreated:ddd HH:mm}:    {sleepAt,5:H:mm} ÷ {wakenAt,5:H:mm} = {(wakenAt - sleepAt),5:h\\:mm}    ttl: {ttlwd,5:h\\:mm}");

        if (wakenAt > hr24ofTheDate)
          break;
      }
    }

    if (now < hr24ofTheDate) // if today - then consider now as t2.
      ttlwd += (now - prevWaken);
    else
      Debug.Write($"");

    Debug.Write($"  ==> up time on   {hr00ofTheDate:ddd}    {ttlwd,5:h\\:mm}         {sw.ElapsedMilliseconds,5:N0}ms \n");

    //nogo: requires to be ran as admin: using (var reader = new EventLogReader(new EventLogQuery("Security", PathType.LogName, $@"<QueryList><Query Id='0' Path='Security'><Select Path='Security'>*[System[(EventID=4802 or EventID=4803) and TimeCreated[@SystemTime&gt;='{date.ToUniversalTime():o}']]]</Select></Query></QueryList>"))) //tu: logging screensaver vents:  https://i.stack.imgur.com/WtXOv.png

    return ttlwd;
  }

  public static DateTime GetDays1rstBootTime(DateTime hr00ofTheDate)
  {
    var hr24ofTheDate = hr00ofTheDate.AddDays(1);
    var rv = hr24ofTheDate;
    try
    {
      using var reader = new EventLogReader(new EventLogQuery("System", PathType.LogName, /*qryBootUpsOnly*/qryBootUpTmChg(hr00ofTheDate, hr24ofTheDate))); //sep 2018
      for (var er = (EventLogRecord?)reader.ReadEventSafe(); null != er && er.TimeCreated.HasValue; er = (EventLogRecord?)reader.ReadEventSafe())
        if (rv > er.TimeCreated.Value)
          rv = er.TimeCreated.Value;
    }
    catch (Exception ex) { Trace.WriteLine(ex.Message, MethodInfo.GetCurrentMethod()?.ToString()); }

    return rv;
  }
  static DateTime GetDaysLastSsDnTime(DateTime hr00ofTheDate)
  {
    var hr24ofTheDate = hr00ofTheDate.AddDays(1);
    var rv = hr00ofTheDate;
    try
    {
      using var reader = new EventLogReader(new EventLogQuery(_aavLogName, PathType.LogName, qryScrSvr(_ssrDn, hr00ofTheDate, hr24ofTheDate)));
      for (var er = (EventLogRecord?)reader.ReadEventSafe(); null != er && er.TimeCreated.HasValue; er = (EventLogRecord?)reader.ReadEventSafe())
        if (rv < er.TimeCreated.Value)
          rv = er.TimeCreated.Value;
    }
    catch (Exception ex) { Trace.WriteLine(ex.Message, MethodInfo.GetCurrentMethod()?.ToString()); }

    return rv;
  }
  static DateTime GetDaysLastBootTime(DateTime hr00ofTheDate, bool ignoreReboots = true)
  {
    var hr24ofTheDate = hr00ofTheDate.AddDays(1);
    var rv = hr00ofTheDate;
    try
    {
      using var reader = new EventLogReader(new EventLogQuery("System", PathType.LogName, qryBootUpsOnly(hr00ofTheDate, hr24ofTheDate)));
      for (var er = (EventLogRecord?)reader.ReadEventSafe(); null != er && er.TimeCreated.HasValue; er = (EventLogRecord?)reader.ReadEventSafe())
      {
        if (!ignoreReboots)
        {
          var bootUpTime = er.TimeCreated.Value;
          using var reader2 = new EventLogReader(new EventLogQuery("System", PathType.LogName, BootDnWithin5min(bootUpTime)));
          if ((EventLogRecord?)reader2.ReadEventSafe() != null) // this is a reboot - ignore it since it is not a session start.
            continue;
        }

        if (rv < er.TimeCreated.Value)
          rv = er.TimeCreated.Value;
      }
    }
    catch (Exception ex) { Trace.WriteLine(ex.Message, MethodInfo.GetCurrentMethod()?.ToString()); }

    return rv;
  }
  static DateTime GetDaysLastSsUpTime(DateTime hr00ofTheDate)
  {
    //sep 2019: if (hr00ofTheDate == DateTime.Today) return DateTime.Now;

    var hr24ofTheDate = hr00ofTheDate.AddDays(1);
    var rv = hr00ofTheDate;

    try
    {
      using var reader = new EventLogReader(new EventLogQuery(_aavLogName, PathType.LogName, qryScrSvr(_ssrUp, hr00ofTheDate, hr24ofTheDate)));
      for (var er = (EventLogRecord?)reader.ReadEventSafe(); null != er && er.TimeCreated.HasValue; er = (EventLogRecord?)reader.ReadEventSafe())
        if (rv < er.TimeCreated.Value)
          rv = er.TimeCreated.Value;
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
      using var reader = new EventLogReader(new EventLogQuery("System", PathType.LogName, qry));
      for (var er = (EventLogRecord?)reader.ReadEventSafe(); null != er && er.TimeCreated.HasValue; er = (EventLogRecord?)reader.ReadEventSafe())
        if (rv < er.TimeCreated.Value)
          rv = er.TimeCreated.Value;
    }
    catch (Exception ex) { Trace.WriteLine(ex.Message, MethodInfo.GetCurrentMethod()?.ToString()); throw; }

    return rv;
  }


  static int _ssto = -1; public static int Ssto // ScreenSaveTimeOut - !!!: don't forget to add a grace period of 1 min when calculating the idle time
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

  static EventRecord? ReadEventSafe(this EventLogReader reader) { try { return reader.ReadEvent(); } catch (Exception ex) { Trace.WriteLine(ex.Message, MethodInfo.GetCurrentMethod()?.ToString()); return null; } }

  static string qryScrSvr(int upOrDn, DateTime a, DateTime b) => $@"<QueryList><Query Id='0' Path='{_aavLogName}'><Select Path='{_aavLogName}'>*[System[Provider[@Name='{_aavSource}'] and (Level=4 or Level=0) and ( EventID={upOrDn} and TimeCreated[@SystemTime&gt;='{a.ToUniversalTime():o}'] and TimeCreated[@SystemTime&lt;='{b.ToUniversalTime():o}'] )]]</Select></Query></QueryList>";
  static string qryBootUpTmChg(DateTime a, DateTime b) => $@"<QueryList><Query Id='0' Path='System'><Select Path='System'>*[System[Provider[@Name='Microsoft-Windows-Kernel-General'] and (Level=4 or Level=0) and (EventID={_bootUp_12} or EventID={_syTime_01}) and TimeCreated[@SystemTime&gt;='{a.ToUniversalTime():o}'] and TimeCreated[@SystemTime&lt;='{b.ToUniversalTime():o}']]]</Select></Query></QueryList>";
  static string qryBootUpsOnly(DateTime a, DateTime b) => $@"<QueryList><Query Id='0' Path='System'><Select Path='System'>*[System[Provider[@Name='Microsoft-Windows-Kernel-General'] and (Level=4 or Level=0) and (EventID={_bootUp_12}) and TimeCreated[@SystemTime&gt;='{a.ToUniversalTime():o}'] and TimeCreated[@SystemTime&lt;='{b.ToUniversalTime():o}']]]</Select></Query></QueryList>";
  static string BootDnWithin5min(DateTime bootUpTime, int min = -5) => $@"<QueryList><Query Id='0' Path='System'><Select Path='System'>*[System[Provider[@Name='Microsoft-Windows-Kernel-General'] and (Level=4 or Level=0) and (EventID={_bootDn_13}) and TimeCreated[@SystemTime&gt;='{bootUpTime.AddMinutes(min).ToUniversalTime():o}'] and TimeCreated[@SystemTime&lt;='{bootUpTime.ToUniversalTime():o}']]]</Select></Query></QueryList>";
  static string qryBootAndWakeUps(DateTime a, DateTime b) =>
//Both wake and boot up:           Kernel-General 12 - up   	OR      Power-TroubleShooter 1 
$@"<QueryList><Query Id='0' Path='System'><Select Path='System'>*[System[ (
(Provider[@Name='Microsoft-Windows-Kernel-General'] and (EventID={_bootUp_12} or EventID={_syTime_01})) or 
(Provider[@Name='Microsoft-Windows-Power-Troubleshooter'] and EventID={_syTime_01}) )  
and TimeCreated[@SystemTime&gt;='{a.ToUniversalTime():o}'] and TimeCreated[@SystemTime&lt;='{b.ToUniversalTime():o}'] ]] </Select></Query></QueryList>";//   <QueryList><Query Id='0' Path='System'><Select Path='System'>*[System[ Provider[@Name='Microsoft-Windows-Kernel-General'] and (Level=4 or Level=0) and (EventID={_bootUp}) and TimeCreated[@SystemTime&gt;='{a.ToUniversalTime():o}'] and TimeCreated[@SystemTime&lt;='{b.ToUniversalTime():o}'] ]]</Select></Query></QueryList>";

  const int _ssrUp = 7101, _ssrDn = 7102, _bootUp_12 = 12, _bootDn_13 = 13, _syTime_01 = 1, _graceEvLogAndLockPeriodSec = 60; // when waking from hibernation: 12 is nowhere to be seen, 1 is there.
  const string _app = "Application", _sec = "Security", _sys = "System", _aavSource = "AavSource", _aavLogName = "AavNewLog";
}
