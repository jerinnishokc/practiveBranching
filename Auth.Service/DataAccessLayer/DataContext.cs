using Auth.Service.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Auth.Service.DataAccessLayer
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base (options)
        {
                            
        }

        public virtual DbSet<Sample> Sample { get; set; }

        public virtual DbSet<User> User { get; set; }
    }
}
