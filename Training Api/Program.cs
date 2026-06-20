using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Npgsql.EntityFrameworkCore;
using System.Text;
using Training_Api.Context;
using Training_Api.Interface;
using Training_Api.Middleware;
using Training_Api.Models;
using Training_Api.Repository;
using Training_Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddScoped<IAuthServices, AuthServices>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddScoped<IUserServices, UserServices>();
builder.Services.AddScoped<IWorkoutRepository, WorkoutRepository>();
builder.Services.AddScoped<IWorkoutServices, WorkoutServices>();


builder.Services.AddDbContext<AddDbContext>(options => options.UseNpgsql(builder.Configuration["Database:ConnectionsString"], optionsSql => optionsSql.EnableRetryOnFailure()));

builder.Services.AddAuthentication("Bearer")
.AddJwtBearer("Bearer", options =>
{

    var secretKey = builder.Configuration["JWTSetting:SecretKey"] ?? "1234";

    options.TokenValidationParameters = new TokenValidationParameters
    {

        ValidateIssuer = true,
        ValidIssuer = "MyApp",

        ValidateAudience = true,
        ValidAudience = "User",

        ValidateLifetime = true,

        ValidateIssuerSigningKey = true,

        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),

    };

});

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorize",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Description = "Write Referens Token",
        Scheme = "bearer",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        BearerFormat = "JWT",


    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }

            },
            new string[] { }



        }


    });


});

var app = builder.Build();



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseMiddleware<MyExceptionsMiddleware>();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();

app.Run();
