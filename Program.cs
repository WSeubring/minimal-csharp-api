using MinAPISeparateFile;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

UserEndpoints.Map(app);

app.Run();

