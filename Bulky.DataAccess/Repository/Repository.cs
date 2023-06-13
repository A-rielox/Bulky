using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IRepository;
using System.Linq.Expressions;

namespace Bulky.DataAccess.Repository;

public class Repository<T> : IRepository<T> where T : class
{
	private readonly ApplicationDbContext _db;

	public Repository(ApplicationDbContext db)
	{
		_db = db;
	}




	//////////////////////////////////////////
	//////////////////////////////////////////
	public void Add(T entity)
	{
		_db.Set<T>().Add(entity);
	}



	//////////////////////////////////////////
	//////////////////////////////////////////
	public T Get(Expression<Func<T, bool>> filter)
	{
		IQueryable<T> query = _db.Set<T>();
		query = query.Where(filter);

		return query.FirstOrDefault();
	}



	//////////////////////////////////////////
	//////////////////////////////////////////
	public IEnumerable<T> GetAll()
	{
		IQueryable<T> query = _db.Set<T>();

		return query.ToList();
	}



	//////////////////////////////////////////
	//////////////////////////////////////////
	public void Remove(T entity)
	{
		_db.Set<T>().Remove(entity);
	}


	//////////////////////////////////////////
	//////////////////////////////////////////
	public void RemoveRange(IEnumerable<T> entities)
	{
		_db.Set<T>().RemoveRange(entities);
	}
}
