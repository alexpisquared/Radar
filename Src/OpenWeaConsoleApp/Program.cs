using System.Drawing;
using OpenWeaSvc;
using Pastel; //tu: https://www.nuget.org/packages/Pastel
using StandardLib.Helpers;
using XSD.CLS;

var lattitude0 = 43.837;
var lontitude0 = 79.496;

var siteList = XmlFileSerializer.Load<OpenWeaConsoleApp.siteList>(@"C:\g\Radar\Src\OpenWeaWpfApp\weather.gc.ca\SiteList.xml");

var i = 0;
var sds = new List<siteData>();
foreach (var siteCode in siteList.site)
{
  var sd = await OpenWea.GetEnvtCa(siteCode.code);

  if (sd is null || sd.location is null || sd.currentConditions is null) { continue; }


  if (!(double.TryParse(sd.location.name.lat.Replace("N", ""), out var lattitudeL) && double.TryParse(sd.location.name.lon.Replace("W", ""), out var lontitudeL))) { Console.WriteLine($"  "); continue; }

  if (!(double.TryParse(sd.currentConditions?.station?.lat.Replace("N", ""), out var lattitudeS) && double.TryParse(sd.currentConditions?.station?.lon.Replace("W", ""), out var lontitudeS))) { Console.WriteLine($"  "); continue; }

  sd.DistanceLocation = Math.Sqrt(Math.Pow(lattitudeL - lattitude0, 2) + Math.Pow(lontitudeL - lontitude0, 2));

  sd.DistanceLocation = CalculateDistance(lattitudeL, lontitudeL, lattitude0, lontitude0);

  sd.DistanceStation = CalculateDistance(lattitudeS, lontitudeS, lattitude0, lontitude0); // Math.Sqrt(Math.Pow(lattitudeS - lattitude0, 2) + Math.Pow(lontitudeS - lontitude0, 2));

  i = DrawLine(i, sd);

  sds.Add(sd);
}

Console.Write("\n");
i = 0;
foreach (var sd in sds.OrderByDescending(r => r.DistanceStation))
{
  if (sd is null || sd.location is null || sd.currentConditions is null) { continue; }

  i = DrawLine(i, sd);

  sds.Add(sd);
}

Console.WriteLine("End\a".Pastel(Color.Cyan));

static double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
{
  const double EarthRadiusKm = 6371;
  var dLat = ToRadians(lat2 - lat1);
  var dLon = ToRadians(lon2 - lon1);

  var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
          Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
          Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

  var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

  var distance = EarthRadiusKm * c;
  return distance;
}

static double ToRadians(double degrees)
{
  return degrees * Math.PI / 180;
}

static int DrawLine(int i, siteData? sd)
{
  Console.Write($"{++i,3} ".Pastel(Color.Brown));
  Console.Write($"{sd.DistanceStation,9:N1}".Pastel(Color.LightBlue).PastelBg("840"));
  Console.Write($" {sd.currentConditions?.station,-145}  ".Pastel(Color.DarkCyan).PastelBg("102030"));
  Console.Write($"{sd.DistanceLocation,9:N1}".Pastel(Color.Blue).PastelBg("888"));
  Console.Write($" {sd.location}  ".Pastel(Color.Gray).PastelBg("555"));
  Console.Write("\n");
  return i;
}