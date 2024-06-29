using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenWeaLib.weather.gc.ca;

// NOTE: Generated code may require at least .NET Framework 4.5 or .NET Core/Standard 2.0.
/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
[System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
public partial class climatedata
{

  private string langField;

  private climatedataStationinformation stationinformationField;

  private climatedataFlag[] legendField;

  private climatedataMonth[] monthField;

  private string schemaLocationField;

  /// <remarks/>
  public string lang
  {
    get {
      return this.langField;
    }
    set {
      this.langField = value;
    }
  }

  /// <remarks/>
  public climatedataStationinformation stationinformation
  {
    get {
      return this.stationinformationField;
    }
    set {
      this.stationinformationField = value;
    }
  }

  /// <remarks/>
  [System.Xml.Serialization.XmlArrayItemAttribute("flag", IsNullable = false)]
  public climatedataFlag[] legend
  {
    get {
      return this.legendField;
    }
    set {
      this.legendField = value;
    }
  }

  /// <remarks/>
  [System.Xml.Serialization.XmlElementAttribute("month")]
  public climatedataMonth[] month
  {
    get {
      return this.monthField;
    }
    set {
      this.monthField = value;
    }
  }

  /// <remarks/>
  [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified, Namespace = "http://www.w3.org/TR/xmlschema-1/")]
  public string schemaLocation
  {
    get {
      return this.schemaLocationField;
    }
    set {
      this.schemaLocationField = value;
    }
  }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class climatedataStationinformation
{

  private string nameField;

  private string province_or_territoryField;

  private decimal latitudeField;

  private decimal longitudeField;

  private decimal elevationField;

  private uint climate_identifierField;

  private uint wmo_identifierField;

  private string tc_identifierField;

  /// <remarks/>
  public string name
  {
    get {
      return this.nameField;
    }
    set {
      this.nameField = value;
    }
  }

  /// <remarks/>
  public string province_or_territory
  {
    get {
      return this.province_or_territoryField;
    }
    set {
      this.province_or_territoryField = value;
    }
  }

  /// <remarks/>
  public decimal latitude
  {
    get {
      return this.latitudeField;
    }
    set {
      this.latitudeField = value;
    }
  }

  /// <remarks/>
  public decimal longitude
  {
    get {
      return this.longitudeField;
    }
    set {
      this.longitudeField = value;
    }
  }

  /// <remarks/>
  public decimal elevation
  {
    get {
      return this.elevationField;
    }
    set {
      this.elevationField = value;
    }
  }

  /// <remarks/>
  public uint climate_identifier
  {
    get {
      return this.climate_identifierField;
    }
    set {
      this.climate_identifierField = value;
    }
  }

  /// <remarks/>
  public uint wmo_identifier
  {
    get {
      return this.wmo_identifierField;
    }
    set {
      this.wmo_identifierField = value;
    }
  }

  /// <remarks/>
  public string tc_identifier
  {
    get {
      return this.tc_identifierField;
    }
    set {
      this.tc_identifierField = value;
    }
  }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class climatedataFlag
{

  private string symbolField;

  private string descriptionField;

  /// <remarks/>
  public string symbol
  {
    get {
      return this.symbolField;
    }
    set {
      this.symbolField = value;
    }
  }

  /// <remarks/>
  public string description
  {
    get {
      return this.descriptionField;
    }
    set {
      this.descriptionField = value;
    }
  }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class climatedataMonth
{

  private climatedataMonthDay[] dayField;

  private byte indexField;

  /// <remarks/>
  [System.Xml.Serialization.XmlElementAttribute("day")]
  public climatedataMonthDay[] day
  {
    get {
      return this.dayField;
    }
    set {
      this.dayField = value;
    }
  }

  /// <remarks/>
  [System.Xml.Serialization.XmlAttributeAttribute()]
  public byte index
  {
    get {
      return this.indexField;
    }
    set {
      this.indexField = value;
    }
  }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class climatedataMonthDay
{

  private climatedataMonthDayTemperature[] temperatureField;

  private climatedataMonthDayPrecipitation[] precipitationField;

  private climatedataMonthDayPop popField;

  private byte indexField;

  /// <remarks/>
  [System.Xml.Serialization.XmlElementAttribute("temperature")]
  public climatedataMonthDayTemperature[] temperature
  {
    get {
      return this.temperatureField;
    }
    set {
      this.temperatureField = value;
    }
  }

  /// <remarks/>
  [System.Xml.Serialization.XmlElementAttribute("precipitation")]
  public climatedataMonthDayPrecipitation[] precipitation
  {
    get {
      return this.precipitationField;
    }
    set {
      this.precipitationField = value;
    }
  }

  /// <remarks/>
  public climatedataMonthDayPop pop
  {
    get {
      return this.popField;
    }
    set {
      this.popField = value;
    }
  }

  /// <remarks/>
  [System.Xml.Serialization.XmlAttributeAttribute()]
  public byte index
  {
    get {
      return this.indexField;
    }
    set {
      this.indexField = value;
    }
  }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class climatedataMonthDayTemperature
{

  private string classField;

  private string unitsField;

  private string unitTypeField;

  private ushort yearField;

  private bool yearFieldSpecified;

  private string periodField;

  private decimal valueField;

  /// <remarks/>
  [System.Xml.Serialization.XmlAttributeAttribute()]
  public string @class
  {
    get {
      return this.classField;
    }
    set {
      this.classField = value;
    }
  }

  /// <remarks/>
  [System.Xml.Serialization.XmlAttributeAttribute()]
  public string units
  {
    get {
      return this.unitsField;
    }
    set {
      this.unitsField = value;
    }
  }

  /// <remarks/>
  [System.Xml.Serialization.XmlAttributeAttribute()]
  public string unitType
  {
    get {
      return this.unitTypeField;
    }
    set {
      this.unitTypeField = value;
    }
  }

  /// <remarks/>
  [System.Xml.Serialization.XmlAttributeAttribute()]
  public ushort year
  {
    get {
      return this.yearField;
    }
    set {
      this.yearField = value;
    }
  }

  /// <remarks/>
  [System.Xml.Serialization.XmlIgnoreAttribute()]
  public bool yearSpecified
  {
    get {
      return this.yearFieldSpecified;
    }
    set {
      this.yearFieldSpecified = value;
    }
  }

  /// <remarks/>
  [System.Xml.Serialization.XmlAttributeAttribute()]
  public string period
  {
    get {
      return this.periodField;
    }
    set {
      this.periodField = value;
    }
  }

  /// <remarks/>
  [System.Xml.Serialization.XmlTextAttribute()]
  public decimal Value
  {
    get {
      return this.valueField;
    }
    set {
      this.valueField = value;
    }
  }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class climatedataMonthDayPrecipitation
{

  private string classField;

  private string unitsField;

  private string unitTypeField;

  private ushort yearField;

  private string periodField;

  private decimal valueField;

  /// <remarks/>
  [System.Xml.Serialization.XmlAttributeAttribute()]
  public string @class
  {
    get {
      return this.classField;
    }
    set {
      this.classField = value;
    }
  }

  /// <remarks/>
  [System.Xml.Serialization.XmlAttributeAttribute()]
  public string units
  {
    get {
      return this.unitsField;
    }
    set {
      this.unitsField = value;
    }
  }

  /// <remarks/>
  [System.Xml.Serialization.XmlAttributeAttribute()]
  public string unitType
  {
    get {
      return this.unitTypeField;
    }
    set {
      this.unitTypeField = value;
    }
  }

  /// <remarks/>
  [System.Xml.Serialization.XmlAttributeAttribute()]
  public ushort year
  {
    get {
      return this.yearField;
    }
    set {
      this.yearField = value;
    }
  }

  /// <remarks/>
  [System.Xml.Serialization.XmlAttributeAttribute()]
  public string period
  {
    get {
      return this.periodField;
    }
    set {
      this.periodField = value;
    }
  }

  /// <remarks/>
  [System.Xml.Serialization.XmlTextAttribute()]
  public decimal Value
  {
    get {
      return this.valueField;
    }
    set {
      this.valueField = value;
    }
  }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class climatedataMonthDayPop
{

  private string unitsField;

  private decimal valueField;

  /// <remarks/>
  [System.Xml.Serialization.XmlAttributeAttribute()]
  public string units
  {
    get {
      return this.unitsField;
    }
    set {
      this.unitsField = value;
    }
  }

  /// <remarks/>
  [System.Xml.Serialization.XmlTextAttribute()]
  public decimal Value
  {
    get {
      return this.valueField;
    }
    set {
      this.valueField = value;
    }
  }
}

