namespace OpenWeaWpfApp;
public partial class MainPlotViewWin : WindowBase
{
  public MainPlotViewWin()
  {
    InitializeComponent();
    //MouseLeftButtonDown += (s, e) => DragMove();
    KeyUp += async (s, e) =>
    {
      switch (e.Key)
      {
        case Key.C: Beep.Play(); ((PlotViewModel)DataContext).ClearPlot(); break;
        case Key.I: Beep.Play(); plotBR.InvalidatePlot(true); break;
        case Key.J: Beep.Play(); plotBR.InvalidatePlot(false); break;
        case Key.R: Hand.Play(); ((PlotViewModel)DataContext).PopulateAll(null); goto case Key.I;
        //case Key.Escape: base.OnKeyUp(e); e.Handled = true; Close(); break;
        default: await Task.Delay(333); break;
      }
    };

    Topmost = Debugger.IsAttached;

    Title = $"OpenWeaWpfApp - {string.Join(',', Environment.GetCommandLineArgs())}";
  }

  async void OnLoadad(object sender, RoutedEventArgs e)
  {
    try
    {
      await Task.Delay(1);

      ((PlotViewModel)DataContext).PopulateAll("Silent");  // only lines chart is drawn.
    }
    catch (Exception ex)
    {
      WriteLine($"■─■─■ {ex.Message} \n\t {ex} ■─■─■");
      if (Debugger.IsAttached) Debugger.Break(); else _ = MessageBox.Show(ex.ToString(), ex.Message, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.ServiceNotification);
    }
  }
  void OnClose(object sender, RoutedEventArgs e) => Close();

  void OnPoplte(object sender, RoutedEventArgs e) =>  ((PlotViewModel)DataContext).PopulateAll(null);  // only lines chart is drawn.
  void OnShowPocBin(object sender, RoutedEventArgs e) => new PocBin().Show();
}
