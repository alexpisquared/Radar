using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Windows.Media.Imaging;
namespace PixelMeasure;
public static class PicMea // 2023: the latest!!!
{
  const int _x0 = 246, _y0 = 255, _radiusInPixelsX = 40, _radiusInPixelsY = 40;     //const int _x0 = 525, _y0 = 240, _radiusInPixelsX = 4, _radiusInPixelsY = 100; // color pallete area
  public static async Task<double> CalcMphInTheAreaAsync(string url, int x0 = _x0, int y0 = _y0) => CalcMphInTheArea(BitmapImage2Bitmap(await GetBitmapImageFromUrl(url)), x0, y0);
  public static async Task<BitmapImage> GetBitmapImageFromUrl(string url)
  {
    using var client = new HttpClient();
    using var response = await client.GetAsync(url);
    using var stream = await response.Content.ReadAsStreamAsync();
    var image = new BitmapImage();
    image.BeginInit();
    image.StreamSource = stream;
    image.CacheOption = BitmapCacheOption.OnLoad;
    image.EndInit();
    return image;
  }
  public static Bitmap BitmapImage2Bitmap(BitmapImage bitmapImage)
  {
    using var outStream = new MemoryStream();
    BitmapEncoder enc = new BmpBitmapEncoder();
    enc.Frames.Add(BitmapFrame.Create(bitmapImage));
    enc.Save(outStream);
    return new Bitmap(outStream);
  }
  public static Image BitmapImage2DrawingImage(BitmapImage bitmapImage)
  {
    using var outStream = new MemoryStream();
    BitmapEncoder enc = new BmpBitmapEncoder();
    enc.Frames.Add(BitmapFrame.Create(bitmapImage));
    enc.Save(outStream);
    var bitmap = new Bitmap(outStream);

    return bitmap;
  }

