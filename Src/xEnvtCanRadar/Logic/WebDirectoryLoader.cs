namespace Radar;

public class WebDirectoryLoader
{
  const int lastMany = 60;
  public async Task<List<string>> ParseFromHtmlUsingRegex(string url)
  {
    using var client = new HttpClient();
    var response = await client.GetAsync(url).ConfigureAwait(false);
    if (response == null || response.StatusCode == System.Net.HttpStatusCode.NotFound) throw new Exception($"@@@@@@@@@@ {url}  is problematic!!!");
    var html = await response.Content.ReadAsStringAsync();
    return new Regex("<a href=\".*\">(?<name>.*)</a>").Matches(html).Where(r => r.Success).TakeLast(lastMany).Select(r => r.Groups["name"].ToString()).ToList();//.ForEach(r => WriteLine(r.Groups["name"]));
  }
}