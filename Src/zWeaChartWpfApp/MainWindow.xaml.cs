using System.Windows;

namespace zWeaChartWpfApp;

public partial class MainWindow : Window
{
  public MainWindow()
  {
    InitializeComponent();
  }

  void OnClose(object sender, RoutedEventArgs e) => Close();

  async void OnLoadad(object sender, RoutedEventArgs e)
  {
    //await Task.Delay(100);
    var ataContext = new MainViewModel();
    bool rv = await ataContext.PopulateAsync();
    var rrv = rv;
    DataContext = ataContext;
  }
}
