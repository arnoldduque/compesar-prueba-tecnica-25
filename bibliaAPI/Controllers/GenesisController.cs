using bibliaAPI.Model.Input;
using bibliaAPI.Model.Output;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace bibliaAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GenesisController(ILogger<GenesisController> logger,
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        BibleContext dbContext) : ControllerBase
    {
        private readonly ILogger<GenesisController> _logger = logger;
        private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
        private readonly IConfiguration _configuration = configuration;

        [HttpGet("{capitulo:int}", Name = "Genesis")]
        public async Task<IActionResult> GetAsync(int capitulo)
        {
            var apiUrl = _configuration["ExternalApi:BibleAPI:GenesisUrl"];
            var consulta = new Consulta();
            var tasks = new List<Task>();
            var semaphore = new SemaphoreSlim(1);
            var lockObj = new object();
            int waitAsyncCount = 0;
            consulta.Genesis = [];

            for (int i = 0; i < capitulo; i++)
            {
                await semaphore.WaitAsync();
                Interlocked.Increment(ref waitAsyncCount);
                int chapter = i + 1;
                tasks.Add(Task.Run(async () =>
                {
                    try
                    {
                        var client = _httpClientFactory.CreateClient();
                        int maxRetries = 3;
                        int retryCount = 0;
                        TimeSpan delay = TimeSpan.FromSeconds(18);                        

                        while (retryCount <= maxRetries)
                        {
                            var response = await client.GetAsync($"{apiUrl}/{chapter}");
                            if (response.IsSuccessStatusCode)
                            {
                                var content = await response.Content.ReadAsStringAsync();
                                var book = JsonSerializer.Deserialize<BibleBook>(content);                                
                                if (book != null)
                                {
                                    lock (lockObj)
                                    {
                                        var libro = new Libro
                                        {
                                            Chapter = chapter,
                                            Verses = []
                                        };
                                        foreach (var v in book!.verses!)
                                        {
                                            libro.Verses!.Add(new Verso
                                            {
                                                Verse = v.verse,
                                                Text = v.text
                                            });
                                        }

                                        consulta.Genesis.Add(libro);
                                    }
                                }
                                break;
                            }
                            else if ((int)response.StatusCode == 429)
                            {
                                _logger.LogWarning($"Received '429 TooManyRequests' for chapter {chapter}. Retry {retryCount + 1}/{maxRetries}.");

                                if (response.Headers.TryGetValues("Retry-After", out var values) && int.TryParse(values.FirstOrDefault(), out int retryAfterSeconds))
                                {
                                    delay = TimeSpan.FromSeconds(retryAfterSeconds);
                                }
                                await Task.Delay(delay);
                                retryCount++;
                            }
                            else if ((int)response.StatusCode == 404)
                            {
                                _logger.LogError($" ---------------------> External API call failed for chapter {chapter}. StatusCode: {response.StatusCode}");
                                i = capitulo;
                                break;
                            }
                            else
                            {
                                _logger.LogError($" ---------------------> External API call failed for chapter {chapter}. StatusCode: {response.StatusCode}");
                                break;
                            }
                        }
                    }
                    finally
                    {
                        semaphore.Release();
                    }
                }));
            }
            dbContext.Consultas.Add(consulta);
            await Task.WhenAll(tasks);            
            await dbContext.SaveChangesAsync();        
            _logger.LogInformation("SemaphoreSlim.WaitAsync was called {Count} times.", waitAsyncCount);
            return Ok(consulta);
        }
    }
}
