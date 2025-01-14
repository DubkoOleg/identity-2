using MapsterMapper;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using OlMag.Manufacture2.Helpers.OperationResult;

namespace OlMag.Manufacture2.Controllers.Base;

/// <summary>
/// Base API controller
/// </summary>
/// <param name="mapper">Mapper</param>
/// <param name="logger">Logger</param>
public abstract class BaseApiController(IMapper mapper, ILogger logger) : ControllerBase
{
    /// <summary>
    /// Mapper
    /// </summary>
    protected readonly IMapper Mapper = mapper;

    /// <summary>
    /// Logger
    /// </summary>
    protected readonly ILogger? Logger = logger;

    /// <summary>
    /// Create empty ActionResult
    /// </summary>
    /// <typeparam name="T">Type of result</typeparam>
    /// <param name="result">Result</param>
    /// <returns>Action result</returns>
    protected ActionResult EmptyResult<T>(OperationResult<T> result)
    {
        if (result.Result)
            return NoContent();

        return EmptyFailedActionResult(result);
    }

    /// <summary>
    /// Create ActionResult
    /// </summary>
    /// <param name="result">Result</param>
    /// <returns>Action result</returns>
    protected ActionResult Result(OperationResult result)
    {
        if (result.Result)
            return Ok();

        return FailedActionResult(result);
    }

    /// <summary>
    /// Create ActionResult
    /// </summary>
    /// <typeparam name="T">Type of result</typeparam>
    /// <param name="result">Result</param>
    /// <param name="successStatusCode">Success status code</param>
    /// <returns>Action result</returns>
    protected ActionResult<T> Result<T>(OperationResult<T> result, int successStatusCode)
    {
        return result.Result
            ? StatusCode(successStatusCode, result.Data)
            : FailedActionResult(result);
    }

    /// <summary>
    /// Create ActionResult
    /// </summary>
    /// <typeparam name="T">Type of result</typeparam>
    /// <param name="result">Result</param>
    /// <returns>Action result</returns>
    protected ActionResult<T> Result<T>(OperationResult<T> result)
    {
        return result.Result
            ? StatusCode(result.StatusCode, result.Data)
            : FailedActionResult(result);
    }

    /// <summary>
    /// Create ActionResult
    /// </summary>
    /// <typeparam name="T">Type of result</typeparam>
    /// <param name="result">Result</param>
    /// <returns>Action result</returns>
    private ActionResult EmptyFailedActionResult<T>(OperationResult<T> result)
    {
        Logger?.LogError(result.Error);

        var data = new ObjectResult(result)
        {
            StatusCode = result.Error?.Status ?? result.StatusCode,
            ContentTypes = { ContentTypes.ProblemJson },
            DeclaredType = typeof(OperationResult<T>),
        };

        return data;
    }

    /// <summary>
    /// Create ActionResult
    /// </summary>
    /// <typeparam name="T">Type of result</typeparam>
    /// <param name="result">Result</param>
    /// <returns>Action result</returns>
    private ActionResult<T> FailedActionResult<T>(OperationResult<T> result)
    {
        Logger?.LogError(result.Error);

        var data = new ObjectResult(result)
        {
            StatusCode = result.Error?.Status ?? result.StatusCode,
            ContentTypes = { ContentTypes.ProblemJson },
            DeclaredType = typeof(OperationResult<T>),
        };

        return data;
    }

    /// <summary>
    /// Create ActionResult
    /// </summary>
    /// <param name="result">Result</param>
    /// <returns>Action result</returns>
    private ActionResult FailedActionResult(OperationResult result)
    {
        Logger?.LogError(result.Error);

        var data = new ObjectResult(result)
        {
            StatusCode = result.Error?.Status ?? result.StatusCode,
            ContentTypes = { ContentTypes.ProblemJson },
            DeclaredType = typeof(OperationResult),
        };

        return data;
    }

    /// <summary>
    /// Create ActionResult
    /// </summary>
    /// <param name="message">Message</param>
    /// <returns>Action result</returns>
    protected ActionResult BadRequestResult(string message)
    {
        Logger?.LogError("Bad request: {message}", message);

        return new ObjectResult(message)
        {
            StatusCode = StatusCodes.Status400BadRequest,
            ContentTypes = { ContentTypes.ProblemJson },
            DeclaredType = typeof(string)
        };
    }

    /// <summary>
    /// Create ActionResult
    /// </summary>
    /// <typeparam name="TOut">Input type of result</typeparam>
    /// <typeparam name="TIn">Output type of result</typeparam>
    /// <param name="result">Result</param>
    /// <returns>Action result</returns>
    protected ActionResult<TOut> Result<TOut, TIn>(OperationResult<TIn> result)
    {
        var data = result.Data == null ? default : Mapper.Map<TOut>(result.Data);

        if (result.Result)
        {
            return StatusCode(result.StatusCode, data);
        }

        var problemResult = OperationResultExtensions.Failed(result, data);
        return (ActionResult)((IConvertToActionResult)ControllerExtensions.FailedActionResult(problemResult,
            Logger)).Convert();
    }

    /// <summary>
    /// Create ActionResult
    /// </summary>
    /// <typeparam name="T">Type of result</typeparam>
    /// <param name="result">Result</param>
    /// <param name="url">Url</param>
    /// <returns>Action result</returns>
    protected ActionResult<T> CreatedResult<T>(OperationResult<T> result, string? url)
    {
        if (result.Result)
        {
            return !string.IsNullOrEmpty(url)
                ? Created(url, result.Data)
                : StatusCode(result.StatusCode, result.Data);
        }

        return FailedActionResult(result);
    }

    /// <summary>
    /// Create ActionResult
    /// </summary>
    /// <typeparam name="TOut">Input type of result</typeparam>
    /// <typeparam name="TIn">Output type of result</typeparam>
    /// <param name="result">Result</param>
    /// <param name="url">Url</param>
    /// <returns>Action result</returns>
    protected ActionResult<TOut> CreatedResult<TOut, TIn>(OperationResult<TIn> result, string? url)
    {
        var data = result.Data == null ? default : Mapper.Map<TOut>(result.Data);
        if (result.Result)
        {
            return !string.IsNullOrEmpty(url)
                ? Created(url, data)
                : StatusCode(result.StatusCode, data);
        }

        var problemResult = OperationResultExtensions.Failed(result, data);
        return FailedActionResult(problemResult);
    }
}