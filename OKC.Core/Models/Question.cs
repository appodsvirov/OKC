using Nest;

namespace OKC.Core.Models;

public class Question
{
    [Keyword]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [Text(Analyzer = "russian")]
    public string QuestionText { get; set; }

    [Nested]
    public List<AnswerOption> Options { get; set; }

    [Text(Analyzer = "russian")]
    public string CorrectAnswer { get; set; }
}