namespace OpenWeaSvc.Response;

// NOTE: Generated code may require at least .NET Framework 4.5 or .NET Core/Standard 2.0.
/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
[System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
public partial class SiteDataEnvCa2026
{

  private string? licenseField;

  private siteDataDateTime[]? dateTimeField;

  private siteDataLocation? locationField;

  private object? warningsField;

  private siteDataCurrentConditions? currentConditionsField;

  private siteDataForecastGroup? forecastGroupField;

  private siteDataHourlyForecastGroup? hourlyForecastGroupField;

  private siteDataRiseSet? riseSetField;

  /// <remarks/>
  public string license
  {
    get => licenseField; set => licenseField = value;
  }

  /// <remarks/>
  [System.Xml.Serialization.XmlElementAttribute("dateTime")]
  public siteDataDateTime[] dateTime
  {
    get => dateTimeField; set => dateTimeField = value;
  }

  /// <remarks/>
  public siteDataLocation location
  {
    get => locationField; set => locationField = value;
  }

  /// <remarks/>
  public object warnings
  {
    get => warningsField; set => warningsField = value;
  }

  /// <remarks/>
  public siteDataCurrentConditions currentConditions
  {
    get => currentConditionsField; set => currentConditionsField = value;
  }

  /// <remarks/>
  public siteDataForecastGroup forecastGroup
  {
    get => forecastGroupField; set => forecastGroupField = value;
  }

  /// <remarks/>
  public siteDataHourlyForecastGroup hourlyForecastGroup
  {
    get => hourlyForecastGroupField; set => hourlyForecastGroupField = value;
  }

  /// <remarks/>
  public siteDataRiseSet riseSet
  {
    get => riseSetField; set => riseSetField = value;
  }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class siteDataDateTime
{

  private ushort yearField;

  private siteDataDateTimeMonth? monthField;

  private siteDataDateTimeDay? dayField;

  private byte hourField;

  private byte minuteField;

  private ulong timeStampField;

  private string? textSummaryField;

  private string? nameField;

  private string? zoneField;

  private sbyte uTCOffsetField;

  /// <remarks/>
  public ushort year
  {
    get => yearField; set => yearField = value;
  }

  /// <remarks/>
  public siteDataDateTimeMonth month
  {
    get => monthField; set => monthField = value;
  }

  /// <remarks/>
  public siteDataDateTimeDay day
  {
    get => dayField; set => dayField = value;
  }

  /// <remarks/>
  public byte hour
  {
    get => hourField; set => hourField = value;
  }

  /// <remarks/>
  public byte minute
  {
    get => minuteField; set => minuteField = value;
  }

  /// <remarks/>
  public ulong timeStamp
  {
    get => timeStampField; set => timeStampField = value;
  }

  /// <remarks/>
  public string textSummary
  {
    get => textSummaryField; set => textSummaryField = value;
  }

  /// <remarks/>
  [System.Xml.Serialization.XmlAttributeAttribute()]
  public string name
  {
    get => nameField; set => nameField = value;
  }

  /// <remarks/>
  [System.Xml.Serialization.XmlAttributeAttribute()]
  public string zone
  {
    get => zoneField; set => zoneField = value;
  }

  /// <remarks/>
  [System.Xml.Serialization.XmlAttributeAttribute()]
  public sbyte UTCOffset
  {
    get => uTCOffsetField; set => uTCOffsetField = value;
  }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class siteDataDateTimeMonth
{

  private string? nameField;

  private byte valueField;

  /// <remarks/>
  [System.Xml.Serialization.XmlAttributeAttribute()]
  public string name
  {
    get => nameField; set => nameField = value;
  }

  /// <remarks/>
  [System.Xml.Serialization.XmlTextAttribute()]
  public byte Value
  {
    get => valueField; set => valueField = value;
  }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class siteDataDateTimeDay
{

  private string? nameField;

  private byte valueField;

  /// <remarks/>
  [System.Xml.Serialization.XmlAttributeAttribute()]
  public string name
  {
    get => nameField; set => nameField = value;
  }

  /// <remarks/>
  [System.Xml.Serialization.XmlTextAttribute()]
  public byte Value
  {
    get => valueField; set => valueField = value;
  }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class siteDataLocation
{

  private string? continentField;

  private siteDataLocationCountry? countryField;

  private siteDataLocationProvince? provinceField;

  private siteDataLocationName? nameField;

  private string? regionField;

  /// <remarks/>
  public string continent
  {
    get => continentField; set => continentField = value;
  }

  /// <remarks/>
  public siteDataLocationCountry country
  {
    get => countryField; set => countryField = value;
  }

  /// <remarks/>
  public siteDataLocationProvince province
  {
    get => provinceField; set => provinceField = value;
  }

  /// <remarks/>
  public siteDataLocationName name
  {
    get => nameField; set => nameField = value;
  }

  /// <remarks/>
  public string region
  {
    get => regionField; set => regionField = value;
  }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class siteDataLocationCountry
{

  private string? codeField;

  private string? valueField;

  /// <remarks/>
  [System.Xml.Serialization.XmlAttributeAttribute()]
  public string code
  {
    get => codeField; set => codeField = value;
  }

  /// <remarks/>
  [System.Xml.Serialization.XmlTextAttribute()]
  public string Value
  {
    get => valueField; set => valueField = value;
  }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class siteDataLocationProvince
{

  private string? codeField;

  private string? valueField;

  /// <remarks/>
  [System.Xml.Serialization.XmlAttributeAttribute()]
  public string code
  {
    get => codeField; set => codeField = value;
  }

  /// <remarks/>
  [System.Xml.Serialization.XmlTextAttribute()]
  public string Value
  {
    get => valueField; set => valueField = value;
  }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class siteDataLocationName
{

  private string? codeField;

  private string? latField;

  private string? lonField;

  private string? valueField;

  /// <remarks/>
  [System.Xml.Serialization.XmlAttributeAttribute()]
  public string code
  {
    get => codeField; set => codeField = value;
  }

  /// <remarks/>
  [System.Xml.Serialization.XmlAttributeAttribute()]
  public string lat
  {
    get => latField; set => latField = value;
  }

  /// <remarks/>
  [System.Xml.Serialization.XmlAttributeAttribute()]
  public string lon
  {
    get => lonField; set => lonField = value;
  }

  /// <remarks/>
  [System.Xml.Serialization.XmlTextAttribute()]
  public string Value
  {
    get => valueField; set => valueField = value;
  }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class siteDataCurrentConditions
{

  private siteDataCurrentConditionsStation? stationField;

  private siteDataCurrentConditionsDateTime[]? dateTimeField;

  private object? conditionField;

  private siteDataCurrentConditionsIconCode? iconCodeField;

  private siteDataCurrentConditionsTemperature? temperatureField;

  private siteDataCurrentConditionsDewpoint? dewpointField;

  private siteDataCurrentConditionsPressure? pressureField;

  private siteDataCurrentConditionsRelativeHumidity? relativeHumidityField;

  private siteDataCurrentConditionsWind? windField;

  /// <remarks/>
  public siteDataCurrentConditionsStation station
  {
    get => stationField; set => stationField = value;
  }

  /// <remarks/>
  [System.Xml.Serialization.XmlElementAttribute("dateTime")]
  public siteDataCurrentConditionsDateTime[] dateTime
  {
    get => dateTimeField; set => dateTimeField = value;
  }

  /// <remarks/>
  public object condition
  {
    get => conditionField; set => conditionField = value;
  }

  /// <remarks/>
  public siteDataCurrentConditionsIconCode iconCode
  {
    get => iconCodeField; set => iconCodeField = value;
  }

  /// <remarks/>
  public siteDataCurrentConditionsTemperature temperature
  {
    get => temperatureField; set => temperatureField = value;
  }

  /// <remarks/>
  public siteDataCurrentConditionsDewpoint dewpoint
  {
    get => dewpointField; set => dewpointField = value;
  }

  /// <remarks/>
  public siteDataCurrentConditionsPressure pressure
  {
    get => pressureField; set => pressureField = value;
  }

  /// <remarks/>
  public siteDataCurrentConditionsRelativeHumidity relativeHumidity
  {
    get => relativeHumidityField; set => relativeHumidityField = value;
  }

  /// <remarks/>
  public siteDataCurrentConditionsWind wind
  {
    get => windField; set => windField = value;
  }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class siteDataCurrentConditionsStation
{

  private string? codeField;

  private string? latField;

  private string? lonField;

  private string? valueField;

  /// <remarks/>
  [System.Xml.Serialization.XmlAttributeAttribute()]
  public string code
  {
    get => codeField; set => codeField = value;
  }

  /// <remarks/>
  [System.Xml.Serialization.XmlAttributeAttribute()]
  public string lat
  {
    get => latField; set => latField = value;
  }

  /// <remarks/>
  [System.Xml.Serialization.XmlAttributeAttribute()]
  public string lon
  {
    get => lonField; set => lonField = value;
  }

  /// <remarks/>
  [System.Xml.Serialization.XmlTextAttribute()]
  public string Value
  {
    get => valueField; set => valueField = value;
  }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class siteDataCurrentConditionsDateTime
{

  private ushort yearField;

  private siteDataCurrentConditionsDateTimeMonth? monthField;

  private siteDataCurrentConditionsDateTimeDay? dayField;

  private byte hourField;

  private byte minuteField;

  private ulong timeStampField;

  private string? textSummaryField;

  private string? nameField;

  private string? zoneField;

  private sbyte uTCOffsetField;

  /// <remarks/>
  public ushort year
  {
    get => yearField; set => yearField = value;
  }

  /// <remarks/>
  public siteDataCurrentConditionsDateTimeMonth month
  {
    get => monthField; set => monthField = value;
  }

  /// <remarks/>
  public siteDataCurrentConditionsDateTimeDay day
  {
    get => dayField; set => dayField = value;
  }

  /// <remarks/>
  public byte hour
  {
    get => hourField; set => hourField = value;
  }

  /// <remarks/>
  public byte minute
  {
    get => minuteField; set => minuteField = value;
  }

  /// <remarks/>
  public ulong timeStamp
  {
    get => timeStampField; set => timeStampField = value;
  }

  /// <remarks/>
  public string textSummary
  {
    get => textSummaryField; set => textSummaryField = value;
  }

  /// <remarks/>
  [System.Xml.Serialization.XmlAttributeAttribute()]
  public string name
  {
    get => nameField; set => nameField = value;
  }

  /// <remarks/>
  [System.Xml.Serialization.XmlAttributeAttribute()]
  public string zone
  {
    get => zoneField; set => zoneField = value;
  }

  /// <remarks/>
  [System.Xml.Serialization.XmlAttributeAttribute()]
  public sbyte UTCOffset
  {
    get => uTCOffsetField; set => uTCOffsetField = value;
  }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class siteDataCurrentConditionsDateTimeMonth
{

  private string? nameField;

  private byte valueField;

  /// <remarks/>
  [System.Xml.Serialization.XmlAttributeAttribute()]
  public string name
  {
    get => nameField; set => nameField = value;
  }

  /// <remarks/>
  [System.Xml.Serialization.XmlTextAttribute()]
  public byte Value
  {
    get => valueField; set => valueField = value;
  }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class siteDataCurrentConditionsDateTimeDay
{

  private string? nameField;

  private byte valueField;

  /// <remarks/>
  [System.Xml.Serialization.XmlAttributeAttribute()]
  public string name
  {
    get => nameField; set => nameField = value;
  }

  /// <remarks/>
  [System.Xml.Serialization.XmlTextAttribute()]
  public byte Value
  {
    get => valueField; set => valueField = value;
  }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class siteDataCurrentConditionsIconCode
{

  private string? formatField;

  /// <remarks/>
  [System.Xml.Serialization.XmlAttributeAttribute()]
  public string format
  {
    get => formatField; set => formatField = value;
  }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class siteDataCurrentConditionsTemperature
{

  private string? unitTypeField;

  private string? unitsField;

  private byte qaValueField;

  private decimal valueField;

  /// <remarks/>
  [System.Xml.Serialization.XmlAttributeAttribute()]
  public string unitType
  {
    get => unitTypeField; set => unitTypeField = value;
  }

  /// <remarks/>
  [System.Xml.Serialization.XmlAttributeAttribute()]
  public string units
  {
    get => unitsField; set => unitsField = value;
  }

  /// <remarks/>
  [System.Xml.Serialization.XmlAttributeAttribute()]
  public byte qaValue
  {
    get => qaValueField; set => qaValueField = value;
  }

  /// <remarks/>
  [System.Xml.Serialization.XmlTextAttribute()]
  public decimal Value
  {
    get => valueField; set => valueField = value;
  }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class siteDataCurrentConditionsDewpoint
{

  private string? unitTypeField;

  private string? unitsField;

  private decimal valueField;

  /// <remarks/>
  [System.Xml.Serialization.XmlAttributeAttribute()]
  public string unitType
  {
    get => unitTypeField; set => unitTypeField = value;
  }

  /// <remarks/>
  [System.Xml.Serialization.XmlAttributeAttribute()]
  public string units
  {
    get => unitsField; set => unitsField = value;
  }

  /// <remarks/>
  [System.Xml.Serialization.XmlTextAttribute()]
  public decimal Value
  {
    get => valueField; set => valueField = value;
  }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class siteDataCurrentConditionsPressure
{

  private string? unitTypeField;

  private string? unitsField;

  private decimal changeField;

  private string? tendencyField;

  private decimal valueField;

  /// <remarks/>
  [System.Xml.Serialization.XmlAttributeAttribute()]
  public string unitType
  {
    get => unitTypeField; set => unitTypeField = value;
  }

  /// <remarks/>
  [System.Xml.Serialization.XmlAttributeAttribute()]
  public string units
  {
    get => unitsField; set => unitsField = value;
  }

  /// <remarks/>
  [System.Xml.Serialization.XmlAttributeAttribute()]
  public decimal change
  {
    get => changeField; set => changeField = value;
  }

  /// <remarks/>
  [System.Xml.Serialization.XmlAttributeAttribute()]
  public string tendency
  {
    get => tendencyField; set => tendencyField = value;
  }

  /// <remarks/>
  [System.Xml.Serialization.XmlTextAttribute()]
  public decimal Value
  {
    get => valueField; set => valueField = value;
  }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class siteDataCurrentConditionsRelativeHumidity
{

  private string? unitsField;

  private byte qaValueField;

  private byte valueField;

  /// <remarks/>
  [System.Xml.Serialization.XmlAttributeAttribute()]
  public string units
  {
    get => unitsField; set => unitsField = value;
  }

  /// <remarks/>
  [System.Xml.Serialization.XmlAttributeAttribute()]
  public byte qaValue
  {
    get => qaValueField; set => qaValueField = value;
  }

  /// <remarks/>
  [System.Xml.Serialization.XmlTextAttribute()]
  public byte Value
  {
    get => valueField; set => valueField = value;
  }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class siteDataCurrentConditionsWind
{

  private siteDataCurrentConditionsWindSpeed? speedField;

  private siteDataCurrentConditionsWindGust? gustField;

  private siteDataCurrentConditionsWindDirection? directionField;

  private siteDataCurrentConditionsWindBearing? bearingField;

  /// <remarks/>
  public siteDataCurrentConditionsWindSpeed speed
  {
    get => speedField; set => speedField = value;
  }

  /// <remarks/>
  public siteDataCurrentConditionsWindGust gust
  {
    get => gustField; set => gustField = value;
  }

  /// <remarks/>
  public siteDataCurrentConditionsWindDirection direction
  {
    get => directionField; set => directionField = value;
  }

  /// <remarks/>
  public siteDataCurrentConditionsWindBearing bearing
  {
    get => bearingField; set => bearingField = value;
  }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class siteDataCurrentConditionsWindSpeed
{

  private string? unitTypeField;

  private string? unitsField;

  private byte qaValueField;

  private byte valueField;

  /// <remarks/>
  [System.Xml.Serialization.XmlAttributeAttribute()]
  public string unitType
  {
    get => unitTypeField; set => unitTypeField = value;
  }

  /// <remarks/>
  [System.Xml.Serialization.XmlAttributeAttribute()]
  public string units
  {
    get => unitsField; set => unitsField = value;
  }

  /// <remarks/>
  [System.Xml.Serialization.XmlAttributeAttribute()]
  public byte qaValue
  {
    get => qaValueField; set => qaValueField = value;
  }

  /// <remarks/>
  [System.Xml.Serialization.XmlTextAttribute()]
  public byte Value
  {
    get => valueField; set => valueField = value;
  }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class siteDataCurrentConditionsWindGust
{

  private string? unitTypeField;

  private string? unitsField;

  private byte qaValueField;

  private byte valueField;

  /// <remarks/>
  [System.Xml.Serialization.XmlAttributeAttribute()]
  public string unitType
  {
    get => unitTypeField; set => unitTypeField = value;
  }

  /// <remarks/>
  [System.Xml.Serialization.XmlAttributeAttribute()]
  public string units
  {
    get => unitsField; set => unitsField = value;
  }

  /// <remarks/>
  [System.Xml.Serialization.XmlAttributeAttribute()]
  public byte qaValue
  {
    get => qaValueField; set => qaValueField = value;
  }

  /// <remarks/>
  [System.Xml.Serialization.XmlTextAttribute()]
  public byte Value
  {
    get => valueField; set => valueField = value;
  }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class siteDataCurrentConditionsWindDirection
{

  private byte qaValueField;

  private string? valueField;

  /// <remarks/>
  [System.Xml.Serialization.XmlAttributeAttribute()]
  public byte qaValue
  {
    get => qaValueField; set => qaValueField = value;
  }

  /// <remarks/>
  [System.Xml.Serialization.XmlTextAttribute()]
  public string Value
  {
    get => valueField; set => valueField = value;
  }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class siteDataCurrentConditionsWindBearing
{

  private string? unitsField;

  private byte qaValueField;

  private decimal valueField;

  /// <remarks/>
  [System.Xml.Serialization.XmlAttributeAttribute()]
  public string units
  {
    get => unitsField; set => unitsField = value;
  }

  /// <remarks/>
  [System.Xml.Serialization.XmlAttributeAttribute()]
  public byte qaValue
  {
    get => qaValueField; set => qaValueField = value;
  }

  /// <remarks/>
  [System.Xml.Serialization.XmlTextAttribute()]
  public decimal Value
  {
    get => valueField; set => valueField = value;
  }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class siteDataForecastGroup
{

  private siteDataForecastGroupDateTime[]? dateTimeField;

  private siteDataForecastGroupRegionalNormals? regionalNormalsField;

  private siteDataForecastGroupForecast[]? forecastField;

  /// <remarks/>
  [System.Xml.Serialization.XmlElementAttribute("dateTime")]
  public siteDataForecastGroupDateTime[] dateTime
  {
    get => dateTimeField; set => dateTimeField = value;
  }

  /// <remarks/>
  public siteDataForecastGroupRegionalNormals regionalNormals
  {
    get => regionalNormalsField; set => regionalNormalsField = value;
  }

  /// <remarks/>
  [System.Xml.Serialization.XmlElementAttribute("forecast")]
  public siteDataForecastGroupForecast[] forecast
  {
    get => forecastField; set => forecastField = value;
  }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class siteDataForecastGroupDateTime
{

  private ushort yearField;

  private siteDataForecastGroupDateTimeMonth? monthField;

  private siteDataForecastGroupDateTimeDay? dayField;

  private byte hourField;

  private byte minuteField;

  private ulong timeStampField;

  private string? textSummaryField;

  private string? nameField;

  private string? zoneField;

  private sbyte uTCOffsetField;

  /// <remarks/>
  public ushort year
  {
    get => yearField; set => yearField = value;
  }

  /// <remarks/>
  public siteDataForecastGroupDateTimeMonth month
  {
    get => monthField; set => monthField = value;
  }

  /// <remarks/>
  public siteDataForecastGroupDateTimeDay day
  {
    get => dayField; set => dayField = value;
  }

  /// <remarks/>
  public byte hour
  {
    get => hourField; set => hourField = value;
  }

  /// <remarks/>
  public byte minute
  {
    get => minuteField; set => minuteField = value;
  }

  /// <remarks/>
  public ulong timeStamp
  {
    get => timeStampField; set => timeStampField = value;
  }

  /// <remarks/>
  public string textSummary
  {
    get => textSummaryField; set => textSummaryField = value;
  }

  /// <remarks/>
  [System.Xml.Serialization.XmlAttributeAttribute()]
  public string name
  {
    get => nameField; set => nameField = value;
  }

  /// <remarks/>
  [System.Xml.Serialization.XmlAttributeAttribute()]
  public string zone
  {
    get => zoneField; set => zoneField = value;
  }

  /// <remarks/>
  [System.Xml.Serialization.XmlAttributeAttribute()]
  public sbyte UTCOffset
  {
    get => uTCOffsetField; set => uTCOffsetField = value;
  }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class siteDataForecastGroupDateTimeMonth
{

  private string? nameField;

  private byte valueField;

  /// <remarks/>
  [System.Xml.Serialization.XmlAttributeAttribute()]
  public string name
  {
    get => nameField; set => nameField = value;
  }

  /// <remarks/>
  [System.Xml.Serialization.XmlTextAttribute()]
  public byte Value
  {
    get => valueField; set => valueField = value;
  }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class siteDataForecastGroupDateTimeDay
{

  private string? nameField;

  private byte valueField;

  /// <remarks/>
  [System.Xml.Serialization.XmlAttributeAttribute()]
  public string name
  {
    get => nameField; set => nameField = value;
  }

  /// <remarks/>
  [System.Xml.Serialization.XmlTextAttribute()]
  public byte Value
  {
    get => valueField; set => valueField = value;
  }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class siteDataForecastGroupRegionalNormals
{

  private string? textSummaryField;

  private siteDataForecastGroupRegionalNormalsTemperature[]? temperatureField;

  /// <remarks/>
  public string textSummary
  {
    get => textSummaryField; set => textSummaryField = value;
  }

  /// <remarks/>
  [System.Xml.Serialization.XmlElementAttribute("temperature")]
  public siteDataForecastGroupRegionalNormalsTemperature[] temperature
  {
    get => temperatureField; set => temperatureField = value;
  }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class siteDataForecastGroupRegionalNormalsTemperature
{

  private string? unitTypeField;

  private string? unitsField;

  private string? classField;

  private byte valueField;

  /// <remarks/>
  [System.Xml.Serialization.XmlAttributeAttribute()]
  public string unitType
  {
    get => unitTypeField; set => unitTypeField = value;
  }

  /// <remarks/>
  [System.Xml.Serialization.XmlAttributeAttribute()]
  public string units
  {
    get => unitsField; set => unitsField = value;
  }

  /// <remarks/>
  [System.Xml.Serialization.XmlAttributeAttribute()]
  public string @class
  {
    get => classField; set => classField = value;
  }

  /// <remarks/>
  [System.Xml.Serialization.XmlTextAttribute()]
  public byte Value
  {
    get => valueField; set => valueField = value;
  }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class siteDataForecastGroupForecast
{

  private siteDataForecastGroupForecastPeriod? periodField;

  private string? textSummaryField;

  private siteDataForecastGroupForecastCloudPrecip? cloudPrecipField;

  private siteDataForecastGroupForecastAbbreviatedForecast? abbreviatedForecastField;

  private siteDataForecastGroupForecastTemperatures? temperaturesField;

  private siteDataForecastGroupForecastWinds? windsField;

  private siteDataForecastGroupForecastPrecipitation? precipitationField;

  private object? windChillField;

  private siteDataForecastGroupForecastVisibility? visibilityField;

  private siteDataForecastGroupForecastUV? uvField;

  private siteDataForecastGroupForecastRelativeHumidity? relativeHumidityField;

  private siteDataForecastGroupForecastHumidex? humidexField;

  /// <remarks/>
  public siteDataForecastGroupForecastPeriod period
  {
    get => periodField; set => periodField = value;
  }

  /// <remarks/>
  public string textSummary
  {
    get => textSummaryField; set => textSummaryField = value;
  }

  /// <remarks/>
  public siteDataForecastGroupForecastCloudPrecip cloudPrecip
  {
    get => cloudPrecipField; set => cloudPrecipField = value;
  }

  /// <remarks/>
  public siteDataForecastGroupForecastAbbreviatedForecast abbreviatedForecast
  {
    get => abbreviatedForecastField; set => abbreviatedForecastField = value;
  }

  /// <remarks/>
  public siteDataForecastGroupForecastTemperatures temperatures
  {
    get => temperaturesField; set => temperaturesField = value;
  }

  /// <remarks/>
  public siteDataForecastGroupForecastWinds winds
  {
    get => windsField; set => windsField = value;
  }

  /// <remarks/>
  public siteDataForecastGroupForecastPrecipitation precipitation
  {
    get => precipitationField; set => precipitationField = value;
  }

  /// <remarks/>
  public object windChill
  {
    get => windChillField; set => windChillField = value;
  }

  /// <remarks/>
  public siteDataForecastGroupForecastVisibility visibility
  {
    get => visibilityField; set => visibilityField = value;
  }

  /// <remarks/>
  public siteDataForecastGroupForecastUV uv
  {
    get => uvField; set => uvField = value;
  }

  /// <remarks/>
  public siteDataForecastGroupForecastRelativeHumidity relativeHumidity
  {
    get => relativeHumidityField; set => relativeHumidityField = value;
  }

  /// <remarks/>
  public siteDataForecastGroupForecastHumidex humidex
  {
    get => humidexField; set => humidexField = value;
  }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class siteDataForecastGroupForecastPeriod
{

  private string? textForecastNameField;

  private string? valueField;

  /// <remarks/>
  [System.Xml.Serialization.XmlAttributeAttribute()]
  public string textForecastName
  {
    get => textForecastNameField; set => textForecastNameField = value;
  }

  /// <remarks/>
  [System.Xml.Serialization.XmlTextAttribute()]
  public string Value
  {
    get => valueField; set => valueField = value;
  }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class siteDataForecastGroupForecastCloudPrecip
{

  private string? textSummaryField;

  /// <remarks/>
  public string textSummary
  {
    get => textSummaryField; set => textSummaryField = value;
  }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class siteDataForecastGroupForecastAbbreviatedForecast
{

  private siteDataForecastGroupForecastAbbreviatedForecastIconCode? iconCodeField;

  private siteDataForecastGroupForecastAbbreviatedForecastPop? popField;

  private string? textSummaryField;

  /// <remarks/>
  public siteDataForecastGroupForecastAbbreviatedForecastIconCode iconCode
  {
    get => iconCodeField; set => iconCodeField = value;
  }

  /// <remarks/>
  public siteDataForecastGroupForecastAbbreviatedForecastPop pop
  {
    get => popField; set => popField = value;
  }

  /// <remarks/>
  public string textSummary
  {
    get => textSummaryField; set => textSummaryField = value;
  }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class siteDataForecastGroupForecastAbbreviatedForecastIconCode
{

  private string? formatField;

  private byte valueField;

  /// <remarks/>
  [System.Xml.Serialization.XmlAttributeAttribute()]
  public string format
  {
    get => formatField; set => formatField = value;
  }

  /// <remarks/>
  [System.Xml.Serialization.XmlTextAttribute()]
  public byte Value
  {
    get => valueField; set => valueField = value;
  }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class siteDataForecastGroupForecastAbbreviatedForecastPop
{

  private string? unitsField;

  private string? valueField;

  /// <remarks/>
  [System.Xml.Serialization.XmlAttributeAttribute()]
  public string units
  {
    get => unitsField; set => unitsField = value;
  }

  /// <remarks/>
  [System.Xml.Serialization.XmlTextAttribute()]
  public string Value
  {
    get => valueField; set => valueField = value;
  }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class siteDataForecastGroupForecastTemperatures
{

  private string? textSummaryField;

  private siteDataForecastGroupForecastTemperaturesTemperature? temperatureField;

  /// <remarks/>
  public string textSummary
  {
    get => textSummaryField; set => textSummaryField = value;
  }

  /// <remarks/>
  public siteDataForecastGroupForecastTemperaturesTemperature temperature
  {
    get => temperatureField; set => temperatureField = value;
  }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class siteDataForecastGroupForecastTemperaturesTemperature
{

  private string? unitTypeField;

  private string? unitsField;

  private string? classField;

  private byte valueField;

  /// <remarks/>
  [System.Xml.Serialization.XmlAttributeAttribute()]
  public string unitType
  {
    get => unitTypeField; set => unitTypeField = value;
  }

  /// <remarks/>
  [System.Xml.Serialization.XmlAttributeAttribute()]
  public string units
  {
    get => unitsField; set => unitsField = value;
  }

  /// <remarks/>
  [System.Xml.Serialization.XmlAttributeAttribute()]
  public string @class
  {
    get => classField; set => classField = value;
  }

  /// <remarks/>
  [System.Xml.Serialization.XmlTextAttribute()]
  public byte Value
  {
    get => valueField; set => valueField = value;
  }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class siteDataForecastGroupForecastWinds
{

  private string? textSummaryField;

  private siteDataForecastGroupForecastWindsWind[]? windField;

  /// <remarks/>
  public string textSummary
  {
    get => textSummaryField; set => textSummaryField = value;
  }

  /// <remarks/>
  [System.Xml.Serialization.XmlElementAttribute("wind")]
  public siteDataForecastGroupForecastWindsWind[] wind
  {
    get => windField; set => windField = value;
  }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class siteDataForecastGroupForecastWindsWind
{

  private siteDataForecastGroupForecastWindsWindSpeed? speedField;

  private siteDataForecastGroupForecastWindsWindGust? gustField;

  private string? directionField;

  private siteDataForecastGroupForecastWindsWindBearing? bearingField;

  private byte indexField;

  private string? rankField;

  /// <remarks/>
  public siteDataForecastGroupForecastWindsWindSpeed speed
  {
    get => speedField; set => speedField = value;
  }

  /// <remarks/>
  public siteDataForecastGroupForecastWindsWindGust gust
  {
    get => gustField; set => gustField = value;
  }

  /// <remarks/>
  public string direction
  {
    get => directionField; set => directionField = value;
  }

  /// <remarks/>
  public siteDataForecastGroupForecastWindsWindBearing bearing
  {
    get => bearingField; set => bearingField = value;
  }

  /// <remarks/>
  [System.Xml.Serialization.XmlAttributeAttribute()]
  public byte index
  {
    get => indexField; set => indexField = value;
  }

  /// <remarks/>
  [System.Xml.Serialization.XmlAttributeAttribute()]
  public string rank
  {
    get => rankField; set => rankField = value;
  }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class siteDataForecastGroupForecastWindsWindSpeed
{

  private string? unitTypeField;

  private string? unitsField;

  private byte valueField;

  /// <remarks/>
  [System.Xml.Serialization.XmlAttributeAttribute()]
  public string unitType
  {
    get => unitTypeField; set => unitTypeField = value;
  }

  /// <remarks/>
  [System.Xml.Serialization.XmlAttributeAttribute()]
  public string units
  {
    get => unitsField; set => unitsField = value;
  }

  /// <remarks/>
  [System.Xml.Serialization.XmlTextAttribute()]
  public byte Value
  {
    get => valueField; set => valueField = value;
  }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class siteDataForecastGroupForecastWindsWindGust
{

  private string? unitTypeField;

  private string? unitsField;

  private byte valueField;

  /// <remarks/>
  [System.Xml.Serialization.XmlAttributeAttribute()]
  public string unitType
  {
    get => unitTypeField; set => unitTypeField = value;
  }

  /// <remarks/>
  [System.Xml.Serialization.XmlAttributeAttribute()]
  public string units
  {
    get => unitsField; set => unitsField = value;
  }

  /// <remarks/>
  [System.Xml.Serialization.XmlTextAttribute()]
  public byte Value
  {
    get => valueField; set => valueField = value;
  }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class siteDataForecastGroupForecastWindsWindBearing
{

  private string? unitsField;

  private byte valueField;

  /// <remarks/>
  [System.Xml.Serialization.XmlAttributeAttribute()]
  public string units
  {
    get => unitsField; set => unitsField = value;
  }

  /// <remarks/>
  [System.Xml.Serialization.XmlTextAttribute()]
  public byte Value
  {
    get => valueField; set => valueField = value;
  }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class siteDataForecastGroupForecastPrecipitation
{

  private object? textSummaryField;

  private siteDataForecastGroupForecastPrecipitationPrecipType[]? precipTypeField;

  /// <remarks/>
  public object textSummary
  {
    get => textSummaryField; set => textSummaryField = value;
  }

  /// <remarks/>
  [System.Xml.Serialization.XmlElementAttribute("precipType")]
  public siteDataForecastGroupForecastPrecipitationPrecipType[] precipType
  {
    get => precipTypeField; set => precipTypeField = value;
  }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class siteDataForecastGroupForecastPrecipitationPrecipType
{

  private string? startField;

  private string? endField;

  private string? valueField;

  /// <remarks/>
  [System.Xml.Serialization.XmlAttributeAttribute()]
  public string start
  {
    get => startField; set => startField = value;
  }

  /// <remarks/>
  [System.Xml.Serialization.XmlAttributeAttribute()]
  public string end
  {
    get => endField; set => endField = value;
  }

  /// <remarks/>
  [System.Xml.Serialization.XmlTextAttribute()]
  public string Value
  {
    get => valueField; set => valueField = value;
  }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class siteDataForecastGroupForecastVisibility
{

  private siteDataForecastGroupForecastVisibilityOtherVisib? otherVisibField;

  /// <remarks/>
  public siteDataForecastGroupForecastVisibilityOtherVisib otherVisib
  {
    get => otherVisibField; set => otherVisibField = value;
  }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class siteDataForecastGroupForecastVisibilityOtherVisib
{

  private string? textSummaryField;

  private string? causeField;

  /// <remarks/>
  public string textSummary
  {
    get => textSummaryField; set => textSummaryField = value;
  }

  /// <remarks/>
  [System.Xml.Serialization.XmlAttributeAttribute()]
  public string cause
  {
    get => causeField; set => causeField = value;
  }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class siteDataForecastGroupForecastUV
{

  private byte indexField;

  private string? textSummaryField;

  private string? categoryField;

  /// <remarks/>
  public byte index
  {
    get => indexField; set => indexField = value;
  }

  /// <remarks/>
  public string textSummary
  {
    get => textSummaryField; set => textSummaryField = value;
  }

  /// <remarks/>
  [System.Xml.Serialization.XmlAttributeAttribute()]
  public string category
  {
    get => categoryField; set => categoryField = value;
  }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class siteDataForecastGroupForecastRelativeHumidity
{

  private string? unitsField;

  private byte valueField;

  /// <remarks/>
  [System.Xml.Serialization.XmlAttributeAttribute()]
  public string units
  {
    get => unitsField; set => unitsField = value;
  }

  /// <remarks/>
  [System.Xml.Serialization.XmlTextAttribute()]
  public byte Value
  {
    get => valueField; set => valueField = value;
  }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class siteDataForecastGroupForecastHumidex
{

  private byte calculatedField;

  private string? textSummaryField;

  /// <remarks/>
  public byte calculated
  {
    get => calculatedField; set => calculatedField = value;
  }

  /// <remarks/>
  public string textSummary
  {
    get => textSummaryField; set => textSummaryField = value;
  }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class siteDataHourlyForecastGroup
{

  private siteDataHourlyForecastGroupDateTime[]? dateTimeField;

  private siteDataHourlyForecastGroupHourlyForecast[]? hourlyForecastField;

  /// <remarks/>
  [System.Xml.Serialization.XmlElementAttribute("dateTime")]
  public siteDataHourlyForecastGroupDateTime[] dateTime
  {
    get => dateTimeField; set => dateTimeField = value;
  }

  /// <remarks/>
  [System.Xml.Serialization.XmlElementAttribute("hourlyForecast")]
  public siteDataHourlyForecastGroupHourlyForecast[] hourlyForecast
  {
    get => hourlyForecastField; set => hourlyForecastField = value;
  }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class siteDataHourlyForecastGroupDateTime
{

  private ushort yearField;

  private siteDataHourlyForecastGroupDateTimeMonth? monthField;

  private siteDataHourlyForecastGroupDateTimeDay? dayField;

  private byte hourField;

  private byte minuteField;

  private ulong timeStampField;

  private string? textSummaryField;

  private string? nameField;

  private string? zoneField;

  private sbyte uTCOffsetField;

  /// <remarks/>
  public ushort year
  {
    get => yearField; set => yearField = value;
  }

  /// <remarks/>
  public siteDataHourlyForecastGroupDateTimeMonth month
  {
    get => monthField; set => monthField = value;
  }

  /// <remarks/>
  public siteDataHourlyForecastGroupDateTimeDay day
  {
    get => dayField; set => dayField = value;
  }

  /// <remarks/>
  public byte hour
  {
    get => hourField; set => hourField = value;
  }

  /// <remarks/>
  public byte minute
  {
    get => minuteField; set => minuteField = value;
  }

  /// <remarks/>
  public ulong timeStamp
  {
    get => timeStampField; set => timeStampField = value;
  }

  /// <remarks/>
  public string textSummary
  {
    get => textSummaryField; set => textSummaryField = value;
  }

  /// <remarks/>
  [System.Xml.Serialization.XmlAttributeAttribute()]
  public string name
  {
    get => nameField; set => nameField = value;
  }

  /// <remarks/>
  [System.Xml.Serialization.XmlAttributeAttribute()]
  public string zone
  {
    get => zoneField; set => zoneField = value;
  }

  /// <remarks/>
  [System.Xml.Serialization.XmlAttributeAttribute()]
  public sbyte UTCOffset
  {
    get => uTCOffsetField; set => uTCOffsetField = value;
  }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class siteDataHourlyForecastGroupDateTimeMonth
{

  private string? nameField;

  private byte valueField;

  /// <remarks/>
  [System.Xml.Serialization.XmlAttributeAttribute()]
  public string name
  {
    get => nameField; set => nameField = value;
  }

  /// <remarks/>
  [System.Xml.Serialization.XmlTextAttribute()]
  public byte Value
  {
    get => valueField; set => valueField = value;
  }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class siteDataHourlyForecastGroupDateTimeDay
{

  private string? nameField;

  private byte valueField;

  /// <remarks/>
  [System.Xml.Serialization.XmlAttributeAttribute()]
  public string name
  {
    get => nameField; set => nameField = value;
  }

  /// <remarks/>
  [System.Xml.Serialization.XmlTextAttribute()]
  public byte Value
  {
    get => valueField; set => valueField = value;
  }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class siteDataHourlyForecastGroupHourlyForecast
{

  private string? conditionField;

  private siteDataHourlyForecastGroupHourlyForecastIconCode? iconCodeField;

  private siteDataHourlyForecastGroupHourlyForecastTemperature? temperatureField;

  private siteDataHourlyForecastGroupHourlyForecastLop? lopField;

  private siteDataHourlyForecastGroupHourlyForecastWindChill? windChillField;

  private siteDataHourlyForecastGroupHourlyForecastHumidex? humidexField;

  private siteDataHourlyForecastGroupHourlyForecastWind? windField;

  private siteDataHourlyForecastGroupHourlyForecastUV? uvField;

  private ulong dateTimeUTCField;

  /// <remarks/>
  public string condition
  {
    get => conditionField; set => conditionField = value;
  }

  /// <remarks/>
  public siteDataHourlyForecastGroupHourlyForecastIconCode iconCode
  {
    get => iconCodeField; set => iconCodeField = value;
  }

  /// <remarks/>
  public siteDataHourlyForecastGroupHourlyForecastTemperature temperature
  {
    get => temperatureField; set => temperatureField = value;
  }

  /// <remarks/>
  public siteDataHourlyForecastGroupHourlyForecastLop lop
  {
    get => lopField; set => lopField = value;
  }

  /// <remarks/>
  public siteDataHourlyForecastGroupHourlyForecastWindChill windChill
  {
    get => windChillField; set => windChillField = value;
  }

  /// <remarks/>
  public siteDataHourlyForecastGroupHourlyForecastHumidex humidex
  {
    get => humidexField; set => humidexField = value;
  }

  /// <remarks/>
  public siteDataHourlyForecastGroupHourlyForecastWind wind
  {
    get => windField; set => windField = value;
  }

  /// <remarks/>
  public siteDataHourlyForecastGroupHourlyForecastUV uv
  {
    get => uvField; set => uvField = value;
  }

  /// <remarks/>
  [System.Xml.Serialization.XmlAttributeAttribute()]
  public ulong dateTimeUTC
  {
    get => dateTimeUTCField; set => dateTimeUTCField = value;
  }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class siteDataHourlyForecastGroupHourlyForecastIconCode
{

  private string? formatField;

  private byte valueField;

  /// <remarks/>
  [System.Xml.Serialization.XmlAttributeAttribute()]
  public string format
  {
    get => formatField; set => formatField = value;
  }

  /// <remarks/>
  [System.Xml.Serialization.XmlTextAttribute()]
  public byte Value
  {
    get => valueField; set => valueField = value;
  }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class siteDataHourlyForecastGroupHourlyForecastTemperature
{

  private string? unitTypeField;

  private string? unitsField;

  private byte valueField;

  /// <remarks/>
  [System.Xml.Serialization.XmlAttributeAttribute()]
  public string unitType
  {
    get => unitTypeField; set => unitTypeField = value;
  }

  /// <remarks/>
  [System.Xml.Serialization.XmlAttributeAttribute()]
  public string units
  {
    get => unitsField; set => unitsField = value;
  }

  /// <remarks/>
  [System.Xml.Serialization.XmlTextAttribute()]
  public byte Value
  {
    get => valueField; set => valueField = value;
  }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class siteDataHourlyForecastGroupHourlyForecastLop
{

  private string? categoryField;

  private string? unitsField;

  private byte valueField;

  /// <remarks/>
  [System.Xml.Serialization.XmlAttributeAttribute()]
  public string category
  {
    get => categoryField; set => categoryField = value;
  }

  /// <remarks/>
  [System.Xml.Serialization.XmlAttributeAttribute()]
  public string units
  {
    get => unitsField; set => unitsField = value;
  }

  /// <remarks/>
  [System.Xml.Serialization.XmlTextAttribute()]
  public byte Value
  {
    get => valueField; set => valueField = value;
  }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class siteDataHourlyForecastGroupHourlyForecastWindChill
{

  private string? unitTypeField;

  /// <remarks/>
  [System.Xml.Serialization.XmlAttributeAttribute()]
  public string unitType
  {
    get => unitTypeField; set => unitTypeField = value;
  }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class siteDataHourlyForecastGroupHourlyForecastHumidex
{

  private string? unitTypeField;

  private string? valueField;

  /// <remarks/>
  [System.Xml.Serialization.XmlAttributeAttribute()]
  public string unitType
  {
    get => unitTypeField; set => unitTypeField = value;
  }

  /// <remarks/>
  [System.Xml.Serialization.XmlTextAttribute()]
  public string Value
  {
    get => valueField; set => valueField = value;
  }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class siteDataHourlyForecastGroupHourlyForecastWind
{

  private siteDataHourlyForecastGroupHourlyForecastWindSpeed? speedField;

  private siteDataHourlyForecastGroupHourlyForecastWindDirection? directionField;

  private siteDataHourlyForecastGroupHourlyForecastWindGust? gustField;

  /// <remarks/>
  public siteDataHourlyForecastGroupHourlyForecastWindSpeed speed
  {
    get => speedField; set => speedField = value;
  }

  /// <remarks/>
  public siteDataHourlyForecastGroupHourlyForecastWindDirection direction
  {
    get => directionField; set => directionField = value;
  }

  /// <remarks/>
  public siteDataHourlyForecastGroupHourlyForecastWindGust gust
  {
    get => gustField; set => gustField = value;
  }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class siteDataHourlyForecastGroupHourlyForecastWindSpeed
{

  private string? unitTypeField;

  private string? unitsField;

  private byte valueField;

  /// <remarks/>
  [System.Xml.Serialization.XmlAttributeAttribute()]
  public string unitType
  {
    get => unitTypeField; set => unitTypeField = value;
  }

  /// <remarks/>
  [System.Xml.Serialization.XmlAttributeAttribute()]
  public string units
  {
    get => unitsField; set => unitsField = value;
  }

  /// <remarks/>
  [System.Xml.Serialization.XmlTextAttribute()]
  public byte Value
  {
    get => valueField; set => valueField = value;
  }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class siteDataHourlyForecastGroupHourlyForecastWindDirection
{

  private string? windDirFullField;

  private string? valueField;

  /// <remarks/>
  [System.Xml.Serialization.XmlAttributeAttribute()]
  public string windDirFull
  {
    get => windDirFullField; set => windDirFullField = value;
  }

  /// <remarks/>
  [System.Xml.Serialization.XmlTextAttribute()]
  public string Value
  {
    get => valueField; set => valueField = value;
  }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class siteDataHourlyForecastGroupHourlyForecastWindGust
{

  private string? unitTypeField;

  private string? unitsField;

  private string? valueField;

  /// <remarks/>
  [System.Xml.Serialization.XmlAttributeAttribute()]
  public string unitType
  {
    get => unitTypeField; set => unitTypeField = value;
  }

  /// <remarks/>
  [System.Xml.Serialization.XmlAttributeAttribute()]
  public string units
  {
    get => unitsField; set => unitsField = value;
  }

  /// <remarks/>
  [System.Xml.Serialization.XmlTextAttribute()]
  public string Value
  {
    get => valueField; set => valueField = value;
  }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class siteDataHourlyForecastGroupHourlyForecastUV
{

  private byte indexField;

  /// <remarks/>
  public byte index
  {
    get => indexField; set => indexField = value;
  }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class siteDataRiseSet
{

  private string? disclaimerField;

  private siteDataRiseSetDateTime[]? dateTimeField;

  /// <remarks/>
  public string disclaimer
  {
    get => disclaimerField; set => disclaimerField = value;
  }

  /// <remarks/>
  [System.Xml.Serialization.XmlElementAttribute("dateTime")]
  public siteDataRiseSetDateTime[] dateTime
  {
    get => dateTimeField; set => dateTimeField = value;
  }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class siteDataRiseSetDateTime
{

  private ushort yearField;

  private siteDataRiseSetDateTimeMonth? monthField;

  private siteDataRiseSetDateTimeDay? dayField;

  private byte hourField;

  private byte minuteField;

  private ulong timeStampField;

  private string? textSummaryField;

  private string? nameField;

  private string? zoneField;

  private sbyte uTCOffsetField;

  /// <remarks/>
  public ushort year
  {
    get => yearField; set => yearField = value;
  }

  /// <remarks/>
  public siteDataRiseSetDateTimeMonth month
  {
    get => monthField; set => monthField = value;
  }

  /// <remarks/>
  public siteDataRiseSetDateTimeDay day
  {
    get => dayField; set => dayField = value;
  }

  /// <remarks/>
  public byte hour
  {
    get => hourField; set => hourField = value;
  }

  /// <remarks/>
  public byte minute
  {
    get => minuteField; set => minuteField = value;
  }

  /// <remarks/>
  public ulong timeStamp
  {
    get => timeStampField; set => timeStampField = value;
  }

  /// <remarks/>
  public string textSummary
  {
    get => textSummaryField; set => textSummaryField = value;
  }

  /// <remarks/>
  [System.Xml.Serialization.XmlAttributeAttribute()]
  public string name
  {
    get => nameField; set => nameField = value;
  }

  /// <remarks/>
  [System.Xml.Serialization.XmlAttributeAttribute()]
  public string zone
  {
    get => zoneField; set => zoneField = value;
  }

  /// <remarks/>
  [System.Xml.Serialization.XmlAttributeAttribute()]
  public sbyte UTCOffset
  {
    get => uTCOffsetField; set => uTCOffsetField = value;
  }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class siteDataRiseSetDateTimeMonth
{

  private string? nameField;

  private byte valueField;

  /// <remarks/>
  [System.Xml.Serialization.XmlAttributeAttribute()]
  public string name
  {
    get => nameField; set => nameField = value;
  }

  /// <remarks/>
  [System.Xml.Serialization.XmlTextAttribute()]
  public byte Value
  {
    get => valueField; set => valueField = value;
  }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class siteDataRiseSetDateTimeDay
{

  private string? nameField;

  private byte valueField;

  /// <remarks/>
  [System.Xml.Serialization.XmlAttributeAttribute()]
  public string name
  {
    get => nameField; set => nameField = value;
  }

  /// <remarks/>
  [System.Xml.Serialization.XmlTextAttribute()]
  public byte Value
  {
    get => valueField; set => valueField = value;
  }
}

