using System;
using System.Collections.Generic;

namespace PostalServiceClassLibrary.DataAccess;

// Интерфейс класса репозитория
interface IRepository<T> : IDisposable
	where T : class
{
	// Получить список всех объектов
	IEnumerable<T> GetList();
	// Получение одного объекта по id
	T GetById(int id);
	// Cоздание объекта
	void Insert(T item);
	// Обновление объекта
	void Update(T item);
	// Удаление объекта по id
	void Delete(int id);
	// Сохранение изменений
	void Save();  
}