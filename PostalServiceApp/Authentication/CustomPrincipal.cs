using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace PostalServiceApp.Authentication;

// Модель пользовательского субьекта(principal) 
public class CustomPrincipal : IPrincipal
{
	private CustomIdentity _identity;

	public CustomIdentity Identity
	{
		get => _identity ?? new AnonymousIdentity();
		set => _identity = value;
	}

	#region IPrincipal Members
	IIdentity IPrincipal.Identity => Identity;

	public bool IsInRole(string role) => 
		Identity.Roles.Contains(role);

	#endregion
}