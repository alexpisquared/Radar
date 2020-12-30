using System;
using System.Diagnostics;

namespace Radar.Properties
{
  internal sealed partial class Settings {
        
    public static double AlarmThreshold { get; internal set; } = 0.2;
    public static DateTime PopUp_LastTime { get; internal set; }

    internal static void Save() => Trace.Write(" throw new NotImplementedException(); \n");
  }
}
