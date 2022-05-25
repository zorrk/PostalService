using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostalServiceClassLibrary.Models.QueriesResults;

public class ReportDistrictResult
{
	public int DisctrictNumbeer { get; set; }
	public string District { get; set; }
	public int? Amount { get; set; }
}