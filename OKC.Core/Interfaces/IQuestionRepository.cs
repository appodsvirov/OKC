using OKC.Core.Models;

namespace OKC.Core.Interfaces;

public interface IQuestionRepository
{
    void Save(IEnumerable<Question> questions);
    IEnumerable<Question> GetAll();
}