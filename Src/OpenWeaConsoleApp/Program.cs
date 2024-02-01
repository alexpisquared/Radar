using OpenWeather2022;
using StandardLib.Helpers;
using Console = Colorful.Console;
using System.Drawing;
using XSD.CLS;

  var lattitude0 = +43.837;
  var lontitude0 = -79.496;


var siteList = XmlFileSerializer.Load<OpenWeaConsoleApp.siteList>(@"C:\g\Radar\Src\OpenWeaWpfApp\JsonResults\SiteList.xml");

int i = 0;
var sds = new List<siteData>();
foreach (var siteCode in siteList.site)
{
  var sd = await OpenWea.GetEnvtCa(siteCode.code);

  if (sd is null || sd.location is null || sd.currentConditions is null) { continue; }

  Console.Write($"{++i,3} {sd.location.province.code} {siteCode.nameEn,-32} ", Color.Yellow);

  if (!(double.TryParse(sd.location.name.lat.Replace("N", ""), out var lattitudeL) && double.TryParse(sd.location.name.lon.Replace("W", ""), out var lontitudeL))) { Console.WriteLine($"  "); continue; }
  if (!(double.TryParse(sd.currentConditions?.station?.lat.Replace("N", ""), out var lattitudeS) && double.TryParse(sd.currentConditions?.station?.lon.Replace("W", ""), out var lontitudeS))) { Console.WriteLine($"  "); continue; }

  sd.DistanceLocation = Math.Sqrt(Math.Pow(lattitudeL - lattitude0, 2) + Math.Pow(lontitudeL - lontitude0, 2));
  sd.DistanceStation  = Math.Sqrt(Math.Pow(lattitudeS - lattitude0, 2) + Math.Pow(lontitudeS - lontitude0, 2));

  Console.Write($" {sd.DistanceStation,5:N1}", Color.Blue);
  Console.Write($" {sd.currentConditions?.station,-145}  ", Color.White);
  
  Console.Write($" {sd.DistanceLocation,5:N1}", Color.Blue);
  Console.Write($" {sd.location}  ", Color.White);

  Console.Write("\n");

  sds.Add(sd);
}

foreach (var sd in sds.OrderByDescending(r=> r.DistanceStation))
{
  if (sd is null || sd.location is null || sd.currentConditions is null) { continue; }

  Console.Write($"{++i,3} {sd.location.province.code}  ", Color.Yellow);

  Console.Write($" {sd.DistanceStation,5:N1}", Color.Blue);
  Console.Write($" {sd.currentConditions?.station,-145}  ", Color.White);

  Console.Write($" {sd.DistanceLocation,5:N1}", Color.Blue);
  Console.Write($" {sd.location}  ", Color.White);

  Console.Write("\n");

  sds.Add(sd);
}



Console.WriteLine("End\a", Color.Cyan);
