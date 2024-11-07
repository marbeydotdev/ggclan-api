using System.Reflection;
using System.Security.Claims;
using System.Threading.RateLimiting;
using Application;
using Domain.Interfaces;
using Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using WebAPI;
using WebAPI.Controllers;
using WebAPI.Services;

var builder = WebApplication.CreateBuilder(args);

var domain = builder.Configuration["Auth0:Domain"];

var corsPolicy = "dev";

builder.Services.AddRateLimiter(opts => opts
    .AddFixedWindowLimiter(policyName: "fixed", options =>
    {
        options.PermitLimit = 20;
        options.Window = TimeSpan.FromSeconds(10);
        options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        options.QueueLimit = 2;
    }));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = $"https://{builder.Configuration["Auth0:Domain"]}/";
        options.Audience = builder.Configuration["Auth0:Audience"];
        options.TokenValidationParameters = new TokenValidationParameters
        {
            NameClaimType = ClaimTypes.NameIdentifier
        };
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];

                // If the request is for our hub...
                var path = context.HttpContext.Request.Path;
                if (!string.IsNullOrEmpty(accessToken) &&
                    (path.StartsWithSegments("/v1/realtime")))
                {
                    // Read the token out of the query string
                    context.Token = accessToken;
                }
                return Task.CompletedTask;
            }
        };
    });

builder.Services
    .AddAuthorization(options =>
    {
        options.AddPolicy(
            "read:messages",
            policy => policy.Requirements.Add(
                new HasScopeRequirement("clan", domain!)
            )
        );
    });

builder.Services.AddSingleton<IAuthorizationHandler, HasScopeHandler>();
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: corsPolicy,
        policy  =>
        {
            policy.WithOrigins("http://localhost:5173").AllowAnyHeader().AllowAnyMethod().AllowCredentials();
        });
});

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddSignalR();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddApplication();

builder.Services.AddInfrastructure();
builder.Services.AddAutoMapper(Assembly.GetEntryAssembly());
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IRealtimeChatService, RealtimeChatService>();
var app = builder.Build();

// verwijder en maak database schema, voor dev
var scope = app.Services.CreateScope();
var db = scope.ServiceProvider.GetRequiredService<GgDbContext>();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    db.Database.EnsureDeleted();
}

db.Database.EnsureCreated();
app.UseRateLimiter();
app.UseHttpsRedirection();
app.UseCors(corsPolicy);
app.UseAuthorization();
app.MapHub<RealtimeHub>("/v1/realtime");
app.MapControllers();
app.Run();