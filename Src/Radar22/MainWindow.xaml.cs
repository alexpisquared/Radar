namespace Radar22;

public partial class MainWindow
{
  public MainWindow()
  {
    InitializeComponent();
    MouseLeftButtonDown += (s, e) => DragMove();
    KeyUp += async (s, e) =>
    {
      switch (e.Key)
      {
        case Key.F1: Hand.Play(); await Task.Yield(); break;
        //case Key.R: Hand.Play(); _ = await ((MainPlotOldVM)DataContext).PopulateAsync(); goto case Key.I;
        //case Key.I: Beep.Play(); plotBR.InvalidatePlot(true); break;
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
      Left = 600;
      Top = -1400;
    }
    else if (Environment.UserDomainName != "RAZER1")
    {
      //WindowState = WindowState.Normal;
      //WindowStartupLocation = WindowStartupLocation.Manual;
      Left = 68;
      Top = 10;
      //Width = 3750;
      //Height = 1080;
    }
  }
}
