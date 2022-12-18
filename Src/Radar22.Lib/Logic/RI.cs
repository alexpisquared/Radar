namespace xEnvtCanRadar.Logic;

public record RI
{
  public string GifUrl { get; set; } = "@@@@@@@@@@@";
  public string FileName { get; internal set; } = default!;
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
}
