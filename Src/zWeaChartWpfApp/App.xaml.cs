using Microsoft.Extensions.Configuration;

namespace zWeaChartWpfApp;

public partial class App : Application
{
  protected override async void OnStartup(StartupEventArgs e)
  {
#if DEBUG_
    var config = new ConfigurationBuilder().AddUserSecrets<App>().Build(); //tu: adhoc usersecrets 
    Trace.WriteLine($"---   WhereAmI: '{config["WhereAmI"]}'       {config["AppSecrets:MagicNumber"]}");

    /* //tu: not storing file in the GitHub:     (https://youtu.be/ASraHYMi808?t=832)
    git update-index --assume-unchanged AppSettings.json
    git update-index --no-assume-unchanged AppSettings.json            */

    var rv = await new OpenWeather2022.OpenWea().ParseJsonToClasses(config["AppSecrets:MagicNumber"]);
    Current.Shutdown();
#endif

    new MainWindow().Show();  

    base.OnStartup(e);
  }
}
