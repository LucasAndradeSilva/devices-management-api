using Microsoft.IdentityModel.Tokens;
using System.Net;
using System.Text;
using System.Text.Json;
using Devices.Application.Common;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Devices.Api.Extensions;

public static class AuthExtensions
{
    public static IServiceCollection AddJwtAuth(this IServiceCollection services, IConfiguration config)
    {
        var key = Encoding.UTF8.GetBytes(config["SecurityKey"]);

        services.AddAuthentication("Bearer")
            .AddJwtBearer("Bearer", options =>
            {
                options.TokenValidationParameters = new()
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };

                options.Events = new JwtBearerEvents
                {
                    OnChallenge = async context =>
                    {
                        context.HandleResponse();

                        context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                        context.Response.ContentType = "application/json";

                        var result = Result<object>.Failure(
                            message: "Unauthorized",
                            statusCode: HttpStatusCode.Unauthorized,
                            errors: new List<string> { "Authentication is required" });

                        var json = JsonSerializer.Serialize(result);
                        await context.Response.WriteAsync(json);
                    },

                    OnForbidden = async context =>
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                        context.Response.ContentType = "application/json";

                        var result = Result<object>.Failure(
                            message: "Forbidden",
                            statusCode: HttpStatusCode.Forbidden,
                            errors: new List<string> { "You do not have permission to access this resource" });

                        var json = JsonSerializer.Serialize(result);
                        await context.Response.WriteAsync(json);
                    }
                };
            });

        services.AddAuthorization();

        return services;
    }
}