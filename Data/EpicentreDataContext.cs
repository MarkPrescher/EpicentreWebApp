using Epicentre.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Epicentre.Data
{
    public class EpicentreDataContext : DbContext
    {
        public EpicentreDataContext(DbContextOptions<EpicentreDataContext> options)
            : base(options)
        {
        }

        public DbSet<CovidTest> CovidTest { get; set; }
        public DbSet<CovidVaccination> CovidVaccination { get; set; }
    }
}