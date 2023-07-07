namespace Radar22.Lib.Logic;

public class WebDirectoryLoader
{
  public async Task<List<RadarImageInfo>> ParseFromHtmlUsingRegex(string gigUrl, string precipitnFilter, int takeLastCount = 11)
  {
    try
    {
      using var client = new HttpClient();
      var response = await client.GetAsync(gigUrl).ConfigureAwait(false);
      if (response == null || response.StatusCode == System.Net.HttpStatusCode.NotFound) throw new Exception($"@@@@@@@@@@  {gigUrl}  is problematic!!!");
      var html = await response.Content.ReadAsStringAsync();

      foreach (var item in new Regex("<a href=\".*\">(?<name>.*)</a>").Matches(html)
        .Where(r => r.Success)
        .Select(r => r./*Groups["name"].*/ToString())
        .Where(r => r.Contains(precipitnFilter, StringComparison.OrdinalIgnoreCase))
        .TakeLast(takeLastCount)
        .OrderBy(r => r)
        .ToList()) { WriteLine(item); }

      return ParseFromHtmlUsingRegex2(gigUrl, precipitnFilter, takeLastCount, html);
    }
    catch (Exception ex) { WriteLine(ex.Message); throw; }
  }

  /// <summary>
  ///  parse the following lines from the html:
  ///   <img src="/icons/image2.gif" alt="[IMG]"> <a href = "202307051400_ONT_PRECIPET_SNOW_A11Y.gif" > 202307051400_ONT_PRECIPET_SNOW_A11Y.gif</a> 2023-07-05 14:05   18K
  ///   <img src="/icons/image2.gif" alt="[IMG]"> <a href = "202307051400_ONT_PRECIPET_SNOW_WT.gif" > 202307051400_ONT_PRECIPET_SNOW_WT.gif</a>   2023-07-05 14:05   19K
  ///   <img src="/icons/image2.gif" alt="[IMG]"> <a href = "202307051406_ONT_PRECIPET_RAIN_A11Y.gif" > 202307051406_ONT_PRECIPET_RAIN_A11Y.gif</a> 2023-07-05 14:11   18K
  ///    
  ///   <img src="/icons/image2.gif" alt="[IMG]"> <a href="202307051400_ONT_PRECIPET_SNOW_A11Y.gif">202307051400_ONT_PRECIPET_SNOW_A11Y.gif</a> 2023-07-05 14:05   18K  
  ///   <img src="/icons/image2.gif" alt="[IMG]"> <a href="202307051400_ONT_PRECIPET_SNOW_WT.gif">202307051400_ONT_PRECIPET_SNOW_WT.gif</a>   2023-07-05 14:05   19K  
  ///   <img src="/icons/image2.gif" alt="[IMG]"> <a href="202307051406_ONT_PRECIPET_RAIN_A11Y.gif">202307051406_ONT_PRECIPET_RAIN_A11Y.gif</a> 2023-07-05 14:11   18K  
  ///  into a list of RadarImageInfo objects:
  /// </summary>
  /// <param name="html"></param>
  /// <returns></returns>
  public List<RadarImageInfo> ParseFromHtmlUsingRegex2(string gigUrl, string precipitnFilter, int takeLastCount, string html)
  {
    var sw = Stopwatch.StartNew();
    var list = new List<RadarImageInfo>();
    try
    {
      var regex = new Regex(@"<a href="".*"">(?<FileName2>.*?)</a>.* (?<FileSizeКb>\d+)K", RegexOptions.Compiled | RegexOptions.Multiline);
      var matches = regex.Matches(html)
        .Where(r => r.Groups["FileName2"].ToString().Contains(precipitnFilter, StringComparison.OrdinalIgnoreCase))
        .TakeLast(takeLastCount)
        .OrderBy(r => r.Groups["FileName2"].ToString());
      var i = 0;
      foreach (var match in matches)
      {
        list.Add(new RadarImageInfo
        {
          Index = i++,
          FileName = match.Groups["FileName2"].Value,
          FileSizeКb = int.Parse(match.Groups["FileSizeКb"].Value),
          GifUrl = $"{gigUrl}/{match.Groups["FileName2"].Value}",
        });
      }
    }
    catch (Exception ex) { WriteLine($"■─■═■  {sw.Elapsed.TotalSeconds:N1}s  {ex.Message}  ■═■─■"); if (Debugger.IsAttached) Debugger.Break(); else { /*System.Windows.Clipboard.SetText(url);*/ throw; } }
    finally         /**/ { WriteLine($"+++++  {sw.Elapsed.TotalSeconds:N1}s  Success."); }

    return list;
  }

  public double CalulateSlope(List<RadarImageInfo> list)
  {
    double slope = 0;

    for (var i = 0; i < list.Count - 1; i++)
    {
      slope += list[i + 1].FileSizeКb - list[i].FileSizeКb;
    }

    return slope / list.Count;
  }

  public double CalulateAvgSize(List<RadarImageInfo> list) => list.Average(r => r.FileSizeКb);
}