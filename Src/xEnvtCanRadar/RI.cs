namespace xEnvtCanRadar
{
  public record RI
  {
    public string GifUrl { get; set; } = "@@@@@@@@@@@";
    public string FileName { get; internal set; }
    public DateTimeOffset ImgTime { get; internal set; }
  }
}