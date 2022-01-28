using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddMvc();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var Configuration = builder.Configuration;
builder.Services.AddAuthentication(option =>
{
    option.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie(option => 
{
    option.LoginPath = "/signin";
    option.LogoutPath = "/signout";
})
.AddGitHub(options => 
{
    options.ClientId = Configuration["GitHub:ClientId"];
    options.ClientSecret = Configuration["GitHub:ClientSecret"];
    options.EnterpriseDomain = Configuration["GitHub:EnterpriseDomain"];
    options.Scope.Add("user:email");

    options.CallbackPath = "/myars/signin-github";
});


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
