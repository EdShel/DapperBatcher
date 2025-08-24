namespace EdShel.DapperBatcher;

internal enum BatchedCommandKind
{
    None,
    Single,
    SingleOrDefault,
    First,
    FirstOrDefault,
}
