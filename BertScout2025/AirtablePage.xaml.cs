using BertScout2025.Databases;
using BertScout2025.Models;

namespace BertScout2025;

public partial class AirtablePage
{
    private readonly LocalDatabase db = new();

    public AirtablePage()
    {
        InitializeComponent();
    }

    private async void AirtableSend_Clicked(object sender, EventArgs e)
    {
        try
        {
            AirtableSend.IsEnabled = false;
            AirtableUpdatedLabel.Text = "Sending, please wait...";
            AirtableResults.Text = "";
            InvalidateMeasure();

            List<TeamMatch> matches = await db.GetItemsAsync();
            var count = await AirtableService.AirtableSendRecords(matches);
            if (count < 0)
            {
                AirtableUpdatedLabel.Text = $"Error while sending some records to Airtable";
            }
            else
            {
                var showS = (count == 1) ? "" : "s";
                AirtableUpdatedLabel.Text = $"Sent {count} record{showS} to Airtable";
            }
            foreach (TeamMatch item in matches
                .OrderBy(x => $"{x.MatchNumber,3}{x.TeamNumber,5}"))
            {
                if (item.Changed)
                {
                    item.Changed = false;
                    await db.SaveItemAsync(item);
                    if (AirtableResults.Text.Length > 0)
                        AirtableResults.Text += "\r\n";
                    AirtableResults.Text += $"Match {item.MatchNumber,3} - Team {item.TeamNumber,5}";
                }
            }
        }
        catch (Exception ex)
        {
            AirtableResults.Text = ex.Message;
        }
        finally
        {
            AirtableSend.IsEnabled = true;
            Globals.item = new();
            Globals.viewFormBody = false;
        }
    }

    private void VerticalStackLayout_SizeChanged(object sender, EventArgs e)
    {
        ScrollResults.HeightRequest = cp.Height - ScrollResults.Y;
    }
}