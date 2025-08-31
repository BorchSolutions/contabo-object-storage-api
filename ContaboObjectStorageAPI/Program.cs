using Amazon.S3;
using ContaboObjectStorageAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure AWS S3 for Contabo
builder.Services.AddSingleton<IAmazonS3>(provider =>
{
    var configuration = provider.GetService<IConfiguration>();

    var config = new AmazonS3Config()
    {
        ServiceURL = configuration["ContaboS3:ServiceUrl"], // ej: https://eu2.contabostorage.com
        ForcePathStyle = true, // Important for Contabo
        UseHttp = false
    };

    return new AmazonS3Client(
        configuration["ContaboS3:AccessKey"],
        configuration["ContaboS3:SecretKey"],
        config
    );
});

builder.Services.AddScoped<IS3Service, S3Service>();

// Configure CORS for Angular
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", builder =>
    {
        builder.WithOrigins("http://localhost:4200") // Adjust for your Angular dev server
               .AllowAnyMethod()
               .AllowAnyHeader()
               .AllowCredentials();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAngular");
app.UseAuthorization();
app.MapControllers();

app.Run();