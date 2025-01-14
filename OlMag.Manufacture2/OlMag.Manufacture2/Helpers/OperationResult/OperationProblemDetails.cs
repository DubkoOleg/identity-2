using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;

namespace OlMag.Manufacture2.Helpers.OperationResult;

/// <summary>
///     Operation problem details. See also <seealso cref="ProblemDetails" />.
///     <remarks>HTTP API responses based on <seealso href="https://www.rfc-editor.org/rfc/rfc7807" /></remarks>
/// </summary>

public class OperationProblemDetails : ValidationProblemDetails, IErrorProvider<OperationProblemDetails>
{
    private const string DefaultErrorDetail = "Internal error";

    /// <summary>
    /// <inheritdoc cref="ValidationProblemDetails.Errors" />
    /// </summary>
    /// <example>{"path1": ["error"], "path2": ["error"]}</example>
    [JsonPropertyName("errors")]
    public new IDictionary<string, string[]>? Errors { get; set; }
    /// <summary>
    /// <inheritdoc cref="ProblemDetails.Extensions" />
    /// </summary>
    /// <example>{"key1": {}, "key2": {}}</example>
    [JsonPropertyName("extensions")]
    public new IDictionary<string, object?>? Extensions { get; set; }

    /// <summary>
    /// <inheritdoc cref="ProblemDetails.Type" />
    /// </summary>
    /// <example>https://dev.multicartshop.com/errors/types/product</example>
    [JsonPropertyName("type")]
    public new string? Type { get; set; }

    /// <summary>
    /// <inheritdoc cref="ProblemDetails.Title" />
    /// </summary>
    /// <example>Product errors</example>
    [JsonPropertyName("title")]
    public new string? Title { get; set; }

    /// <summary>
    /// <inheritdoc cref="ProblemDetails.Status" />
    /// </summary>
    /// <example>400</example>
    [JsonPropertyName("status")]
    public new int? Status { get; set; }

    /// <summary>
    /// <inheritdoc cref="ProblemDetails.Detail" />
    /// </summary>
    /// <example>Product is out of stock</example>
    [JsonPropertyName("detail")]
    public new string? Detail { get; set; }

    /// <summary>
    /// <inheritdoc cref="ProblemDetails.Instance" />
    /// </summary>
    /// <example>https://dev.multicartshop.com/errors/types/</example>
    [JsonPropertyName("instance")]
    public new string? Instance { get; set; }

    /// <summary>
    ///     <inheritdoc cref="IErrorProvider{TError}" />
    /// </summary>
    public static implicit operator OperationProblemDetails(string? source)
    {
        return ErrorProcessing.CreateProblemDetails(source ?? DefaultErrorDetail)
            .ToOperationProblemDetails();
    }

    /// <summary>
    ///     <inheritdoc cref="IErrorProvider{TError}" />
    /// </summary>
    public static implicit operator string(OperationProblemDetails? error)
    {
        return error?.Detail ?? DefaultErrorDetail;
    }

    /// <summary>
    ///     <inheritdoc cref="IErrorProvider{TError}" />
    /// </summary>
    public bool Equals(string? value, StringComparison comparisonType)
    {
        return Detail?.Equals(value, comparisonType) ?? false;
    }
}