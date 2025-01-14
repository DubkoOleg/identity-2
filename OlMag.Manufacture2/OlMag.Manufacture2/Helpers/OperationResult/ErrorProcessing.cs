using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text;

namespace OlMag.Manufacture2.Helpers.OperationResult;

/// <summary>
///     Error processing helper
/// </summary>
public static class ErrorProcessing
{
    private const int DefaultFailedStatusCode = 500;

    /// <summary>
    ///     Create Problem Details Object for general error
    ///     <see href="https://dev.multicartshop.com/errors/types/general.html"></see>
    /// </summary>
    /// <param name="detail">A human-readable explanation specific to this occurrence of the problem</param>
    /// <param name="statusCode">The HTTP status code</param>
    /// <param name="modelStateDictionary">Optional dictionary with validation information</param>
    /// <param name="instance">A URI reference that identifies the specific occurrence of the problem</param>
    /// <returns>ProblemDetails object</returns>
    public static ProblemDetails CreateProblemDetails(string detail, int statusCode = DefaultFailedStatusCode,
        ModelStateDictionary? modelStateDictionary = default, string? instance = default)
    {
        return CreateProblemDetails(GetTitleByStatusCode(statusCode) ?? ConstantsOperationResult.GeneralErrorTitle,
            GetProblemDetailsType(ConstantsOperationResult.GeneralErrorType), detail, statusCode, modelStateDictionary, instance);
    }

    /// <summary>
    ///     Create Problem Details Object for general error
    ///     <see href="https://dev.multicartshop.com/errors/types/general.html"></see>
    /// </summary>
    /// <param name="exception">Source exception</param>
    /// <returns>ProblemDetails object</returns>
    public static ProblemDetails CreateProblemDetails(this Exception exception)
    {
        var problemDetails = ErrorProcessing.CreateProblemDetails(ConstantsOperationResult.GeneralErrorTitle,
            ErrorProcessing.GetProblemDetailsType(ConstantsOperationResult.GeneralErrorType), exception.ToString(),
            instance: exception.Source);

        return problemDetails;
    }

    internal static string? GetTitleByStatusCode(int statusCode)
    {
        var status = (HttpStatusCode)statusCode;
        var statusStr = status.ToString();

        if (int.TryParse(statusStr, out _))
            return null;

        var sb = new StringBuilder();
        foreach (var ch in statusStr)
        {
            if (char.IsUpper(ch) && sb.Length > 0)
                sb.Append(' ');

            sb.Append(ch);
        }

        return sb.ToString();
    }

    internal static string? GetDefaultInstanceByStatusCode(int statusCode, string problemTypeName)
    {
        if (!HttpResponseExtensions.IsErrorStatusCode(statusCode))
            return null;

        var status = (HttpStatusCode)statusCode;
        var statusStr = status.ToString();

        return $"{ConstantsOperationResult.ProblemUriHost}/{problemTypeName}/{statusStr.ToLowerInvariant()}";
    }

    /// <summary>
    ///     Generator a URI reference that identifies the problem type
    /// </summary>
    /// <param name="problemTypeName">Problem type</param>
    /// <returns></returns>
    public static string GetProblemDetailsType(string problemTypeName)
    {
        return $"{ConstantsOperationResult.ProblemUriHost}/{problemTypeName}";
    }

    /// <summary>
    ///     Create Problem Details Object for general error
    ///     <see href="https://dev.multicartshop.com/errors/types/general.html"></see>
    /// </summary>
    /// <typeparam name="T">Generic type parent ResultOrError record</typeparam>
    /// <param name="result">Parent ResultOrError record</param>
    /// <param name="modelStateDictionary">Optional dictionary with validation information</param>
    /// <returns>ProblemDetails object</returns>
    /// <exception cref="ArgumentException">Argument result is not failed</exception>
    public static ProblemDetails CreateProblemDetails<T>(this ResultOrError<T> result,
        ModelStateDictionary? modelStateDictionary = default)
    {
        return result.CreateProblemDetails(
            ErrorProcessing.GetTitleByStatusCode(result.Code) ?? ConstantsOperationResult.GeneralErrorTitle,
            ErrorProcessing.GetProblemDetailsType(ConstantsOperationResult.GeneralErrorType),
            modelStateDictionary);
    }

    /// <summary>
    ///     Create Problem Details Object for general error
    ///     <see href="https://dev.multicartshop.com/errors/types/general.html"></see>
    /// </summary>
    /// <param name="result">Parent ResultOrError record</param>
    /// <param name="modelStateDictionary">Optional dictionary with validation information</param>
    /// <returns>ProblemDetails object</returns>
    /// <exception cref="ArgumentException">Argument result is not failed</exception>
    public static ProblemDetails CreateProblemDetails(this ResultOrError result,
        ModelStateDictionary? modelStateDictionary = default)
    {
        return result.CreateProblemDetails(
            ErrorProcessing.GetTitleByStatusCode(result.Code) ?? ConstantsOperationResult.GeneralErrorTitle,
            ErrorProcessing.GetProblemDetailsType(ConstantsOperationResult.GeneralErrorType),
            modelStateDictionary);
    }

    internal static ProblemDetails CreateProblemDetails(string title, string type, string detail, int code = DefaultFailedStatusCode,
        ModelStateDictionary? modelStateDictionary = default, string? instance = default)
    {
        ProblemDetails details;

        if (modelStateDictionary == default)
        {
            details = new ProblemDetails();
        }
        else
        {
            var modelState = new ModelStateDictionary(modelStateDictionary);
            details = new ValidationProblemDetails(modelState);
        }

        details.Detail = detail;
        details.Title = title;
        details.Status = code;
        details.Type = type;

        details.Instance = instance;

        return details;
    }

    internal static ProblemDetails CreateProblemDetails(this ResultOrError result, string title, string type,
        ModelStateDictionary? modelStateDictionary = default, string? instance = default)
    {
        ValidateSource(result);

        return CreateProblemDetails(title, type, result.Error.StringError, result.Code, modelStateDictionary, instance);
    }

    internal static ProblemDetails CreateProblemDetails<T>(this ResultOrError<T> result, string title, string type,
        ModelStateDictionary? modelStateDictionary = default, string? instance = default)
    {
        ValidateSource(result);

        return CreateProblemDetails(title, type, result.Error.StringError, result.Code, modelStateDictionary, instance);
    }

    private static void ValidateSource(ResultOrError result)
    {
        if (result.Result)
            throw new ArgumentException($"Only negative results can be source for {nameof(ProblemDetails)}", nameof(result));

        if (!HttpResponseExtensions.IsErrorStatusCode(result.Code))
            throw new ArgumentException(
                $"Only error status code ({nameof(HttpResponseExtensions.IsErrorStatusCode)}) can be source for {nameof(ProblemDetails)}",
                nameof(result));
    }
}