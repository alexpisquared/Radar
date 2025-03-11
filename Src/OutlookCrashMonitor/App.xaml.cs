using System.Windows;

namespace OutlookCrashMonitor;
public partial class App : Application
{
  protected override void OnExit(ExitEventArgs e)
  {
    base.OnExit(e);
    System.IO.File.AppendAllText(@"C:\temp\temp.txt", $"{DateTime.Now:HH:mm:ss} ■\n");
  }
}

