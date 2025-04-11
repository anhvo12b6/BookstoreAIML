using API;
using BookstoreAIML.Models;
using BookstoreAIML.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class GeminiController : ControllerBase
{
    private readonly DataContext _context;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly HttpClient _httpClient;

    private const string ApiKey = "AIzaSyCEIY0OZZZT6EBpbZO7ZBIPt4remqoZLHE";
    private const string GeminiUrl = $"https://generativelanguage.googleapis.com/v1/models/gemini-2.0-flash:generateContent?key={ApiKey}";

    public GeminiController(DataContext context, IHttpClientFactory httpClientFactory)
    {
        _context = context;
        _httpClientFactory = httpClientFactory;
        _httpClient = httpClientFactory.CreateClient();
    }

    [HttpPost("chat")]
    public async Task<IActionResult> Chat([FromBody] ChatRequest request)
    {
        var requestBody = new
        {
            contents = new[]
            {
                new {
                    parts = new[] { new { text = request.Message } }
                }
            }
        };

        var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(GeminiUrl, content);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            return StatusCode((int)response.StatusCode, $"Lỗi: {error}");
        }

        var responseContent = await response.Content.ReadAsStringAsync();
        var json = JsonDocument.Parse(responseContent);
        var result = json.RootElement
            .GetProperty("candidates")[0]
            .GetProperty("content")
            .GetProperty("parts")[0]
            .GetProperty("text")
            .GetString();

        return Ok(new { reply = result });
    }

    private async Task<List<ProductViewModel>> GetRelevantProducts(string userQuestion)
    {
        return await _context.Products.Take(10).ToListAsync();

    }

    private string BuildPrompt(string userQuestion, List<ProductViewModel> products)
    {
        var productList = string.Join("\n", products.Select(p => $"- {p.Title}: {p.Description}, giá {p.Price} VND"));

        return $"""
    Bạn là một trợ lý ảo tên là Sờ Gu. Trả lời bằng tiếng Việt, thân thiện và hữu ích.

    Dưới đây là danh sách các sản phẩm (sách) hiện có trong cửa hàng:
    {productList}

    Nếu người dùng hỏi về sách, hãy dùng thông tin trong danh sách để trả lời.
    Nếu người dùng hỏi về kiến thức ngoài sách (như học lập trình, lời khuyên, gợi ý...), bạn cũng có thể trả lời như một trợ lý thông minh.

    Câu hỏi của người dùng: "{userQuestion}"
    """;
    }



    private async Task<string> AskGeminiAsync(string prompt)
    {
        var body = new
        {
            contents = new[]
            {
                new {
                    parts = new[] { new { text = prompt } }
                }
            }
        };

        var response = await _httpClient.PostAsJsonAsync(GeminiUrl, body);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            return $"Lỗi gọi Gemini API: {error}";
        }

        var result = await response.Content.ReadFromJsonAsync<JsonElement>();

        return result.GetProperty("candidates")[0]
                     .GetProperty("content")
                     .GetProperty("parts")[0]
                     .GetProperty("text")
                     .GetString();
    }

    [HttpPost("ask")]
    public async Task<IActionResult> Ask([FromBody] string question)
    {
        if (string.IsNullOrWhiteSpace(question))
            return BadRequest("Câu hỏi không được để trống.");

        var products = await GetRelevantProducts(question);
        var prompt = BuildPrompt(question, products);
        var answer = await AskGeminiAsync(prompt);
        return Ok(new { reply = answer });
        Console.WriteLine("===== Prompt gửi Gemini =====");
        Console.WriteLine(prompt);

    }
}
