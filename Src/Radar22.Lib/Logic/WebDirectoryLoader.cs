namespace Radar;

public class WebDirectoryLoader
{
  public async Task<List<string>> ParseFromHtmlUsingRegex(string gigUrl, string endsWith = ".gif", int takeLastCount = 11)
  {
    using var client = new HttpClient();
    var response = await client.GetAsync(gigUrl).ConfigureAwait(false);
    if (response == null || response.StatusCode == System.Net.HttpStatusCode.NotFound) throw new Exception($"@@@@@@@@@@  {gigUrl}  is problematic!!!");
    var html = await response.Content.ReadAsStringAsync();
    return new Regex("<a href=\".*\">(?<name>.*)</a>").Matches(html)
      .Where(r => r.Success)
      .Select(r => r.Groups["name"].ToString())
      .Where(r => r.EndsWith(endsWith, StringComparison.OrdinalIgnoreCase))
      .TakeLast(takeLastCount)
      .OrderBy(r => r)
      .ToList();
  }
}