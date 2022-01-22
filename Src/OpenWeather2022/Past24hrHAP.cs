using HtmlAgilityPack;

namespace OpenWeather2022;

public class Past24hrHAP
{
  public List<Meteo> GetIt(string url)
  {
    List<Meteo> rv = new();
    string dateStr = default!;

    var web = new HtmlWeb();
    var doc = web.Load(url);

    foreach (var tr in doc.DocumentNode.SelectNodes("//table/tbody"))
    {
      WriteLine($"\n============ tr.ChildNodes.Count {tr.ChildNodes.Count}:");

      foreach (var r in tr.ChildNodes.Where(n => n.ChildNodes.Any()))
      {
        var nodes = r.ChildNodes;
        var cnt = nodes.Count;

        var i = 0;
        Write($"\n{cnt}:");
        foreach (var n in nodes) Write($" {i++}:'{n.InnerText.Replace("\n", "").Replace("\t", "").Replace("    ", " ").Replace("   ", " ").Replace("  ", " ").Trim()}' ");
        
        if (nodes.Count == 1)
        {
          dateStr = nodes[0].InnerText.Trim();
        }
        else if (nodes.Count >= 17)
        {
          try
          {
            var e4 = new Meteo { TempAir = -999 };
            e4.TakenAt = Convert.ToDateTime(dateStr + ' ' + nodes[1].InnerText);

            e4.Conditions = nodes[3].InnerText;

            var c5 = nodes[5].InnerText.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
            if (c5.Count() > 1)
              e4.TempAir = double.Parse(c5[1].Trim(' ').Trim('\n').Trim(' ').Trim('\n').Trim('↑').Trim('↓').Trim('(').Trim(')').Replace("(", "").Trim());
            else
              e4.TempAir = double.Parse(nodes[5].InnerText.Trim(' ').Trim('\n').Trim(' ').Trim('\n').Trim('↑').Trim('↓').Trim());

            var c7 = nodes[7].InnerText.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
            if (c7.Count() > 1)
              e4.Humidity = double.Parse(c7[1].Trim(' ').Trim('\n').Trim(' ').Trim('\n').Trim('↑').Trim('↓').Trim('(').Trim(')').Replace("(", "").Trim());
            else
              e4.Humidity = double.Parse(nodes[7].InnerText);

            //var c9 = c[9].InnerText.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
            //if (c9.Count() > 1)
            //  e4.DewPoint = double.Parse(c9[1].Trim(' ').Trim('\n').Trim(' ').Trim('\n').Trim('↑').Trim('↓').Trim('(').Trim(')').Replace("(", "").Trim());
            //else
            //  e4.DewPoint = double.Parse(c[9].InnerText);

            e4.Pressure = double.Parse(nodes[cnt - 8].InnerText);
            e4.Visibility = double.TryParse(nodes[cnt - 4].InnerText, out double d) ? d : 0;
            if (cnt != 27 && !nodes[13].InnerText.Contains("*"))
              e4.Humidex = double.Parse(nodes[13].InnerText);

            var s = nodes[11].InnerText.Trim();
            var w = s.Split(new char[] { ' ', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            switch (w.Length)
            {
              case 1: e4.WindDir = w[0]; e4.WindKmH = 0; break;
              case 2: e4.WindDir = w[0]; e4.WindKmH = Convert.ToDouble(w[1]); break;
              case 3: break;
              case 4: e4.WindDir = w[0]; e4.WindKmH = Convert.ToDouble(w[1]); e4.WindGust = Convert.ToDouble(w[3]); break;
              default: break;
            }

            if (e4 != null && e4.TempAir != -999)
              rv.Add(e4);
          }
          catch (Exception ex) { WriteLine($"@@@@@@@@ {ex.Message} \n\t {ex} @@@@@@@@@@"); if (Debugger.IsAttached) Debugger.Break(); else throw; }
        }
      }
    }

    //old: for (int curpos = 0, i = 0; i < 25; i++)			{				var  e = process1hourButtonvilleLikeEntry(ref s, ref sDate, ref html, ref curpos);				if (e != null && e.Pressure > 0)					ecdList.Add(e);			}

    //`for (int i = 0; i < ecdList.Count; i++) Console.WriteLine("{0,2}) {1}", i, ecdList[i].ToString());

    return rv;
  }
}

public class Meteo
{
  DateTime _t = DateTime.Now; public DateTime TakenAt { get => _t; set => _t = value; }
  public bool IsValid => true;
  public int SiteId { get; set; }
  public double TempAir { get; set; }
  public double TempSea { get; set; }
  public double Humidity { get; set; }
  public double DewPoint { get; set; }
  public double WindKmH { get; set; }
  public double WindGust { get; set; }
  public string WindDir { get; set; } = default!;
  public double Pressure { get; set; }
  public double Visibility { get; set; }
  public double? Humidex { get; set; }
  public double TempYesterdayMin { get; set; }
  public double TempYesterdayMax { get; set; }
  public double TempNormMin { get; set; }
  public double TempNormMax { get; set; }
  public double TempExtrMin { get; set; } // no data at the assoiciated page ==> goto http://www.weatheroffice.gc.ca/almanac/almanac_e.html?ykz
  public double TempExtrMax { get; set; }
  public int YearExtrMin { get; set; }
  public int YearExtrMax { get; set; }

  DateTime _r = DateTime.Today.AddHours(06); public DateTime SunRise { get => _r; set => _r = value; }
  DateTime _s = DateTime.Today.AddHours(18); public DateTime SunSet { get => _s; set => _s = value; }


  public double WaveHeight { get; set; }
  public double WavePeriod { get; set; }
  public string Conditions { get; set; } = default!;
  public string ConditiImg { get; set; } = default!;
  public string RawSrcText { get; set; } = default!;

  public override string ToString() => string.Format("{0}\t{1,3:N0}\t{2,3:N0}\t{3,3:N0}\t{4,3:N0}\t{5,3:N0}\t{6,6:N1}\t{7,3:N0}\t{8,3:N0}\t{9,3:N0}\t{10,3:N0}\t{11,3:N0}\t{12,3:N0} \t{13} \t{14} \t{15} \t{16}",
      TakenAt.ToString("ddMMMyy HH:mm"),       // 0
      TempAir,                                 // 1                                          
      Humidity,                                // 2
      DewPoint,                                // 3
      WindKmH,                                 // 4
      WindGust,                                // 5
      Pressure,                                // 6
      Visibility,                              // 7
      Humidex,                                 // 8
      TempYesterdayMin,  // 9
      TempYesterdayMax,  // 10
      TempNormMin,                             // 11
      TempNormMax,                             // 12
      SunRise.ToString("ddMMMyy HH:mm"),       // 13
      SunSet.ToString("ddMMMyy HH:mm"),        // 14
      Conditions,                              // 15
      WindDir);                                // 16
}
