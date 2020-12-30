﻿using AAV.Sys.Helpers;
using AAV.WPF.Helpers;
using Radar.Properties;
using RadarLib;
using RadarPicCollect;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Radar
{
  public partial class App : Application
  {
    public static readonly DateTime Started = DateTime.Now;
    const string
      help = "help",
      sayEvenNoRain = "SayEvenNoRain",
      justUiNotElse = "justUiNotElse",
      sayUpTimeNoUI = "sayUpTimeNoUI",
      sayUpTimeShow = "sayUpTimeShow",
      showIfRainCmn = "ShowIfOnOrComingSoon";

    protected override async void OnStartup(StartupEventArgs e)
    {
      base.OnStartup(e);      //Bpr.BeepBgn2();

      Application.Current.DispatcherUnhandledException += UnhandledExceptionHndlr.OnCurrentDispatcherUnhandledException;     //new SpeechSynthesizer().Speak("Testing");			new SpeechSynthesizer().SpeakAsync("Testing");

      Tracer.SetupTracingOptions("Radar", new TraceSwitch("Verbose-ish", "See ScrSvr for the model.") { Level = TraceLevel.Verbose }, false);

      try
      {
        scheduleAutoAppShutDownIfAsked(e);

        await Task.Delay(25);
        var uptime = EvLogHelper.CurrentSessionDuration();

        switch (e.Args.Length > 0 ? e.Args[0] : "")
        {
          default:
          case help:
          case justUiNotElse: new RadarAnimation(true, Settings.Default.AlarmThreshold).ShowDialog(); break;
          case showIfRainCmn: if (sayRainOnOrComing(e.Args, uptime))                    /**/ goto default; break;
          case sayUpTimeShow: sayRainOnOrComing(e.Args, uptime);                        /**/ goto default;
          case sayUpTimeNoUI: if (uptime.TotalMinutes > 20) Synth.Speak(upTimeMsg(uptime, "No UI.")); break;
        }

        var eois = await EvLogHelper.UpdateEvLogToDb(0, $"Radar.exe {e.Args.FirstOrDefault()}");
        Trace.WriteLine($"{eois} events found/saved to db."); // System.Threading.Thread.Sleep(3999);            Synth.Speak($"{eois} events found");

        //Bpr.BeepEnd2();
      }
      catch (Exception ex)
      {
        AAV.Sys.Helpers.Bpr.BeepBigError();
        Synth.Speak(ex.Message);
      }
      finally
      {
        await Task.Delay(5000); // what are we waiting for?
        App.Current.Shutdown();
      }
    }

    bool sayRainOnOrComing(string[] args, TimeSpan uptime)
    {
      var rainAndUptimeMsg = (uptime.TotalMinutes > 20) ? upTimeMsg(uptime, "") : "";
      try
      {
        var maxPicsToGet = 2; // 1 hr + 10 min.
        var minRainPace = Settings.Default.AlarmThreshold; // .01 - .05
        var rpc = new RadarPicCollector();
        rpc.DownloadRadarPics_MostRecent_RAIN_only(maxPicsToGet);
        if (rpc.Pics.Count < maxPicsToGet)
          rainAndUptimeMsg += ($"Not enough radar pictures have been acquired: {rpc.Pics.Count}, while {maxPicsToGet} is needed.");
        else
        {
          if (rpc.Pics[0].Measure > minRainPace)
          {
            rainAndUptimeMsg += ($"Raining...");
            return true;
          }
          else
          {
            var ptn = projectoinToNow(maxPicsToGet, rpc);
            rainAndUptimeMsg += (ptn > minRainPace ? "Rain is coming." : (args.Length > 1 && args[1].Equals(sayEvenNoRain) ? "No rain to be seen" : ""));
            return ptn > minRainPace;
          }
        }
      }
      catch (Exception ex) { System.Diagnostics.Trace.WriteLine(ex.Message, System.Reflection.MethodInfo.GetCurrentMethod().Name); if (System.Diagnostics.Debugger.IsAttached) System.Diagnostics.Debugger.Break(); throw; }
      finally
      {

        if ((args.Length > 1 && args[1].Equals("ShowLsaPopup")) || isDue(uptime))
          showLongStretchAlertPopup(uptime, rainAndUptimeMsg);
        else if (rainAndUptimeMsg.Length > 0)
          Synth.Speak(rainAndUptimeMsg);
      }

      return true; // show anyway in case of issues.
    }

    bool isDue(TimeSpan uptime)
    {
#if DEBUG
      var timeLimitHr = .01;
#else
      var timeLimitHr = 1;
#endif
      if (uptime.TotalHours < timeLimitHr)
      {
        Debug.WriteLine //                Synth.Speak //           
          ($"Hidden as has been standing only for {uptime.TotalMinutes:N0} minutes.");
      }
      //else if (DateTime.Now < Settings.Default.PopUp_LastTime.AddMinutes(Settings.Default.PopUp_IntervalToSkipMin))
      //{
      //  Synth.Speak($"Hidden, as already checked only {(DateTime.Now - Settings.Default.PopUp_LastTime).TotalMinutes:N0} minutes ago.");
      //}
      else
      {
        return true;
      }

      return false;
    }

    void showLongStretchAlertPopup(TimeSpan uptime, string rainAndUptimeMsg)
    {
      //too annoying: Synth.SpeakAsync($"Showing");
      Settings.Default.PopUp_LastTime = DateTime.Now;
      Settings.Default.Save();
      new View.LongStretchAlertPopup(uptime, rainAndUptimeMsg, Synth).ShowDialog();
    }

    static string upTimeMsg(TimeSpan uptime, string src)
    {
      var msg = $"{src} Up for " +
        (uptime.TotalHours > 1 ? $"{uptime.Hours} hour{(uptime.Hours > 1 ? "s" : "")} and " : "") +
        $"{(uptime.Minutes - uptime.Minutes % 5)} minutes. "; //$"{uptime.Minutes} minute{((uptime.Minutes == 1 || (uptime.Minutes > 20 && uptime.Minutes % 10 == 1)) ? "" : "s")}.";

      return msg;
    }
    static double projectoinToNow(int maxPicsToGet, RadarPicCollector rpc)
    {
      var latest = 0;
      var earlst = maxPicsToGet - 1;
      return
          (DateTime.Now.ToOADate() - rpc.Pics[latest].ImageTime.ToOADate())
          * (rpc.Pics[latest].Measure - rpc.Pics[earlst].Measure)
          / (rpc.Pics[latest].ImageTime.ToOADate() - rpc.Pics[earlst].ImageTime.ToOADate())
          + rpc.Pics[latest].Measure;
    }
    void scheduleAutoAppShutDownIfAsked(StartupEventArgs e)
    {
      if (e.Args.Length > 1 && int.TryParse(e.Args[1], out var units))
        scheduleAutoAppShutDown(units, e.Args.Length > 2 ? e.Args[2] : "min");
    }
    async void scheduleAutoAppShutDown(int units, string ut)
    {
      var k =
        ut == "sec" ? 1000 :
        ut == "min" ? 60000 :
        ut == "hour" ? 360000 :
        60000;

      await Task.Delay(k * units);
      //Bpr.BeepFinish();
      App.Current.Shutdown();
    }
    SpeechSynthesizer _ss; public SpeechSynthesizer Synth { get { if (_ss == null) { _ss = new SpeechSynthesizer(); _ss.SpeakAsyncCancelAll(); _ss.Rate = 6; _ss.Volume = Environment.UserDomainName.Equals("CORP", StringComparison.OrdinalIgnoreCase) ? 70 : 90/*0-100*/; _ss.SelectVoiceByHints(gender: VoiceGender.Female); } return _ss; } }
  }
}
