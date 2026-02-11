namespace Radar22.Lib.Logic;

public class RadarImageInfo
{
  public string FileName { get; internal set; } = default!;
  public int FileSizeКb { get; set; } = -1;
  public string GifUrl { get; set; } = "@@@@@@@@@@@";
  public DateTimeOffset ImgTime => ToLocalDate(FileName);
  static DateTimeOffset ToLocalDate(string gifName)
  {
    var yyyyMMddHHmm = gifName.Split('_')[0];

    /// 20220203T2006Z_MSC_Radar-DPQPE_CASKR_Rain.gif 2022-02-03 20:
    /// 202202032200_CASKR_COMP_PRECIPET_SNOW.gif  
    if (!DateTime.TryParseExact(yyyyMMddHHmm, new string[] { "yyyyMMddTHHmmZ", "yyyyMMddHHmm" }, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out var utc)) //var lcl = DateTime.ParseExact(yyyyMMddHHmm, "yyyyMMddHHmm", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal).ToLocalTime(); //tu: UTC to Local time.
      throw new ArgumentOutOfRangeException(nameof(yyyyMMddHHmm), yyyyMMddHHmm, "■─■─■ Can you imagine?!?!?!");

    return utc.ToLocalTime(); //tu: UTC to Local time.
  }

  //bool _letGet;


  public int Index { get; internal set; }

  public async Task<long> GetFileSizeAsync(string url)
  {
    var sw = Stopwatch.StartNew();
    try
    {
      using var client = new HttpClient();
      var response = await client.GetAsync(url).ConfigureAwait(false);
      if (response.IsSuccessStatusCode && response.Content.Headers.Contains("Content-Length"))
        return response.Content.Headers.ContentLength ?? -888;

      if (response == null || response.StatusCode == System.Net.HttpStatusCode.NotFound)
        return -4;
    }
    catch (Exception ex) { WriteLine($"■─■═■  {sw.Elapsed.TotalSeconds:N1}s  {url}  {ex.Message}  ■═■─■"); if (Debugger.IsAttached) Debugger.Break(); else { /*System.Windows.Clipboard.SetText(url);*/ throw; } }
    finally         /**/ { WriteLine($"+++++  {sw.Elapsed.TotalSeconds:N1}s  {url}  Success."); }

    return -3;
  }
}
