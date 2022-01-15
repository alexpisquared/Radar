namespace zWeaChartWpfApp;

public partial class MainWindow : Window
{
  MainViewModel ataContext = new();
  public MainWindow()
  {
    InitializeComponent();
    MouseLeftButtonDown += (s, e) => DragMove();
    KeyUp += async (s, e) =>
    {
      switch (e.Key)
      {
        case Key.I:
          Beep.Play();
          Plot1.InvalidatePlot(true); break;
        case Key.R:
          Hand.Play();
          _ = await ataContext.PopulateAsync();
          Plot1.InvalidatePlot(true); break;
        default: Plot1.InvalidatePlot(true); break;
        case Key.Escape: base.OnKeyUp(e); Close(); break;
      }
    };
  }

  void OnClose(object sender, RoutedEventArgs e) => Close();

  async void OnLoadad(object sender, RoutedEventArgs e)
  {
    _ = await ataContext.PopulateAsync();
    DataContext = ataContext;
  }
}
