using BertScout2025.Databases;
using BertScout2025.Models;
using System.Collections.ObjectModel;

namespace BertScout2025;

public partial class ListPage
{
    private readonly LocalDatabase db = new();

    public MatchItem matchItem = new();

    public ObservableCollection<MatchItem> MatchesList { get; set; } = [];

    public ListPage()
    {
        InitializeComponent();
        LoadMatchesAsync();
        MatchesListView.ItemsSource = MatchesList;
    }

    private async void LoadMatchesAsync()
    {
        // clear the current matches
        MatchesList.Clear();

        List<TeamMatch> matches = await db.GetItemsAsync();
        foreach (TeamMatch item in matches
            .OrderBy(x => $"{x.MatchNumber,3}{x.TeamNumber,5}"))
        {
            MatchesList.Add(
                new MatchItem()
                {
                    MatchNumber = item.MatchNumber,
                    TeamNumber = item.TeamNumber,
                    ScoutName = item.ScoutName,
                    Changed = item.Changed
                }
            );
        }
    }

    private async void OpenMatchButton_Clicked(object sender, EventArgs e)
    {
        Button btn = (Button)sender;

        // safer way to get match and team - no hardcoded positions
        int pos1 = btn.Text.IndexOf('-');
        int pos2 = btn.Text.IndexOf('-', pos1 + 1);
        string matchSub = btn.Text[..pos1].Replace("* ", "").Replace("Match", "").Trim();
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
        LoadMatchesAsync();
    }
}

public class MatchItem
{
    public int MatchNumber { get; set; }
    public int TeamNumber { get; set; }
    public string ScoutName { get; set; } = "";
    public bool Changed { get; set; }

    public string Text
    {
        get
        {
            var changedFlag = Changed ? "* " : "";
            return $"{changedFlag}Match {MatchNumber,3} - Team {TeamNumber,5} - {ScoutName}";
        }
    }
}
