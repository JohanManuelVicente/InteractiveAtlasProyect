using Microsoft.EntityFrameworkCore;
using InteractiveAtlas.Domain.Entities;

namespace InteractiveAtlas.Infrastucture
{
    public class InteractiveAtlasDbContext : DbContext
    {
        public InteractiveAtlasDbContext(DbContextOptions<InteractiveAtlasDbContext> options) : base(options) { }

        public DbSet<Province> Provinces { get; set; }

        public DbSet<TypicalProduct> TypicalProducts { get; set; }

        public DbSet<TouristAttraction> TouristAttractions { get; set; }

        public DbSet<QuizQuestion> QuizQuestions { get; set; }

        public DbSet<QuizAnswer> QuizAnswers { get; set; }
    }
}
