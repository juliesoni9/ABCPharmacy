using PharmacyApi.Models;
using PharmacyApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddSingleton(sp =>
    new JsonFileStore<Medicine>(sp.GetRequiredService<IWebHostEnvironment>(), "medicines.json"));
builder.Services.AddSingleton(sp =>
    new JsonFileStore<SaleRecord>(sp.GetRequiredService<IWebHostEnvironment>(), "sales.json"));
builder.Services.AddSingleton<IMedicineService, MedicineService>();
builder.Services.AddSingleton<ISaleService, SaleService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AngularApp", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors("AngularApp");
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
