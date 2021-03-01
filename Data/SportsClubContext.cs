using Microsoft.EntityFrameworkCore;
using MvcSportsClub.Models;

namespace MvcSportsClub.Data {
    public class SportsClubContext:DbContext {
        public SportsClubContext(DbContextOptions<SportsClubContext> options)
            : base(options){
        }

        // This code creates a DbSet property for each entity set.
        // In Entity Framework terminology;
        //   - an entity set typically corresponds to a database table, 
        //   - and an entity corresponds to a row in the table.
        public DbSet<Workout> Workouts { get; set; }
        public DbSet<Member> Members { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }

        /*        Property names for collections are typically plural(Students rather than Student),
         *        but developers disagree about whether table names should be pluralized or not.
         *        Here we override the default behavior by specifying singular table names in the DbContext. 
         *        To do that, add the following highlighted code after the last DbSet property.
        */
        // OnModelCreating 'stuurt' de creatie van migrations
        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.Entity<Member>().ToTable("Member");
            modelBuilder.Entity<Workout>().ToTable("Workout");
            modelBuilder.Entity<Enrollment>().ToTable("Enrollment");

            modelBuilder.Entity<Member>().HasData(FakeData.FakeMembers);
            modelBuilder.Entity<Workout>().HasData(FakeData.FakeWorkouts);
            modelBuilder.Entity<Enrollment>().HasData(FakeData.FakeEnrollments);
        }
    }
}
