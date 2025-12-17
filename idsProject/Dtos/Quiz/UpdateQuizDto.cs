namespace idsProject.Dtos.Quiz
{
    public class UpdateQuizDto
    {
        public string Title { get; set; } = null!;
        public int PassingScore { get; set; }
        public int? TimeLimit { get; set; }

    }
}
