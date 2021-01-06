using AAV.Sys.Ext;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;

namespace RadarPicCollect
{
  public static class PicMea
  {
    public static double CalcMphInTheArea(Bitmap bmp, DateTime imgTime, int radiusInPixels = 40) // only for dark theme ...i think (Jan 2021)
    {
      if (bmp == null) return -1;

      var sw = Stopwatch.StartNew();
      try
      {
        int ttlArea = 0, ttlMPH = 0;                                                    //var hmAry = new Color[] { bmp.GetPixel(249, 259), bmp.GetPixel(250, 259), bmp.GetPixel(249, 260), bmp.GetPixel(250, 260) };
        var bkgr = Color.FromArgb(255, 153, 153, 102);              //var hml = new List<Color>();
        var mmcnt = new Dictionary<int, int>();
        var cases = new SortedDictionary<string, int>();

        for (int x0 = 250, x = -radiusInPixels; x < radiusInPixels; x++)
        {
          for (int y0 = 260, y = -radiusInPixels; y < radiusInPixels; y++, ttlArea++)
          {
            var pc = bmp.GetPixel(x0 + x, y0 + y);
            switch (pc.ToArgb())
            {
              default: addOrIncrementCount(cases, $"case {pc.ToArgb(),9}: addOrIncrementCount(mmcnt, -1); ttlMPH += -1; break; // {pc.Name,10}   RGB: {pc.R,3} {pc.G,3} {pc.B,3}   <Rectangle Fill=\"#{pc.Name}\" />"); break; //dev: addIfN(mmcnt, -003); 

              // lite
              case -00039220: addOrIncrementCount(mmcnt, -1); ttlMPH += -1; break; //   ffff66cc   RGB: 255 102 204   <Rectangle Fill="#ffff66cc" />       1
              case -13369549: addOrIncrementCount(mmcnt, -1); ttlMPH += -1; break; //   ff33ff33   RGB:  51 255  51   <Rectangle Fill="#ff33ff33" />       2
              case -06684775: addOrIncrementCount(mmcnt, 01); ttlMPH += 01; break; //   ff99ff99   RGB: 153 255 153   <Rectangle Fill="#ff99ff99" />    0.1 - 1 mm/hr => 1

              // dark
              case -10092391: addOrIncrementCount(mmcnt, 2000); ttlMPH += 2000; break;   // ff660099, 660099   RGB: 102  153  => .
              case -06736948: addOrIncrementCount(mmcnt, 1500); ttlMPH += 1500; break;   // ff9933cc, 9933cc   RGB: 153 51 204  => .
              case -00064871: addOrIncrementCount(mmcnt, 1000); ttlMPH += 1000; break;   // ffff0299, ff0299   RGB: 255 2 153  => .
              case -00065536: addOrIncrementCount(mmcnt, 0750); ttlMPH += 0750; break;   // ffff0000, ff0000   RGB: 255    => .
              case -00039424: addOrIncrementCount(mmcnt, 0500); ttlMPH += 0500; break;   // FFFF6600,    ???   RGB: 51 51 102  => .
              case -00026368: addOrIncrementCount(mmcnt, 0360); ttlMPH += 0360; break;   // ffff9900, ff9900   RGB: 255 153   => .
              case -00013312: addOrIncrementCount(mmcnt, 0240); ttlMPH += 0240; break;   // ffffcc00, ffcc00   RGB: 255 204   0  => .
              case -00000205: addOrIncrementCount(mmcnt, 0180); ttlMPH += 0180; break;   // ffffff33, ffff33   RGB: 255 255  51  => .
              case -16751104: addOrIncrementCount(mmcnt, 0120); ttlMPH += 0120; break;   // ff006600, 006600   RGB:   0 102   0  => .
              case -16738048: addOrIncrementCount(mmcnt, 0080); ttlMPH += 0080; break;   // ff009900, 009900   RGB:   0 153   0  => .
              case -16724992: addOrIncrementCount(mmcnt, 0040); ttlMPH += 0040; break;   // ff00cc00, 00cc00   RGB:   0 204   0  => .
              case -16711834: addOrIncrementCount(mmcnt, 0020); ttlMPH += 0020; break;   // ff00ff66, 00ff66   RGB:   0 255 102  => .
              case -16737793: addOrIncrementCount(mmcnt, 0010); ttlMPH += 0010; break;   // ff0099ff, 0099ff   RGB:   0 153 255  => .
              case -06697729: addOrIncrementCount(mmcnt, 0001); ttlMPH += 0001; break;   // ff99ccff, 99ccff   RGB: 153 204 255  => .

              // lite
              case -00723724:                                         //   fff4f4f4   RGB: 244 244 244   <Rectangle Fill="#fff4f4f4" />    3154
              case -06710887:                                         //   ff999999   RGB: 153 153 153   <Rectangle Fill="#ff999999" />      21
              case -06840138:                                         //   ff97a0b6   RGB: 151 160 182   <Rectangle Fill="#ff97a0b6" />    1399

              // dark
              case -13421722:                                                  // ff333366, RGB:  51  51 102  => Lake
              case -00000001:                                                  // ffffffff, RGB: 255 255 255  => Cross
              case -06710938: addOrIncrementCount(mmcnt, 0000); break;                   // ff999966, RGB: 153 153 102  => Land
            }
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

        //77 Debug.WriteLine(":> {0:HH:mm} {1,5:N1} ms for {2} radiusInPixels => {3} area in pixels  ===> {4:N5} mm/h/km²", imgTime, sw.Elapsed.TotalMilliseconds, radiusInPixels, ttlArea, rv);

        foreach (var c in cases)
        {
          Debug.WriteLine($"   {c.Key}   {c.Value,5}");
        }

        return rv;
      }
      catch (Exception ex) { ex.Log(); return -2; }
    }

    static void addOrIncrementCount(Dictionary<int, int> mmcnt, int k) { if (mmcnt.ContainsKey(k)) mmcnt[k]++; else mmcnt.Add(k, 1); }
    static void addOrIncrementCount(SortedDictionary<string, int> cases, string s) { if (cases.ContainsKey(s)) cases[s]++; else cases.Add(s, 1); }
  }
}
