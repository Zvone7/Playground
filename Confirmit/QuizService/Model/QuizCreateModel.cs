namespace QuizService.Model
{
    public class QuizCreateModel
    {
        public QuizCreateModel(string title)
        {
            Title = title;
        }
        public QuizCreateModel()
        {

        }

        public string Title { get; set; }
    }
}