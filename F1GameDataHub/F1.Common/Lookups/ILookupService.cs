namespace F1.Common.Lookups
{
    public interface ILookupService<T> where T : LookupItem
    {
        IReadOnlyList<T> GetAll();
        T? GetById(int id);
    }
}
