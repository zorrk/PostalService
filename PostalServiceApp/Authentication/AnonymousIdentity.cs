using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostalServiceApp.Authentication;

// Анонимная личность - пустые данные
public class AnonymousIdentity : CustomIdentity
{
	public AnonymousIdentity()
		: base(string.Empty, string.Empty, new string[] { })
	{ }
}