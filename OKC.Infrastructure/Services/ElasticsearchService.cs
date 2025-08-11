using Nest;
using OKC.Core.Models;

namespace OKC.Infrastructure.Services;

public class ElasticsearchService
{
    public ElasticClient Client { get; }

    public ElasticsearchService(string uri = "http://localhost:9200")
    {
        var settings = new ConnectionSettings(new Uri(uri))
            .DefaultIndex("questions")
            .DefaultMappingFor<Question>(m => m
                .IndexName("questions")
            );

        Client = new ElasticClient(settings);
    }

    public async Task CreateIndexAsync()
    {
        await Client.Indices.CreateAsync("questions", c => c
            .Map<Question>(m => m.AutoMap())
        );
    }

    public async Task IndexQuestionsAsync(IEnumerable<Question> questions)
    {
        await Client.IndexManyAsync(questions);
    }

    public async Task<IEnumerable<Question>> SearchAsync(string query)
    {
        var response = await Client.SearchAsync<Question>(s => s
            .Query(q => q
                .MultiMatch(m => m
                    .Fields(f => f
                        .Field(ff => ff.QuestionText)
                        .Field(ff => ff.CorrectAnswer)
                        .Field(ff => ff.Options.First().Text)
                    )
                    .Query(query)
                    .Fuzziness(Fuzziness.Auto)
                )
            )
            .Highlight(h => h
                .Fields(f => f
                    .Field("*")
                    .PreTags("<mark>")
                    .PostTags("</mark>")
                )
            )
        );

        return response.Hits.Select(h =>
        {
            h.Source.Id = h.Id;
            return h.Source;
        });
    }

    public async Task DeleteIndexAsync()
    {
        await Client.Indices.DeleteAsync("questions");
    }

    public async Task RecreateIndexAsync()
    {
        await DeleteIndexAsync();
        await CreateIndexAsync();
    }
}