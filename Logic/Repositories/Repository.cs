namespace Logic.Repositories;

public abstract class Repository<T> where T : class
{
    protected readonly OnlineTheatreDbContext _context;

    protected Repository(OnlineTheatreDbContext context)
    {
        _context = context;
    }

    public IReadOnlyList<T> GetList() => _context.Set<T>().ToList();

    public T? GetById(long id) => _context.Set<T>().Find(id);

    public void Add(T entity)
    {
        _context.Set<T>().Add(entity);
    }

    public void SaveChanges() => _context.SaveChanges();
}