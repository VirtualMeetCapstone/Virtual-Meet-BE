#if DEBUG
using GOCAP.Api.Hubs;

Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development", EnvironmentVariableTarget.Process);
#endif

var builder = WebApplication.CreateBuilder(args);

//builder.WebHost.ConfigureHttps(); // Configure https
builder.Logging.ConfigureLogging(builder.Configuration);
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

// SignalR
builder.Services.AddSignalR();

// Add services to the container.
builder.Services.AddCorsPolicy()
                .AddJwtAuthentication(builder.Configuration)
                .AddServices(builder.Configuration)
                .AddResponseCompression(options => options.EnableForHttps = true)
                .AddAutoMapper(typeof(ModelMapperProfileBase),
                               typeof(EntityMapperProfileBase))
                .AddFluentValidation(Assembly.Load("gocap.api.validation"))
                ;

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseCustomExceptionHandler();
app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseMiddleware<PermissionsControlMiddleware>();
app.UseAuthorization();
app.UseCors();
app.MapHub<ChatHub>("/chatHub");

// Configure the HTTP request pipeline.

app.UseSwagger();
app.UseSwaggerUI();
if (app.Environment.IsProduction())
{
    app.UseResponseCompression();
}

app.MapControllers();

await app.RunAsync();
