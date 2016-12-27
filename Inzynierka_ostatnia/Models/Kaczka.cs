using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace Inzynierka_ostatnia.Models
{
    public class Kaczka
    {
        public int id { get; set; }
        public int id_matki { get; set; }
        public int id_ojca { get; set; }
        public DateTime wyklucie { get; set; }
        public bool zyje { get; set; }
        public bool zmiana { get; set; }
    }

    public class KaczkaDBCtxt:DbContext
    {
        public DbSet<Kaczka> Kaczki { get; set; }  
        public DbSet<User> Users { get; set; }    
        public DbSet<User_types> User_typess { get; set; }  
    }

    
}