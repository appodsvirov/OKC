using Nest;

namespace OKC.Core.Models;
public class AnswerOption
{
    [Text]
    public string Text { get; set; }

    public bool IsCorrect { get; set; }
}