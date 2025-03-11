using System.Diagnostics;
using System.Windows;

namespace OutlookCrashMonitor;
public partial class App : Application
{
  long started = Stopwatch.GetTimestamp();
  protected override void OnExit(ExitEventArgs e)
  {
    base.OnExit(e);
    System.IO.File.AppendAllText(@"C:\temp\temp.txt", $"{DateTime.Now:HH:mm:ss}  {Stopwatch.GetElapsedTime(started):mmss} ■\n");
  }
}

