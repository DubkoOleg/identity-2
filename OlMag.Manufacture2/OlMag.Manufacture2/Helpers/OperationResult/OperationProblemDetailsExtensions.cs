using Microsoft.AspNetCore.Mvc;

namespace OlMag.Manufacture2.Helpers.OperationResult;

/// <summary>
///     Operation problem details extension methods
/// </summary>
public static class OperationProblemDetailsExtensions
{
    internal const string ValidationErrorMessage =
        "Request data validation error. There is an issue with the data you provided. Please double-check the entered values and try again";

    /// <summary>
    ///     Implicit conversion from ResultOrError. See also <seealso cref="ResultOrError{T}" />
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <returns></returns>
    public static OperationProblemDetails ToOperationProblemDetails<T>(this ResultOrError<T> source)
    {
        return source.CreateProblemDetails().ToOperationProblemDetails();
    }

    /// <summary>
    ///     Implicit conversion from ResultOrError. See also <seealso cref="ResultOrError" />
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static OperationProblemDetails ToOperationProblemDetails(this ResultOrError source)
    {
        return source.CreateProblemDetails().ToOperationProblemDetails();
    }

    /// <summary>
    ///     Implicit conversion from base <seealso cref="ProblemDetails">ProblemDetails</seealso>
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static OperationProblemDetails ToOperationProblemDetails(this ProblemDetails source)
    {
        var result = new OperationProblemDetails
        {
            Detail = source.Detail ?? ValidationErrorMessage,
            Status = source.Status,
            Instance = source.Instance,
            Title = source.Title,
            Type = source.Type,
            Extensions = source.Extensions
        };

        if (!source.CheckErrorType())
        {
            result.Title =
                (source.Status.HasValue ? ErrorProcessing.GetTitleByStatusCode(source.Status.Value) : null) ??
                ConstantsOperationResult.GeneralErrorTitle;
            result.Type = ErrorProcessing.GetProblemDetailsType(ConstantsOperationResult.GeneralErrorType);
        }

        if (source is not ValidationProblemDetails validationProblemDetails) return result;

        result.Errors = validationProblemDetails.Errors;
        return result;
    }

    /// <summary>
    ///     Implicit conversion from <seealso cref="Exception">Exception</seealso>
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static OperationProblemDetails ToOperationProblemDetails(this Exception source)
    {
        var result = source.CreateProblemDetails().ToOperationProblemDetails();
        return result;
    }

    /// <summary>
    ///     Add errors
    /// </summary>
    /// <param name="source"></param>
    /// <param name="errors"></param>
    /// <returns></returns>
    public static OperationProblemDetails AddErrors(this OperationProblemDetails source,
        params KeyValuePair<string, string[]>[] errors)
    {
        if (source.Errors == null)
        {
            source.Errors = new Dictionary<string, string[]>(errors);
            return source;
        }

        foreach (var error in errors)
            source.Errors.TryAddOrConcat(error.Key, error.Value);

        return source;
    }

    internal static IDictionary<string, string[]> TryAddOrConcat(this IDictionary<string, string[]> source, string key,
        string[] error)
    {
        if (source.TryGetValue(key, out var errors))
            errors = errors.Concat(error).ToArray();
        else
            errors = error;

        source[key] = errors;
        return source;
    }
}