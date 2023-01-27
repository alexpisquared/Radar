using AmbienceLib;

namespace OpenWeaWpfApp;
public partial class MainPlotViewWin : WindowBase
{
  readonly Bpr bpr = new();

  public MainPlotViewWin(ILogger logger) : base(logger)
  {
    InitializeComponent();

    KeyUp += async (s, e) =>
    {
      switch (e.Key)
      {
        case Key.C: bpr.Error(); ((PlotViewModel)DataContext).ClearPlot(); break;
        case Key.I: bpr.Error(); plotBR.InvalidatePlot(true); break;
        case Key.J: bpr.Error(); plotBR.InvalidatePlot(false); break;
        case Key.R: Hand.Play(); ((PlotViewModel)DataContext).PopulateAll(null); goto case Key.I;
        default: await Task.Delay(333); break;
      }
    };

    Topmost = Debugger.IsAttached;

    KeepOpenReason = null; // nothing to hold on to.

    Title = $"OpenWeaWpfApp - {string.Join(',', Environment.GetCommandLineArgs())}";
  }

  async void OnLoadad(object sender, RoutedEventArgs e)
  {
    try
    {
      ((PlotViewModel)DataContext).PopulateAll("Silent");  // only lines chart is drawn.
      await Task.Delay(1);
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
  void OnActivated(object sender, EventArgs e) { radar1.IsPlaying = true; }
  void OnDeActivtd(object sender, EventArgs e) { radar1.IsPlaying = false; }
}