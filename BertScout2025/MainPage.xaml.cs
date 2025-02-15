using BertScout2025.Databases;
using BertScout2025.Models;

namespace BertScout2025
{
    public partial class MainPage : ContentPage
    {
        private readonly LocalDatabase db = new();

        private TeamMatch item = new();

        public MainPage()
        {
            InitializeComponent();
            CommentPicker.Items.Clear();

            foreach (string s in CommentList)
            {
                CommentPicker.Items.Add(s);
            }

            if (Globals.viewFormBody)
            {
                TeamNumber.Text = Globals.item.TeamNumber.ToString();
                MatchNumber.Text = Globals.item.MatchNumber.ToString();
                ScoutName.Text = Globals.item.ScoutName;

                this.item = Globals.item;
                Globals.viewFormBody = false;

                Load_Match();
            }
        }

        private void Load_Match()
        {
            // show the values on the screen
            FillFields(item);
            // disable the top row while entering
            EnableTopRow(false);
        }

        //IEnumerable<ConnectionProfile> profiles = Connectivity.Current.ConnectionProfiles;
        private async void Start_Clicked(object sender, EventArgs e)
        {
            if (Start.Text == "Start")
            {
                // check that all fields are valid
                if (!ValidateMatchNumber(MatchNumber.Text)) return;
                if (!ValidateTeamNumber(TeamNumber.Text)) return;

                // get integer values for later use
                var match = int.Parse(MatchNumber.Text);
                var team = int.Parse(TeamNumber.Text);

                // get existing record
                item = await db.GetTeamMatchAsync(match, team);

                // check they entered a scout name
                if (item == null && !ValidateScoutName(ScoutName.Text)) return;

                // update screen fields without leading zeros
                MatchNumber.Text = match.ToString();
                TeamNumber.Text = team.ToString();

                // delete the match
                if (ScoutName.Text == "DELETE")
                {
                    bool answer = await DisplayAlert("Confirm", "Are you sure you want to delete this match?", "OK", "Cancel");
                    if (answer)
                    {
                        await db.DeleteTeamMatchAsync(match, team);
                    }
                    MatchNumber.Text = "";
                    TeamNumber.Text = "";
                    ScoutName.Text = "";
                    // re-enable top row and focus on team number
                    EnableTopRow(true);
                    TeamNumber.Focus();
                    return;
                }

                // if not found, create new record
                item ??= new()
                {
                    MatchNumber = match,
                    TeamNumber = team,
                    ScoutName = ScoutName.Text,
                    Comments = "",
                };

                Load_Match();
            }
            else if (Start.Text == "Save")
            {
                // store the screen fields in the record
                SaveFields();

                /*
                if (profiles.Contains(ConnectionProfile.WiFi) && false)
                {
                    AirtablePage aPage = new AirtablePage();
                    var uselessTask = Task.Run(() => aPage.AirtableSender());
                    uselessTask.Wait();
                    var useless = uselessTask.Result;
                }
                */

                // prepare for next match
                var match = int.Parse(MatchNumber.Text);
                var newMatch = Math.Min(match + 1, 999);
                MatchNumber.Text = newMatch.ToString();
                TeamNumber.Text = "";
                ClearAllFields();
                // re-enable top row and focus on team number
                EnableTopRow(true);
                TeamNumber.Focus();
            }
        }

        private void SaveFields()
        {
            // store the screen fields into the record
            StoreFields(item);

            // save to database
            item.Changed = true;
            var taskSave = Task.Run(() => db.SaveItemAsync(item));
            taskSave.Wait();
        }

        // Autonomous

        private void ButtonAutoLeave_Clicked(object sender, EventArgs e)
        {
            item.Auto_Leave = !item.Auto_Leave;
            ButtonAutoLeave.BackgroundColor = (item.Auto_Leave ? Colors.Green : Colors.Gray);
            SaveFields();
        }

        private void ButtonAutoCoralL1Minus_Clicked(object sender, EventArgs e)
        {
            if
                (item.Auto_Coral_L1 > 0)
            {
                item.Auto_Coral_L1--;
                LabelAutoCoralL1.Text = item.Auto_Coral_L1.ToString();
                SaveFields();
            }
        }

        private void ButtonAutoCoralL1Plus_Clicked(object sender, EventArgs e)
        {
            item.Auto_Coral_L1++;
            LabelAutoCoralL1.Text = item.Auto_Coral_L1.ToString();
            SaveFields();
        }