  public static Bitmap DrawSquare(Bitmap bmp, int x0, int y0)
  {
    // draw a 40x40 red square at the x0, yo center of the bmp:
    using var g = Graphics.FromImage(bmp);
    using var p = new Pen(Color.Red, 2);
    g.DrawRectangle(p, x0 - 20, y0 - 20, 40, 40);
    return bmp;
  }
  public static double CalcMphInTheArea(Bitmap bmp, int x0, int y0)
  {
    if (bmp == null) return -1;

    var sw = Stopwatch.StartNew();
    try
    {
      int ttlArea = 0, ttlMPH = 0;                                                    //var hmAry = new Color[] { bmp. GetPixel(249, 259), bmp. GetPixel(250, 259), bmp. GetPixel(249, 260), bmp. GetPixel(250, 260) };
      var bkgr = Color.FromArgb(255, 153, 153, 102);              //var hml = new List<Color>();
      var mmcnt = new Dictionary<int, int>();
      var cases = new SortedDictionary<string, int>();

      for (var x = -_radiusInPixelsX; x < _radiusInPixelsX; x++)
        for (var y = -_radiusInPixelsY; y < _radiusInPixelsY; y++, ttlArea++)
        {
          var pc = bmp.GetPixel(x0 + x, y0 + y);
          switch (pc.ToArgb())
          {
            default: addOrIncrementCount(cases, $"case {pc.ToArgb(),9}: addOrIncrementCount(mmcnt, -1); ttlMPH += -1; break; // {pc.Name,10}   RGB: {pc.R,3} {pc.G,3} {pc.B,3}   <Rectangle Fill=\"#{pc.Name}\" />"); break; //dev: addIfN(mmcnt, -003); 

            // lite
            case -13434778: addOrIncrementCount(mmcnt, 2000); ttlMPH += 2000; break; // 330066   RGB:  51   0 102   <Rectangle Fill="#ff330066" />     224
            case -00039220: addOrIncrementCount(mmcnt, 0240); ttlMPH += 0240; break; // ff66cc   RGB: 255 102 204   <Rectangle Fill="#ffff66cc" />       1
            case -13369549: addOrIncrementCount(mmcnt, 0020); ttlMPH += 0020; break; // 33ff33   RGB:  51 255  51   <Rectangle Fill="#ff33ff33" />       2
            case -06684775: addOrIncrementCount(mmcnt, 0001); ttlMPH += 0001; break; // 99ff99   RGB: 153 255 153   <Rectangle Fill="#ff99ff99" />    0.1 - 1 mm/hr => 1
            // dark
            case -10092391: addOrIncrementCount(mmcnt, 2000); ttlMPH += 2000; break; // 660099   RGB: 102     153  
            case -06736948: addOrIncrementCount(mmcnt, 1500); ttlMPH += 1500; break; // 9933cc   RGB: 153  51 204  
            case -00064871: addOrIncrementCount(mmcnt, 1000); ttlMPH += 1000; break; // ff0299   RGB: 255   2 153  
            case -00065536: addOrIncrementCount(mmcnt, 0750); ttlMPH += 0750; break; // ff0000   RGB: 255          
            case -00039424: addOrIncrementCount(mmcnt, 0500); ttlMPH += 0500; break; // FF6600   RGB:  51  51 102  
            case -00026368: addOrIncrementCount(mmcnt, 0360); ttlMPH += 0360; break; // ff9900   RGB: 255 153      
            case -00013312: addOrIncrementCount(mmcnt, 0240); ttlMPH += 0240; break; // ffcc00   RGB: 255 204   0  
            case -00000205: addOrIncrementCount(mmcnt, 0180); ttlMPH += 0180; break; // ffff33   RGB: 255 255  51  
            case -16751104: addOrIncrementCount(mmcnt, 0120); ttlMPH += 0120; break; // 006600   RGB:   0 102   0  
            case -16738048: addOrIncrementCount(mmcnt, 0070); ttlMPH += 0070; break; // 009900   RGB:   0 153   0  
            case -16724992: addOrIncrementCount(mmcnt, 0040); ttlMPH += 0040; break; // 00cc00   RGB:   0 204   0  
            case -16711834: addOrIncrementCount(mmcnt, 0020); ttlMPH += 0020; break; // 00ff66   RGB:   0 255 102  
            case -16737793: addOrIncrementCount(mmcnt, 0010); ttlMPH += 0010; break; // 0099ff   RGB:   0 153 255  
            case -06697729: addOrIncrementCount(mmcnt, 0001); ttlMPH += 0001; break; // 99ccff   RGB: 153 204 255  

            // lite
            case -00723724: // f4f4f4   RGB: 244 244 244  => 
            case -06710887: // 999999   RGB: 153 153 153  => 
            case -06840138: // 97a0b6   RGB: 151 160 182  => 
            case -16777216: // 000000   RGB:   0   0   0  => 
            // drk
            case -13421722: // 333366   RGB:  51  51 102  => Lake
            case -00000001: // ffffff   RGB: 255 255 255  => Cross
            case -06710938: // 999966   RGB: 153 153 102  => Land
              break;
          }
        }

#if flase
      var ttl = 0;
      ttl += dbgShow(mmcnt, 2000);
      ttl += dbgShow(mmcnt, 1500);
      ttl += dbgShow(mmcnt, 1000);
      ttl += dbgShow(mmcnt, 0750);
      ttl += dbgShow(mmcnt, 0500);
      ttl += dbgShow(mmcnt, 0360);
      ttl += dbgShow(mmcnt, 0240);
      ttl += dbgShow(mmcnt, 0180);
      ttl += dbgShow(mmcnt, 0120);
      ttl += dbgShow(mmcnt, 0080);
      ttl += dbgShow(mmcnt, 0040);
      ttl += dbgShow(mmcnt, 0020);
      ttl += dbgShow(mmcnt, 0010);
      ttl += dbgShow(mmcnt, 0001);
      ttl += dbgShow(mmcnt, 0000);
      int dbgShow(Dictionary<int, int> mmcnt, int k) { return mmcnt.ContainsKey(k) ? mmcnt[k] : 0; }           //	Debug.WriteLine(mmcnt.ContainsKey(k) ? mmcnt[k] : 0, k.ToString("0####"));
#endif

      var rv = ttlMPH * .1 / ttlArea;

      //77       Debug.WriteLine($":> mea @    {sw.Elapsed.TotalMilliseconds,5:N1} ms for  {_radiusInPixelsX}x{_radiusInPixelsY} radiusInPixels X*Y   => {ttlArea} area in pixels  ===> {rv:N5} mm/h/km²      {cases.Count} new cases");

      foreach (var c in cases) Debug.WriteLine($"      {c.Key}   {c.Value,5}");

      return rv;
    }
    catch (Exception) { /*ex.Log();*/ return -2; }
  }
  static void addOrIncrementCount(Dictionary<int, int> mmcnt, int k) { if (mmcnt.ContainsKey(k)) mmcnt[k]++; else mmcnt.Add(k, 1); }
  static void addOrIncrementCount(SortedDictionary<string, int> cases, string s) { if (cases.ContainsKey(s)) cases[s]++; else cases.Add(s, 1); }
}
