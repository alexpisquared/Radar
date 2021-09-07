using EventLogLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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

    void OnTick(object? s, EventArgs ea)
    {
      var sw = Stopwatch.StartNew();

      var dt0 = EventLogHelper.GetDays1rstBootTime(_start.Date);
      var lst = EventLogHelper.GetAllEventsOfInterest(_start.Date, _start.Date.AddDays(0.9999999));
      var ssn = EventLogHelper.CurrentSessionDuration();
      var idl = EventLogHelper.GetTotalIdleByScrSvr(_start.Date);

      var now = DateTimeOffset.Now;
      var raw = now - dt0;

      Title = $"{ssn.TotalMinutes:N0}";
      tb0.Text = $"{ssn:h\\:mm}";
      tb1.Text = $"{raw:h\\:mm}  -  {idl:h\\:mm}  =  {raw - idl:h\\:mm}";

#if DEBUG
      tb0.Text += $"{ssn:\\:ss}";
      tb1.Text += $"{raw - idl:\\:ss}";
#endif

      var ttlWrk = TimeSpan.Zero;
      var ttlIdl = TimeSpan.Zero; // scrsvr + timeout + grace minute
      var ttlOff = TimeSpan.Zero;
      var nsl = new SortedList<DateTime, EvOfIntFlag>
      {
        { _start.Date, EvOfIntFlag.Who_Knows_What }
      };
      var prev = nsl.First();
      foreach (var e in lst)
      {
        Trace.Write($"{e.Key:HH:mm:ss} \t {e.Value}  ==>  ");

        if (prev.Value == EvOfIntFlag.Who_Knows_What) // first entry for the day
        {
          switch (e.Value)
          {
            case EvOfIntFlag.ScreenSaverrUp:
            case EvOfIntFlag.ShutAndSleepDn: ttlWrk = e.Key - _start.Date; break;
            case EvOfIntFlag.ScreenSaverrDn: ttlIdl = e.Key - _start.Date; break;
            case EvOfIntFlag.BootAndWakeUps: ttlOff = e.Key - _start.Date; break;
            case EvOfIntFlag.Day1stAmbiguos:
            case EvOfIntFlag.Was_Off_Ignore:
            case EvOfIntFlag.Was_On__Ignore:
            case EvOfIntFlag.Who_Knows_What:
            default: Trace.WriteLine($"{e.Key:HH:mm:ss} \t {e.Value}   -----  //todo:"); break;
          }
        }
        else
        {
          switch (e.Value)
          {
            case EvOfIntFlag.ScreenSaverrUp: ttlWrk = e.Key - prev.Key; break;
            case EvOfIntFlag.ShutAndSleepDn:
              switch (prev.Value)
              {
                case EvOfIntFlag.ScreenSaverrUp: ttlIdl = e.Key - prev.Key; break;
                case EvOfIntFlag.BootAndWakeUps:
                case EvOfIntFlag.ScreenSaverrDn: ttlWrk = e.Key - prev.Key; break;
                case EvOfIntFlag.ShutAndSleepDn:
                case EvOfIntFlag.Day1stAmbiguos:
                case EvOfIntFlag.Was_Off_Ignore:
                case EvOfIntFlag.Was_On__Ignore:
                case EvOfIntFlag.Who_Knows_What:
                default: Trace.WriteLine($"{e.Key:HH:mm:ss} \t {prev.Value}  ==>  {e.Value}   -----  //todo:"); break;
              }
              break;

            case EvOfIntFlag.ScreenSaverrDn:
              switch (prev.Value)
              {
                case EvOfIntFlag.ScreenSaverrUp: ttlIdl = e.Key - prev.Key; break;
                case EvOfIntFlag.BootAndWakeUps:
                case EvOfIntFlag.ScreenSaverrDn:
                case EvOfIntFlag.ShutAndSleepDn:
                case EvOfIntFlag.Day1stAmbiguos:
                case EvOfIntFlag.Was_Off_Ignore:
                case EvOfIntFlag.Was_On__Ignore:
                case EvOfIntFlag.Who_Knows_What:
                default: Trace.WriteLine($"{e.Key:HH:mm:ss} \t {prev.Value}  ==>  {e.Value}   -----  //todo:"); break;
              }
              break;

            case EvOfIntFlag.BootAndWakeUps:
              ttlOff = e.Key - prev.Key;
              switch (prev.Value)
              {
                case EvOfIntFlag.ScreenSaverrUp:
                case EvOfIntFlag.BootAndWakeUps:
                case EvOfIntFlag.ScreenSaverrDn:
                case EvOfIntFlag.ShutAndSleepDn:
                case EvOfIntFlag.Day1stAmbiguos:
                case EvOfIntFlag.Was_Off_Ignore:
                case EvOfIntFlag.Was_On__Ignore:
                case EvOfIntFlag.Who_Knows_What:
                default: Trace.WriteLine($"{e.Key:HH:mm:ss} \t {prev.Value}  ==>  {e.Value}   -----  //todo:"); break;
              }
              break;

            case EvOfIntFlag.Day1stAmbiguos:
            case EvOfIntFlag.Was_Off_Ignore:
            case EvOfIntFlag.Was_On__Ignore:
            case EvOfIntFlag.Who_Knows_What:
            default: Trace.WriteLine($"{e.Key:HH:mm:ss} \t {e.Value}   -----  //todo:"); break;
          }
        }

        Trace.WriteLine($"{ttlWrk + ttlIdl + ttlOff:h\\:mm\\:ss}  =  {ttlWrk:h\\:mm\\:ss} + {ttlIdl:h\\:mm\\:ss} + {ttlOff:h\\:mm\\:ss}  ");

        prev = e;
      }

      tb2.Text = $"{sw.ElapsedMilliseconds:N0}";
    }
  }
}