        private void ButtonAutoCoralL2Minus_Clicked(object sender, EventArgs e)
        {
            if
                (item.Auto_Coral_L2 > 0)
            {
                item.Auto_Coral_L2--;
                LabelAutoCoralL2.Text = item.Auto_Coral_L2.ToString();
                SaveFields();
            }
        }

        private void ButtonAutoCoralL2Plus_Clicked(object sender, EventArgs e)
        {
            item.Auto_Coral_L2++;
            LabelAutoCoralL2.Text = item.Auto_Coral_L2.ToString();
            SaveFields();
        }

        private void ButtonAutoCoralL3Minus_Clicked(object sender, EventArgs e)
        {
            if
                (item.Auto_Coral_L3 > 0)
            {
                item.Auto_Coral_L3--;
                LabelAutoCoralL3.Text = item.Auto_Coral_L3.ToString();
                SaveFields();
            }
        }

        private void ButtonAutoCoralL3Plus_Clicked(object sender, EventArgs e)
        {
            item.Auto_Coral_L3++;
            LabelAutoCoralL3.Text = item.Auto_Coral_L3.ToString();
            SaveFields();
        }

        private void ButtonAutoCoralL4Minus_Clicked(object sender, EventArgs e)
        {
            if
                (item.Auto_Coral_L4 > 0)
            {
                item.Auto_Coral_L4--;
                LabelAutoCoralL4.Text = item.Auto_Coral_L4.ToString();
                SaveFields();
            }
        }

        private void ButtonAutoCoralL4Plus_Clicked(object sender, EventArgs e)
        {
            item.Auto_Coral_L4++;
            LabelAutoCoralL4.Text = item.Auto_Coral_L4.ToString();
            SaveFields();
        }

        private void ButtonAutoProcessorMinus_Clicked(object sender, EventArgs e)
        {
            if
                (item.Auto_Processor > 0)
            {
                item.Auto_Processor--;
                LabelAutoProcessor.Text = item.Auto_Processor.ToString();
                SaveFields();
            }
        }
        private void ButtonAutoProcessorPlus_Clicked(object sender, EventArgs e)
        {
            item.Auto_Processor++;
            LabelAutoProcessor.Text = item.Auto_Processor.ToString();
            SaveFields();
        }

        private void ButtonAutoBargeMinus_Clicked(object sender, EventArgs e)
        {
            if
                (item.Auto_Barge > 0)
            {
                item.Auto_Barge--;
                LabelAutoBarge.Text = item.Auto_Barge.ToString();
                SaveFields();
            }
        }
        private void ButtonAutoBargePlus_Clicked(object sender, EventArgs e)
        {
            item.Auto_Barge++;
            LabelAutoBarge.Text = item.Auto_Barge.ToString();
            SaveFields();
        }

        // Teleop

        private void ButtonTeleCoralL1Minus_Clicked(object sender, EventArgs e)
        {
            if
                (item.Tele_Coral_L1 > 0)
            {
                item.Tele_Coral_L1--;
                LabelTeleCoralL1.Text = item.Tele_Coral_L1.ToString();
                SaveFields();
            }
        }
        private void ButtonTeleCoralL1Plus_Clicked(object sender, EventArgs e)
        {
            item.Tele_Coral_L1++;
            LabelTeleCoralL1.Text = item.Tele_Coral_L1.ToString();
            SaveFields();
        }

        private void ButtonTeleCoralL2Minus_Clicked(object sender, EventArgs e)
        {
            if
                (item.Tele_Coral_L2 > 0)
            {
                item.Tele_Coral_L2--;
                LabelTeleCoralL2.Text = item.Tele_Coral_L2.ToString();
                SaveFields();
            }
        }
        private void ButtonTeleCoralL2Plus_Clicked(object sender, EventArgs e)
        {
            item.Tele_Coral_L2++;
            LabelTeleCoralL2.Text = item.Tele_Coral_L2.ToString();
            SaveFields();
        }

