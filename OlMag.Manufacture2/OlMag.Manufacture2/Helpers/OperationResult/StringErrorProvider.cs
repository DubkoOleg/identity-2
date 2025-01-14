using System.Text.Json.Serialization;

namespace OlMag.Manufacture2.Helpers.OperationResult;

public interface IErrorProvider<TError> where TError : IErrorProvider<TError>
{
    static abstract implicit operator TError(string? source);
    static abstract implicit operator string(TError? error);

    bool Equals(string? value, StringComparison comparisonType);
}

public record StringErrorProvider
([property: JsonPropertyName("error")/*,Newtonsoft.Json: JsonProperty("error")*/]
    string StringError) : IErrorProvider<StringErrorProvider>
{
    public bool Equals(string? value, StringComparison comparisonType)
    {
        return StringError?.Equals(value, comparisonType) ?? false;
    }

    public static implicit operator StringErrorProvider(string? source)
    {
        return new StringErrorProvider(source ?? String.Empty);
    }

    public static implicit operator string(StringErrorProvider? error)
    {
        return error?.StringError ?? String.Empty;
    }

    public override string ToString()
    {
        return StringError;
    }
}