using OlMag.Manufacture2.Helpers.OperationResult;
using FluentAssertions;

namespace OlMag.Manufacture2.Tests.Maintenance;

public static class OperationResultTestExtensions
{
    public static OperationResultAssertions<T, TValue> Should<T, TValue>(this T? instance)
        where T : OperationResult<TValue>
    {
        instance.Should().NotBeNull();

        return new OperationResultAssertions<T, TValue>(instance!);
    }
}