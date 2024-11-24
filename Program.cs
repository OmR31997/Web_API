using DotNetEnv;
using EntertaimentLib_API.Services;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Newtonsoft.Json;


namespace EntertaimentLib_API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Load environment variables from .env file (for local development)
            // Load environment variables from .env for local development
            if (builder.Environment.IsDevelopment())
            {
                DotNetEnv.Env.Load(); // Loads the .env file
            }



            // Retrieve Firebase credentials path from environment variable
            // Get the file path to Firebase credentials
            // Retrieve Firebase credentials path from environment variable
            string? firebaseKeyPath = Environment.GetEnvironmentVariable("FIREBASE_CREDENTIAL_PATH") ?? "/etc/secrets/firebase-key.json";

            if (string.IsNullOrEmpty(firebaseKeyPath))
            {
                Console.WriteLine("Environment variable 'FIREBASE_CREDENTIAL_PATH' is not set or is empty.");
            }
            else
            {
                Console.WriteLine($"Firebase credential file path: {firebaseKeyPath}");

                try
                {
                    // Read the JSON content from the file
                    string json = File.ReadAllText(firebaseKeyPath);

                    // Initialize Firebase
                    FirebaseApp.Create(new AppOptions
                    {
                        Credential = GoogleCredential.FromJson(json)
                    });

                    Console.WriteLine("Firebase initialized successfully.");
                }
                catch (FileNotFoundException ex)
                {
                    Console.WriteLine($"File not found: {ex.Message}");
                }
                catch (JsonReaderException ex)
                {
                    Console.WriteLine($"JSON Parsing Error: {ex.Message}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Unexpected error: {ex.Message}");
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
