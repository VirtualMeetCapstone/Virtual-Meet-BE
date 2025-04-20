#if DEBUG
using GOCAP.Database;

Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development", EnvironmentVariableTarget.Process);
#endif

var builder = WebApplication.CreateBuilder(args);

//builder.WebHost.ConfigureHttps(); // Configure https
builder.Logging.ConfigureLogging(builder.Configuration);
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

builder.Services
            .AddIpRateLimiting(builder.Configuration)
            .AddSignalR();

// Add services to the container.
builder.Services.AddCorsPolicy()
                .AddJwtAuthentication(builder.Configuration)
                .AddHttpContextAccessor()
                .AddServices(builder.Configuration)
                .AddResponseCompression(options => options.EnableForHttps = true)
                .AddAutoMapper(typeof(ModelMapperProfileBase),
                               typeof(EntityMapperProfileBase))
                .AddFluentValidation(Assembly.Load("gocap.api.validation"))
                .AddControllers()
                ;
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();
var mongoDbConnection = builder.Configuration.GetConnectionString("MongoDbConnection");

if (string.IsNullOrEmpty(mongoDbConnection))
{
    throw new ApplicationException("MongoDB connection string is missing in configuration");
}

// Tên database có thể lấy từ cấu hình hoặc hardcode nếu cần
var mongoDatabaseName = "GOCAP"; // Hoặc lấy từ cấu hình nếu có

builder.Services.AddSingleton<AppMongoDbContext>(_ =>
    new AppMongoDbContext(
        mongoDatabaseName,
        mongoDbConnection
    ));

var app = builder.Build();
var isProductionEnviroment = app.Environment.IsProduction();
if (!isProductionEnviroment)
{
    app.UseMiddleware<RequestResponseLoggingMiddleware>();
}
app.UseCustomExceptionHandler();
if (isProductionEnviroment)
{
    app.UseHsts(); 
}
app.UseHttpsRedirection();
if (isProductionEnviroment)
{
    app.UseResponseCompression();
}
app.UseCors();
app.UseRouting();
app.UseWebSockets();
app.UseIpRateLimiting();

app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<PermissionsControlMiddleware>();
app.UseSwagger();
app.UseSwaggerUI();

app.MapHub<ChatHub>("/chatHub");
app.MapHub<ChatOutsideRoomHub>("/chatOutsideRoomHub");

app.MapHub<RoomHub>("/roomHub");
app.MapControllers();

await app.RunAsync();
