using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ToDoWebApiPaulCarter.Models;
using static Azure.Core.HttpHeader;

namespace ToDoWebApiPaulCarter.Data
{

    public class ToDoContext : DbContext
    {
        public ToDoContext (DbContextOptions<ToDoContext> options)
            : base(options)
        {
        }

        public ToDoContext() { }

        public virtual DbSet<Note> Notes { get; set; }

    }
}
