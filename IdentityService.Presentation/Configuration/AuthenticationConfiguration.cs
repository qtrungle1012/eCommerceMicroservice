using IdentityService.Infrastructure.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace IdentityService.Presentation.Configuration
{
    public static class AuthenticationConfiguration
    {
        public static IServiceCollection AddJwtAuthentication(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var jwtSettings = configuration.GetSection("JwtSettings").Get<JwtSettings>();
            var key = Encoding.ASCII.GetBytes(jwtSettings!.SecretKey);

            // 1️⃣ Authentication
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ClockSkew = TimeSpan.Zero,
                    RoleClaimType = "role" // dùng claim "role" để check policy
                };

                //options.Events = new JwtBearerEvents
                //{
                //    OnChallenge = async context =>
                //    {
                //        context.HandleResponse(); // chặn default 401
                //        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                //        context.Response.ContentType = "application/json";

                //        await context.Response.WriteAsJsonAsync(new
                //        {
                //            Code = 401,
                //            Message = "Unauthorized: token missing or invalid"
                //        });
                //    },
                //    OnForbidden = async context =>
                //    {
                //        context.Response.StatusCode = StatusCodes.Status403Forbidden;
                //        context.Response.ContentType = "application/json";

                //        await context.Response.WriteAsJsonAsync(new
                //        {
                //            Code = 403,
                //            Message = "Forbidden: you do not have permission"
                //        });
                //    }
                //};
            });

            // 3️⃣ Authorization (role-based + placeholder scope-based)
            services.AddAuthorization(options =>
            {
                // Role-based policies
                options.AddPolicy("RequireAdminRole", policy =>
                    policy.RequireRole("ADMIN"));
                options.AddPolicy("RequireSellerRole", policy =>
                    policy.RequireRole("SELLER"));
                options.AddPolicy("RequireUserRole", policy =>
                    policy.RequireRole("USER"));

            });

            return services;
        }
    }
}
