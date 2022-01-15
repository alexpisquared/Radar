namespace OpenWeather2022.Response;

// NOTE: Generated code may require at least .NET Framework 4.5 or .NET Core/Standard 2.0.
/// <remarks/>
[Serializable()]
[System.ComponentModel.DesignerCategory("code")]
[System.Xml.Serialization.XmlType(AnonymousType = true)]
[System.Xml.Serialization.XmlRoot(Namespace = "", IsNullable = false)]
public partial class current
{

  private currentCity cityField;

  private currentTemperature temperatureField;

  private currentFeels_like feels_likeField;

  private currentHumidity humidityField;

  private currentPressure pressureField;

  private currentWind windField;

  private currentClouds cloudsField;

  private currentVisibility visibilityField;

  private currentPrecipitation precipitationField;

  private currentWeather weatherField;

  private currentLastupdate lastupdateField;

  /// <remarks/>
  public currentCity city
  {
    get => cityField;
    set => cityField = value;
  }

  /// <remarks/>
  public currentTemperature temperature
  {
    get => temperatureField;
    set => temperatureField = value;
  }

  /// <remarks/>
  public currentFeels_like feels_like
  {
    get => feels_likeField;
    set => feels_likeField = value;
  }

  /// <remarks/>
  public currentHumidity humidity
  {
    get => humidityField;
    set => humidityField = value;
  }

  /// <remarks/>
  public currentPressure pressure
  {
    get => pressureField;
    set => pressureField = value;
  }

  /// <remarks/>
  public currentWind wind
  {
    get => windField;
    set => windField = value;
  }

  /// <remarks/>
  public currentClouds clouds
  {
    get => cloudsField;
    set => cloudsField = value;
  }

  /// <remarks/>
  public currentVisibility visibility
  {
    get => visibilityField;
    set => visibilityField = value;
  }

  /// <remarks/>
  public currentPrecipitation precipitation
  {
    get => precipitationField;
    set => precipitationField = value;
  }

  /// <remarks/>
  public currentWeather weather
  {
    get => weatherField;
    set => weatherField = value;
  }

  /// <remarks/>
  public currentLastupdate lastupdate
  {
    get => lastupdateField;
    set => lastupdateField = value;
  }
}

/// <remarks/>
[Serializable()]
[System.ComponentModel.DesignerCategory("code")]
[System.Xml.Serialization.XmlType(AnonymousType = true)]
public partial class currentCity
{

  private currentCityCoord coordField;

  private string countryField;

  private short timezoneField;

  private currentCitySun sunField;

  private uint idField;

  private string nameField;

  /// <remarks/>
  public currentCityCoord coord
  {
    get => coordField;
    set => coordField = value;
  }

  /// <remarks/>
  public string country
  {
    get => countryField;
    set => countryField = value;
  }

  /// <remarks/>
  public short timezone
  {
    get => timezoneField;
    set => timezoneField = value;
  }

  /// <remarks/>
  public currentCitySun sun
  {
    get => sunField;
    set => sunField = value;
  }

  /// <remarks/>
  [System.Xml.Serialization.XmlAttribute()]
  public uint id
  {
    get => idField;
    set => idField = value;
  }

  /// <remarks/>
  [System.Xml.Serialization.XmlAttribute()]
  public string name
  {
    get => nameField;
    set => nameField = value;
  }
}

/// <remarks/>
[Serializable()]
[System.ComponentModel.DesignerCategory("code")]
[System.Xml.Serialization.XmlType(AnonymousType = true)]
public partial class currentCityCoord
{

  private decimal lonField;

  private decimal latField;

  /// <remarks/>
  [System.Xml.Serialization.XmlAttribute()]
  public decimal lon
  {
    get => lonField;
    set => lonField = value;
  }

  /// <remarks/>
  [System.Xml.Serialization.XmlAttribute()]
  public decimal lat
  {
    get => latField;
    set => latField = value;
  }
}

/// <remarks/>
[Serializable()]
[System.ComponentModel.DesignerCategory("code")]
[System.Xml.Serialization.XmlType(AnonymousType = true)]
public partial class currentCitySun
{

