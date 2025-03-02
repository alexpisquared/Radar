using System.Windows;

namespace OutlookCrashMonitor;
public partial class MainWindow : Window
{
  int _crashCount_O = 0, _crashCount_E = 0, f = 0;

  public MainWindow() { InitializeComponent(); MouseLeftButtonDown += (s, e) => DragMove(); }

  async void OnLoaded(object sender, RoutedEventArgs e)
  {
    Opacity = 0.5;
    await Task.Delay(_gracePeriodSec * 1_000);
    Opacity = 1;

    while (true)
    {
      var dt = DateTime.Now - _prevChange;
      tbkWatch1.Text = $"{dt.TotalMinutes + 4:N1}";

      if (dt.TotalMinutes is (> 14 and < 15) or
                             (> 28 and < 29)) // check/restart Outlook every ~15 minutes <== should be sufficient for never missing a meeting.
      {
        CheckBothGetReport(ref _crashCount_O, ref _crashCount_E);
        WinAPI.Beep1st(200 + 800 * (f % 4), 240 / (1 + (f++ % 4)));
      }

      await Task.Delay(ProcessExplorer.IsDbg ? 5_950 : 14_960);
    }
  }

  void CheckBothGetReport(ref int crashCount_O, ref int crashCount_E)
  {
    tbkReport.Text = !ProcessExplorer.IsRunningCheckAndRestartIfFalse_Explorer()
        ? $"Explorer restarted {++crashCount_E} times!"
        : $"Explorer is running OK (but restarted {crashCount_O} times)";

    tbkReport.Text += !ProcessExplorer.IsRunningCheckAndRestartIfFalse_Outlook()
        ? $"Outlook restarted {++crashCount_O} times!"
        : $"Outlook is running OK (but restarted {crashCount_O} times)";
  }

  void OnBeep(object sender, RoutedEventArgs e) { WinAPI.Beep1st(400, 80); ; }
  void OnEnter(object sender, RoutedEventArgs e) { WinAPI.Beep1st(400, 80); CheckBothGetReport(ref _crashCount_O, ref _crashCount_E); }
  void OnClose(object sender, RoutedEventArgs e) { WinAPI.Beep1st(300, 80); Close(); }

  const int _gracePeriodSec = 3;
  readonly DateTime _prevChange = DateTime.Now;
  async void OnMouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
  {
    var dt = DateTime.Now - _prevChange;
    if (dt.TotalSeconds < _gracePeriodSec) // check/restart Outlook every ~15 minutes <== should be sufficient for never missing a meeting.
    {
      WinAPI.Beep(8_000, 80);
      return;
    }

    Opacity = 0.5;
    WinAPI.Beep(180, 200);
    await Task.Delay(360);
    Application.Current.Shutdown();
  }
}