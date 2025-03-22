using System.Text;
using Inforce.UrlShortener.Application.Database;
using Inforce.UrlShortener.Application.Interfaces;
using Inforce.UrlShortener.Web;
using Inforce.UrlShortener.Web.Middleware;
using Microsoft.IdentityModel.Tokens;
using Sqids;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults
builder.AddServiceDefaults();

builder.Services.AddProblemDetails();
builder.Services.AddOpenApi();

builder.Services.AddAuthentication("Bearer").AddJwtBearer(
    config =>
    {
        config.TokenValidationParameters = new TokenValidationParameters
        {
            ValidAudience = builder.Configuration["Jwt:Audience"],
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"] ?? "")
            ),
            ValidateLifetime = false
        };
    });
builder.Services.AddAuthorization();

builder.Services.AddSwaggerGen();

// Connect to db using aspire service discovery
builder.AddNpgsqlDbContext<AppDbContext>("UrlShortenerDb");

builder.Services
    .Scan(x => x.FromAssemblyOf<IUrlShortenerService>()
        .AddClasses(classes => classes.Where(type => type.Name.EndsWith("Service")))
        .AsImplementedInterfaces()
        .WithTransientLifetime());

builder.Services.AddTransient<DbSeeder>();

builder.Services.AddSingleton<SqidsEncoder<int>>();

builder.Services.AddControllersWithViews();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbSeeder = scope.ServiceProvider.GetRequiredService<DbSeeder>();
    await dbSeeder.Seed();
}

app.UseMiddleware<RequestLoggingMiddleware>();

app.MapDefaultEndpoints();

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}
    
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Login}/{id?}")
    .WithStaticAssets();

app.Run();