
namespace OpenWeaWpfApp;

public partial class MainPlotOldWindow : Window
{
  public MainPlotOldWindow()
  {
    InitializeComponent();
    MouseLeftButtonDown += (s, e) => DragMove();
    KeyUp += async (s, e) =>
    {
      switch (e.Key)
      {
        case Key.R: _ = await ((MainViewModel)DataContext).PopulateAsync(); goto case Key.I;
        case Key.I: plotBR.InvalidatePlot(true); break;
        case Key.Escape: base.OnKeyUp(e); e.Handled = true; Close(); break;
        default: break;
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

  void OnClose(object sender, RoutedEventArgs e) => Close();

  async void OnLoadad(object sender, RoutedEventArgs e)
  {
    try
    {
      await Task.Delay(1);

      SctrPtTPFVgn.TrackerFormatString += "{Tag}"; // Oxyplot ScatterPoint custom Label
      SctrPtTPFMis.TrackerFormatString += "{Tag}"; // Oxyplot ScatterPoint custom Label
      SctrPtTPFPhc.TrackerFormatString += "{Tag}"; // Oxyplot ScatterPoint custom Label

      _ = await ((MainViewModel)DataContext).PopulateAsync();  // only lines chart is drawn.
    }
    catch (Exception ex)
    {
      WriteLine($"■─■─■ {ex.Message} \n\t {ex} ■─■─■");
      if (Debugger.IsAttached) Debugger.Break(); else MessageBox.Show(ex.ToString(), ex.Message, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.ServiceNotification);
    }
  }

  async void OnPoplte(object sender, RoutedEventArgs e) => _ = await ((MainViewModel)DataContext).PopulateAsync();  // only lines chart is drawn.

    void OnPocBin(object sender, RoutedEventArgs e)
    {
      new PocBin().Show();
  }
}
