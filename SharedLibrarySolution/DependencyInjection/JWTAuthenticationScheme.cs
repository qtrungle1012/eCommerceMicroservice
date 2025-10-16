using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace SharedLibrarySolution.DependencyInjection
{
    public static class JWTAuthenticationScheme
    {
        //"JwtSettings": {
        //"SecretKey": "my_super_secret_key_123",
        //"Issuer": "MyAuthServer",
        //"Audience": "MyClientApp"
        //} cần để cấu hình ở mỗi service để có thể sử dụng lớp này -> builder.Services.AddJWTAuthenticationScheme(builder.Configuration);

        public static IServiceCollection AddJWTAuthenticationScheme(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtSettings = configuration.GetSection("JwtSettings"); // lấy cấu hình jwt trong json

            var secretKey = jwtSettings["SecretKey"];
            var issuer = jwtSettings["Issuer"];
            var audience = jwtSettings["Audience"];

            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = issuer,
                        ValidAudience = audience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!)),
                        ClockSkew = TimeSpan.Zero
                    };
                });

            return services;
        }
    }
}
