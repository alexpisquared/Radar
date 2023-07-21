namespace OpenWeaWpfApp.Views;
public partial class MainPlotViewUsrCtrl : UserControl
{
  public MainPlotViewUsrCtrl()
  {
    InitializeComponent();

  }

  async void OnLoadad(object sender, RoutedEventArgs e)
  {
    try
    {
    DataContext = this.FindParentWindow().DataContext;
      ((PlotViewModel)DataContext).PopulateAll("Silent");  // only lines chart is drawn.
      await Task.Delay(1);
    }
    catch (Exception ex)
    {
      WriteLine(ex.ToString());
      //ex.Pop(_lgr);
    }
  }
  void OnPoplte(object sender, RoutedEventArgs e) => ((PlotViewModel)DataContext).PopulateAll(null);  // only lines chart is drawn.
  void OnPocBin(object sender, RoutedEventArgs e) => new PocBin().Show();
  void OnDragMove(object s, MouseButtonEventArgs e)
  {
    if (e.LeftButton != MouseButtonState.Pressed) return;

    this.FindParentWindow().DragMove();

    e.Handled = true;
  }
}