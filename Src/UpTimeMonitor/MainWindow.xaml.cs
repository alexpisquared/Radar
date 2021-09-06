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

      OnTick(null, new EventArgs());

#if DEBUG
      new DispatcherTimer(new TimeSpan(0, 0, 0, 0, 333), DispatcherPriority.Background, OnTick, Dispatcher.CurrentDispatcher).Start();
#else
      new DispatcherTimer(new TimeSpan(0, 0, 1, 0, 0), DispatcherPriority.Background, OnTick, Dispatcher.CurrentDispatcher).Start();
#endif

      _start = DateTimeOffset.Now;
    }

    void OnTick(object? s, EventArgs e)
    {
      var uptime = EventLogHelper.CurrentSessionDuration();

      Title = $"{uptime.TotalMinutes:N0}";
      tb1.Text = $"{uptime.TotalHours:N2} h";

      tb2.Text = $"{(DateTimeOffset.Now - _start).TotalHours:N2} h";
      tb3.Text = $"{(DateTimeOffset.Now - _start).TotalHours:N2} h";
    }
  }
}
