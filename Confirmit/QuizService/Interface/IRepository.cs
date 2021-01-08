using QuizService.Model.Domain;
using System;
using System.Linq;

namespace QuizService.Infrastructure
{
    public interface IRepository<T>
    {
        IQueryable<T> Get();
        T Get(Int32 id);
    }
    public interface IQuizRepository : IRepository<Quiz> { }
    public interface IQuestionRepository : IRepository<Question> { }
    public interface IAnswerRepository : IRepository<Answer> { }
}
