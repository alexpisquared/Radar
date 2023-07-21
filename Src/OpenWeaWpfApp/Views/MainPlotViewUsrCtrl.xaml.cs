namespace OpenWeaWpfApp.Views;
public partial class MainPlotViewUsrCtrl : UserControl
{
  public MainPlotViewUsrCtrl() => InitializeComponent();

  async void OnLoadad(object sender, RoutedEventArgs e)
  {
    try
    {
      ((PlotViewModel)DataContext).PopulateAll("Silent");  // only lines chart is drawn.
      await Task.Delay(1);
    }
    catch (Exception ex)
    {
      WriteLine(ex.ToString());
      //ex.Pop(_lgr);
    }
  }
  async void OnPoplte(object sender, RoutedEventArgs e) => _ = await ((MainViewModel)DataContext).PopulateAsync();  // only lines chart is drawn.
  void OnPocBin(object sender, RoutedEventArgs e) => new PocBin().Show();
}