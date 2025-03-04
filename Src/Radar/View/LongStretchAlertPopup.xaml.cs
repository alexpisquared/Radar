using AsLink;
using Radar.Logic;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Radar.View
{
  public partial class LongStretchAlertPopup //: AAV.WPF.Base.WindowBase
  {
    readonly TimeSpan _uptime;
    readonly SpeechSynth _synth;

    public LongStretchAlertPopup(TimeSpan uptime, SpeechSynth Synth)
    {
      _uptime = uptime;
      _synth = Synth;
      InitializeComponent();
      PreviewTouchDown += (s, e) => CaptureTouch(e.TouchDevice);

      Loaded += onLoaded;
      new DispatcherTimer(TimeSpan.FromSeconds(1), DispatcherPriority.Background, new EventHandler((s, e) => StandingTime = EvLogHelper.CurrentSessionDuration()), Dispatcher.CurrentDispatcher).Start(); //tu: one-line timer
      dailyChart1.TrgDateC = DateTime.Today;
      DataContext = this;
      HeaderTitle = $"Uptime Watch - Time for a break {(uptime.TotalHours < 1.5 ? "?" : "!!!")}  (init-l up time {uptime:h\\:mm} hr)";
    }

    async void onLoaded(object s, RoutedEventArgs e)
    {
      //? await Task.Delay(5000);

      Topmost = true;
      WindowStartupLocation = WindowStartupLocation.CenterScreen;
      WindowState = WindowState.Normal;

      dailyChart1.ClearDrawAllSegmentsForSinglePC(Environment.MachineName, "Red");
    }

    public static readonly DependencyProperty StandingTimeProperty = DependencyProperty.Register("StandingTime", typeof(TimeSpan), typeof(LongStretchAlertPopup), new PropertyMetadata()); public TimeSpan StandingTime { get => (TimeSpan)GetValue(StandingTimeProperty); set => SetValue(StandingTimeProperty, value); }
    public static readonly DependencyProperty HeaderTitleProperty = DependencyProperty.Register("HeaderTitle", typeof(string), typeof(LongStretchAlertPopup), new PropertyMetadata($"Up-Time Watch - Time for a break?")); public string HeaderTitle { get => (string)GetValue(HeaderTitleProperty); set => SetValue(HeaderTitleProperty, value); }

    async void onExtendXXMin(object s, RoutedEventArgs e)
    {
      var c = ((Button)s)?.Content?.ToString()?.Replace("_", "").Replace(" ", "");
      if (int.TryParse(c, out var min))
      {
        Visibility = Visibility.Collapsed;
        await Task.Delay(min * 60 * 1000);
        Visibility = Visibility.Visible;
        _synth.SpeakFAF($"Hm. {min} minute extension has passed.");
      }
    }

    void onShowRadar(object sender, RoutedEventArgs e) => new RadarAnimation(Settings.AlarmThreshold).Show();

    const int _gracePeriodSec = 3;
    readonly DateTime _prevChange = DateTime.Now;
    async void OnMouseEnter(object sender, MouseEventArgs e)
    {
      var dt = DateTime.Now - _prevChange;
      if (dt.TotalSeconds < _gracePeriodSec) // check/restart Outlook every ~15 minutes <== should be sufficient for never missing a meeting.
      {
        WinAPI.Beep(8_000, 80);
        return;
      }

      Opacity = 0.5;
      WinAPI.Beep(6000, 64);
      await Task.Delay(32);
      Application.Current.Shutdown();
    }
  }
}
