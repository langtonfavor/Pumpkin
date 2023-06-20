using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

public class BeerRandomEmailService : BackgroundService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private readonly ILogger<BeerRandomEmailService> _logger;

    public BeerRandomEmailService(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<BeerRandomEmailService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            // Check if it's Friday
            if (DateTime.Today.DayOfWeek == DayOfWeek.Friday)
            {
                try
                {
                    // Call the /beer/random endpoint
                    var httpClient = _httpClientFactory.CreateClient();
                    var response = await httpClient.GetAsync("http://localhost:7112/api/beer/random");
                    if (response.IsSuccessStatusCode)
                    {
                        var beer = await response.Content.ReadAsStringAsync();

                        // Send the beer data to the pre-populated email list
                        // Implement your email sending logic here
                        SendEmailToPrePopulatedList(beer);

                        _logger.LogInformation("Beer random email sent successfully.");
                    }
                    else
                    {
                        _logger.LogError("Failed to retrieve random beer.");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while sending the beer random email.");
                }
            }

            // Wait for 2 weeks (14 days) before checking again
            await Task.Delay(TimeSpan.FromDays(14), stoppingToken);
        }
    }

    private void SendEmailToPrePopulatedList(string beerData)
    {
        // Implement your email sending logic here
        // Use the beer data to compose the email content
    }
}
