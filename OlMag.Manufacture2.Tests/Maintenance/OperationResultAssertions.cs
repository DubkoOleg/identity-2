using FluentAssertions.Execution;
using FluentAssertions.Primitives;
using FluentAssertions;
using OlMag.Manufacture2.Helpers.OperationResult;

namespace OlMag.Manufacture2.Tests.Maintenance;

public class OperationResultAssertions<T, TValue> : ReferenceTypeAssertions<OperationResult<TValue>, OperationResultAssertions<T, TValue>> where T : OperationResult<TValue>
{

    public OperationResultAssertions(OperationResult<TValue> instance) : base(instance)
    { }

    public AndConstraint<OperationResultAssertions<T, TValue>> BeTrue(string because = "", params object[] becauseArgs)
    {
        return Be(true);
    }

    public AndConstraint<OperationResultAssertions<T, TValue>> BeFalse(string because = "", params object[] becauseArgs)
    {
        return Be(false);
    }

    public AndConstraint<OperationResultAssertions<T, TValue>> Be(bool expected, string because = "", params object[] becauseArgs)
    {
        Execute.Assertion
            .BecauseOf(because, becauseArgs)
            .ForCondition(Subject.Result == expected)
            .FailWith("Expected {0} but actual result {1}. {reason}", expected, Subject.Result)
            .Then
            .ForCondition(!expected || Subject.Data != null)
            .FailWith("Successfully result must be contain data, but actual is null. {reason}")
            .Then
            .ForCondition(!expected || Subject.Error == null)
            .FailWith("Successfully result don't contain error value. Expected null, but actual is {0}", Subject.Error)
            .Then
            .ForCondition(expected || Subject.Error != null)
            .FailWith("Failed result must contain error value. Expected not null, but actual is null")
            ;

        return new AndConstraint<OperationResultAssertions<T, TValue>>(this);
    }

    protected override string Identifier => nameof(OperationResult<TValue>);
}