using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostalServiceClassLibrary.Models.QueriesResults;

public class ReportPublicationResult
{
	public string PubIndex { get; set; }
	public string PubType { get; set; }
	public string Title { get; set; }
	public int Duration_Average { get; set; }
	public int? Amount { get; set; }
}