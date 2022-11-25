global using RpgCharacter.Models;
using RpgCharacter.Services.CharacterService;
using RpgCharacter.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Filters;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<DataContext>(options=>{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

// Add security filter
builder.Services.AddSwaggerGen(c=>{

    c.AddSecurityDefinition("oauth2", new Microsoft.OpenApi.Models.OpenApiSecurityScheme{

        Description="standard authorization header using the bearer scheme, e.g. \"bearer {token}\"",
        In=Microsoft.OpenApi.Models.ParameterLocation.Header,
        Name="Authorization",
        Type=Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey
    });

    c.OperationFilter<SecurityRequirementsOperationFilter>();
});
builder.Services.AddScoped<ICharacterService,CharacterService>();
builder.Services.AddScoped<IAuthRepository,AuthRepository>();
builder.Services.AddAutoMapper(typeof(Program).Assembly);
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options=>options.TokenValidationParameters=new TokenValidationParameters
{
    ValidateIssuerSigningKey=true,
    IssuerSigningKey=new SymmetricSecurityKey(System.Text.Encoding.UTF8.
    GetBytes(builder.Configuration.GetSection("AppSettings:Token").Value)),
    ValidateIssuer=false,
    ValidateAudience=false
}
);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
