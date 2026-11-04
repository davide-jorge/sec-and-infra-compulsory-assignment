using Api;
using Infrastructure;
using LinqToDB;
using Service;


var builder = WebApplication.CreateBuilder(args);


var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
                       ?? throw new InvalidOperationException("Database connection string is missing.");


var options = new DataOptions<myDbConnection>(
    new DataOptions().UsePostgreSQL(connectionString));
builder.Services.AddScoped<myDbConnection>(_ =>
    new myDbConnection(options));


builder.Services.ConfigureDomainServices(
    builder.Configuration);


builder.Services.AddControllers();
builder.Services.AddOpenApiDocument();
builder.Services.AddCors();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<MyExceptionHandler>();


var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.UseExceptionHandler();
app.UseCors(config => config.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin().SetIsOriginAllowed(_=>true));
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.UseOpenApi();
app.UseSwaggerUi();

app.Run();