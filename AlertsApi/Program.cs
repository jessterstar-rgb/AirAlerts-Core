using System.Net.Http.Headers;
using AlertsApi.Endpoints;
using AlertsApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient<AlertsInUaClient>(client =>
{
    client.BaseAddress = new Uri("https://api.alerts.in.ua/");
    var token = builder.Configuration["AlertsInUa:Token"];
    client.DefaultRequestHeaders.Authorization =
        new AuthenticationHeaderValue("Bearer", token);
});

builder.Services.AddHostedService<AlertPoller>();

var app = builder.Build();

app.UseWebSockets();

WebSocketEndpoints.Map(app);
AlertEndpoints.Map(app);

app.Run();
