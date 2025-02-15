using BertScout2025.Databases;
using BertScout2025.Models;
using System.Collections.ObjectModel;

namespace BertScout2025;

public partial class ListPage
{
    private readonly LocalDatabase db = new();

    public MatchItem matchItem = new();

    public ListPage()
    {
        InitializeComponent();
        MatchesListView.ItemsSource = matchItem.MatchesList;
    }
    private async void ShowMatchesAsync()
    {
        // clear the current matches
        matchItem.Clear();

        List<TeamMatch> matches = await db.GetItemsAsync();
        foreach (TeamMatch item in matches
            .OrderBy(x => $"{x.MatchNumber,3}{x.TeamNumber,5}"))
        {
            matchItem.Add(new MatchItem() { Match = $"Match {item.MatchNumber,3} - Team {item.TeamNumber,5} - {item.ScoutName}" });
        }
    }

    private async void OpenMatchButton_Clicked(object sender, EventArgs e)
    {
        Button btn = (Button)sender;
        
        // safer way to get match and team - no hardcoded positions
        int pos1 = btn.Text.IndexOf('-');
        int pos2 = btn.Text.IndexOf('-', pos1 + 1);
        string matchSub = btn.Text[..pos1].Replace("Match", "").Trim();
        string teamSub = btn.Text[(pos1 + 1)..pos2].Replace("Team", "").Trim();
        int match = int.Parse(matchSub);
        int team = int.Parse(teamSub);

        Globals.item = await db.GetTeamMatchAsync(match, team);
        Globals.viewFormBody = true;

        Routing.RegisterRoute("mainpage", typeof(MainPage));
        await Shell.Current.GoToAsync("mainpage");
    }

    private void ShowMatchButton_Clicked(object sender, EventArgs e)
    {
        ShowMatchesAsync();
    }
}

public class MatchItem
{
    public string Match { get; set; } = "";
    public ObservableCollection<MatchItem> MatchesList { get; set; } = new ObservableCollection<MatchItem>();

    public void Add(MatchItem m)
    {
        MatchesList.Add(m);
    }
    public void Clear()
    {
        MatchesList.Clear();
    }
}