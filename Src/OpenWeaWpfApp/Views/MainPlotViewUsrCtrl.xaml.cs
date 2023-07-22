namespace OpenWeaWpfApp.Views;
public partial class MainPlotViewUsrCtrl : UserControl
{
  public MainPlotViewUsrCtrl() => InitializeComponent();

  void OnLoadad(object sender, RoutedEventArgs e)
  {
    if (DesignerProperties.GetIsInDesignMode(this)) return; //tu: design mode

    try
    {
      // Assign DataContext to the viewmodel registered in AppStartHelper.cs OpenWeaWpfApp.AppStartHelper.InitOpenWeaServices(IServiceCollection services) method.
      dynamic ac = Application.Current;
      var sp = ac.ServiceProvider;
      DataContext = sp.GetService(typeof(PlotViewModel));
      
      ((PlotViewModel?)DataContext)?.PopulateAll("Silent");  // only lines chart is drawn.
    }
    catch (Exception ex)    {      ex.Pop();    }
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