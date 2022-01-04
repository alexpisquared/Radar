namespace Radar.OpenWeatherResponse;


public class RootobjectTimeMachin
{
  public float lat { get; set; }
  public float lon { get; set; }
  public string timezone { get; set; }
  public int timezone_offset { get; set; }
  public Current current { get; set; }
  public Hourly[] hourly { get; set; }
}

public class Current
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
  public Weather1[] weather { get; set; }
}

public class Weather1
{
  public int id { get; set; }
  public string main { get; set; }
  public string description { get; set; }
  public string icon { get; set; }
}
