using QuizService.Infrastructure;
using QuizService.Model.Domain;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace QuizService.Db
{
    public class QuestionRepository : IQuestionRepository
    {
        private readonly LocalDbContext _context;

        public QuestionRepository(LocalDbContext context)
        {
            _context = context;
        }
        public IQueryable<Question> Get()
        {
            return _context.Questions.Select(x => x);
        }
        public Question Get(Int32 id)
        {
            return _context.Questions.FirstOrDefault(y => y.Id == id);
        }
    }
}
