namespace OpenWeaWpfApp;
public partial class MainPlotViewWin // : WindowBase
{
  readonly SpeechSynth _syn;
  readonly ILogger _lgr;
  readonly IBpr _bpr;

  public MainPlotViewWin(ILogger lgr, IBpr bpr, SpeechSynth snt) : base(lgr)
  {
    InitializeComponent();

    _lgr = lgr;
    _bpr = bpr;
    _syn = snt;

    KeyUp += async (s, e) =>
    {
      switch (e.Key)
      {
        case Key.C: _bpr.Error(); ((PlotViewModel)DataContext).ClearPlot(); break;
        case Key.I: _bpr.Error(); plotBR.InvalidatePlot(true); break;
        case Key.J: _bpr.Error(); plotBR.InvalidatePlot(false); break;
        case Key.R: _bpr.Error(); ((PlotViewModel)DataContext).PopulateAll(null); goto case Key.I;
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
      ex.Pop(_lgr);
    }
  }
  void OnClose(object sender, RoutedEventArgs e) => Close();
  void OnPoplte(object sender, RoutedEventArgs e) => ((PlotViewModel)DataContext).PopulateAll(null);  // only lines chart is drawn.
  void OnShowPocBin(object sender, RoutedEventArgs e) => new PocBin().Show();
  void OnActivated(object sender, EventArgs e) => radar1.IsPlaying = true;  /*_syn.SpeakFAF("Activated", volumePercent: 5);*/
  void OnDeActivtd(object sender, EventArgs e) => radar1.IsPlaying = false; /*_syn.SpeakFAF("Deactivated", volumePercent: 5);*/
}