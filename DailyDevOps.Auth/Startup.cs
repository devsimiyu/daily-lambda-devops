using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using DailyDevOps.Auth.Model;
using DailyDevOps.Auth.Service;
using DailyDevOps.Auth.Util;
using Microsoft.AspNetCore.Mvc;

namespace DailyDevOps.Auth;

public class Startup
{
    private readonly IConfiguration _configuration;

    public Startup(IConfiguration configuration)
        => _configuration = configuration ?? throw new Exception($"{nameof(IConfiguration)} is required");

    /// <summary>
    /// Add services to the container.
    /// </summary>
    /// <param name="services"></param>
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<RsaKeyGenerator>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddHealthChecks();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
    }

    /// <summary>
    /// Configure the HTTP request pipeline.
    /// </summary>
    /// <param name="app"></param>
    /// <param name="env"></param>
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseRouting();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapHealthChecks("auth/health-check");

            endpoints.MapPost("auth/login", ([FromBody] LoginDto loginDto, [FromServices] IAuthService authService) =>
            {
                var token = authService.Login(loginDto);

                return Results.Ok(token);
            })
            .AddEndpointFilter(async (context, next) =>
            {
                var loginDto = context.GetArgument<LoginDto>(0);
                var validationContext = new ValidationContext(loginDto);
                var validationResult = new List<ValidationResult>();
                var isValid = Validator.TryValidateObject(loginDto, validationContext, validationResult);

                if (isValid)
                {
                    return await next(context);
                }
                
                var errors = validationResult.Aggregate(new Dictionary<string, string[]>(), (errors, result) =>
                {
                    errors.Add(result.MemberNames.First(), [result.ErrorMessage]);

                    return errors;
                });

                return Results.ValidationProblem(errors);
            });
        });
    }
}


