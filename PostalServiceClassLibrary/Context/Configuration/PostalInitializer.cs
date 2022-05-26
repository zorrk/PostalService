using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostalServiceClassLibrary.Context.Configuration
{
	internal class PostalInitializer : IDatabaseInitializer<PostalDbContext>
	{
		// Инициализация базы данных
		public void InitializeDatabase(PostalDbContext db)
		{
			// Создание базы при отсутствии
			if (!db.Database.Exists())
			{

				AppDomain.CurrentDomain.SetData("DataDirectory", Directory.GetCurrentDirectory());

				var dataDir = Directory.GetCurrentDirectory() + "\\App_Data";

				if (!System.IO.Directory.Exists(dataDir))
					System.IO.Directory.CreateDirectory(dataDir);

				db.Database.Create();

				new SeedData(db).Fill();

				db.SaveChanges();
			}
		}
	}
}
