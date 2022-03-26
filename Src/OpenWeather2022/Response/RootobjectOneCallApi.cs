namespace OpenWeather2022.Response;

public partial record RootobjectOneCallApi
{
  public float lat { get; set; }
  public float lon { get; set; }
  public string timezone { get; set; } = default!;
  public int timezone_offset { get; set; }
  public Current current { get; set; } = default!;
  public Minutely[] minutely { get; set; } = default!;
  public Hourly[] hourly { get; set; } = default!;
  public Daily[] daily { get; set; } = default!;
}

public partial record Current
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
  public float wind_speed { get; set; } = 111; // m/s
  public int wind_deg { get; set; }
  public float wind_gust { get; set; }
  public Weather[] weather { get; set; }= default!;
  public Alert[] alerts { get; set; }   = default!;
  public Snow snow { get; set; } = default!;
}

public partial record Minutely
{
  public int dt { get; set; }
  public float precipitation { get; set; }
}

public partial record Hourly
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
  public Weather[] weather { get; set; } = default!;
  public float pop { get; set; }
  public Snow snow { get; set; }= default!;
  public Rain rain { get; set; } = default!;
}

public partial record Snow
{
  public float _1h { get; set; }
}

public partial record Rain
{
  public float _1h { get; set; }
}

public partial record Daily
{
  public int dt { get; set; }
  public int sunrise { get; set; }
  public int sunset { get; set; }
  public int moonrise { get; set; }
  public int moonset { get; set; }
  public float moon_phase { get; set; }
  public Temp temp { get; set; } = default!;
  public Feels_Like feels_like { get; set; } = default!;
  public int pressure { get; set; }
  public int humidity { get; set; }
  public float dew_point { get; set; }
  public float wind_speed { get; set; }
  public int wind_deg { get; set; }
  public float wind_gust { get; set; }
  public Weather[] weather { get; set; } = default!;
  public int clouds { get; set; }
  public float pop { get; set; }
  public float uvi { get; set; }
  public float rain { get; set; }
  public float snow { get; set; }
}

public partial record Temp
{
  public float day { get; set; }
  public float min { get; set; }
  public float max { get; set; }
  public float night { get; set; }
  public float eve { get; set; }
  public float morn { get; set; }
}

public partial record Feels_Like
{
  public float day { get; set; }
  public float night { get; set; }
  public float eve { get; set; }
  public float morn { get; set; }
}

public partial record Alert
{
  public string sender_name { get; set; }= default!;
  public string _event { get; set; } = default!;
  public int start { get; set; }
  public int end { get; set; }
  public string description { get; set; }= default!;
  public string[] tags { get; set; } = default!;
}
