namespace OpenWeaWpfApp;

public static class UnhandledExceptionHndlr // Core 3
{
  public static void OnCurrentDispatcherUnhandledException(object s, DispatcherUnhandledExceptionEventArgs e)
  {
    if (e != null)
      e.Handled = true;

    var report = $"{s?.GetType().Name}.\n{e?.Exception.Message + e?.Exception.InnerException?.Message + e?.Exception.InnerException?.InnerException?.Message}\n\n{e?.Exception.StackTrace}";

    try
    {
      if (Debugger.IsAttached)
        Debugger.Break();
      else if (MessageBox.Show(report, "Unhandled Exception - Do you want to continue?", MessageBoxButton.YesNo, MessageBoxImage.Error, MessageBoxResult.Yes) == MessageBoxResult.No)
        Application.Current.Shutdown(44);
    }
    catch (Exception ex)
    {
      Environment.FailFast($"An error occurred while reportikng an error ...\n\n ...{ex.Message}...\n\n ...{report}", ex); //tu: http://blog.functionalfun.net/2013/05/how-to-debug-silent-crashes-in-net.html // Capturing dump files with Windows Error Reporting: Db a key at HKEY_LOCAL_MACHINE\Software\Microsoft\Windows\Windows Error Reporting\LocalDumps\[Your Application Exe FileName]. In that key, create a string value called DumpFolder, and set it to the folder where you want dumps to be written. Then create a DWORD value called DumpType with a value of 2.
    }
  }
}
