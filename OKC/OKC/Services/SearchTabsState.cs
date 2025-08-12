using OKC.Models;

namespace OKC.Services;

public class SearchTabsState
{
    public List<SearchTab> Tabs { get; } = new();
    public SearchTab ActiveTab { get; private set; }

    public event Action OnChange;

    public void SetTabs(List<SearchTab> tabs)
    {
        Tabs.Clear();
        Tabs.AddRange(tabs);
        ActiveTab = Tabs.FirstOrDefault();
    }

    public void SelectTab(SearchTab tab)
    {
        ActiveTab = tab;
    }

    private void NotifyStateChanged() => OnChange?.Invoke();
}
