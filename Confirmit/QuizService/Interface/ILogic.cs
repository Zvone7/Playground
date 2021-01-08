using QuizService.Model;
using QuizService.Model.Domain;
using System;
using System.Collections.Generic;

namespace QuizService.Infrastructure
{
    /// TODO - in order to get the most out of this pattern, QuizResponseModel and QuizCreateModel 
    ///         should be linked through inheritance and an Automapper
    ///       

    public interface ILogic<T> { }
    public interface IQuizLogic : ILogic<Quiz>
    {
        IEnumerable<QuizResponseModel> Get();
        QuizResponseModel Get(Int32 id);
        Boolean TestResponse(Int32 id, Int32 qid, AnswerTest answerTest);
    }
}
