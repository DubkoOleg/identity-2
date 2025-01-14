using System.Net.Http.Headers;
using System.Net;
using System.Text.Json;

namespace OlMag.Manufacture2.Helpers.OperationResult;

public static class HttpResponseExtensions
{
    internal static ResultOrError<T> DeserializeResponse<T>(string content, T? def, ILogger? logger,
        JsonSerializerOptions? serializerOptions)
    {
        try
        {
            return string.IsNullOrWhiteSpace(content)
                ? ResultOrError<T>.Successful(def, StatusCodes.Status204NoContent)
                : JsonSerializer.Deserialize<T>(content, serializerOptions ?? SerializerOptionsHelper.SerializerOptions)!;
        }
        catch (Exception e)
        {
            logger?.LogError(e, $"Fail json convert. Data:\r\n{content}");
            return ResultOrError<T>.Failed(e, def);
        }
    }

    public static bool IsErrorStatusCode(this HttpResponseMessage response)
    {
        return response.StatusCode.IsErrorStatusCode();
    }

    public static bool IsErrorStatusCode(this HttpStatusCode statusCode)
    {
        var sc = (int)statusCode;
        return IsErrorStatusCode(sc);
    }

    public static bool IsErrorStatusCode(int statusCode)
    {
        return statusCode is > 399 and < 600;
    }

    public static bool IsServerErrorStatusCode(int statusCode)
    {
        return statusCode is > 499 and < 600;
    }

    public static bool IsServerErrorStatusCode(this HttpStatusCode statusCode)
    {
        var sc = (int)statusCode;
        return IsServerErrorStatusCode(sc);
    }

    public static bool IsClientErrorStatusCode(int statusCode)
    {
        return statusCode is > 399 and < 500;
    }

    public static bool IsClientErrorStatusCode(this HttpStatusCode statusCode)
    {
        var sc = (int)statusCode;
        return IsClientErrorStatusCode(sc);
    }

    public static HttpResponseHeaders AddOnce(this HttpResponseHeaders headers, string name, string value)
    {
        headers.Remove(name);
        headers.Add(name, value);

        return headers;
    }
}