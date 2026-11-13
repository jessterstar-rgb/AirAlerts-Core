var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.UseWebSockets();

WebSocketEndpoints.Map(app);
AlertEndpoints.Map(app);

app.Run();