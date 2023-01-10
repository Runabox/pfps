using Amazon.S3;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Pfps.API.Data;
using Pfps.API.Models;
using Pfps.API.Services;

namespace Pfps.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Default
            builder.Services.AddSwaggerGen();
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddAutoMapper(typeof(Program));

            builder.Services.AddDbContext<PfpsContext>(o => o.UseNpgsql(builder.Configuration.GetConnectionString("Main")));
            builder.Services.AddHttpClient("Discord", x => x.BaseAddress = new Uri("https://discord.com/api/v9/"));
            builder.Services.AddHttpClient("Google", x => x.BaseAddress = new Uri("https://google.com/"));

            // AWS
            builder.Services.AddAWSService<IAmazonS3>();
            builder.Services.AddDefaultAWSOptions(builder.Configuration.GetAWSOptions());
            builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

            // Services
            builder.Services.AddScoped<IDiscordService, DiscordService>();
            builder.Services.AddScoped<IFileService, FileService>();
            builder.Services.AddScoped<IAuditLogger, AuditLogger>();
            builder.Services.AddScoped<IReCaptchaService, ReCaptchaService>();

            builder.Services.Configure<PfpsOptions>(builder.Configuration.GetSection("Defaults"));

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // Allow all CORS
            app.UseCors(x => 
                x.AllowAnyMethod()
                .AllowAnyHeader()
                .SetIsOriginAllowed(origin => true) // allow any origin
                .AllowCredentials());               // allow credentials

            app.UseAuthorization();
            app.MapControllers();
            await app.RunAsync();
        }
    }
}