using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Text;
using Newtonsoft.Json; // Add this line

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "PunkAPI", Description = "Keep track of your tasks", Version = "v1" });
});

// Add services
builder.Services.AddHttpClient();

// Enable CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyOrigin() // Allow requests from any origin
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

// Enable Swagger
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "PunkApiService v1");
});

// Configure endpoints
app.MapGet("/api/beer/menu", async (HttpContext httpContext) =>
{
    var httpClient = httpContext.RequestServices.GetRequiredService<HttpClient>();
    var response = await httpClient.GetAsync("https://api.punkapi.com/v2/beers");
    if (response.IsSuccessStatusCode)
    {
        var beers = await response.Content.ReadAsStringAsync();
        httpContext.Response.StatusCode = 200;
        httpContext.Response.ContentType = "application/json";
        await httpContext.Response.WriteAsync(beers, Encoding.UTF8); // Write the beers as JSON response
    }
    else
    {
        httpContext.Response.StatusCode = (int)response.StatusCode;
        var errorResponse = JsonConvert.SerializeObject("Failed to retrieve beer menu");
        await httpContext.Response.WriteAsync(errorResponse, Encoding.UTF8);
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
        httpContext.Response.StatusCode = 200;
        httpContext.Response.ContentType = "application/json";
        await httpContext.Response.WriteAsync(beer, Encoding.UTF8); // Write the beer as JSON response
    }
    else
    {
        httpContext.Response.StatusCode = (int)response.StatusCode;
        var errorResponse = JsonConvert.SerializeObject("Failed to retrieve beer by ID");
        await httpContext.Response.WriteAsync(errorResponse, Encoding.UTF8);
    }
});

app.MapGet("/beer/random", async (HttpContext httpContext) =>
{
    var httpClient = httpContext.RequestServices.GetRequiredService<HttpClient>();
    var response = await httpClient.GetAsync("https://api.punkapi.com/v2/beers/random");
    if (response.IsSuccessStatusCode)
    {
        var beer = await response.Content.ReadAsStringAsync();
        httpContext.Response.StatusCode = 200;
        httpContext.Response.ContentType = "application/json";
        await httpContext.Response.WriteAsync(beer, Encoding.UTF8); // Write the beer as JSON response
    }
    else
    {
        httpContext.Response.StatusCode = (int)response.StatusCode;
        var errorResponse = JsonConvert.SerializeObject("Failed to retrieve random beer");
        await httpContext.Response.WriteAsync(errorResponse, Encoding.UTF8);
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
        httpContext.Response.StatusCode = 200;
        httpContext.Response.ContentType = "application/json";
        await httpContext.Response.WriteAsync(beers, Encoding.UTF8); // Write the beers as JSON response
    }
    else
    {
        httpContext.Response.StatusCode = (int)response.StatusCode;
        var errorResponse = JsonConvert.SerializeObject("Failed to search for beers");
        await httpContext.Response.WriteAsync(errorResponse, Encoding.UTF8);
    }
});

// Enable CORS
app.UseCors();

app.Run();
