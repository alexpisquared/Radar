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


      //again:
      TimeSpan ttlWrk, ttlIdl, ttlOff;
      DateTime dayStartAt;
      EvOfIntFlag dayStartEv;
      GetSplit(lst, out now, out ttlWrk, out ttlIdl, out ttlOff, out dayStartAt, out dayStartEv);

      Trace.WriteLine($"Day Start {dayStartAt}  {dayStartEv}    {ttlWrk + ttlIdl + ttlOff:hh\\:mm\\:ss}  =  {ttlWrk:h\\:mm\\:ss} + {ttlIdl:h\\:mm\\:ss} + {ttlOff:hh\\:mm\\:ss}    <== dt ");

      tb2.Text = $"{sw.ElapsedMilliseconds:N0}";
    }

    private void GetSplit(SortedList<DateTime, EvOfIntFlag> lst, out DateTimeOffset now, out TimeSpan ttlWrk, out TimeSpan ttlIdl, out TimeSpan ttlOff, out DateTime dayStartAt, out EvOfIntFlag dayStartEv)
    {
      ttlWrk = TimeSpan.Zero;
      ttlIdl = TimeSpan.Zero;
      ttlOff = TimeSpan.Zero;
      var prev = new SortedList<DateTime, EvOfIntFlag> { { _start.Date, EvOfIntFlag.Who_Knows_What } }.First();
      foreach (var e in lst)
      {
        if (prev.Value == EvOfIntFlag.Who_Knows_What) // first entry for the day
        {
          switch (e.Value)
          {
            case EvOfIntFlag.ScreenSaverrUp:
            case EvOfIntFlag.ShutAndSleepDn: ttlWrk = e.Key - _start.Date; break;
            case EvOfIntFlag.ScreenSaverrDn: ttlIdl = e.Key - _start.Date; break;
            case EvOfIntFlag.BootAndWakeUps: ttlOff = e.Key - _start.Date; break;
            default: Trace.WriteLine($"{e.Key:HH:mm:ss} \t {e.Value}   -----  //todo: 000 "); break;
          }
        }
        else
        {
          switch (e.Value)
          {
            case EvOfIntFlag.ScreenSaverrUp: ttlWrk += e.Key - prev.Key; break;

            case EvOfIntFlag.ShutAndSleepDn:
              switch (prev.Value)
              {
                case EvOfIntFlag.ScreenSaverrUp: ttlIdl += e.Key - prev.Key; break;
                case EvOfIntFlag.BootAndWakeUps:
                case EvOfIntFlag.ScreenSaverrDn: ttlWrk += e.Key - prev.Key; break;
                default: Trace.Write($"\t {prev.Value}  ==>  {e.Value}   -----  //todo: 111 \t"); break;
              }
              break;

            case EvOfIntFlag.ScreenSaverrDn:
              switch (prev.Value)
              {
                case EvOfIntFlag.BootAndWakeUps:
                case EvOfIntFlag.ScreenSaverrUp: ttlIdl += e.Key - prev.Key; break;
                default: Trace.Write($"\t {prev.Value}  ==>  {e.Value}   -----  //todo: 222 \t"); break;
              }
              break;

            case EvOfIntFlag.BootAndWakeUps:
              switch (prev.Value)
              {
                case EvOfIntFlag.ShutAndSleepDn:
                case EvOfIntFlag.ScreenSaverrUp:
                case EvOfIntFlag.BootAndWakeUps: ttlOff += e.Key - prev.Key; break;
                default: Trace.Write($"\t {prev.Value}  ==>  {e.Value}   -----  //todo: 333 \t"); break;
              }
              break;

            default: Trace.Write($"\t {e.Value}   -----  //todo: 444 \t"); break;
          }
        }

        Trace.Write($"  {prev.Value} --> {e.Value}   + {e.Key - prev.Key:h\\:mm\\:ss}  ==>  {e.Key:HH:mm:ss}  =?=  ");
        Trace.WriteLine($"{ttlWrk + ttlIdl + ttlOff:hh\\:mm\\:ss}  =  {ttlWrk:h\\:mm\\:ss} + {ttlIdl:h\\:mm\\:ss} + {ttlOff:hh\\:mm\\:ss}    <== dt ");

        prev = e;
      }

      now = DateTimeOffset.Now;

      ttlWrk += now - prev.Key;

      Trace.Write($"  {prev.Value} --> {"n o w",14}   + {now - prev.Key:h\\:mm\\:ss}  ==>  {now:HH:mm:ss}  =?=  "); Trace.WriteLine($"{ttlWrk + ttlIdl + ttlOff:hh\\:mm\\:ss}  =  {ttlWrk:h\\:mm\\:ss} + {ttlIdl:h\\:mm\\:ss} + {ttlOff:hh\\:mm\\:ss}    <== dt ");

      //if (Debugger.IsAttached)      {        Debugger.Break();        Trace.WriteLine($"");        goto again;      }

      dayStartAt = lst.First().Key;
      dayStartEv = lst.First().Value;
    }
  }
}
