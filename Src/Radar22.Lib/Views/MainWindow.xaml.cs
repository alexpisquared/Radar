using AmbienceLib;

namespace xEnvtCanRadar;

public partial class MainWindow : Window
{
  readonly Bpr bpr = new();
  public MainWindow()
  {
    InitializeComponent();
    MouseLeftButtonDown += (s, e) => DragMove();
    KeyUp += async (s, e) =>
    {
      switch (e.Key)
      {
        case Key.F1: bpr.Error(); await Task.Yield(); break;
        //case Key.R: bpr.Error(); _ = await ((MainViewModel)DataContext).PopulateAsync(); goto case Key.I;
        //case Key.I: bpr.Error(); plotBR.InvalidatePlot(true); break;
        case Key.Escape: base.OnKeyUp(e); e.Handled = true; Close(); break;
        default: break;
      }
    };

    if (Debugger.IsAttached)
    {
      Topmost = true;
      if (Environment.UserDomainName != "RAZER1")
      {
        //WindowState = WindowState.Normal;
        //WindowStartupLocation = WindowStartupLocation.Manual;
        Left = 100;
        Top = 100;
        //Width = 3750;
        //Height = 1080;
      }
      //WindowState = WindowState.Normal;
      //WindowStartupLocation = WindowStartupLocation.Manual;
      //Left = 1920;
      //Top = -1120;
      //Width = 3000;
      //Height = 1080;
    }
    else if (Environment.UserDomainName == "RAZER1")
    {
      Left = 3100;
      Top = -1200;
    }
    else if (Environment.UserDomainName != "RAZER1")
    {
      //WindowState = WindowState.Normal;
      //WindowStartupLocation = WindowStartupLocation.Manual;
      Left = 100;
      Top = 100;
      //Width = 3750;
      //Height = 1080;
    }
  }
}