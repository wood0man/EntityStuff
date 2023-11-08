using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Entity.Models;

namespace Entity.Data
{
    public class EntityContext : DbContext
    {
        public EntityContext (DbContextOptions<EntityContext> options)
            : base(options)
        {
        }

        public DbSet<Entity.Models.items> items { get; set; } = default!;

        public DbSet<Entity.Models.book>? book { get; set; }
        public DbSet<Entity.Models.mycata>categories { get; set; } = default!;
        public DbSet<Entity.Models.orders>? orders { get; set; }

    }
    

}
