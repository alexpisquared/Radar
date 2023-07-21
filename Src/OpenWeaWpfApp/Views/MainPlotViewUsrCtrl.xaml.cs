namespace OpenWeaWpfApp.Views;
public partial class MainPlotViewUsrCtrl : UserControl
{
  public MainPlotViewUsrCtrl() => InitializeComponent();

  async void OnPoplte(object sender, RoutedEventArgs e) => _ = await ((MainViewModel)DataContext).PopulateAsync();  // only lines chart is drawn.
  void OnPocBin(object sender, RoutedEventArgs e) => new PocBin().Show();
}