        private void ButtonTeleCoralL3Minus_Clicked(object sender, EventArgs e)
        {
            if
                (item.Tele_Coral_L3 > 0)
            {
                item.Tele_Coral_L3--;
                LabelTeleCoralL3.Text = item.Tele_Coral_L3.ToString();
                SaveFields();
            }
        }
        private void ButtonTeleCoralL3Plus_Clicked(object sender, EventArgs e)
        {
            item.Tele_Coral_L3++;
            LabelTeleCoralL3.Text = item.Tele_Coral_L3.ToString();
            SaveFields();
        }

        private void ButtonTeleCoralL4Minus_Clicked(object sender, EventArgs e)
        {
            if
                (item.Tele_Coral_L4 > 0)
            {
                item.Tele_Coral_L4--;
                LabelTeleCoralL4.Text = item.Tele_Coral_L4.ToString();
                SaveFields();
            }
        }
        private void ButtonTeleCoralL4Plus_Clicked(object sender, EventArgs e)
        {
            item.Tele_Coral_L4++;
            LabelTeleCoralL4.Text = item.Tele_Coral_L4.ToString();
            SaveFields();
        }

        private void ButtonTeleProcessorMinus_Clicked(object sender, EventArgs e)
        {
            if
                (item.Tele_Processor > 0)
            {
                item.Tele_Processor--;
                LabelTeleProcessor.Text = item.Tele_Processor.ToString();
                SaveFields();
            }
        }
        private void ButtonTeleProcessorPlus_Clicked(object sender, EventArgs e)
        {
            item.Tele_Processor++;
            LabelTeleProcessor.Text = item.Tele_Processor.ToString();
            SaveFields();
        }

        private void ButtonTeleBargeMinus_Clicked(object sender, EventArgs e)
        {
            if
                (item.Tele_Barge > 0)
            {
                item.Tele_Barge--;
                LabelTeleBarge.Text = item.Tele_Barge.ToString();
                SaveFields();
            }
        }
        private void ButtonTeleBargePlus_Clicked(object sender, EventArgs e)
        {
            item.Tele_Barge++;
            LabelTeleBarge.Text = item.Tele_Barge.ToString();
            SaveFields();
        }

        // Endgame

        private void ButtonEndgameParked_Clicked(object sender, EventArgs e)
        {
            SetButton_Parked(!item.Endgame_Parked);
            if (item.Endgame_Parked)
            {
                SetButton_ShallowCage(false);
                SetButton_DeepCage(false);
            }
            SaveFields();
        }

        private void ButtonEndgameShallowCage_Clicked(object sender, EventArgs e)
        {
            SetButton_ShallowCage(!item.Endgame_Shallow_Cage);
            if (item.Endgame_Shallow_Cage)
            {
                SetButton_Parked(false);
                SetButton_DeepCage(false);
            }
            SaveFields();
        }

        private void ButtonEndgameDeepCage_Clicked(object sender, EventArgs e)
        {
            SetButton_DeepCage(!item.Endgame_Deep_Cage);
            if (item.Endgame_Deep_Cage)
            {
                SetButton_Parked(false);
                SetButton_ShallowCage(false);
            }
            SaveFields();
        }

        private void CommentPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CommentPicker.SelectedIndex < 0)
                return;
            if (Comments.Text == null)
                Comments.Text = "";
            else if (Comments.Text.Length > 0 && !Comments.Text.EndsWith(' '))
                Comments.Text += " ";
            Comments.Text += CommentPicker.SelectedItem.ToString() + " ";
            CommentPicker.SelectedIndex = -1;
            SaveFields();
        }

        private void Comments_TextChanged(object sender, TextChangedEventArgs e)
        {
            var temp = Comments?.Text ?? "";
            if (temp.Length > 250)
            {
                temp = temp[0..250];
                Comments!.Text = temp;
            }
            item.Comments = temp;
        }

        #region ButtonEvents

        private void SetButton_Parked(bool value)
        {
            item.Endgame_Parked = value;
            ButtonEndgameParked.BackgroundColor = (value ? Colors.Green : Colors.Gray);
        }

        private void SetButton_ShallowCage(bool value)
        {
            item.Endgame_Shallow_Cage = value;
            ButtonEndgameShallowCage.BackgroundColor = (value ? Colors.Green : Colors.Gray);
        }

        private void SetButton_DeepCage(bool value)
        {
            item.Endgame_Deep_Cage = value;
            ButtonEndgameDeepCage.BackgroundColor = (value ? Colors.Green : Colors.Gray);
        }

        #endregion
    }
}
