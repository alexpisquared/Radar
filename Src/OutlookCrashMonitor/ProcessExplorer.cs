﻿using System.Diagnostics;
using System.IO;
namespace OutlookCrashMonitor;
/*
Auth, roles, etc.
https://youtu.be/Nw4AZs1kLAs?t=14335

Net9.0:
https://github.com/gefrank/Mango-Microservices/tree/master

Mail Send: TrainCloud.Microservices.Email/TrainCloud.Microservices.Email/Services/Email/EmailService.cs at master · traincloud-net/TrainCloud.Microservices.Email

check if Outlook crashed:
*/

public class ProcessExplorer
{
  const string
      _outlookProcessName = "olk",
      _explorerProcessName = "explorer",
      _explorerProcessPath = @"C:\Windows\explorer.exe";
  static string
      _outlookProcessPath = @$"C:\Users\{Environment.UserName}\AppData\Local\Microsoft\WindowsApps\olk.exe";

  public static bool IsRunningCheckAndRestartIfFalse_Outlook(string processPathName) => IsRunningCheckAndRestartIfFalse(processPathName);
  public static bool IsRunningCheckAndRestartIfFalse_Explorer(string processPathName) => IsRunningCheckAndRestartIfFalse(processPathName);
  public static bool IsRunningCheckAndRestartIfFalse(string processPath)
  {
    try
    {
      if (IsRunning(Path.GetFileNameWithoutExtension(processPath)))
      {
        return true;
      }

      _ = Process.Start(processPath);
    }
    catch (Exception ex) { Debug.WriteLine(ex.Message); Console.Beep(444, 444); }

    return false;
  }
  public static bool IsRunning(string processName)
  {
    foreach (var process in Process.GetProcessesByName(processName)) Trace.WriteLine($"+ {process.ProcessName,-32}  {process.Id,6}  {process.HandleCount,6}  {process.MainWindowHandle,6}  {process.MainWindowTitle}");

    if (Process.GetProcessesByName(processName).Where(r => r.MainWindowHandle > 0 && r.MainWindowTitle.Length == 0).Any())
    {
      return true;
    }

    if (Process.GetProcesses().FirstOrDefault(p => p.ProcessName == processName) is not null)
    {
      return true;
    }

    foreach (var process in Process.GetProcesses().Where(p => p.ProcessName == processName))
    {
      Trace.WriteLine($"+ {process.ProcessName,-32} | {process.MainWindowTitle,-32}");
    }

    return false;
  }

#if DEBUG
  public static bool IsDbg => true;
#else
  public static bool IsDbg => false;
#endif
}