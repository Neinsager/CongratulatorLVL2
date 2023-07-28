using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;

namespace CongratulatorLVL2
{
    internal class ApplicationContext : DbContext
    {
        public DbSet<Person> Persons { get; set; }

        public ApplicationContext()
        {
            Database.EnsureCreated();
        }

        public class DateOnlyConverter : ValueConverter<DateOnly, DateTime>
        {
            public DateOnlyConverter()
                : base(dateOnly =>
                        dateOnly.ToDateTime(TimeOnly.MinValue),
                        dateTime => DateOnly.FromDateTime(dateTime)) { }
        }

        protected override void ConfigureConventions(ModelConfigurationBuilder builder)
        {
            builder.Properties<DateOnly>()
                .HaveConversion<DateOnlyConverter>()
                .HaveColumnType("date");
            base.ConfigureConventions(builder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=helloappdb;Trusted_Connection=True;");
        }
    }
}
