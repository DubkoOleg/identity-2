namespace OlMag.Manufacture2.Helpers.OperationResult;

interface IResultOrError<TValue, TError> where TError : IErrorProvider<TError>
{
    static abstract ResultOrError<TValue, TError> Failed(TError error, TValue? data = default,
        int code = ResultOrErrorExtensions.DefaultFailedStatusCode);

    static abstract ResultOrError<TValue, TError> Failed<TOtherValue>(
        ResultOrError<TOtherValue, TError> parent, TValue? data = default, TError? error = default);

    static abstract ResultOrError<TValue, TError> Successful(TValue data,
        int code = ResultOrErrorExtensions.DefaultSuccessStatusCode);

    static abstract ResultOrError<TValue, TError> Successful<TOtherValue>(ResultOrError<TOtherValue, TError> parent,
        TValue data);
}

interface IResultOrError<T>
{
    static abstract ResultOrError<T>
        Successful(T data, int code = ResultOrErrorExtensions.DefaultSuccessStatusCode);

    static abstract ResultOrError<T> Failed(string error, T? data = default,
        int code = ResultOrErrorExtensions.DefaultFailedStatusCode);

    static abstract ResultOrError<T> Failed(Exception error, T? data = default,
        int code = ResultOrErrorExtensions.DefaultFailedStatusCode);

    static abstract ResultOrError<T> Failed<TOtherValue>(ResultOrError<TOtherValue> parent,
        T? data = default, string? error = default);

    static abstract ResultOrError<T> Failed(ResultOrError parent, T? data = default, string? error = default);
}

interface IResultOrError
{
    static abstract ResultOrError Successful(int code = ResultOrErrorExtensions.DefaultSuccessStatusCode);

    static abstract ResultOrError<T> Successful<T>(T data,
        int code = ResultOrErrorExtensions.DefaultSuccessStatusCode);

    static abstract ResultOrError Failed(string error, int code = ResultOrErrorExtensions.DefaultFailedStatusCode);

    static abstract ResultOrError<T> Failed<T>(string error, T? data = default,
        int code = ResultOrErrorExtensions.DefaultFailedStatusCode);

    static abstract ResultOrError Failed(Exception e, int code = ResultOrErrorExtensions.DefaultFailedStatusCode);

    static abstract ResultOrError<T> Failed<T>(Exception e,
        int code = ResultOrErrorExtensions.DefaultFailedStatusCode);

    static abstract ResultOrError<T> Failed<T>(ResultOrError<T> parent, T? data = default, string? error = default);
    static abstract ResultOrError<T> Failed<T>(ResultOrError parent, T? data = default, string? error = default);
}

public static class ResultOrErrorExtensions
{
    public const int DefaultSuccessStatusCode = 200;
    public const int DefaultFailedStatusCode = 400;
    public const int DefaultExceptionStatusCode = 500;

    private static readonly ResultOrError SuccessResult =
        new(true, default!, true, DefaultSuccessStatusCode);

    public static ResultOrError Failed(string error, int code = DefaultFailedStatusCode)
    {
        return new ResultOrError(false, error, false, code);
    }

    public static ResultOrError Failed(Exception error,
        int code = DefaultExceptionStatusCode)
    {
        return Failed(error.Message, code);
    }

    public static ResultOrError Successful(int code = DefaultSuccessStatusCode)
    {
        return code == SuccessResult.Code ? SuccessResult : new ResultOrError(true, default!, true, code);
    }

    public static ResultOrError<T> Failed<T>(string error, T? data = default,
        int code = DefaultFailedStatusCode)
    {
        if (data is ResultOrError) throw new Exception("Maybe incorrect data type implicit");

        var result = new ResultOrError<T>(data!, error, false, code);

        return result;
    }

    public static ResultOrError<T> Successful<T>(T data,
        int code = DefaultSuccessStatusCode)
    {
        return new ResultOrError<T>(data, default!, true, code);
    }

    public static ResultOrError<T> Failed<T>(Exception error, T? data = default,
        int code = DefaultExceptionStatusCode)
    {
        return Failed(error.ToString(), data, code);
    }

    public static ResultOrError<T> Failed<T>(ResultOrError parent, T? data = default, string? error = default)
    {
        return Failed(error ?? parent.Error, data, parent.Code);
    }

    public static ResultOrError<TValue, TError> Failed<TValue, TError>(TError error, TValue? data = default,
        int code = DefaultFailedStatusCode) where TError : IErrorProvider<TError>
    {
        if (data is ResultOrError) throw new Exception("Maybe incorrect data type implicit");

        var result = new ResultOrError<TValue, TError>(data!, error, false, code);

        return result;
    }

    public static ResultOrError<TValue, TError> Failed<TValue, TError>(Exception error, TValue? data = default,
        int code = DefaultExceptionStatusCode) where TError : IErrorProvider<TError>
    {
        return Failed((TError)error.Message, data, code);
    }

    public static ResultOrError<TValue, TError> Successful<TValue, TError>(TValue data,
        int code = DefaultSuccessStatusCode) where TError : IErrorProvider<TError>
    {
        return new ResultOrError<TValue, TError>(data, default!, true, code);
    }

    public static ResultOrError<IEnumerable<ResultOrError>> TestError(this IEnumerable<ResultOrError> source)
    {
        var data = source.ToArray();
        var failed = data.Where(res => !res).ToArray();

        if (failed.Length == 0) return Successful(data.AsEnumerable());

        var success = data.Where(res => res);
        var error = String.Join(';', failed.Select(x => x.Error));
        var code = failed.Select(x => x.Code).Max();

        return Failed(error, success, code == 0 ? 400 : code);
    }

    public static ResultOrError<IEnumerable<ResultOrError>> TestError<T>(this IEnumerable<ResultOrError<T>> source)
    {
        return source.Select(x => (ResultOrError)x).TestError();
    }
}