using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;

namespace PostalServiceApp.Infrastructure;

public static class Constants
{
	public static readonly List<string> DurationsList = new() { "1", "3", "6", "9", "12" };

	public static string AppTitle = "Почтовая система";
}