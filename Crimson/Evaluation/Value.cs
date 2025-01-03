using Crimson.Parsing;

namespace Crimson.Evaluation;

internal enum ValueType
{
    NumType,
    NullType,
    BoolType,
}

internal abstract class Value
{
    internal virtual ValueType ValueKind { get; }
}

internal class NumValue : Value
{
    internal readonly double Value;

    internal NumValue(double value)
    {
        Value = value;
    }

    internal override ValueType ValueKind
    { get { return ValueType.NumType; } }

    public override string ToString()
    {
        return String.Format("(NumValue\tValue: {0})", Value);
    }
}

internal class BoolValue : Value
{
    internal readonly bool Value;

    internal BoolValue(bool value)
    {
        Value = value;
    }

    internal override ValueType ValueKind
    { get { return ValueType.BoolType; } }

    public override string ToString()
    {
        return String.Format("(BoolValue\tValue: {0})", Value);
    }
}

internal class NilValue : Value
{
    internal readonly string Value = "nil";

    internal override ValueType ValueKind
    { get { return ValueType.BoolType; } }

    public override string ToString()
    {
        return String.Format("(Nil\tValue: {0})", Value);
    }
}
