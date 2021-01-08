using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Moq;
using Newtonsoft.Json;
using QuizService.Model;
using QuizService.Model.Domain;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Xunit;

namespace QuizService.Tests
{
    public class QuizzesControllerTest
    {
        const string QuizApiEndPoint = "/api/quizzes/";
        private const string Q1_ANSWER = "A woodchuck would chuck as much wood as a woodchuck could chuck if a woodchuck could chuck wood.";
        private const string Q2_ANSWER = "Witches don't care about time.";

        [Fact]
        public async Task ChallengeTask4()
        {

            using (var testHost = new TestServer(new WebHostBuilder()
                .UseStartup<Startup>()))
            {
                var client = testHost.CreateClient();

                var quizCreate = new QuizCreateModel() { Title = "Toungetwister quiz" };
                var quizUri = await PostAsync(quizCreate, testHost, client, $"{QuizApiEndPoint}");
                Assert.True(Int32.TryParse(quizUri.Split('/').LastOrDefault(), out Int32 quizid));

                // question 1 setup
                var q1 = new QuestionCreateModel { Text = "How much wood would a woodchuck chuck if a woodchuck could chuck wood?" };
                var q1uri = await PostAsync(q1, testHost, client, $"{quizUri}/questions/");
                Assert.True(Int32.TryParse(q1uri.Split('/').LastOrDefault(), out Int32 q1id));
                // question 1 answers setup
                var q1a1 = new AnswerCreateModel() { Text = Q1_ANSWER };
                var q1a1uri = await PostAsync(q1a1, testHost, client, $"{q1uri}/answers/");
                Assert.True(Int32.TryParse(q1a1uri.Split('/').LastOrDefault(), out Int32 q1a1id));
                var q1a2 = new AnswerCreateModel() { Text = "Like, a lot." };
                var q1a2uri = await PostAsync(q1a2, testHost, client, $"{q1uri}/answers/");
                Assert.True(Int32.TryParse(q1a2uri.Split('/').LastOrDefault(), out Int32 q1a2id));
                // question 1 correct answer setup
                var q1updated = new QuestionUpdateModel() { CorrectAnswerId = q1a1id, Text = q1.Text };
                var q1updateResult = await PutAsync(q1updated, testHost, client, $"{QuizApiEndPoint}{quizid}/questions/{q1id}/");
                Assert.True(q1updateResult);

                // question 2 setup
                var q2 = new Question { Text = "If two witches would watch two watches, which witch would watch which watch?" };
                var q2uri = await PostAsync(q2, testHost, client, $"{quizUri}/questions");
                Assert.True(Int32.TryParse(q2uri.Split('/').LastOrDefault(), out Int32 q2id));
                // question 2 answers setup
                var q2a1 = new AnswerCreateModel() { Text = "Whaat?" };
                var q2a1uri = await PostAsync(q2a1, testHost, client, $"{q2uri}/answers/");
                Assert.True(Int32.TryParse(q2a1uri.Split('/').LastOrDefault(), out Int32 q2a1id));
                var q2a2 = new AnswerCreateModel() { Text = Q2_ANSWER };
                var q2a2uri = await PostAsync(q2a2, testHost, client, $"{q2uri}/answers/");
                Assert.True(Int32.TryParse(q2a2uri.Split('/').LastOrDefault(), out Int32 q2a2id));
                // question 2 correct answer setup
                var q2updated = new QuestionUpdateModel() { CorrectAnswerId = q2a2id, Text = q1.Text };
                var q2updateResult = await PutAsync(q2updated, testHost, client, $"{QuizApiEndPoint}{quizid}/questions/{q2id}/");
                Assert.True(q2updateResult);

                // get the quiz
                var quiz = await GetAsync<QuizResponseModel>(testHost, client, $"{QuizApiEndPoint}{quizid}");
                Assert.NotNull(quiz);

                // test both questions as answers
                var q1try = await PostAsync(new AnswerTest { Text = Q1_ANSWER }, testHost, client, $"{QuizApiEndPoint}{quizid}/questions/{q1id}/answertest");
                Assert.True(Int32.TryParse(q1try.Split('/').LastOrDefault(), out Int32 q1tryResult));
                var q2try = await PostAsync(new AnswerTest { Text = Q2_ANSWER }, testHost, client, $"{QuizApiEndPoint}{quizid}/questions/{q2id}/answertest");
                Assert.True(Int32.TryParse(q2try.Split('/').LastOrDefault(), out Int32 q2tryResult));

                Assert.True(q1tryResult + q2tryResult == 2);
            }
        }

