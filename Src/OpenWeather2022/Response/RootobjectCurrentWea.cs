namespace OpenWeather2022.Response;

public record RootobjectCurrentWea
{
  public Coord coord { get; set; }
  public Weather[] weather { get; set; }
  public string _base { get; set; }
  public Main main { get; set; }
  public int visibility { get; set; }
  public Wind wind { get; set; }
  public Clouds clouds { get; set; }
  public int dt { get; set; }
  public Sys sys { get; set; }
  public int timezone { get; set; }
  public int id { get; set; }
  public string name { get; set; }
  public int cod { get; set; }
}

public record Main
{
  public float temp { get; set; }
  public float feels_like { get; set; }
  public float temp_min { get; set; }
  public float temp_max { get; set; }
  public int pressure { get; set; }
  public int humidity { get; set; }
}

public record Wind
{
  public float speed { get; set; }
  public int deg { get; set; }
  public float gust { get; set; }
}

public record Clouds
{
  public int all { get; set; }
}

public record Sys
{
  public int type { get; set; }
  public int id { get; set; }
  public string country { get; set; }
  public int sunrise { get; set; }
  public int sunset { get; set; }
}
