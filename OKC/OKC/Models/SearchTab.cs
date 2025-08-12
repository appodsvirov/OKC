using OKC.Core.Models;

namespace OKC.Models;
public class SearchTab
{
    public string FilePath { get; set; }
    public string TabTitle { get; set; }
    public string IndexName { get; set; }
    public string SearchQuery { get; set; } = string.Empty;
    public List<Question> Results { get; set; } = new();
}
