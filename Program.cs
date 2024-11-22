using DotNetEnv;
using EntertaimentLib_API.Services;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;

namespace EntertaimentLib_API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Load environment variables from .env file (for local development)
            DotNetEnv.Env.Load(); // This loads the .env file

            // Retrieve Firebase credentials path from environment variable
            string? firebaseKeyEnv = Environment.GetEnvironmentVariable("FIREBASE_CREDENTIAL_PATH");

            Console.WriteLine($"Checking if file exists at: {firebaseKeyEnv}");
            
            if (string.IsNullOrEmpty(firebaseKeyEnv))
            {
                Console.WriteLine("Environment variable 'FIREBASE_CREDENTIAL_PATH' is not set or is empty.");
            }
            else
            {
                Console.WriteLine($"Environment variable path: {firebaseKeyEnv}");
                if (File.Exists(firebaseKeyEnv))
                {
                    Console.WriteLine("File exists.");
                }
                else
                {
                    Console.WriteLine("File does not exist or cannot be accessed. Verifying full path...");
                    Console.WriteLine($"Current directory: {Directory.GetCurrentDirectory()}");
                    Console.WriteLine($"Absolute path: {Path.GetFullPath(firebaseKeyEnv)}");
                }
            }




            /* ***START CORS Configuration Block */
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AppSpecificOrigin", policyBuilder =>
                {
                    policyBuilder.WithOrigins("http://localhost:4200")
                                 .AllowAnyHeader()
                                 .AllowAnyMethod();
                });
            });
            /* ***END CORS Configuration Block */

            // Add services to the container.
            builder.Services.AddControllers();

            // Register ApplicationDbContext and FeedBackService
            builder.Services.AddSingleton<FirebaseService>();  // Add FirebaseService as a singleton
            builder.Services.AddScoped<IFeedBackService, FirebaseService>();  // Register FeedBackService as scoped

            // Configure Swagger/OpenAPI
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}