  private DateTime riseField;

  private DateTime setField;

  /// <remarks/>
  [System.Xml.Serialization.XmlAttribute()]
  public DateTime rise
  {
    get => riseField;
    set => riseField = value;
  }

  /// <remarks/>
  [System.Xml.Serialization.XmlAttribute()]
  public DateTime set
  {
    get => setField;
    set => setField = value;
  }
}

/// <remarks/>
[Serializable()]
[System.ComponentModel.DesignerCategory("code")]
[System.Xml.Serialization.XmlType(AnonymousType = true)]
public partial class currentTemperature
{

  private decimal valueField;

  private decimal minField;

  private decimal maxField;

  private string unitField;

  /// <remarks/>
  [System.Xml.Serialization.XmlAttribute()]
  public decimal value
  {
    get => valueField;
    set => valueField = value;
  }

  /// <remarks/>
  [System.Xml.Serialization.XmlAttribute()]
  public decimal min
  {
    get => minField;
    set => minField = value;
  }

  /// <remarks/>
  [System.Xml.Serialization.XmlAttribute()]
  public decimal max
  {
    get => maxField;
    set => maxField = value;
  }

  /// <remarks/>
  [System.Xml.Serialization.XmlAttribute()]
  public string unit
  {
    get => unitField;
    set => unitField = value;
  }
}

/// <remarks/>
[Serializable()]
[System.ComponentModel.DesignerCategory("code")]
[System.Xml.Serialization.XmlType(AnonymousType = true)]
public partial class currentFeels_like
{

  private decimal valueField;

  private string unitField;

  /// <remarks/>
  [System.Xml.Serialization.XmlAttribute()]
  public decimal value
  {
    get => valueField;
    set => valueField = value;
  }

  /// <remarks/>
  [System.Xml.Serialization.XmlAttribute()]
  public string unit
  {
    get => unitField;
    set => unitField = value;
  }
}

/// <remarks/>
[Serializable()]
[System.ComponentModel.DesignerCategory("code")]
[System.Xml.Serialization.XmlType(AnonymousType = true)]
public partial class currentHumidity
{

  private byte valueField;

  private string unitField;

  /// <remarks/>
  [System.Xml.Serialization.XmlAttribute()]
  public byte value
  {
    get => valueField;
    set => valueField = value;
  }

  /// <remarks/>
  [System.Xml.Serialization.XmlAttribute()]
  public string unit
  {
    get => unitField;
    set => unitField = value;
  }
}

/// <remarks/>
[Serializable()]
[System.ComponentModel.DesignerCategory("code")]
[System.Xml.Serialization.XmlType(AnonymousType = true)]
public partial class currentPressure
{

  private ushort valueField;

  private string unitField;

  /// <remarks/>
  [System.Xml.Serialization.XmlAttribute()]
  public ushort value
  {
    get => valueField;
    set => valueField = value;
  }

  /// <remarks/>
  [System.Xml.Serialization.XmlAttribute()]
  public string unit
  {
    get => unitField;
    set => unitField = value;
  }
}

/// <remarks/>
[Serializable()]
[System.ComponentModel.DesignerCategory("code")]
[System.Xml.Serialization.XmlType(AnonymousType = true)]
public partial class currentWind
{

  private currentWindSpeed speedField;

  private currentWindGusts gustsField;

  private currentWindDirection directionField;

  /// <remarks/>
  public currentWindSpeed speed
  {
    get => speedField;
    set => speedField = value;
  }

  /// <remarks/>
  public currentWindGusts gusts
  {
    get => gustsField;
    set => gustsField = value;
  }

  /// <remarks/>
  public currentWindDirection direction
  {
    get => directionField;
    set => directionField = value;
  }
}

/// <remarks/>
[Serializable()]
[System.ComponentModel.DesignerCategory("code")]
[System.Xml.Serialization.XmlType(AnonymousType = true)]
public partial class currentWindSpeed
{

  private decimal valueField;

  private string unitField;

  private string nameField;

