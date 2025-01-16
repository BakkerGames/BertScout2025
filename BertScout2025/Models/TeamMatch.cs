using SQLite;
using System.Text.Json;

namespace BertScout2025.Models;

public class TeamMatch : BaseModel
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [Indexed(Name = "TeamMatchUnique", Order = 1, Unique = true)]
    public int MatchNumber { get; set; }

    [Indexed(Name = "TeamMatchUnique", Order = 2, Unique = true)]
    public int TeamNumber { get; set; }

    [Indexed(Unique = true)]
    public string Uuid { get; set; } = "";

    public string AirtableId { get; set; } = "";

    public bool Changed { get; set; }

    public bool Deleted { get; set; }

    public string ScoutName { get; set; } = "";

    // autonomous

    public bool Auto_Leave { get; set; } = false;
    public int Auto_Coral_L1 { get; set; }
    public int Auto_Coral_L2 { get; set; }
    public int Auto_Coral_L3 { get; set; }
    public int Auto_Coral_L4 { get; set; }
    public int Auto_Processor { get; set; }
    public int Auto_Net { get; set; }

    // teleop

    public int Tele_Coral_L1 { get; set; }
    public int Tele_Coral_L2 { get; set; }
    public int Tele_Coral_L3 { get; set; }
    public int Tele_Coral_L4 { get; set; }
    public int Tele_Processor { get; set; }
    public int Tele_Net { get; set; }
    public bool Tele_Coop { get; set; } = false;

    // end game

    public bool Endgame_Parked { get; set; } = false;
    public bool Endgame_Shallow_Cage { get; set; } = false;
    public bool Endgame_Deep_Cage { get; set; } = false;

    // overall

    public string Comments { get; set; } = "";

    public override string ToString()
    {
        return JsonSerializer.Serialize(this, WriteOptions);
    }
}
