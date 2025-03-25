using BertScout2025.Models;

namespace BertScout2025;

public partial class MainPage
{
    public void EnableTopRow(bool enable)
    {
        MatchNumber.IsEnabled = true;
        TeamNumber.IsEnabled = true;
        FormBody.IsVisible = !enable;
        Start.Text = enable ? "Start" : "Save";
    }

    public static bool ValidateMatchNumber(string matchNumber)
    {
        if (!int.TryParse(matchNumber, out int mNumber))
        {
            return false;
        }
        if (mNumber < 1)
        {
            return false;
        }
        return true;
    }

    public static bool ValidateTeamNumber(string teamNumber)
    {
        if (!int.TryParse(teamNumber, out int tNumber))
        {
            return false;
        }
        if (tNumber < 1)
        {
            return false;
        }
        return true;
    }

    public static bool ValidateScoutName(string scoutName)
    {
        if (string.IsNullOrWhiteSpace(scoutName))
        {
            return false;
        }
        else if (scoutName.Equals("nft", StringComparison.OrdinalIgnoreCase))
        {
            throw new SystemException("Crash!");
        }
        else if (scoutName.Equals("skibidi", StringComparison.OrdinalIgnoreCase))
        {
            throw new SystemException("No.");
        }
        return true;
    }

    public void ClearAllFields()
    {
        Comments.Text = "";
    }

    private void FillFields(TeamMatch item)
    {
        SetButton_Leave(item.Auto_Leave);

        LabelAutoCoralL1.Text = item.Auto_Coral_L1.ToString();
        LabelAutoCoralL2.Text = item.Auto_Coral_L2.ToString();
        LabelAutoCoralL3.Text = item.Auto_Coral_L3.ToString();
        LabelAutoCoralL4.Text = item.Auto_Coral_L4.ToString();
        LabelAutoProcessor.Text = item.Auto_Processor.ToString();
        LabelAutoBarge.Text = item.Auto_Barge.ToString();

        LabelTeleCoralL1.Text = item.Tele_Coral_L1.ToString();
        LabelTeleCoralL2.Text = item.Tele_Coral_L2.ToString();
        LabelTeleCoralL3.Text = item.Tele_Coral_L3.ToString();
        LabelTeleCoralL4.Text = item.Tele_Coral_L4.ToString();
        LabelTeleProcessor.Text = item.Tele_Processor.ToString();
        LabelTeleBarge.Text = item.Tele_Barge.ToString();

        SetButton_Parked(item.Endgame_Parked);
        SetButton_ShallowCage(item.Endgame_Shallow_Cage);
        SetButton_DeepCage(item.Endgame_Deep_Cage);

        DefensePicker.SelectedIndex = item.Defense_Score;

        Comments.Text = item.Comments;
        CommentPicker.SelectedIndex = -1;
    }

    private void StoreFields(TeamMatch item)
    {
        if (string.IsNullOrWhiteSpace(item.ScoutName))
            item.ScoutName = ScoutName.Text;
        // everything else handled by Clicked/Changed events
    }
}