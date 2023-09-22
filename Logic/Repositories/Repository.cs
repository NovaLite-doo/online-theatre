namespace Logic.Repositories;

public abstract class Repository<T> where T : class
{
    protected readonly OnlineTheatreDbContext Context;

    protected Repository(OnlineTheatreDbContext context)
    {
        Context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public IReadOnlyList<T> GetList() => Context.Set<T>().ToList();

    public T? GetById(long id) => Context.Set<T>().Find(id);

    public void Add(T entity)
    {
        Context.Set<T>().Add(entity);
    }

    public void SaveChanges() => Context.SaveChanges();
}