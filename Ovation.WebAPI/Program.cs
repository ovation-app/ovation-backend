using Ovation.Application;
using Ovation.Application.Constants;
using Ovation.Persistence;
using Ovation.Persistence.Observability;
using Ovation.Persistence.Observability.Tracing;
using Ovation.Persistence.Services;
using Ovation.WebAPI;
using Sentry.OpenTelemetry;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddHttpContextAccessor();
builder.Services.AddAntiforgery(o => o.SuppressXFrameOptionsHeader = true);
builder.Services.AddSignalR();

builder.Services.ConfigurePersistence(builder.Configuration);
builder.Services.ConfigureClients();
builder.Services.ConfigureApplication();


builder.Services.AddMvc(
    options => options.EnableEndpointRouting = false)
    .AddNewtonsoftJson(opt => opt.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);


//builder.Services.AddCors(p => p.AddPolicy(Constant.CORS, policy =>
//{
//    policy.WithOrigins("https://dev.ovation.network").AllowAnyHeader().AllowAnyMethod().AllowCredentials();
//}));

builder.Services.AddCors(p => p.AddPolicy(Constant.CORS, builder =>
{
    builder.SetIsOriginAllowed(host => true).AllowAnyHeader().AllowAnyMethod().AllowCredentials();
}));

builder.Services.AddSwaggerDoc(builder.Configuration);

// add service scopes here
builder.Services.ConfigureJWT(builder.Configuration);

// OpenTelemetry
builder.Services.AddCustomOpenTelemetry(builder.Configuration);

// Sentry
builder.WebHost.UseCustomSentry(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(Constant.CORS);

app.UseRouting();

//app.UseSentryTracing();

app.UseAuthentication();

app.UseAuthorization();

app.MapHub<NotificationService>("notification");

app.UseCookiePolicy();

app.UseStaticFiles();

app.UseHttpsRedirection();

app.MapControllers();

var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
var url = $"http://0.0.0.0:{port}";

app.Run();
