using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostalServiceApp.Authentication;

// Интерфейс сервиса аутентификации
public interface IAuthenticationService
{
	User AuthenticateUser(string username, string password);
	void LogIn(string username, string password);

	event EventHandler OnAuthenticate;
}