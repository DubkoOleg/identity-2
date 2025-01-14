using System.Diagnostics.CodeAnalysis;

namespace OlMag.Manufacture2.Helpers.OperationResult;

public record ResultOrError<TValue, TError> : IResultOrError<TValue, TError> where TError : IErrorProvider<TError>
{
    public static readonly ResultOrError<TValue, TError> Unit = new(default, default, true, 100);

    public TError Error { get; init; } = default!;
    public TValue Data { get; init; } = default!;

    public int Code { get; init; }

    [MemberNotNullWhen(returnValue: false, member: nameof(Error))]
    [MemberNotNullWhen(returnValue: true, member: nameof(Data))]
    public bool Result { get; init; }

    public static implicit operator bool(ResultOrError<TValue, TError> source)
    {
        return source.Result;
    }

    public static implicit operator ResultOrError(ResultOrError<TValue, TError> source)
    {
        return source.Result
            ? ResultOrError.Successful(source.Code)
            : ResultOrError.Failed(source.Error, code: source.Code);
    }

    public ResultOrError(TValue? data, TError? error, bool result, int code)
    {
        Data = data!;
        Error = error!;
        Result = result;
        Code = code;
    }

    public override string ToString()
    {
        return Result ? $"Successful. Data: {Data?.ToString()}" : $"Error {Error?.ToString()}. Code: {Code}";
    }

    public static ResultOrError<TValue, TError> Failed(TError error, TValue? data = default, int code = 400)
    {
        return ResultOrErrorExtensions.Failed(error, data, code);
    }

    public static ResultOrError<TValue, TError> Successful(TValue data, int code = ResultOrErrorExtensions.DefaultSuccessStatusCode)
    {
        return ResultOrErrorExtensions.Successful<TValue, TError>(data, code);
    }

    public static ResultOrError<TValue, TError> Successful<TOtherValue>(
        ResultOrError<TOtherValue, TError> parent, TValue data)
    {
        return ResultOrErrorExtensions.Successful<TValue, TError>(data, parent.Code);
    }

    public static ResultOrError<TValue, TError> Failed<TOtherValue>(ResultOrError<TOtherValue, TError> parent,
        TValue? data = default, TError? error = default)
    {
        return ResultOrErrorExtensions.Failed(error ?? parent.Error, data, parent.Code);
    }
}

public record ResultOrError<T> : ResultOrError<T, StringErrorProvider>, IResultOrError<T>
{
    public new static readonly ResultOrError<T> Unit = new();

    public ResultOrError(T? data, StringErrorProvider? error, bool result, int code) : base(data, error, result, code)
    { }

    protected ResultOrError() : base(default, default, true, 100)
    { }

    public static implicit operator ResultOrError(ResultOrError<T> source)
    {
        return source.Result
            ? ResultOrError.Successful(source.Code)
            : ResultOrError.Failed(source.Error, code: source.Code);
    }

    [return: NotNull]
    public static implicit operator T?(ResultOrError<T> source)
    {
        return source.Result
            ? source.Data
            : throw new Exception("Failed data type implicit for failed ResultOrError. Use ResultOrError<T>.Data");
    }

    public static implicit operator ResultOrError<T>(T source)
    {
        return ResultOrErrorExtensions.Successful(source);
    }

    public new static ResultOrError<T> Successful(T data, int code = ResultOrErrorExtensions.DefaultSuccessStatusCode)
    {
        return ResultOrErrorExtensions.Successful(data, code);
    }

    public static ResultOrError<T> Failed(string error, T? data = default, int code = ResultOrErrorExtensions.DefaultFailedStatusCode)
    {
        return ResultOrErrorExtensions.Failed(error, data, code);
    }

    public static ResultOrError<T> Failed(Exception error, T? data = default, int code = ResultOrErrorExtensions.DefaultFailedStatusCode)
    {
        return ResultOrErrorExtensions.Failed(error, data, code);
    }

    public static ResultOrError<T> Failed<TOtherValue>(ResultOrError<TOtherValue> parent, T? data = default,
        string? error = default)
    {
        return ResultOrErrorExtensions.Failed(error ?? (string)parent.Error, data, parent.Code);
    }

    public static ResultOrError<T> Failed(ResultOrError parent, T? data = default, string? error = default)
    {
        return ResultOrErrorExtensions.Failed(error ?? (string)parent.Error, data, parent.Code);
    }
}

public record ResultOrError : ResultOrError<bool, StringErrorProvider>, IResultOrError
{
    public new static readonly ResultOrError Unit = new();

    public ResultOrError(bool data, StringErrorProvider? error, bool result, int code) : base(data, error, result,
        code)
    { }

    protected ResultOrError() : base(default, default, true, 100)
    { }

    public static implicit operator bool(ResultOrError source)
    {
        return source.Result;
    }

    public static ResultOrError Successful(int code = ResultOrErrorExtensions.DefaultSuccessStatusCode)
    {
        return ResultOrErrorExtensions.Successful(code);
    }

    public static ResultOrError<T> Successful<T>(T data, int code = ResultOrErrorExtensions.DefaultSuccessStatusCode)
    {
        return ResultOrErrorExtensions.Successful(data, code);
    }

    public static ResultOrError Failed(string error, int code = ResultOrErrorExtensions.DefaultFailedStatusCode)
    {
        return ResultOrErrorExtensions.Failed(error, code);
    }

    public static ResultOrError<T> Failed<T>(string error, T? data = default, int code = ResultOrErrorExtensions.DefaultFailedStatusCode)
    {
        return ResultOrErrorExtensions.Failed(error, data, code);
    }

    public static ResultOrError Failed(Exception e, int code = ResultOrErrorExtensions.DefaultFailedStatusCode)
    {
        return ResultOrErrorExtensions.Failed(e, code);
    }

    public static ResultOrError<T> Failed<T>(Exception e, int code = ResultOrErrorExtensions.DefaultFailedStatusCode)
    {
        return ResultOrErrorExtensions.Failed<T>(e, default, code);
    }

    public static ResultOrError<T> Failed<T>(ResultOrError<T> parent, T? data = default, string? error = default)
    {
        return ResultOrErrorExtensions.Failed(error ?? parent.Error, data, parent.Code);
    }

    public static ResultOrError<T> Failed<T>(ResultOrError parent, T? data = default, string? error = default)
    {
        return ResultOrErrorExtensions.Failed(error ?? parent.Error, data, parent.Code);
    }
}