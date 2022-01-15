using System.Windows;

namespace zWeaChartWpfApp;

public partial class MainWindow : Window
{
  public MainWindow() => InitializeComponent();

  void OnClose(object sender, RoutedEventArgs e) => Close();

  async void OnLoadad(object sender, RoutedEventArgs e)
  {
    MainViewModel ataContext = new();
    _ = await ataContext.PopulateAsync();
    DataContext = ataContext;
    Plot1.InvalidatePlot(true);
  }
}
