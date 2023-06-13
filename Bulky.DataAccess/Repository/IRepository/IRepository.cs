using System.Linq.Expressions;

namespace Bulky.DataAccess.Repository.IRepository;

public interface IRepository<T> where T : class
{
	IEnumerable<T> GetAll();

	T Get(Expression<Func<T, bool>> filter);

	void Add(T entity);
	// void Update(T entity); va en el especifico, junto con save
	void Remove(T entity);
	void RemoveRange(IEnumerable<T> entities);
}

