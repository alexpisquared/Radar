using Microsoft.Extensions.Configuration;
using System.Windows;

namespace OutlookCrashMonitor;
public partial class MainWindow : Window
{
  const int _gracePeriodSec = 3;
  readonly double _periodInMin;
  readonly string _explorerProcessPath, _outlookProcessPath;
  int _crashCount_O = 0, _crashCount_E = 0, f = 0;

  public MainWindow()
  {
    InitializeComponent();
    MouseLeftButtonDown += (s, e) => DragMove();

    var builder = new ConfigurationBuilder()
        .AddUserSecrets<MainWindow>();

    var configuration = builder.Build();
    _periodInMin = double.Parse(configuration["PeriodInMin"] ?? "9");
    _outlookProcessPath = configuration["_outlookProcessPath"] ?? "9";
    _explorerProcessPath = configuration["_explorerProcessPath"] ?? "9";
  }
  /*
  {
  "_outlookProcessPath": "C:\\Users\\apigida\\AppData\\Local\\Microsoft\\WindowsApps\\olk.exe",
  "_explorerProcessPath": "C:\\Windows\\explorer.exe",
  "EmailClient": "",
  "ChatClient": "",
  "Explorer": "",
  "PeriodInMin": "9",
  "WhereAmI": "OoutlookCrashMonitor - user secrets"
}*/
  async void OnLoaded(object sender, RoutedEventArgs e)
  {
    Opacity = 0.3;
    await Task.Delay(TimeSpan.FromSeconds(_gracePeriodSec));
    Opacity = 1;

    tbkReportL.Text = $"{_periodInMin + 4:N1}";
    tbkReportR.Text = CheckBothGetReport(ref _crashCount_O, ref _crashCount_E);

    while (true)
    {
      var dt = DateTime.Now - _prevChange;
      tbkReportC.Text = $"{dt.TotalMinutes + 4:N1}";

      if ((dt.TotalMinutes > (_periodInMin * 1) && dt.TotalMinutes < (_periodInMin * 1 + 1)) ||
          (dt.TotalMinutes > (_periodInMin * 2) && dt.TotalMinutes < (_periodInMin * 2 + 1)) ||
          (dt.TotalMinutes > (_periodInMin * 3) && dt.TotalMinutes < (_periodInMin * 3 + 1)) ||
          (dt.TotalMinutes > (_periodInMin * 4) && dt.TotalMinutes < (_periodInMin * 4 + 1))) // check/restart Outlook every ~15 minutes <== should be sufficient for never missing a meeting.
      {
        tbkReportR.Text = CheckBothGetReport(ref _crashCount_O, ref _crashCount_E);
        WinAPI.Beep1st(200 + 800 * (f % 4), 240 / (1 + (f++ % 4)));
      }

      await Task.Delay(ProcessExplorer.IsDbg ? 5_950 : 14_960);
    }
  }

  string CheckBothGetReport(ref int crashCount_O, ref int crashCount_E) =>
      //(!ProcessExplorer.IsRunningCheckAndRestartIfFalse_Explorer() ? $"Explorer restarted {++crashCount_E} times!" : $"Explorer is running OK (but restarted {crashCount_E} times)") +
      !ProcessExplorer.IsRunningCheckAndRestartIfFalse_Outlook(_outlookProcessPath) ? $"Outlook crashed/restarted {++crashCount_O} times!" : $"Outlook crashed {crashCount_O}     Restarted {crashCount_O} times";

  void OnBeep(object sender, RoutedEventArgs e) { WinAPI.Beep1st(400, 80); ; }
  void OnEnter(object sender, RoutedEventArgs e) { WinAPI.Beep1st(400, 80); _ = CheckBothGetReport(ref _crashCount_O, ref _crashCount_E); }
  void OnClose(object sender, RoutedEventArgs e) { WinAPI.Beep1st(300, 80); Close(); }

  readonly DateTime _prevChange = DateTime.Now;
  async void OnMouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
  {
    var dt = DateTime.Now - _prevChange;
    if (dt.TotalSeconds < _gracePeriodSec) // check/restart Outlook every ~15 minutes <== should be sufficient for never missing a meeting.
    {
      WinAPI.Beep(8_000, 80);
      return;
    }

    Opacity = 0.3;
    WinAPI.Beep(180, 200);
    await Task.Delay(200);
    Application.Current.Shutdown();
  }
}