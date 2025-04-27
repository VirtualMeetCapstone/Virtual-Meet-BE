#if DEBUG

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

// Create a startup service to warm up HTTP clients
builder.Services.AddHostedService<HttpClientWarmupService>();

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
app.MapHub<QuizHub>("/quizHub");

app.MapControllers();

await app.RunAsync();
