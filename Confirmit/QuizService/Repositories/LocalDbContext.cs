using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QuizService.Model.Domain;
using System.Data.Common;

namespace QuizService.Db
{
    public partial class LocalDbContext : DbContext
    {
        private readonly DbConnection _connection;

        public LocalDbContext(DbConnection connection)
        {
            _connection = connection;
        }
        public DbSet<Quiz> Quizzes { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Answer> Answers { get; set; }

        public static readonly ILoggerFactory MyLoggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddConsole();
        });

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options
                .UseSqlite(_connection)
                .UseLoggerFactory(MyLoggerFactory);
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Quiz>(entity =>
            {
                entity.ToTable("Quiz");
            });
            modelBuilder.Entity<Question>(entity =>
            {
                entity.ToTable("Question");
            });

            modelBuilder.Entity<Answer>(entity=>
            {
                entity.ToTable("Answer");
            });
        }
    }
}
