namespace QuizService.Model
{
    public class QuestionCreateModel
    {
        public QuestionCreateModel(string text)
        {
            Text = text;
        }
        public QuestionCreateModel()
        {

        }

        public string Text { get; set; }
    }
}
