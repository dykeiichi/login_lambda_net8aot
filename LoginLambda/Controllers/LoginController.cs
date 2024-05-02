using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using Shared;
using JsonSerializer = System.Text.Json.JsonSerializer;
using System;
using Shared.DataAccess;
using LoginLambda.Models;

namespace LoginLambda.Controllers
{
    public static class LoginController
    {

        internal static ILogger? Logger;
        internal static IUsersDAO? DataAccess;
    
        public static async Task SignUp(HttpContext context)
        {
            if(DataAccess is null) {
                await context.WriteResponse(HttpStatusCode.OK, "{ \"message\": \"Error something happend :c\", \"code\": 400}");
                Logger?.LogError("DataAcces is null");
                return;
            }
            try {
                User? User = (User?)JsonSerializer.Deserialize(context.Request.Body, typeof(User), ApiSerializerContext.Default);
                if (User is not null) {
                    if (DataAccess.GetUser(User.Username) is null) {
                        if (UserContext.SignUp(User, out UserContext UserToPut)) {
                            await DataAccess.PutUser(UserToPut);
                            await context.WriteResponse(HttpStatusCode.OK, "{ \"message\": \"User sign up completed\", \"code\": 200}");
                            Logger?.LogInformation("User sign up completed");
                            return;
                        }
                        Logger?.LogInformation("Error while Signup");
                        await context.WriteResponse(HttpStatusCode.OK, "{ \"message\": \"Error while Signup\", \"code\": 400}");
                        return;
                    }
                    await context.WriteResponse(HttpStatusCode.OK, "{ \"message\": \"Error User Already exists\", \"code\": 400}");
                    Logger?.LogInformation("Error while GetUser");
                    return;
                } else {
                    await context.WriteResponse(HttpStatusCode.OK, "{ \"message\": \"Error body should not be null\", \"code\": 400}");
                    Logger?.LogInformation("Error body should not be null");
                    return;
                }
            } catch (Exception ex) {
                await context.WriteResponse(HttpStatusCode.OK, "{ \"message\": \"Error something happend :c\", \"code\": 400}");
                Logger?.LogError("{}", ex.Message);
                return;
            }
        }

        public static async Task SignIn(HttpContext context) {
            if(DataAccess is null) {
                await context.WriteResponse(HttpStatusCode.OK, "{ \"message\": \"Error something happend :c\", \"code\": 400}");
                Logger?.LogError("DataAcces is null");
                return;
            }
            try {
                User? User = (User?)JsonSerializer.Deserialize(context.Request.Body, typeof(User), ApiSerializerContext.Default);
                if (User is not null) {
                    UserContext? userContext = DataAccess.GetUser(User.Username);
                    if (userContext is not null) {
                        if (userContext.SignIn(User.Password)) {
                            await context.WriteResponse(HttpStatusCode.OK, "{ \"message\": \"User sign in completed\", \"code\": 200}");
                            Logger?.LogInformation("User sign in completed");
                            return;
                        }
                        Logger?.LogInformation("Error Username or Password is not correct"); // Password is not correct
                        await context.WriteResponse(HttpStatusCode.OK, "{ \"message\": \"Error Username or Password is not correct\", \"code\": 400}");
                        return;
                    }
                    await context.WriteResponse(HttpStatusCode.OK, "{ \"message\": \"Error Username or Password is not correct\", \"code\": 400}");
                    Logger?.LogInformation("Error while GetUser"); // Username does not exists
                    return;
                } else {
                    await context.WriteResponse(HttpStatusCode.OK, "{ \"message\": \"Error body should not be null\", \"code\": 400}");
                    Logger?.LogInformation("Error body should not be null");
                    return;
                }
            } catch (Exception ex) {
                await context.WriteResponse(HttpStatusCode.OK, "{ \"message\": \"Error something happend :c\", \"code\": 400}");
                Logger?.LogError("{}", ex.Message);
                return;
            }
        }
    }
}