using System.Reflection;
using System.Security.Claims;
using Application;
using Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using WebAPI;
using WebAPI.Controllers;

var builder = WebApplication.CreateBuilder(args);

var domain = builder.Configuration["Auth0:Domain"];

var corsPolicy = "dev";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = $"https://{builder.Configuration["Auth0:Domain"]}/";
        options.Audience = builder.Configuration["Auth0:Audience"];
        options.TokenValidationParameters = new TokenValidationParameters
        {
            NameClaimType = ClaimTypes.NameIdentifier
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
            policy.WithOrigins("http://localhost:5173").AllowAnyHeader().AllowAnyMethod();
            policy.WithOrigins("https://api.ggclan.net").AllowAnyHeader().AllowAnyMethod();
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
var app = builder.Build();

// verwijder en maak database schema, voor dev
var scope = app.Services.CreateScope();
var db = scope.ServiceProvider.GetRequiredService<GGDbContext>();
db.Database.EnsureCreated();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors(corsPolicy);

app.UseAuthorization();

app.MapHub<ChatHub>("/v1/chat");


app.MapControllers();

app.Run();