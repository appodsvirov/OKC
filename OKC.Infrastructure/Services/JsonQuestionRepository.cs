using Newtonsoft.Json;
using OKC.Core.Interfaces;
using OKC.Core.Models;

namespace OKC.Infrastructure.Services;

public class JsonQuestionRepository : IQuestionRepository
{
    private readonly string _filePath;
    private List<Question> _questions = new List<Question>();

    public JsonQuestionRepository(string filePath)
    {
        _filePath = filePath;
    }

    public void Save(IEnumerable<Question> questions)
    {
        _questions.AddRange(questions);
        File.WriteAllText(_filePath, JsonConvert.SerializeObject(_questions, Formatting.Indented));
    }

    public IEnumerable<Question> GetAll() => _questions;
}