namespace OpenWeaWpfApp;
public partial class PocBin //: Window
{
  readonly Bpr bpr = new();
  public PocBin()
  {
    InitializeComponent();
    MouseLeftButtonDown += (s, e) => DragMove();
    KeyUp += async (s, e) =>
    {
      switch (e.Key)
      {
        case Key.R: bpr.Error(); _ = await ((MainPlotOldVM)DataContext).PopulateAsync(); goto case Key.I;
        case Key.I: bpr.Error(); plotTR.InvalidatePlot(true); plotBR.InvalidatePlot(true); break;
        case Key.Escape: base.OnKeyUp(e); e.Handled = true; Close(); break;
        default: break;
      }
    };

    if (Debugger.IsAttached)
    {
      Topmost = true;
    }
    else
    {
    }

    WindowState = WindowState.Normal;
    WindowStartupLocation = WindowStartupLocation.Manual;
    Left = 1920;
    Top = -1120;
    Width = 3000;
    Height = 1080;
  }

  void OnClose(object sender, RoutedEventArgs e) => Close();

  async void OnLoadad(object sender, RoutedEventArgs e)
  {
    await Task.Delay(1);

    _ = await ((MainPlotOldVM)DataContext).PopulateAsync();  // only lines chart is drawn.
  }

  async void OnPoplte(object sender, RoutedEventArgs e) => _ = await ((MainPlotOldVM)DataContext).PopulateAsync();  // only lines chart is drawn.
}
