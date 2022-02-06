namespace Radar;

public class WebDirectoryLoader
{
  const int lastMany = 100;
  public async Task<List<string>> ParseFromHtmlUsingRegex(string url, string endswith = ".gif")
  {
    using var client = new HttpClient();
    var response = await client.GetAsync(url).ConfigureAwait(false);
    if (response == null || response.StatusCode == System.Net.HttpStatusCode.NotFound) throw new Exception($"@@@@@@@@@@  {url}  is problematic!!!");
    var html = await response.Content.ReadAsStringAsync();
    return new Regex("<a href=\".*\">(?<name>.*)</a>").Matches(html)
      .Where(r => r.Success)
      .Select(r => r.Groups["name"].ToString())
      .Where(r => r.EndsWith(endswith))
      .TakeLast(lastMany).ToList();
  }
}