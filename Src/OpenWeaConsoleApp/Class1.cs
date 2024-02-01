using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenWeaConsoleApp;


// NOTE: Generated code may require at least .NET Framework 4.5 or .NET Core/Standard 2.0.
/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
[System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
public partial class siteList
{

  private siteListSite[] siteField;

  /// <remarks/>
  [System.Xml.Serialization.XmlElementAttribute("site")]
  public siteListSite[] site
  {
    get {
      return this.siteField;
    }
    set {
      this.siteField = value;
    }
  }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class siteListSite
{

  private string nameEnField;

  private string nameFrField;

  private string provinceCodeField;

  private string codeField;

  /// <remarks/>
  public string nameEn
  {
    get {
      return this.nameEnField;
    }
    set {
      this.nameEnField = value;
    }
  }

  /// <remarks/>
  public string nameFr
  {
    get {
      return this.nameFrField;
    }
    set {
      this.nameFrField = value;
    }
  }

  /// <remarks/>
  public string provinceCode
  {
    get {
      return this.provinceCodeField;
    }
    set {
      this.provinceCodeField = value;
    }
  }

  /// <remarks/>
  [System.Xml.Serialization.XmlAttributeAttribute()]
  public string code
  {
    get {
      return this.codeField;
    }
    set {
      this.codeField = value;
    }
  }
}