  /// <remarks/>
  [System.Xml.Serialization.XmlAttribute()]
  public decimal value
  {
    get => valueField;
    set => valueField = value;
  }

  /// <remarks/>
  [System.Xml.Serialization.XmlAttribute()]
  public string unit
  {
    get => unitField;
    set => unitField = value;
  }

  /// <remarks/>
  [System.Xml.Serialization.XmlAttribute()]
  public string name
  {
    get => nameField;
    set => nameField = value;
  }
}

/// <remarks/>
[Serializable()]
[System.ComponentModel.DesignerCategory("code")]
[System.Xml.Serialization.XmlType(AnonymousType = true)]
public partial class currentWindGusts
{

  private decimal valueField;

  /// <remarks/>
  [System.Xml.Serialization.XmlAttribute()]
  public decimal value
  {
    get => valueField;
    set => valueField = value;
  }
}

/// <remarks/>
[Serializable()]
[System.ComponentModel.DesignerCategory("code")]
[System.Xml.Serialization.XmlType(AnonymousType = true)]
public partial class currentWindDirection
{

  private ushort valueField;

  private string codeField;

  private string nameField;

  /// <remarks/>
  [System.Xml.Serialization.XmlAttribute()]
  public ushort value
  {
    get => valueField;
    set => valueField = value;
  }

  /// <remarks/>
  [System.Xml.Serialization.XmlAttribute()]
  public string code
  {
    get => codeField;
    set => codeField = value;
  }

  /// <remarks/>
  [System.Xml.Serialization.XmlAttribute()]
  public string name
  {
    get => nameField;
    set => nameField = value;
  }
}

/// <remarks/>
[Serializable()]
[System.ComponentModel.DesignerCategory("code")]
[System.Xml.Serialization.XmlType(AnonymousType = true)]
public partial class currentClouds
{

  private byte valueField;

  private string nameField;

  /// <remarks/>
  [System.Xml.Serialization.XmlAttribute()]
  public byte value
  {
    get => valueField;
    set => valueField = value;
  }

  /// <remarks/>
  [System.Xml.Serialization.XmlAttribute()]
  public string name
  {
    get => nameField;
    set => nameField = value;
  }
}

/// <remarks/>
[Serializable()]
[System.ComponentModel.DesignerCategory("code")]
[System.Xml.Serialization.XmlType(AnonymousType = true)]
public partial class currentVisibility
{

  private ushort valueField;

  /// <remarks/>
  [System.Xml.Serialization.XmlAttribute()]
  public ushort value
  {
    get => valueField;
    set => valueField = value;
  }
}

/// <remarks/>
[Serializable()]
[System.ComponentModel.DesignerCategory("code")]
[System.Xml.Serialization.XmlType(AnonymousType = true)]
public partial class currentPrecipitation
{

  private string modeField;

  /// <remarks/>
  [System.Xml.Serialization.XmlAttribute()]
  public string mode
  {
    get => modeField;
    set => modeField = value;
  }
}

/// <remarks/>
[Serializable()]
[System.ComponentModel.DesignerCategory("code")]
[System.Xml.Serialization.XmlType(AnonymousType = true)]
public partial class currentWeather
{

  private ushort numberField;

  private string valueField;

  private string iconField;

  /// <remarks/>
  [System.Xml.Serialization.XmlAttribute()]
  public ushort number
  {
    get => numberField;
    set => numberField = value;
  }

  /// <remarks/>
  [System.Xml.Serialization.XmlAttribute()]
  public string value
  {
    get => valueField;
    set => valueField = value;
  }

  /// <remarks/>
  [System.Xml.Serialization.XmlAttribute()]
  public string icon
  {
    get => iconField;
    set => iconField = value;
  }
}

/// <remarks/>
[Serializable()]
[System.ComponentModel.DesignerCategory("code")]
[System.Xml.Serialization.XmlType(AnonymousType = true)]
public partial class currentLastupdate
{

  private DateTime valueField;

  /// <remarks/>
  [System.Xml.Serialization.XmlAttribute()]
  public DateTime value
  {
    get => valueField;
    set => valueField = value;
  }
}
