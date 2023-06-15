using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "PunkAPI", Description = "Keep track of your tasks", Version = "v1" });
});

// Add services
builder.Services.AddHttpClient();

var app = builder.Build();

// Enable Swagger
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "PunkApiService v1");
});


// Configure endpoints
app.MapGet("/beer/menu", async (HttpContext httpContext) =>
{
    var httpClient = httpContext.RequestServices.GetRequiredService<HttpClient>();
    var response = await httpClient.GetAsync("https://api.punkapi.com/v2/beers");
    if (response.IsSuccessStatusCode)
    {
        var beers = await response.Content.ReadAsStringAsync();
        await httpContext.Response.WriteAsJsonAsync(beers); // Write the beers as JSON response
    }
    else
    {
        httpContext.Response.StatusCode = (int)response.StatusCode;
        await httpContext.Response.WriteAsJsonAsync("Failed to retrieve beer menu");
    }
});

app.MapGet("/beer/{id}", async (HttpContext httpContext) =>
{
    var httpClient = httpContext.RequestServices.GetRequiredService<HttpClient>();
    var id = httpContext.Request.RouteValues["id"]?.ToString();
    var response = await httpClient.GetAsync($"https://api.punkapi.com/v2/beers/{id}");
    if (response.IsSuccessStatusCode)
    {
        var beer = await response.Content.ReadAsStringAsync();
        await httpContext.Response.WriteAsJsonAsync(beer); // Write the beer as JSON response
    }
    else
    {
        httpContext.Response.StatusCode = (int)response.StatusCode;
        await httpContext.Response.WriteAsJsonAsync("Failed to retrieve beer by ID");
    }
});

app.MapGet("/beer/random", async (HttpContext httpContext) =>
{
    var httpClient = httpContext.RequestServices.GetRequiredService<HttpClient>();
    var response = await httpClient.GetAsync("https://api.punkapi.com/v2/beers/random");
    if (response.IsSuccessStatusCode)
    {
        var beer = await response.Content.ReadAsStringAsync();
        await httpContext.Response.WriteAsJsonAsync(beer); // Write the beer as JSON response
    }
    else
    {
        httpContext.Response.StatusCode = (int)response.StatusCode;
        await httpContext.Response.WriteAsJsonAsync("Failed to retrieve random beer");
    }
});

app.MapGet("/search", async (HttpContext httpContext) =>
{
    var httpClient = httpContext.RequestServices.GetRequiredService<HttpClient>();
    var query = httpContext.Request.Query["query"];
    var response = await httpClient.GetAsync($"https://api.punkapi.com/v2/beers?beer_name={query}");
    if (response.IsSuccessStatusCode)
    {
        var beers = await response.Content.ReadAsStringAsync();
        await httpContext.Response.WriteAsJsonAsync(beers); // Write the beers as JSON response
    }
    else
    {
        httpContext.Response.StatusCode = (int)response.StatusCode;
        await httpContext.Response.WriteAsJsonAsync("Failed to search for beers");
    }
});

app.Run();
