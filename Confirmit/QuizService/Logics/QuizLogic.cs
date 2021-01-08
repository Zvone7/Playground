using QuizService.Infrastructure;
using QuizService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using static QuizService.Model.QuizResponseModel;

namespace QuizService.BLL
{
    public class QuizLogic : IQuizLogic
    {
        private readonly IQuizRepository _quizRepository;
        private readonly IQuestionRepository _questionRepository;
        private readonly IAnswerRepository _answerRepository;

        public QuizLogic(
            IQuizRepository quizRepository,
            IQuestionRepository questionRepository,
            IAnswerRepository answerRepository)
        {
            _quizRepository = quizRepository;
            _questionRepository = questionRepository;
            _answerRepository = answerRepository;
        }

        public IEnumerable<QuizResponseModel> Get()
        {
            try
            {
                return _quizRepository
                    .Get()
                    .Where(x => x.Id > 0)
                    .Select(quiz =>
                    new QuizResponseModel
                    {
                        Id = quiz.Id,
                        Title = quiz.Title
                    });
            }
            catch (Exception e)
            {
                // TODO logging
                return null;
            }
        }
        public QuizResponseModel Get(Int32 id)
        {
            try
            {
                var quizInDb = _quizRepository.Get(id);
                if (quizInDb == null) return null;
                var questions = _questionRepository.Get();
                var answers = _answerRepository.Get();
                var quiz = new QuizResponseModel
                {
                    Id = quizInDb.Id,
                    Title = quizInDb.Title,
                    Questions = questions
                    .Where(x => x.QuizId == id)
                    .Select(ques => new QuestionItem
                    {
                        Id = ques.Id,
                        Text = ques.Text,
                        CorrectAnswerId = ques.CorrectAnswerId,
                        Answers = answers
                        .Where(x => x.QuestionId == ques.Id)
                        .Select(ans => new AnswerItem
                        {
                            Id = ans.Id,
                            Text = ans.Text
                        })
                    }),
                    Links = new Dictionary<string, string>
                    {
                        {"self", $"/api/quizzes/{id}"},
                        {"questions", $"/api/quizzes/{id}/questions"}
                    }
                };

                return quiz;
            }
            catch (Exception e)
            {
                // TODO logging
                return null;
            }
        }

        public bool TestResponse(int id, int qid, AnswerTest answerTest)
        {
            try
            {
                var quiz = Get(id);
                if (quiz == null) return false;
                var question = quiz.Questions.FirstOrDefault(x => x.Id == qid);
                if (question == null) return false;
                if (question.Answers.FirstOrDefault(
                    ans => ans.Text.Equals(answerTest.Text, StringComparison.InvariantCultureIgnoreCase) &&
                    ans.Id == question.CorrectAnswerId) != null) return true;
                return false;
            }
            catch (Exception e)
            {
                // TODO logging
                return false;
            }
        }
    }
}
