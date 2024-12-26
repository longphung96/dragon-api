using System.Diagnostics.CodeAnalysis;

namespace DragonAPI.Extensions;


public class GenericEnum : IEquatable<GenericEnum>
{
    public GenericEnum(long Id, string Code, string Name, string? Value = null)
    {
        this.Id = Id;
        this.Code = Code;
        this.Name = Name;
        this.Value = Value;
    }

    public long Id { get; }
    public string Code { get; }
    public string Name { get; }
    public string? Value { get; }

    public bool Equals([AllowNull] GenericEnum other)
    {
        if (other is null) return false;
        return this.Id == other.Id && this.Code == other.Code && this.Name == other.Name && this.Value == other.Value;
    }
}