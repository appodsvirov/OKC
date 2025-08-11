using OKC.Core.Interfaces;

namespace OKC.Infrastructure.Services;

public class QuestionProcessor
{
    private readonly IQuestionParser _parser;
    private readonly IQuestionRepository _repository;

    public QuestionProcessor(IQuestionParser parser, IQuestionRepository repository)
    {
        _parser = parser;
        _repository = repository;
    }

    public void ProcessFiles(IEnumerable<string> filePaths)
    {
        foreach (var filePath in filePaths)
        {
            var htmlContent = File.ReadAllText(filePath);
            var questions = _parser.Parse(htmlContent);
            _repository.Save(questions);
        }
    }
}