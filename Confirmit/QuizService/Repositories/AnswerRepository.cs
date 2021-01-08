using QuizService.Infrastructure;
using QuizService.Model.Domain;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace QuizService.Db
{
    public class AnswerRepository : IAnswerRepository
    {
        private readonly LocalDbContext _context;

        public AnswerRepository(LocalDbContext context)
        {
            _context = context;
        }
        public IQueryable<Answer> Get()
        {
            return _context.Answers.Select(x => x);
        }
        public Answer Get(Int32 id)
        {
            return _context.Answers.FirstOrDefault(y => y.Id == id);
        }
    }
}
