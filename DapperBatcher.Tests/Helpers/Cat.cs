namespace EdShel.DapperBatcher.Tests.Helpers;

class Cat
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string CoatColor { get; set; } = null!;

    public override string ToString()
    {
        return $"|{Id}: {Name} ({CoatColor})|";
    }
}
