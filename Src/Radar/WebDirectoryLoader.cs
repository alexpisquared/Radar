using System.Text.RegularExpressions;

namespace Radar;

public class WebDirectoryLoader
{
  public async Task UseRegex(string url)
  {
    using var client = new HttpClient();
    var response = await client.GetAsync(url).ConfigureAwait(false);
    if (response == null || response.StatusCode == System.Net.HttpStatusCode.NotFound) throw new Exception("@@@@@@@@@@");
    var html = await response.Content.ReadAsStringAsync();
    new Regex("<a href=\".*\">(?<name>.*)</a>").Matches(html).Where(r => r.Success).TakeLast(8).ToList().ForEach(r => WriteLine(r.Groups["name"]));
  }
}