        private async Task<T> GetAsync<T>(TestServer testHost, HttpClient client, String relativeUri)
        {
            try
            {
                var response = await client.GetAsync(new Uri(testHost.BaseAddress, relativeUri));
                var obj = JsonConvert.DeserializeObject<T>(await response.Content.ReadAsStringAsync());
                return obj;
            }
            catch (Exception e)
            {
                return default(T);
            }
        }

        private async Task<String> PostAsync<T>(T objToSend, TestServer testHost, HttpClient client, String relativeUri)
        {
            try
            {
                var content = new StringContent(JsonConvert.SerializeObject(objToSend));
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var response = await client.PostAsync(new Uri(testHost.BaseAddress, relativeUri), content);
                var result = await response.Content.ReadAsStringAsync();
                return String.IsNullOrWhiteSpace(result) ? response.Headers.Location.ToString() : result;
            }
            catch (Exception e)
            {
                return null;
            }
        }


        private async Task<Boolean> PutAsync<T>(T objToSend, TestServer testHost, HttpClient client, String relativeUri)
        {
            try
            {
                var content = new StringContent(JsonConvert.SerializeObject(objToSend));
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var response = await client.PutAsync(new Uri(testHost.BaseAddress, relativeUri), content);
                return response.IsSuccessStatusCode;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        [Fact]
        public async Task PostNewQuizAddsQuiz()
        {
            var quiz = new Quiz { Title = "Test title" };
            using (var testHost = new TestServer(new WebHostBuilder()
                .UseStartup<Startup>()))
            {
                var client = testHost.CreateClient();
                var content = new StringContent(JsonConvert.SerializeObject(quiz));
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var response = await client.PostAsync(new Uri(testHost.BaseAddress, $"{QuizApiEndPoint}"),
                    content);
                Assert.Equal(HttpStatusCode.Created, response.StatusCode);
                Assert.NotNull(response.Headers.Location);
            }
        }

        [Fact]
        public async Task AQuizExistGetReturnsQuiz()
        {
            using (var testHost = new TestServer(new WebHostBuilder()
                .UseStartup<Startup>()))
            {
                var client = testHost.CreateClient();
                const long quizId = 1;
                var response = await client.GetAsync(new Uri(testHost.BaseAddress, $"{QuizApiEndPoint}{quizId}"));
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.NotNull(response.Content);
                var quiz = JsonConvert.DeserializeObject<Quiz>(await response.Content.ReadAsStringAsync());
                Assert.Equal(quizId, quiz.Id);
                Assert.Equal("My first quiz", quiz.Title);
            }
        }

        [Fact]
        public async Task AQuizDoesNotExistGetFails()
        {
            using (var testHost = new TestServer(new WebHostBuilder()
                .UseStartup<Startup>()))
            {
                var client = testHost.CreateClient();
                const long quizId = 999;
                var response = await client.GetAsync(new Uri(testHost.BaseAddress, $"{QuizApiEndPoint}{quizId}"));
                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            }
        }

        [Fact]

        public async Task AQuizDoesNotExists_WhenPostingAQuestion_ReturnsNotFound()
        {
            const string QuizApiEndPoint = "/api/quizzes/999/questions";

            using (var testHost = new TestServer(new WebHostBuilder()
                .UseStartup<Startup>()))
            {
                var client = testHost.CreateClient();
                const long quizId = 999;
                var question = new Question
                {
                    QuizId = It.IsAny<Int32>(),
                    Text = "The answer to everything is what?",
                    CorrectAnswerId = It.IsAny<Int32>()
                };
                var content = new StringContent(JsonConvert.SerializeObject(question));
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var response = await client.PostAsync(new Uri(testHost.BaseAddress, $"{QuizApiEndPoint}"), content);
                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            }
        }
    }
}
