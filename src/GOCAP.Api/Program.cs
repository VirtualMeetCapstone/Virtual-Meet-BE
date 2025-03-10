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

// Ip Rate Limiting
builder.Services.AddIpRateLimiting(builder.Configuration);

// SignalR
builder.Services.AddSignalR();

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

builder.Services.AddControllers();
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = null;
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
if (!app.Environment.IsProduction())
{
    app.UseMiddleware<RequestResponseLoggingMiddleware>();
}
app.UseCustomExceptionHandler();
if (app.Environment.IsProduction())
{
    app.UseHsts(); 
}
app.UseHttpsRedirection();
if (app.Environment.IsProduction())
{
    app.UseResponseCompression();
}
app.UseCors();
app.UseRouting();
app.UseIpRateLimiting();

app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<PermissionsControlMiddleware>();

// Configure the HTTP request pipeline.

app.UseSwagger();
app.UseSwaggerUI();

app.MapHub<ChatHub>("/chatHub");
app.MapHub<VideoHub>("/videoHub");
app.MapControllers();

await app.RunAsync();
