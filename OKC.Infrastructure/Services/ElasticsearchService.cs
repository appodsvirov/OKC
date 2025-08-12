using Nest;
using OKC.Core.Models;

namespace OKC.Infrastructure.Services;

public class ElasticsearchService
{
    public ElasticClient Client { get; }

    public ElasticsearchService(string uri = "http://localhost:9200")
    {
        var settings = new ConnectionSettings(new Uri(uri));
        Client = new ElasticClient(settings);
    }

    public async Task CreateIndexWithNgramAsync(string indexName, bool recreate = false)
    {
        if (recreate && (await Client.Indices.ExistsAsync(indexName)).Exists)
            await Client.Indices.DeleteAsync(indexName);

        if ((await Client.Indices.ExistsAsync(indexName)).Exists)
            return;

        var createResponse = await Client.Indices.CreateAsync(indexName, c => c
            .Settings(s => s
                .Analysis(a => a
                    .TokenFilters(tf => tf
                        .EdgeNGram("edge_ngram_filter", eg => eg
                            .MinGram(1)
                            .MaxGram(20)
                        )
                    )
                    .Analyzers(an => an
                        .Custom("ngram_analyzer", ca => ca
                            .Tokenizer("standard")
                            .Filters("lowercase", "edge_ngram_filter")
                        )
                        .Custom("search_analyzer", sa => sa
                            .Tokenizer("standard")
                            .Filters("lowercase")
                        )
                    )
                )
            )
            .Map<Question>(m => m
                .AutoMap()
                .Properties(ps => ps
                    .Text(t => t
                        .Name(n => n.QuestionText)
                        .Analyzer("ngram_analyzer")
                        .SearchAnalyzer("search_analyzer")
                    )
                    .Text(t => t
                        .Name(n => n.CorrectAnswer)
                        .Analyzer("ngram_analyzer")
                        .SearchAnalyzer("search_analyzer")
                    )
                    // Если Options — nested objects с полем Text:
                    .Nested<object>(n => n
                        .Name("options")
                        .Properties(pp => pp
                            .Text(t => t
                                .Name("text")
                                .Analyzer("ngram_analyzer")
                                .SearchAnalyzer("search_analyzer")
                            )
                        )
                    )
                )
            )
        );

        if (!createResponse.IsValid)
            Console.WriteLine("Create index error: " + createResponse.OriginalException?.Message);
    }


    public async Task IndexQuestionsAsync(IEnumerable<Question> questions, string indexName, bool recreate = false)
    {
        await CreateIndexWithNgramAsync(indexName, recreate);
        await Client.IndexManyAsync(questions, indexName);
    }


    public async Task<IEnumerable<Question>> SearchAsync(string query, string indexName)
    {
        if (string.IsNullOrWhiteSpace(query)) return Enumerable.Empty<Question>();

        var words = query.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var response = await Client.SearchAsync<Question>(s => s
            .Index(indexName)
            .Query(q => q
                .MultiMatch(m => m
                        .Fields(f => f
                                .Field(ff => ff.QuestionText)
                                .Field(ff => ff.CorrectAnswer)
                                .Field("options.text") // если нельзя типизированно
                        )
                        .Query(query)
                        .Operator(Operator.And)   // требуем, чтобы все слова присутствовали
                        .Type(TextQueryType.CrossFields) // или BestFields, можно подправить
                )
            )
            .Size(1000)
        );

        if (!response.IsValid) { Console.WriteLine(response.OriginalException); return Enumerable.Empty<Question>(); }
        return response.Documents;
    }
}
