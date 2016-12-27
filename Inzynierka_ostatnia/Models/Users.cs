using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Inzynierka_ostatnia.Models
{
    public class User
    {
        public int id { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public int type { get; set; }
        public DateTime created_at { get; set; }
    }

    public class UserDBCtxt:KaczkaDBCtxt
    {
        public DbSet<User> Users { get; set; }
    }
}