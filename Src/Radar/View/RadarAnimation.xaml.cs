using System;
using System.Threading.Tasks;
using System.Windows;

namespace Radar
{
  public partial class RadarAnimation : AAV.WPF.Base.WindowBase
  {
    public RadarAnimation(double alarmThreshold = .05)
    {
      InitializeComponent();

      if (Environment.GetCommandLineArgs().Length > 1 && Environment.GetCommandLineArgs()[1] == "ModeH")
        Hide();

      Loaded += async (s, e) =>
      {
        if (Environment.GetCommandLineArgs().Length > 1 && Environment.GetCommandLineArgs()[1] == "ShowIfOnOrComingSoon")
        {
          await Task.Delay(180000);

          Close();
          System.Diagnostics.Trace.WriteLine($"{DateTime.Now:yy.MM.dd HH:mm:ss.f} => ::>Application.Current.Shutdown();g");
          Application.Current.Shutdown();
        }
      };

      ruc1.AlarmThreshold = alarmThreshold;
      ruc1.Focus();
    }
  }
}