using System.Diagnostics.CodeAnalysis;

namespace OlMag.Manufacture2.Helpers.OperationResult;

public record OperationResult(
    OperationProblemDetails? Error,
    bool Result,
    int StatusCode = OperationResultExtensions.FailedDefaultStatus)
{
    /// <summary>
    /// Operation problem details
    /// </summary>
    public OperationProblemDetails? Error { get; init; } = Error;

    /// <summary>
    /// Operation result
    /// </summary>
    /// <example>true</example>
    [MemberNotNullWhen(false, nameof(Error))]
    public bool Result { get; init; } = Result;

    /// <summary>
    /// Operation http status code
    /// </summary>
    /// <example>200</example>
    public int StatusCode { get; set; } = StatusCode;
}

/// <summary>
///     Operation result
/// </summary>
/// <typeparam name="T">Type of method payload data</typeparam>
public record OperationResult<T> : OperationResult
{
    /// <summary>
    ///     Operation result
    /// </summary>
    /// <param name="data">Result data</param>
    /// <param name="error">Problem details if exist. <seealso href="https://www.rfc-editor.org/rfc/rfc7807" /></param>
    /// <param name="result">Operation result</param>
    /// <param name="statusCode">Status code</param>
    public OperationResult(T? data, OperationProblemDetails? error, bool result, int statusCode) : base(error, result, statusCode)
    {
        Data = data;
        Error = error;
        Result = result;
        StatusCode = statusCode;
    }

    /// <summary>
    /// Operation data payload
    /// </summary>
    public T? Data { get; init; }

    /// <summary>
    /// Operation problem details
    /// </summary>
    public new OperationProblemDetails? Error { get; init; }

    /// <summary>
    /// Operation result
    /// </summary>
    /// <example>true</example>
    [MemberNotNullWhen(false, nameof(Error))]
    [MemberNotNullWhen(true, nameof(Data))]
    public new bool Result { get; init; }

    /// <summary>
    ///     Implicit conversion from result error. See also <seealso cref="ResultOrError{T}" />
    /// </summary>
    public static implicit operator OperationResult<T>(ResultOrError<T> source)
    {
        return new OperationResult<T>(source.Data, source.Result ? default : source.ToOperationProblemDetails(),
            source.Result, source.Code);
    }

    /// <summary>
    ///     Implicit conversion from data result
    /// </summary>
    /// <param name="source"></param>
    public static implicit operator OperationResult<T>(T source)
    {
        return new OperationResult<T>(source, default, true, OperationResultExtensions.SuccessDefaultStatus);
    }
}