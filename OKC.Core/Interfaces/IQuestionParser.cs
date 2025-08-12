using OKC.Core.Models;

namespace OKC.Core.Interfaces;

public interface IQuestionParser
{
    IEnumerable<Question> Parse(string file, string htmlContent);
}