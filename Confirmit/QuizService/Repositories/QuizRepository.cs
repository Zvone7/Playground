using QuizService.Infrastructure;
using QuizService.Model.Domain;
using System;
using System.Linq;

namespace QuizService.Db
{
    public class QuizRepository : IQuizRepository
    {
        private readonly LocalDbContext _context;

        public QuizRepository(LocalDbContext context)
        {
            _context = context;
        }

        public IQueryable<Quiz> Get()
        {
            return _context.Quizzes.Select(x => x);
        }

        public Quiz Get(Int32 id)
        {
            return _context.Quizzes.FirstOrDefault(y => y.Id == id);
        }
    }
}
