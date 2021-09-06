using AAV.WPF.AltBpr;
using AsLink;
using Radar.Properties;
using SpeechSynthLib.Adapter;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Radar.View
{
  public partial class LongStretchAlertPopup : AAV.WPF.Base.WindowBase
  {
    readonly TimeSpan _uptime;
    readonly string _rainAndUptimeMsg;
    readonly SpeechSynthesizer _synth;

    public LongStretchAlertPopup(TimeSpan uptime, string rainAndUptimeMsg, SpeechSynthesizer Synth)
    {
      _uptime = uptime;
      _rainAndUptimeMsg = rainAndUptimeMsg;
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

      await _synth.SpeakAsync(_rainAndUptimeMsg); //redundant: await ChimerAlt.BeepFD(6000, .2);
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
        await _synth.SpeakAsync($"Hm. {min} minute extension has passed.");
      }
    }

    void onShowRadar(object sender, RoutedEventArgs e) => new RadarAnimation(Settings.AlarmThreshold).Show();
  }
}
