using Microsoft.AspNetCore.Mvc;

namespace OlMag.Manufacture2.Helpers.OperationResult;

internal static class ConstantsOperationResult
{
    public const string ProblemUriHost = "https://**************/errors/types";


    public const string GeneralErrorTitle = "Errors occurred during execution";
    public const string GeneralErrorType = "general";

    private static string[] errorTypes =
    [
        ErrorProcessing.GetProblemDetailsType(GeneralErrorType)
    ];
    public static bool CheckErrorType(this ProblemDetails source)
    {
        return errorTypes
            .Contains(source.Type, StringComparer.OrdinalIgnoreCase);
    }
}