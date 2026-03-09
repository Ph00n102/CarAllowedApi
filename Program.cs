using CarAllowedApi.Data;
using CarAllowedApi.Hubs;
using CarAllowedApi.Services;
using HosxpUi.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<ApplicationDbContext>(opt =>
         opt.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"), new MySqlServerVersion(new Version())));


builder.Services.AddScoped<IJobRequestCarService, JobRequestCarService>();
builder.Services.AddScoped<IImageEmpService, ImageEmpService>();
// builder.Services.AddScoped<IJobNumberService, JobNumberService>();

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder.SetIsOriginAllowed(origin => true) // อนุญาตทุก origin
                   .AllowAnyMethod()
                   .AllowAnyHeader()
                   .AllowCredentials()
                   .WithExposedHeaders("Content-Disposition"); // เปิดเผย headers เพิ่มเติมถ้าจำเป็น
        });
});
builder.Services.AddSignalR();

var app = builder.Build();


app.UseCors("AllowAll");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapHub<JobRequestHub>("/jobRequestHub");
app.MapHub<JobDistributeHub>("/jobDistributeHub");

// app.Urls.Add("http://*:80");

app.Run();
