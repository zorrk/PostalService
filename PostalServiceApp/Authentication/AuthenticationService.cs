using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PostalServiceApp.Authentication;

// Сервис аутентификации личности ползователя
public class AuthenticationService : IAuthenticationService, IAuthNotify
{
	// Событие при прохождении аутентификации для подписки извне сервиса
	public event EventHandler OnAuthenticate;

	// Свойство для проверки текущей аутентификации
	public bool IsAuthenticated => Thread.CurrentPrincipal.Identity.IsAuthenticated;

	// Метод работы авторизации
	public void LogIn(string username, string password)
	{
		// Получение учетных данных в методе аутентификации
		User user = AuthenticateUser(username, password);

		//Получить текущий субъект потока
		if (Thread.CurrentPrincipal is not CustomPrincipal customPrincipal)
			throw new ArgumentException("Субъект потока по умолчанию должен быть установлен в качетве CustomPrincipal при старте приложения");

		// Авторизация с помощью полученых валидных данных
		customPrincipal.Identity = new CustomIdentity(user.Username, user.Email, user.Roles);

		// Оповестить подписчиков на событие авторизации
		OnAuthenticate?.Invoke(this, EventArgs.Empty);
	}


	// Процесс аутентификации личности пользователя по передаваемым имени и паролю
	public User AuthenticateUser(string username, string clearTextPassword)
	{
		// Создание объекта данных личности на основе наличия записи с переданными данными
		// (здесь происходит вычисление хэша пароля и поиск соответствия,
		//  в качестве соли для хэша в данном случае используется имя пользователя)
		InternalUserData userData =
			_users.FirstOrDefault(u =>
				u.Username.Equals(username) && u.HashedPassword.Equals(CalculateHash(clearTextPassword, u.Username)));

		// Исключение при отсутствии соответствий
		if (userData == null)
			throw new UnauthorizedAccessException("Access denied. Please provide some valid credentials.");

		// Возврат найденных данных личности
		return new User(userData.Username, userData.Email, userData.Roles);
	}

	// Метод вычисления хэша
	private static string CalculateHash(string clearTextPassword, string salt)
	{
		// Конвертация пароля с солью в массив байтов
		byte[] saltedHashBytes = Encoding.UTF8.GetBytes(clearTextPassword + salt);
		
		// Использование алгоритма SHA256 для вычисления хэша
		HashAlgorithm algorithm = new SHA256Managed();
		byte[] hash = algorithm.ComputeHash(saltedHashBytes);

		// Возврат хэша в формате строки Base64
		return Convert.ToBase64String(hash);
	}


	// Выйти из системы, установить текущую личность анонимной 
	public void LogOut()
	{
		if (Thread.CurrentPrincipal is CustomPrincipal customPrincipal)
		{
			customPrincipal.Identity = new AnonymousIdentity();
			OnAuthenticate?.Invoke(this, EventArgs.Empty);
		}
	}

	// Инкапсулированная модель для обработки данных при авторизации
	private class InternalUserData
	{
		public InternalUserData(string username, string email, string hashedPassword, string[] roles)
		{
			Username = username;
			Email = email;
			HashedPassword = hashedPassword;
			Roles = roles;
		}

		public string Username { get; private set; }
		public string Email { get; private set; }
		public string HashedPassword { get; private set; }
		public string[] Roles { get; private set; }
	}


	// Хардкод хранимых учетных данных
	// TODO: перенести в таблицу БД
	private static readonly List<InternalUserData> _users = new()
	{
		new InternalUserData("manager", "manager@company.com",
			"V2V85pEXHWKapDNhwEGut5My0wjjvb5WDzszJYuQDBY=", new[] { "Managers" }),
		new InternalUserData("operator", "operator@company.com",
			"ReYFdDUg5W73g7HajNRsX/RS91fFDo1xlDpzG7dNKLI=", new[] { "Operators" })
	};

}