using EventLogLib;
using System;
using System.Diagnostics;
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
      new DispatcherTimer(new TimeSpan(0, 0, 0, 0, 999), DispatcherPriority.Background, OnTick, Dispatcher.CurrentDispatcher).Start();
      if (Environment.MachineName == "D21-MJ0AWBEV") /**/ { Top = 1608; Left = 1928; }
      if (Environment.MachineName == "RAZER1")       /**/ { Top = 1600; Left = 10; }
#else
      new DispatcherTimer(new TimeSpan(0, 0, 1, 0, 0), DispatcherPriority.Background, OnTick, Dispatcher.CurrentDispatcher).Start();
#endif

    }

    void OnTick(object? s, EventArgs e)
    {
      var sw = Stopwatch.StartNew();
      var now = DateTimeOffset.Now;
      var uptime = EventLogHelper.CurrentSessionDuration();
      var idle = EventLogHelper.GetIdleGapsTotal(_start.Date);
      var raw = now - EventLogHelper.GetDays1rstBootTime(_start.Date);

      Title = $"{uptime.TotalMinutes:N0}";
      tb0.Text = $"{uptime:h\\:mm}";
      tb1.Text = $"{raw:h\\:mm}  -  {idle:h\\:mm}  =  {raw - idle:h\\:mm}";

#if DEBUG
      tb0.Text += $"{uptime:\\:ss}";
      tb1.Text += $"{raw - idle:\\:ss}";
#endif

      tb2.Text = $"{sw.ElapsedMilliseconds:N0} ms";
    }
  }
}
