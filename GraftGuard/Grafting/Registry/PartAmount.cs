using System.Diagnostics.CodeAnalysis;

namespace GraftGuard.Grafting.Registry;

/// <summary>
/// Small Struct which allows easy storage of amounts of parts
/// </summary>
internal readonly struct PartAmount
{
    public readonly PartDefinition Part;
    public readonly int Amount;
    private readonly int _hashCode;
    public PartAmount(PartDefinition part, int amount)
    {
        Part = part;
        Amount = amount;
        // Save the part's hash code for quicker dictionary lookup
        _hashCode = part.Name.GetHashCode();
    }

    public override int GetHashCode() => _hashCode;

    public override bool Equals([NotNullWhen(true)] object obj)
    {
        return obj is PartAmount resource && (resource.Part.Name == Part.Name);
    }
}