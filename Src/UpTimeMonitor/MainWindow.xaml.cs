﻿using EventLogLib;
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
    const char _w = '▓', _i = '■', _o = '·', _e = '~';

    public MainWindow()
    {
      InitializeComponent();

      MouseLeftButtonDown += (s, e) => DragMove();
      KeyUp += (s, e) => { switch (e.Key) { default: break; case Key.Escape: base.OnKeyUp(e); Close(); break; } };
      _start = DateTimeOffset.Now;

      OnTick(null, new EventArgs());

#if DEBUG
      new DispatcherTimer(new TimeSpan(0, 0, 0, 2, 500), DispatcherPriority.Background, OnTick, Dispatcher.CurrentDispatcher).Start();
      if (Environment.MachineName == "D21-MJ0AWBEV") /**/ { Top = 1608; Left = 1928; }
      if (Environment.MachineName == "RAZER1")       /**/ { Top = 1600; Left = 10; }
#else
      new DispatcherTimer(new TimeSpan(0, 0, 1, 0, 0), DispatcherPriority.Background, OnTick, Dispatcher.CurrentDispatcher).Start();
#endif

    }

    void OnTick(object? s, EventArgs ea)
    {
      var sw = Stopwatch.StartNew();

      //var dt0 = EventLogHelper.GetDays1rstBootTime(_start.Date);
      var lst = EventLogHelper.GetAllEventsOfInterest(_start.Date, _start.Date.AddDays(0.9999999));
      var ssn = EventLogHelper.CurrentSessionDuration();
      //var idl = EventLogHelper.GetTotalIdleByScrSvr(_start.Date);

      var now = DateTimeOffset.Now;

      again:
      TimeSpan ttlWrk, ttlIdl, ttlOff;
      DateTime dayStartAt;
      EvOfIntFlag dayStartEv;

      var report = GetSplit(lst, now, out ttlWrk, out ttlIdl, out ttlOff, out dayStartAt, out dayStartEv);

      Trace.WriteLine($"{report}\nDay Start {dayStartAt}    {dayStartEv}    {ttlWrk + ttlIdl + ttlOff,5:h\\:mm}  =  {ttlWrk,5:h\\:mm} + {ttlIdl,5:h\\:mm} + {ttlOff,5:h\\:mm}    ");

      Title = $"{ssn.TotalMinutes:N0}";
      tb3.Text = $"{ssn,5:h\\:mm}      {ttlWrk,5:h\\:mm}";

      tb4.Text = report;

      //if (Debugger.IsAttached) { Debugger.Break(); Trace.WriteLine($""); goto again; }

      tb2.Text = $"{sw.ElapsedMilliseconds:N0}";
    }

    string GetSplit(SortedList<DateTime, EvOfIntFlag> lst, DateTimeOffset now, out TimeSpan ttlWrk, out TimeSpan ttlIdl, out TimeSpan ttlOff, out DateTime dayStartAt, out EvOfIntFlag dayStartEv)
    {
      var report = $"{"Event",-14} +   dT     At   Wrk{_w} + Idl{_i} + Off{_o}  Hr         1           2           3\n";

      var it = ' ';
      var dt = TimeSpan.Zero;
      ttlWrk = TimeSpan.Zero;
      ttlIdl = TimeSpan.Zero;
      ttlOff = TimeSpan.Zero;
      var prev = new SortedList<DateTime, EvOfIntFlag> { { _start.Date, EvOfIntFlag.Who_Knows_What } }.First();
      foreach (var ev in lst)
      {
        var er = "";
        if (prev.Value == EvOfIntFlag.Who_Knows_What) // first entry for the day
        {
          dt = ev.Key - _start.Date;
          switch (ev.Value)
          {
            case EvOfIntFlag.ScreenSaverrUp:
            case EvOfIntFlag.ShutAndSleepDn: it = _w; ttlWrk = dt; break;
            case EvOfIntFlag.ScreenSaverrDn: it = _i; ttlIdl = dt; break;
            case EvOfIntFlag.BootAndWakeUps: it = _o; ttlOff = dt; break;
            default: it = _e; er += $"■ 1st day's entry at {ev.Key,5:H:mm} \t {ev.Value}   -----  //todo: 000 "; break;
          }
        }
        else
        {
          dt = ev.Key - prev.Key;
          switch (ev.Value)
          {
            case EvOfIntFlag.ScreenSaverrUp: it = _w; ttlWrk += dt; break;

            case EvOfIntFlag.ShutAndSleepDn:
              switch (prev.Value)
              {
                case EvOfIntFlag.ShutAndSleepDn: // rare but happenned.
                case EvOfIntFlag.BootAndWakeUps:
                case EvOfIntFlag.ScreenSaverrDn: it = _w; ttlWrk += dt; break;
                case EvOfIntFlag.ScreenSaverrUp: it = _i; ttlIdl += dt; break;
                default: it = _e; er += $" //todo: 111 \t"; break;
              }
              break;

            case EvOfIntFlag.ScreenSaverrDn:
              switch (prev.Value)
              {
                case EvOfIntFlag.BootAndWakeUps:
                case EvOfIntFlag.ScreenSaverrUp: it = _i; ttlIdl += dt; break;
                default: it = _e; er += $" //todo: 222 \t"; break;
              }
              break;

            case EvOfIntFlag.BootAndWakeUps:
              switch (prev.Value)
              {
                case EvOfIntFlag.ShutAndSleepDn:
                case EvOfIntFlag.ScreenSaverrUp:
                case EvOfIntFlag.BootAndWakeUps: it = _o; ttlOff += dt; break;
                default: it = _e; er += $" //todo: 333 \t"; break;
              }
              break;

            default: it = _e; er += $"\t {ev.Value}   -----  //todo: 444 \t"; break;
          }
        }

        report += $"{ev.Value}  {dt,5:h\\:mm}  {ev.Key,5:H:mm}  {ttlWrk,5:h\\:mm}  {ttlIdl,5:h\\:mm}  {ttlOff,5:h\\:mm}  {(prev.Value == EvOfIntFlag.Who_Knows_What ? "" : new string(it, (int)(.5 + (dt.TotalMinutes / 5.0))))} {(Math.Abs((ttlWrk + ttlIdl + ttlOff - ev.Key.TimeOfDay).TotalSeconds) > 10 ? " Off by " + (ttlWrk + ttlIdl + ttlOff - ev.Key.TimeOfDay).TotalSeconds.ToString("N0") + "s " : " ")}{er}\n";

        prev = ev;
      }

      dt = now - prev.Key;
      it = _w; ttlWrk += dt; // could be ScrSvr - how to tell.

      report += $"{"n o w",-14} +{dt,5:h\\:mm}  {now,5:H:mm}  {ttlWrk,5:h\\:mm} +{ttlIdl,5:h\\:mm} +{ttlOff,5:h\\:mm}  {(prev.Value == EvOfIntFlag.Who_Knows_What ? "" : new string(it, (int)(.5 + (dt.TotalMinutes / 5.0))))}\n";

      dayStartAt = lst.First().Key;
      dayStartEv = lst.First().Value;

      return report;
    }
  }
}
