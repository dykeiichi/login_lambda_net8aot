using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Net;
using JsonSerializer = System.Text.Json.JsonSerializer;
using System.IO;

namespace Shared;

static class ResponseWriter
{
    public static async Task WriteResponse(this HttpContext context, HttpStatusCode statusCode)
    {
        await context.WriteResponse(statusCode, "");
    }
    
    public static async Task WriteResponse<TResponseType>(this HttpContext context, HttpStatusCode statusCode, TResponseType body) where TResponseType : class
    {
        context.Response.StatusCode = (int)statusCode;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync(JsonSerializer.Serialize(body, typeof(TResponseType), ApiSerializerContext.Default));
    }
    
    public static async Task WriteResponse(this HttpContext context, HttpStatusCode statusCode, string body)
    {
        context.Response.StatusCode = (int)statusCode;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync(body);
    }

    public static string StreamToString(Stream stream)
    {
        stream.Position = 0;
        using StreamReader reader = new(stream, System.Text.Encoding.UTF8);
        return reader.ReadToEnd();
    } 
}