using System;
using System.Drawing;

namespace RadarPicCollect
{
  public class PicDetail
	{
		double _measure = -99; // cm/hr/km^2

		public PicDetail(Bitmap pic1, DateTime localTime1, string stationName, Point picOffset, string cacheName)
		{
			Bitmap = pic1;
			ImageTime = localTime1;
			StationName = stationName;
			PicOffset = picOffset;
			CacheName = cacheName;
		}

		public Bitmap Bitmap { get; set; }
		public DateTime ImageTime { get; set; }
		public string StationName { get; set; }
		public Point PicOffset { get; set; }
		public string CacheName { get; set; }
    public double Measure => _measure == -99 && Bitmap != null ? _measure = PicMea.CalcMphInTheArea(Bitmap, ImageTime) : _measure; // cm/hr/km^2
	}
}
