using System.Windows;

namespace zWeaChartWpfApp;

public partial class MainWindow : Window
{
  public MainWindow()
  {
    InitializeComponent();
    DataContext = new MainViewModel();
  }


  void OnClose(object sender, RoutedEventArgs e) => Close();
}
