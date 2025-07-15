using System.Diagnostics;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using server;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();
builder.Configuration.AddJsonFile("admin-creds.json");
builder.Services.AddSingleton<RedisCache>();

var app = builder.Build();

var init = () =>
{
    var cache = app.Services.GetService<RedisCache>();

    if (cache is null)
        throw new InvalidOperationException("Cache should never be null");
    
    var adminPassword = app.Configuration.GetValue<string>("adminPassword");

    if (adminPassword is null)
    {
        throw new Exception("No admin Password");
    }

    cache.SetUser("admin", adminPassword);
    cache.LockUser("admin", adminPassword, TimeSpan.FromDays(10000000)); // Release me....
};

app.MapGet("/", ([FromQuery] string username, [FromServices] RedisCache cache) =>
{
    if (username == "admin")
        return "nice try";

    return cache.GetUser(username) ?? "Username doesn't exist";
});

app.MapPost("/", ([FromBody] UserData userData, [FromServices] RedisCache cache) =>
{
    var username = userData.Username;

    if (username is null or "admin")
        return HttpStatusCode.BadRequest;

    if (cache.GetUser(username) is not null)
        return HttpStatusCode.BadRequest;

    cache.SetUser(
        username,
        userData.Password
    );

    return HttpStatusCode.Created;
});

app.MapPut("/", ([FromBody] UserData userData, [FromServices] RedisCache cache) =>
{
    var username = userData.Username;

    if (username is null or "admin")
    {
        return HttpStatusCode.BadRequest;
    }

    if (cache.GetUser(username) is null)
        return HttpStatusCode.BadRequest;

    cache.SetUser(
        username,
        userData.Password
    );

    return HttpStatusCode.OK;
});

app.MapDelete("/", ([FromQuery] string username, [FromServices] RedisCache cache) =>
{
    if (username is null or "admin")
    {
        return HttpStatusCode.BadRequest;
    }

    cache.DeleteUser(username);

    return HttpStatusCode.NoContent;
});

app.MapPost("/shell", ([FromBody] CommandData commandData, [FromServices] RedisCache cache) =>
{
    var username = commandData.Username;
    var password = commandData.Password;
    var command = commandData.Command;

    if (username is not "admin" || cache.GetUser("admin") != password)
        return "You need to be admin!!!";


    using var cmd = new Process
    {
        StartInfo = new ProcessStartInfo
        {
            FileName = "bash",
            Arguments = $"-c \"{command}\"",
            RedirectStandardOutput = true,
            UseShellExecute = false // required for redirection
        }
    };
    cmd.Start();
    return cmd.StandardOutput.ReadToEnd();
});

init();
app.Run();


record UserData(string Username, string Password);

record CommandData(string Username, string Password, string Command);