using Microsoft.AspNetCore.Mvc;

namespace OlMag.Manufacture2.Helpers.OperationResult;

/// <summary>
/// Controller extensions methods cast <see cref="OperationResult{T}"/> to <see cref="ActionResult{TValue}"/>
/// </summary>
public static class ControllerExtensions
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="controller"></param>
    /// <param name="result"></param>
    /// <param name="logger"></param>
    /// <returns></returns>
    public static ActionResult<OperationResult<T>> ToActionResult<T>(this ControllerBase controller,
        ResultOrError<T> result, ILogger? logger = default)
    {
        var operationResult = result.ToOperationResult();

        return ToActionResult(controller, operationResult, logger);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="controller"></param>
    /// <param name="result"></param>
    /// <param name="data"></param>
    /// <param name="logger"></param>
    /// <returns></returns>
    public static ActionResult<OperationResult<T>> ToActionResult<T>(this ControllerBase controller,
        ResultOrError result, T? data = default, ILogger? logger = default)
    {
        var operationResult =
            result.Result
                ? OperationResultExtensions.Success(data)
                : OperationResultExtensions.Failed<T>(result.ToOperationProblemDetails());

        return ToActionResult(controller, operationResult, logger);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="controller"></param>
    /// <param name="result"></param>
    /// <param name="logger"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static ActionResult<OperationResult<T>> ToActionResult<T>(this ControllerBase controller,
        OperationResult<T> result, ILogger? logger = default)
    {
        if (result.Result) return SuccessActionResult(controller, result, logger);

        var returnData = FailedActionResult(result, logger);
        return returnData;
    }

    private static ActionResult<OperationResult<T>> SuccessActionResult<T>(ControllerBase controller,
        OperationResult<T> result, ILogger? logger = default)
    {
        return result.StatusCode switch
        {
            StatusCodes.Status201Created => controller.Created(string.Empty, result),
            StatusCodes.Status204NoContent => controller.NoContentResult<T>(logger),

            _ => controller.StatusCode(result.StatusCode, result)
        };
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="controller"></param>
    /// <param name="result"></param>
    /// <param name="data"></param>
    /// <param name="logger"></param>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TFailed"></typeparam>
    /// <returns></returns>
    public static ActionResult<OperationResult<T>> ToActionResult<T, TFailed>(this ControllerBase controller,
        OperationResult<TFailed> result, T? data = default, ILogger? logger = default)
    {
        if (result.Result)
        {
            if (data == null)
            {
                throw new ArgumentException(
                    $"Successfully result of type {typeof(OperationResult<TFailed>)} not be converted to {typeof(OperationResult<T>)} without data",
                    nameof(data));
            }

            return controller.StatusCode(result.StatusCode,
                OperationResultExtensions.Success(data, result.StatusCode));
        }

        var failedResult = OperationResultExtensions.Failed(result.Error, data);
        var returnData = FailedActionResult(failedResult);

        return returnData;
    }

    /// <summary>
    /// Forbidden result for controller
    /// </summary>
    /// <typeparam name="T">Data type</typeparam>
    /// <param name="controller">Controller</param>
    /// <param name="message">Error message</param>
    /// <param name="data">Error data (optional)</param>
    /// <param name="logger">Logger (optional)</param>
    /// <returns>Action result containing operation result</returns>
    public static ActionResult<OperationResult<T>> ForbiddenResult<T>(this ControllerBase controller,
        string? message = null, T? data = default, ILogger? logger = default)
    {
        return controller.ToActionResult(
            OperationResultExtensions.Failed(message ?? "Access denied", StatusCodes.Status403Forbidden, data),
            logger);
    }

    /// <summary>
    /// Not found result for controller
    /// </summary>
    /// <typeparam name="T">Data type</typeparam>
    /// <param name="controller">Controller</param>
    /// <param name="message">Error message</param>
    /// <param name="data">Error data (optional)</param>
    /// <param name="logger">Logger (optional)</param>
    /// <returns>Action result containing operation result</returns>
    public static ActionResult<OperationResult<T>> NotFoundResult<T>(this ControllerBase controller,
        string? message = null, T? data = default, ILogger? logger = default)
    {
        return controller.NotFound(OperationResultExtensions.Failed(message ?? "Not found", 
            StatusCodes.Status404NotFound, data));
    }

    /// <summary>
    /// Bad request result for controller
    /// </summary>
    /// <typeparam name="T">Data type</typeparam>
    /// <param name="controller">Controller</param>
    /// <param name="message">Error message</param>
    /// <param name="data">Error data (optional)</param>
    /// <param name="logger">Logger (optional)</param>
    /// <returns>Action result containing operation result</returns>
    public static ActionResult<OperationResult<T>> BadRequestResult<T>(this ControllerBase controller,
        string? message = null, T? data = default, ILogger? logger = default)
    {
        return controller.ToActionResult(
            OperationResultExtensions.Failed(message ?? "Bad request", StatusCodes.Status400BadRequest, data),
            logger);
    }

    /// <summary>
    /// Ok result for controller
    /// </summary>
    /// <typeparam name="T">Data type</typeparam>
    /// <param name="controller">Controller</param>
    /// <param name="data">Result data</param>
    /// <param name="logger">Logger (optional)</param>
    /// <returns>Action result containing operation result</returns>
    public static ActionResult<OperationResult<T>> OkResult<T>(this ControllerBase controller, T data,
        ILogger? logger = default)
    {
        return controller.Ok(OperationResultExtensions.Success(data, StatusCodes.Status200OK));
    }

    /// <summary>
    /// Created result for controller
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="controller"></param>
    /// <param name="data"></param>
    /// <param name="location"></param>
    /// <param name="logger"></param>
    /// <returns></returns>
    public static ActionResult<OperationResult<T>> CreatedResult<T>(this ControllerBase controller, string location,
        T data, ILogger? logger = default)
    {
        return controller.Created(location, OperationResultExtensions.Success(data, StatusCodes.Status201Created));
    }

    /// <summary>
    /// NoContent result for controller
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="controller"></param>
    /// <param name="logger"></param>
    /// <returns></returns>
    public static ActionResult<OperationResult<T>> NoContentResult<T>(this ControllerBase controller, ILogger? logger = default)
    {
        return controller.NoContent();
    }

    /// <summary>
    /// Create ActionResult for failed <see cref="OperationResult{T}"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="result"></param>
    /// <param name="logger"></param>
    /// <returns></returns>
    public static ActionResult<OperationResult<T>> FailedActionResult<T>(OperationResult<T> result, ILogger? logger = default)
    {
        if (result.Result)
            throw new Exception(
                $"{nameof(FailedActionResult)} may be execute only for failed {nameof(OperationResult)}");

        logger?.LogError(result.Error);

        var data = new ObjectResult(result)
        {
            StatusCode = result.Error?.Status ?? result.StatusCode,
            ContentTypes = { ContentTypes.ProblemJson },
            DeclaredType = typeof(OperationResult<T>)
        };

        return data;
    }

    /// <summary>
    /// Create ActionResult for failed <see cref="OperationResult"/>
    /// </summary>
    /// <param name="result"></param>
    /// <param name="logger"></param>
    /// <returns></returns>
    public static ActionResult<OperationResult> FailedActionResult(OperationResult result, ILogger? logger = default)
    {
        if (result.Result)
            throw new Exception(
                $"{nameof(FailedActionResult)} may be execute only for failed {nameof(OperationResult)}");

        logger?.LogError(result.Error);

        var data = new ObjectResult(result)
        {
            StatusCode = result.Error?.Status ?? result.StatusCode,
            ContentTypes = { ContentTypes.ProblemJson },
            DeclaredType = typeof(OperationResult)
        };

        return data;
    }

    /// <summary>
    /// <see cref="ResultOrError"/> to ActionResult
    /// </summary>
    /// <param name="controller"></param>
    /// <param name="result"></param>
    /// <param name="logger"></param>
    /// <returns></returns>
    public static ActionResult<OperationResult> ToActionResult(this ControllerBase controller, ResultOrError result,
        ILogger? logger = default)
    {
        var operationResult = result.ToOperationResult();

        return ToActionResult(controller, operationResult, logger);
    }


    /// <summary>
    /// <see cref="OperationResult"/> to ActionResult
    /// </summary>
    /// <param name="controller"></param>
    /// <param name="result"></param>
    /// <param name="logger"></param>
    /// <returns></returns>
    public static ActionResult<OperationResult> ToActionResult(this ControllerBase controller,
        OperationResult result, ILogger? logger = default)
    {
        if (result.Result) return SuccessActionResult(controller, result, logger);

        var returnData = FailedActionResult(result, logger);
        return returnData;
    }

    /// <summary>
    /// Create ActionResult for success <see cref="OperationResult"/>
    /// </summary>
    /// <param name="controller"></param>
    /// <param name="result"></param>
    /// <param name="logger"></param>
    /// <returns></returns>
    public static ActionResult<OperationResult> SuccessActionResult(ControllerBase controller,
        OperationResult result, ILogger? logger = default)
    {
        return result.StatusCode switch
        {
            StatusCodes.Status201Created => controller.Created(string.Empty, result),
            StatusCodes.Status204NoContent => controller.NoContent(),

            _ => controller.StatusCode(result.StatusCode, result)
        };
    }
}