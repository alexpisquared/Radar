﻿namespace OpenWeaWpfApp;

public partial class MainWindow : Window
{
  public MainWindow()
  {
    InitializeComponent();
    MouseLeftButtonDown += (s, e) => DragMove();
    KeyUp += async (s, e) =>
    {
      switch (e.Key)
      {
        case Key.R: Hand.Play(); _ = await ((MainViewModel)DataContext).PopulateAsync(); goto case Key.I;
        case Key.I: Beep.Play(); plotTR.InvalidatePlot(true); plotBR.InvalidatePlot(true); break;
        case Key.Escape: base.OnKeyUp(e); Close(); break;
        default: break;
      }
    };
  }

  void OnClose(object sender, RoutedEventArgs e) => Close();

  async void OnLoadad(object sender, RoutedEventArgs e) => _ = await ((MainViewModel)DataContext).PopulateAsync();  // only lines chart is drawn.
  async void OnPoplte(object sender, RoutedEventArgs e) => _ = await ((MainViewModel)DataContext).PopulateAsync();  // only lines chart is drawn.
}
