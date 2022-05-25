using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Interop;
using PostalServiceApp.Models;
using PostalServiceClassLibrary.Models;
using DataFormats = System.Windows.DataFormats;

namespace PostalServiceApp.Infrastructure;

public static class Extensions
{
	public static bool IsNullOrEmpty(this string str) =>
		string.IsNullOrEmpty(str);

	public static string ShortName(this Person person) =>
		$"{person.Surname} {person.Name.First()}.{person.Patronymic.First()}.";

	public static FlowDocument Clone(this FlowDocument document)
	{
		FlowDocument copy = new();

		TextRange range = new(document.ContentStart, document.ContentEnd);
			
		using MemoryStream stream = new();
			
		System.Windows.Markup.XamlWriter.Save(range, stream);
			
		range.Save(stream, DataFormats.XamlPackage);
			
		TextRange range2 = new(copy.ContentEnd, copy.ContentEnd);
			
		range2.Load(stream, DataFormats.XamlPackage);

		return copy;
	}

}