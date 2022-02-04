using System.Text.RegularExpressions;

namespace Radar;

public class WebDirectoryLoader
{
  public async Task<List<string>> UseRegex(string url)
  {
    using var client = new HttpClient();
    var response = await client.GetAsync(url).ConfigureAwait(false);
    if (response == null || response.StatusCode == System.Net.HttpStatusCode.NotFound) throw new Exception($"@@@@@@@@@@ {url}  is problematic!!!");
    var html = await response.Content.ReadAsStringAsync();
    return new Regex("<a href=\".*\">(?<name>.*)</a>").Matches(html).Where(r => r.Success).TakeLast(8).Select(r => r.Groups["name"].ToString()).ToList();//.ForEach(r => WriteLine(r.Groups["name"]));
  }
}