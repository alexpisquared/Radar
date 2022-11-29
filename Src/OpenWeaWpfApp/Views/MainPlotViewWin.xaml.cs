namespace OpenWeaWpfApp;

public partial class MainPlotViewWin : Window
{
  public MainPlotViewWin()
  {
    InitializeComponent();
    MouseLeftButtonDown += (s, e) => DragMove();
    KeyUp += async (s, e) =>
    {
      switch (e.Key)
      {
        case Key.C: Beep.Play(); ((PlotViewModel)DataContext).ClearPlot(); break;
        case Key.I: Beep.Play(); plotBR.InvalidatePlot(true); break;
        case Key.J: Beep.Play(); plotBR.InvalidatePlot(false); break;
        case Key.R: Hand.Play(); ((PlotViewModel)DataContext).PopulateAll(); goto case Key.I;
        case Key.Escape: base.OnKeyUp(e); e.Handled = true; Close(); break;
        default: await Task.Delay(333); break;
      }
    };

    if (Debugger.IsAttached)
    {
      Topmost = true;
      //WindowState = WindowState.Normal;
      //WindowStartupLocation = WindowStartupLocation.Manual;
      //Left = 1920;
      //Top = -1120;
      //Width = 3000;
      //Height = 1080;
    }
    else if (Environment.UserDomainName == "RAZER1")
    {
      WindowState = WindowState.Normal;
      WindowStartupLocation = WindowStartupLocation.Manual;
      Left = 1920;
      Top = -1120;
      Width = 3000;
      Height = 1080;
    }

    Title = $"OpenWeaWpfApp - {string.Join(',', Environment.GetCommandLineArgs())}";
  }

  async void OnLoadad(object sender, RoutedEventArgs e)
  {
    try
    {
      await Task.Delay(1);

      ((PlotViewModel)DataContext).PopulateAll();  // only lines chart is drawn.
    }
    catch (Exception ex)
    {
      WriteLine($"@@@@@@@@ {ex.Message} \n\t {ex} @@@@@@@@@@");
      if (Debugger.IsAttached) Debugger.Break(); else _ = MessageBox.Show(ex.ToString(), ex.Message, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.ServiceNotification);
    }
  }
  void OnClose(object sender, RoutedEventArgs e) => Close();
  void OnPoplte(object sender, RoutedEventArgs e) =>  ((PlotViewModel)DataContext).PopulateAll();  // only lines chart is drawn.
  void OnShowPocBin(object sender, RoutedEventArgs e) => new PocBin().Show();
}
