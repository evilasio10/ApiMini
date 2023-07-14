using ApiMini.Model;
using ApiMini.Repositories;
using ApiMini.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(o =>
{
    o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    o.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(o =>
{
    o.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration["Jwt:Secret"])),
        ValidateIssuer = false,
        ValidateAudience = false,
    };
});

builder.Services.AddAuthorization();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();
app.UseSwagger();

app.MapPost("/Login", (AppDbContext context, [FromBody] LoginModel auth) =>
{    
    var user = context.Usuario.Where(p => p.UserName == auth.UserName && p.Password == auth.Password).FirstOrDefault();

    return user is not null ? Results.Ok(new UsuarioAuth
    {
        ide_usuario = user.ide_usuario,
        nome = user.UserName,
        email = user.Email,
        role = user.Role,
        token = TokenService.GerarToken(user, builder.Configuration["Jwt:Secret"])
    }) : Results.NotFound();
});


app.UseSwaggerUI();
app.Run();

