using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using PostalServiceClassLibrary.Context;

namespace PostalServiceClassLibrary.DataAccess;

// Класс - универсальный репозиторий для работы с таблицами базы данных
public class GenericRepository<TEntity> where TEntity : class
{
	internal PostalDbContext Context;
	internal DbSet<TEntity> DbSet;

	public GenericRepository() : this(new PostalDbContext())
	{}

	public GenericRepository(PostalDbContext context)
	{
		Context = context;
		DbSet = context.Set<TEntity>();
	}

	// Получение выборки данных из таблицы с возможностью фильтрации, сортировки и включения в загрузку вложенных свойств
	public virtual IEnumerable<TEntity> Get(
		Expression<Func<TEntity, bool>> filter = null,
		Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
		string includeProperties = "")
	{
		IQueryable<TEntity> query = DbSet;

		if (filter != null)
			query = query.Where(filter);

		query = includeProperties
			.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries)
			.Aggregate(query, (current, s) => current.Include(s));

		return orderBy != null ? orderBy(query).ToList() : query.ToList();
	}

	// Получить записть по id
	public virtual TEntity GetById(object id) => DbSet.Find(id);

	// Вставка новой записи
	public virtual TEntity Insert(TEntity entity) => DbSet.Add(entity);

	// Удаление записи по id
	public virtual void Delete(object id)
	{
		TEntity entityToDelete = DbSet.Find(id);
		Delete(entityToDelete);
	}

	// Удаление записи
	public virtual void Delete(TEntity entityToDelete)
	{
		if (Context.Entry(entityToDelete).State == EntityState.Detached) 
			DbSet.Attach(entityToDelete);
            
		DbSet.Remove(entityToDelete);
	}

	// Изменение записи
	public virtual void Update(TEntity entityToUpdate)
	{
		DbSet.Attach(entityToUpdate);
		Context.Entry(entityToUpdate).State = EntityState.Modified;
	}

	// Сохранение изменений БД
	public void Save() => Context.SaveChanges();
}