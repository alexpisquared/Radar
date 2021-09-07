using EventLogLib;
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace UpTimeMonitor
{
  public partial class MainWindow : Window
  {
    readonly DateTimeOffset _start;

    public MainWindow()
    {
      InitializeComponent();

      MouseLeftButtonDown += (s, e) => DragMove();
      KeyUp += (s, e) => { switch (e.Key) { default: break; case Key.Escape: base.OnKeyUp(e); Close(); break; } };
      _start = DateTimeOffset.Now;

      OnTick(null, new EventArgs());

#if DEBUG
      new DispatcherTimer(new TimeSpan(0, 0, 0, 0, 333), DispatcherPriority.Background, OnTick, Dispatcher.CurrentDispatcher).Start();
#else
      new DispatcherTimer(new TimeSpan(0, 0, 1, 0, 0), DispatcherPriority.Background, OnTick, Dispatcher.CurrentDispatcher).Start();
#endif

    }

    void OnTick(object? s, EventArgs e)
    {
      var uptime = EventLogHelper.CurrentSessionDuration();

      Title = $"{uptime.TotalMinutes:N0}";
      tb0.Text = $"{uptime:h\\:mm}";

      tb1.Text = $"{EventLogHelper.GetTotalPowerUpTimeForTheDay(_start.Date):h\\:mm}";
      tb2.Text = $"{EventLogHelper.GetTotalIdlePlusScrsvrUpTimeForTheDate(_start.Date):h\\:mm}";

      tb3.Text = $"{(DateTimeOffset.Now - _start).TotalHours:N1}";
    }
  }
}
