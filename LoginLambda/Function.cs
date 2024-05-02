using Amazon.Lambda.Serialization.SystemTextJson;
using LoginLambda.Controllers;
using LoginLambda.Shared.DataAccess;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Shared;
using Shared.DataAccess;

var builder = WebApplication.CreateSlimBuilder(args);
            
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolver = ApiSerializerContext.Default;
});

builder.Services.AddSingleton<IUsersDAO, DynamoDbUsers>();

builder.Services.AddAWSLambdaHosting(LambdaEventSource.RestApi, options =>
{
    options.Serializer = new SourceGeneratorLambdaJsonSerializer<ApiSerializerContext>();
});
            
builder.Logging.ClearProviders();
builder.Logging.AddJsonConsole(options =>
{
    options.IncludeScopes = true;
    options.UseUtcTimestamp = true;
    options.TimestampFormat = "hh:mm:ss ";
});

var app = builder.Build();

LoginController.DataAccess = app.Services.GetRequiredService<IUsersDAO>();
LoginController.Logger = app.Logger;

app.MapPost("/signin", LoginController.SignIn);
app.MapPost("/signup", LoginController.SignUp);

app.Run();