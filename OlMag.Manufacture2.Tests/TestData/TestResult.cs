using System.Net;

namespace OlMag.Manufacture2.Tests.TestData;

public class TestResult
{
    public bool Value { get; set; }
    public HttpStatusCode? Code { get; set; }

    public static readonly TestResult Successful = new() { Value = true };
    public static readonly TestResult Failed = new() { Value = false };

    public static TestResult FailedWithCode(HttpStatusCode code)
    {
        return new TestResult { Value = false, Code = code };
    }
}