using Application;
using Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Presentaion.Hubs;
using System;
using System.Text;
using System.Threading.RateLimiting;

namespace Host;

public class Program
{
    const string RATE_LIMIT_POLICY = nameof(RATE_LIMIT_POLICY);

    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        ISignalRServerBuilder signalRBuilder = builder.Services
            .AddSignalR().AddMessagePackProtocol();

#if RELEASE
            signalRBuilder.AddAzureSignalR();
#endif

        builder.Services.AddControllers();

        builder.Services
            .AddEndpointsApiExplorer()
            .AddSwaggerGen(opt =>
            {
                var scheme = new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = JwtBearerDefaults.AuthenticationScheme,
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme.",
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                };

                var requirement = new OpenApiSecurityRequirement
                {
                    [scheme] = Array.Empty<string>()
                };

                opt.AddSecurityRequirement(requirement);
                opt.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, scheme);
            });

        builder.Services
            .AddAuthorization(opt =>
            {
                opt.AddPolicy("Admin", policy => policy.RequireRole("Admin"));
                opt.AddPolicy("User", policy => policy.RequireRole("User"));
            })
            .AddAuthentication()
            .AddJwtBearer(opt =>
            {
                opt.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = builder.Configuration["Issuer"]!,
                    ValidateAudience = true,
                    ValidAudience = builder.Configuration["Aud"]!,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["ISS_KEY"]!)),
                    ClockSkew = TimeSpan.Zero
                };
            });

        builder.Services.AddRateLimiter(opt =>
        {
            opt.AddPolicy(RATE_LIMIT_POLICY, context =>
            {
                var ip = context.Connection.RemoteIpAddress?.ToString();
                return RateLimitPartition.GetConcurrencyLimiter(ip, (key) =>
                {
                    return new ConcurrencyLimiterOptions
                    {
                        PermitLimit = 2,
                        QueueLimit = 1,
                    };
                });
            });
        });

        builder.Services.AddApplication()
            .AddInfrastructure(builder.Environment.IsDevelopment(), builder.Configuration.GetConnectionString("TradingKing")!);

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger().UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseAuthentication().UseAuthorization();

        app.MapHub<ChatHub>("/chat")
            .RequireRateLimiting(RATE_LIMIT_POLICY);

        app.MapControllers()
            .RequireRateLimiting(RATE_LIMIT_POLICY);

        app.Run();
    }
}
