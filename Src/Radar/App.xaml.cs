namespace Radar;
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
  //static SpeechSynthesizer? _synth = null; public static SpeechSynthesizer Synth { get { if (_synth == null) { _synth = new SpeechSynthesizer(); _synth.SpeakAsyncCancelAll(); _synth.Rate = 6; _synth.Volume = 25; /*_synth.SelectVoiceByHints(gender: VoiceGender.Female); */ } return _synth; } }
  static SpeechSynth? _synth = null; public static SpeechSynth Synth => _synth ??= new(new ConfigurationBuilder().AddUserSecrets<App>().Build()["AppSecrets:MagicSpeech"] ?? throw new ArgumentNullException("###################"), true, CC.EnusAriaNeural.Voice);
  protected override async void OnStartup(StartupEventArgs e)
  {
    base.OnStartup(e);

#if DEBUG_
    var url = "https://dd.meteo.gc.ca/radar/PRECIPET/GIF/WKR/";    //var files = System.IO.Directory.GetFiles(@"\\dd.meteo.gc.ca\radar\PRECIPET\GIF\WKR");
    await (new WebDirectoryLoader()).UseRegex(url);

    Shutdown();
#endif
    //Bpr.BeepBgn2();

    Current.DispatcherUnhandledException += UnhandledExceptionHndlr.OnCurrentDispatcherUnhandledException;     //new SpeechSynthesizer().Speak("Testing");			new SpeechSynthesizer().SpeakAsync("Testing");

    Tracer.SetupTracingOptions("Radar", new TraceSwitch("Verbose-ish", "See ScrSvr for the model.") { Level = TraceLevel.Verbose }, false);

    WriteLine($"e.Args[0]: '{e.Args[0]}'.");

    try
    {
      scheduleAutoAppShutDownIfAsked(e);

      await Task.Delay(25);
      var uptime = EvLogHelper.CurrentSessionDuration();

#if !true // when radar source found/implemented:
        if (//(e.Args.Length > 1 && e.Args[1].Equals("ShowLsaPopup")) ||
            isDue(uptime))
          showLongStretchAlertPopup(uptime, "");
        else
          await Synth.Speak($"Too early: {uptime.TotalMinutes:N0} minutes."); //redundant: await ChimerAlt.BeepFD(6000, .2);
#else
      switch (e.Args.Length > 0 ? e.Args[0] : "")
      {
        default:
        case help:
        case justUiNotElse: new RadarAnimation(Settings.AlarmThreshold).ShowDialog(); break;
        case showIfRainCmn: if (sayRainOnOrComing(e.Args, uptime))                    /**/ goto default; break;
        case sayUpTimeShow: sayRainOnOrComing(e.Args, uptime);                        /**/ goto default;
        case sayUpTimeNoUI: if (uptime.TotalMinutes > 20) Synth.SpeakFAF(upTimeMsg(uptime, "No UI.")); break;
      }
#endif

      var eois = await EvLogHelper.UpdateEvLogToDb(0, $"Radar.exe {e.Args.FirstOrDefault()}");
      WriteLine($"{eois} events found/saved to db."); // System.Threading.Thread.Sleep(3999);            Synth.Speak($"{eois} events found");

      //Bpr.BeepEnd2();
    }
    catch (Exception ex)
    {
      //Bpr.BeepBigError();
      Synth.SpeakFAF(ex.Message);
    }
    finally
    {
      await Task.Delay(2500); // what are we waiting for?
      App.Current.Shutdown();
    }
  }

  bool sayRainOnOrComing(string[] args, TimeSpan uptime)
  {
    var rainAndUptimeMsg = (uptime.TotalMinutes > 20) ? upTimeMsg(uptime, "") : "";
    try
    {
      var maxPicsToGet = 2; // 1 hr + 10 min.
      var minRainPace = Settings.AlarmThreshold; // .01 - .05
      var rpc = new RadarPicCollector();
      _ = rpc.DownloadRadarPics_MostRecent_RAIN_only(maxPicsToGet, 10);
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
    catch (Exception ex) { System.Diagnostics.Trace.WriteLine(ex.Message, System.Reflection.MethodInfo.GetCurrentMethod()?.Name); if (System.Diagnostics.Debugger.IsAttached) System.Diagnostics.Debugger.Break(); throw; }
    finally
    {
      if ((args.Length > 1 && args[1].Equals("ShowLsaPopup")) || IsDue(uptime))
        showLongStretchAlertPopup(uptime, rainAndUptimeMsg);
      else if (rainAndUptimeMsg.Length > 0)
        Synth.SpeakFAF(rainAndUptimeMsg);
    }

    return true; // show anyway in case of issues.
  }

  static bool IsDue(TimeSpan uptime)
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
    //else if (DateTime.Now < Settings.PopUp_LastTime.AddMinutes(Settings.PopUp_IntervalToSkipMin))
    //{
    //  Synth.Speak($"Hidden, as already checked only {(DateTime.Now - Settings.PopUp_LastTime).TotalMinutes:N0} minutes ago.");
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
    Settings.PopUp_LastTime = DateTime.Now;
    Settings.Save();
    _ = new View.LongStretchAlertPopup(uptime, rainAndUptimeMsg, Synth).ShowDialog();
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
}