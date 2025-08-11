using Microsoft.AspNetCore.Components.Forms;
using OKC.Core.Interfaces;

namespace OKC.Infrastructure.Services;

public class FileProcessingService
{
    private readonly IQuestionParser _parser;
    private readonly ElasticsearchService _elasticService;

    public FileProcessingService(IQuestionParser parser, ElasticsearchService elasticService)
    {
        _parser = parser;
        _elasticService = elasticService;
    }

    public async Task ProcessFiles(IEnumerable<IBrowserFile> files)
    {
        await _elasticService.RecreateIndexAsync();

        foreach (var file in files)
        {
            using var stream = file.OpenReadStream();
            using var reader = new StreamReader(stream);
            var htmlContent = await reader.ReadToEndAsync();

            var questions = _parser.Parse(htmlContent);
            await _elasticService.IndexQuestionsAsync(questions);
        }
    }
}