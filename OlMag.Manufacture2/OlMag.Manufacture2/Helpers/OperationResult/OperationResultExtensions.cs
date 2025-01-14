using Microsoft.AspNetCore.Mvc;

namespace OlMag.Manufacture2.Helpers.OperationResult;


/// <summary>
///     Operation result extension methods
/// </summary>
public static class OperationResultExtensions
{
    internal const int SuccessDefaultStatus = StatusCodes.Status200OK;
    public const int FailedDefaultStatus = StatusCodes.Status400BadRequest;

    /// <summary>
    ///     Create success operation result from data
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="data"></param>
    /// <param name="status"></param>
    /// <returns></returns>
    public static OperationResult<T> Success<T>(T? data, int? status = default)
    {
        return new OperationResult<T>(data, default, true, status ?? SuccessDefaultStatus);
    }

    /// <summary>
    /// Create success operation result
    /// </summary>
    /// <param name="status"> Status code</param>
    /// <returns></returns>
    public static OperationResult Success(int? status = default)
    {
        return new OperationResult(default, true, StatusCode: status ?? SuccessDefaultStatus);
    }
    
    /// <summary>
    ///     Create failed operation result with optional data
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="data"></param>
    /// <param name="problemDetails"></param>
    /// <returns></returns>
    public static OperationResult<T> Failed<T>(OperationProblemDetails problemDetails, T? data = default)
    {
        return new OperationResult<T>(data, problemDetails, false, problemDetails.Status ?? FailedDefaultStatus);
    }

    /// <summary>
    /// Create failed operation result with optional data
    /// </summary>
    /// <param name="problemDetails"></param>
    /// <returns></returns>
    public static OperationResult Failed(OperationProblemDetails problemDetails)
    {
        return new OperationResult(problemDetails, false, problemDetails.Status ?? FailedDefaultStatus);
    }

    /// <summary>
    /// Create failed operation result with optional data
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="data"></param>
    /// <param name="problemDetails"></param>
    /// <returns></returns>
    public static OperationResult<T> Failed<T>(ProblemDetails problemDetails, T? data = default)
    {
        return Failed(problemDetails.ToOperationProblemDetails(), data);
    }

    /// <summary>
    /// Create failed operation result
    /// </summary>
    /// <param name="problemDetails"></param>
    /// <returns></returns>
    public static OperationResult Failed(ProblemDetails problemDetails)
    {
        return Failed(problemDetails.ToOperationProblemDetails());
    }

    /// <summary>
    /// Create failed operation result
    /// </summary>
    /// <param name="exception"></param>
    /// <returns></returns>
    public static OperationResult Failed(Exception exception)
    {
        return Failed(exception.ToOperationProblemDetails());
    }

    /// <summary>
    /// Create failed operation result
    /// </summary>
    /// <param name="exception"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    public static OperationResult<T> Failed<T>(Exception exception, T? data = default)
    {
        return Failed(exception.ToOperationProblemDetails(), data);
    }

    /// <summary>
    ///     Create failed operation result with optional data and operation result other type
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    public static OperationResult<T> Failed<T>(OperationResult source, T? data = default)
    {
        if (source.Result) throw new ArgumentException("Failed implicit successful OperationResult", nameof(source));

        return new OperationResult<T>(data, source.Error, false, source.StatusCode);
    }

    /// <summary>
    ///     Create failed operation result with error text, optional data and optional status code
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="errorText"></param>
    /// <param name="statusCode"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    public static OperationResult<T> Failed<T>(string errorText, int statusCode = FailedDefaultStatus,
        T? data = default)
    {
        return Failed(
            ErrorProcessing.CreateProblemDetails(errorText, statusCode).ToOperationProblemDetails(), data);
    }

    /// <summary>
    ///     Create failed operation result with error text, optional data and optional status code
    /// </summary>
    /// <param name="errorText"></param>
    /// <param name="statusCode"></param>
    /// <returns></returns>
    public static OperationResult Failed(string errorText,
        int statusCode = FailedDefaultStatus)
    {
        return Failed(ErrorProcessing.CreateProblemDetails(errorText, statusCode).ToOperationProblemDetails());
    }

    /// <summary>
    ///     Create failed operation result for <see cref="ResultOrError"/>
    /// </summary>
    /// <param name="result"></param>
    /// <returns></returns>
    public static OperationResult Failed(ResultOrError result)
    {
        return Failed(result.ToOperationProblemDetails());
    }

    /// <summary>
    ///     Create failed operation result for <see cref="ResultOrError"/>
    /// </summary>
    /// <param name="result"></param>
    /// <param name="data">Optional result value</param>
    /// <returns></returns>
    public static OperationResult<T> Failed<T>(ResultOrError result, T? data = default)
    {
        return Failed(result.ToOperationProblemDetails(), data);
    }

    /// <summary>
    ///     Return no data response with status code Status204NoContent
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static OperationResult<T> NoData<T>()
    {
        return new OperationResult<T>(default, default, true, StatusCodes.Status204NoContent);
    }

    /// <summary>
    ///     Data to operation result
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="data"></param>
    /// <param name="error"></param>
    /// <returns></returns>
    public static OperationResult<T> ToOperationResult<T>(T data, OperationProblemDetails? error = default)
    {
        return error == default(OperationProblemDetails) ? Success(data) : Failed(error, data);
    }
    
    /// <summary>
    ///     Result or error to operation Result
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="data"></param>
    /// <param name="error"></param>
    /// <returns></returns>
    public static OperationResult<T> ToOperationResult<T>(this ResultOrError<T> source, T? data = default,
        OperationProblemDetails? error = default)
    {
        return source.Result
            ? Success(data ?? source.Data)
            : Failed(error ?? source.ToOperationProblemDetails(), data ?? source.Data);
    }

    /// <summary>
    ///     Result or error to operation Result
    /// </summary>
    /// <param name="source"></param>
    /// <param name="error"></param>
    /// <returns></returns>
    public static OperationResult ToOperationResult(this ResultOrError source, OperationProblemDetails? error = default)
    {
        return source.Result ? Success() : Failed(error ?? source.ToOperationProblemDetails());
    }

    /// <summary>
    ///     Test enumerable as error and aggregate error results and success results separately
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="errorDetail"></param>
    /// <returns></returns>
    public static OperationResult<IEnumerable<OperationResult<T>>> TestError<T>(
        this IEnumerable<OperationResult<T>> source,
        string? errorDetail = default)
    {
        var data = source.ToArray();
        var failed = data.Where(res => !res.Result).ToArray();
        var success = data.Where(res => res.Result);

        switch (failed.Length)
        {
            case 0:
                return Success(data.AsEnumerable());
            case 1:
                return Failed(failed.First().Error!, success);
        }

        var result = ToOperationResult(data.AsEnumerable(),
            ResultOrError.Failed(errorDetail ?? "Aggregation error").ToOperationProblemDetails());

        foreach (var failedResult in failed)
        {
            if (failedResult.Error!.Errors != null)
            {
                result.Error!.Errors ??= new Dictionary<string, string[]>();
                foreach (var error in failedResult.Error.Errors)
                {
                    if (result.Error!.Errors.TryGetValue(error.Key, out var errors))
                        errors = errors.Concat(error.Value).ToArray();
                    else
                        errors = error.Value;

                    result.Error.Errors[error.Key] = errors;
                }
            }

            if (failedResult.Error.Extensions == null) continue;

            foreach (var extension in failedResult.Error.Extensions)
            {
                result.Error!.Extensions ??= new Dictionary<string, object?>();
                result.Error!.Extensions.TryAdd(extension.Key, extension.Value);
            }
        }

        return result;
    }
}