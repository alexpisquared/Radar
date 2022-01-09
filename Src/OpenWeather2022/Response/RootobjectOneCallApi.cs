namespace OpenWeather2022.Response;

public record RootobjectOneCallApi
{
  public float lat { get; set; }
  public float lon { get; set; }
  public string timezone { get; set; }
  public int timezone_offset { get; set; }
  public Current current { get; set; }
  public Minutely[] minutely { get; set; }
  public Hourly[] hourly { get; set; }
  public Daily[] daily { get; set; }
}

public record Current
{
  public int dt { get; set; }
  public int sunrise { get; set; }
  public int sunset { get; set; }
  public float temp { get; set; }
  public float feels_like { get; set; }
  public int pressure { get; set; }
  public int humidity { get; set; }
  public float dew_point { get; set; }
  public float uvi { get; set; }
  public int clouds { get; set; }
  public int visibility { get; set; }
  public float wind_speed { get; set; }
  public int wind_deg { get; set; }
  public float wind_gust { get; set; }
  public Weather[] weather { get; set; }
}

public record Minutely
{
  public int dt { get; set; }
  public int precipitation { get; set; }
}

public record Hourly
{
  public int dt { get; set; }
  public float temp { get; set; }
  public float feels_like { get; set; }
  public int pressure { get; set; }
  public int humidity { get; set; }
  public float dew_point { get; set; }
  public float uvi { get; set; }
  public int clouds { get; set; }
  public int visibility { get; set; }
  public float wind_speed { get; set; }
  public int wind_deg { get; set; }
  public float wind_gust { get; set; }
  public Weather[] weather { get; set; }
  public float pop { get; set; }
  public Snow snow { get; set; }
  public Rain rain { get; set; }
}

public record Snow
{
  public float _1h { get; set; }
}

public record Rain
{
  public float _1h { get; set; }
}

public record Daily
{
  public int dt { get; set; }
  public int sunrise { get; set; }
  public int sunset { get; set; }
  public int moonrise { get; set; }
  public int moonset { get; set; }
  public float moon_phase { get; set; }
  public Temp temp { get; set; }
  public Feels_Like feels_like { get; set; }
  public int pressure { get; set; }
  public int humidity { get; set; }
  public float dew_point { get; set; }
  public float wind_speed { get; set; }
  public int wind_deg { get; set; }
  public float wind_gust { get; set; }
  public Weather[] weather { get; set; }
  public int clouds { get; set; }
  public float pop { get; set; }
  public float uvi { get; set; }
  public float rain { get; set; }
  public float snow { get; set; }
}

public record Temp
{
  public float day { get; set; }
  public float min { get; set; }
  public float max { get; set; }
  public float night { get; set; }
  public float eve { get; set; }
  public float morn { get; set; }
}

public record Feels_Like
{
  public float day { get; set; }
  public float night { get; set; }
  public float eve { get; set; }
  public float morn { get; set; }
}