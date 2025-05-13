using Microsoft.AspNetCore.Mvc;
using NewsManagerAPI.Models;

namespace NewsManagerAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NewsController : ControllerBase
{
    private static List<NewsItem> News = new();

    [HttpGet]
    public IActionResult GetAll() => Ok(News);

    [HttpGet("{id}")]
    public IActionResult Get(int id)
    {
        var item = News.FirstOrDefault(x => x.Id == id);
        return item is null ? NotFound() : Ok(item);
    }

    [HttpPost]
    public IActionResult Add([FromBody] NewsItem item)
    {
        item.Id = News.Count > 0 ? News.Max(x => x.Id) + 1 : 1;
        News.Add(item);
        return CreatedAtAction(nameof(Get), new { id = item.Id }, item);
    }

    [HttpPut("{id}")]
    public IActionResult Update(int id, [FromBody] NewsItem updated)
    {
        var item = News.FirstOrDefault(x => x.Id == id);
        if (item is null) return NotFound();

        item.Title = updated.Title;
        item.Description = updated.Description;
        item.Url = updated.Url;
        return NoContent();
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var item = News.FirstOrDefault(x => x.Id == id);
        if (item is null) return NotFound();

        News.Remove(item);
        return NoContent();
    }
[HttpGet("public")]
public async Task<IActionResult> GetFromPublicApi(
    [FromServices] IHttpClientFactory httpClientFactory,
    [FromQuery] string country = "ua",
    [FromQuery] string category = "technology",
    [FromQuery] int pageSize = 5)
{
    var apiKey = "c00be78923ac44c48b41c979a9457d97";
    var url = $"https://newsapi.org/v2/top-headlines?country={country}&category={category}&pageSize={pageSize}&apiKey={apiKey}";

    var client = httpClientFactory.CreateClient();
    var response = await client.GetAsync(url);

    if (!response.IsSuccessStatusCode)
        return StatusCode((int)response.StatusCode, "Error fetching news");

    var content = await response.Content.ReadAsStringAsync();
    var result = System.Text.Json.JsonSerializer.Deserialize<NewsApiResponse>(content, new System.Text.Json.JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true
    });

    return Ok(result?.Articles);
}
}

