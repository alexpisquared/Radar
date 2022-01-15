namespace OpenWeather2022.Response;

public partial record RootobjectFrc5Day3Hr
{
  public string cod { get; set; }
  public int message { get; set; }
  public int cnt { get; set; }
  public ListD5H3[] list { get; set; }
  public City city { get; set; }
}

public partial record City
{
  public int id { get; set; }
  public string name { get; set; }
  public Coord coord { get; set; }
  public string country { get; set; }
  public int population { get; set; }
  public int timezone { get; set; }
  public int sunrise { get; set; }
  public int sunset { get; set; }
}

public partial record ListD5H3
{
  public int dt { get; set; }
  public int sunrise { get; set; }
  public int sunset { get; set; }
  public Main main { get; set; }
  public Weather[] weather { get; set; }
  public Clouds clouds { get; set; }
  public Wind wind { get; set; }
  public int visibility { get; set; }
  public float pop { get; set; }
  public Sys sys { get; set; }
  public string dt_txt { get; set; }
  public Snow snow { get; set; }
}

public partial record Main
{
  //public float temp { get; set; }
  //public float feels_like { get; set; }
  //public float temp_min { get; set; }
  //public float temp_max { get; set; }
  //public int pressure { get; set; }
  public int sea_level { get; set; }
  public int grnd_level { get; set; }
  //public int humidity { get; set; }
  public float temp_kf { get; set; }
}

public partial record Sys
{
  public string pod { get; set; }
}

public partial record Snow
{
  public float _3h { get; set; }
}
