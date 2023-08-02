namespace OpenWeaWpfApp.Views;
public partial class MainPlotViewUsrCtrl
{
    public MainPlotViewUsrCtrl() => InitializeComponent();



    public bool IsActive { get { return (bool)GetValue(IsActiveProperty); } set { SetValue(IsActiveProperty, value); } }
    public static readonly DependencyProperty IsActiveProperty = DependencyProperty.Register("IsActive", typeof(bool), typeof(MainPlotViewUsrCtrl), new PropertyMetadata(default(bool), OnPropertyChanged));
    static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((MainPlotViewUsrCtrl)d).Visibility = (bool)e.NewValue ? Visibility.Visible : Visibility.Collapsed;

    void OnLoaded(object sender, RoutedEventArgs e)
    {
        if (DesignerProperties.GetIsInDesignMode(this)) return; //tu: design mode

        try
        {
            // Assign DataContext to the viewmodel registered in AppStartHelper.cs OpenWeaWpfApp.AppStartHelper.InitOpenWeaServices(IServiceCollection services) method.
            DataContext = (Application.Current as dynamic).ServiceProvider.GetService(typeof(PlotViewModel));

            ((PlotViewModel?)DataContext)?.PopulateAll("Silent");  // only lines chart is drawn.
        }
        catch (Exception ex) { ex.Pop(); }
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