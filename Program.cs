using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OnlineChat;
using System.Text;


var builder = WebApplication.CreateBuilder(args);
// adding database connectino context
builder.Services.AddDbContext<ChatDbContext>(options =>
                options.UseSqlServer(@"Server=localhost;Database=ChatAppDB;Trusted_Connection=True;TrustServerCertificate=True;"));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
           .AddJwtBearer(options =>
           {
               options.TokenValidationParameters = new TokenValidationParameters
               {
                   ValidateIssuer = true,
                   ValidateAudience = true,
                   ValidateLifetime = true,
                   ValidateIssuerSigningKey = true,
                   ValidIssuer = "yourdomain.com", // ??? пока оставлю так
                   ValidAudience = "yourdomain.com", // ??? пока оставлю так
                   IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("ThisIsAVeryStrongKeyWithMoreThan32Chars")) // Ваш секретный ключ ???
               };
           });

builder.Services.AddControllers();
builder.Services.AddSignalR();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "My API",
        Version = "v1",
        Description = "Chat Backend API",
        Contact = new OpenApiContact
        {
            Name = "Rustem",
            Email = "r.gilmutdinov93@gmail.com",
            Url = new Uri("https://localhost:7071/")
        }
    });
});

var app = builder.Build();
app.UseStaticFiles();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("swagger/v1/swagger.json", "My API V1");
        options.IndexStream = () => typeof(Program).Assembly.GetManifestResourceStream("OnlineChat.wwwroot.login.html");
    });

    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API Alternative UI v1");
        c.RoutePrefix = "swagger-alt"; // Второй UI доступен по /swagger-alt
    });
}
app.UseRouting();

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    //_ = endpoints.MapControllers();
    _ = endpoints.MapHub<ChatHub>("/chathub");
});

app.UseHttpsRedirection();

app.MapControllers();

app.Run();