using BertScout2025.Models;
using SQLite;

namespace BertScout2025.Databases;

public class LocalDatabase
{
    public string DatabaseDirPath { get; set; } = "";

    private const string DatabaseFilename = "BertScout2025.db3";

    private const SQLiteOpenFlags Flags =
        // open the database in read/write mode
        SQLiteOpenFlags.ReadWrite |
        // create the database if it doesn't exist
        SQLiteOpenFlags.Create |
        // enable multi-threaded database access
        SQLiteOpenFlags.SharedCache;

    private bool _created = false;
    private SQLiteAsyncConnection Database = new("");

    public LocalDatabase()
    {
#if ANDROID
        if (Directory.Exists("/sdcard/Documents"))
        {
            DatabaseDirPath = "/sdcard/Documents";
        }
#elif WINDOWS
        if (!Directory.Exists("C:\\Temp"))
        {
            Directory.CreateDirectory("C:\\Temp");
        }
        DatabaseDirPath = "C:\\Temp";
#endif
        DatabaseDirPath ??= FileSystem.AppDataDirectory;
    }

    private async Task Init()
    {
        if (_created)
        {
            return;
        }
        var databasePath = Path.Combine(DatabaseDirPath, DatabaseFilename);
        try
        {
            Database = new(databasePath, Flags);
            await Database.CreateTableAsync<TeamMatch>();
            _created = true;
        }
        catch (Exception ex)
        {
            throw new SystemException($"Error initializing database: {databasePath}\r\n{ex.Message}");
        }
    }

    public async Task<List<TeamMatch>> GetItemsAsync()
    {
        await Init();
        return await Database.Table<TeamMatch>()
            .ToListAsync();
    }

    public async Task<List<TeamMatch>> GetChangedItemsAsync()
    {
        await Init();
        return await Database.Table<TeamMatch>()
            .Where(i => i.Changed)
            .ToListAsync();
    }

    public async Task<List<TeamMatch>> GetTeamAllMatches(int team)
    {
        await Init();
        return await Database.Table<TeamMatch>()
            .Where(i => i.TeamNumber == team)
            .OrderBy(i => i.MatchNumber)
            .ToListAsync();
    }

    public async Task<TeamMatch> GetTeamMatchAsync(int match, int team)
    {
        await Init();
        return await Database.Table<TeamMatch>()
            .Where(i => i.MatchNumber == match && i.TeamNumber == team)
            .FirstOrDefaultAsync();
    }

    public async Task<TeamMatch> GetItemAsync(int id)
    {
        await Init();
        return await Database.Table<TeamMatch>()
            .Where(i => i.Id == id)
            .FirstOrDefaultAsync();
    }

    public async Task<int> SaveItemAsync(TeamMatch item)
    {
        await Init();
        if (item.Id != 0)
        {
            return await Database.UpdateAsync(item);
        }
        var oldItem = await GetTeamMatchAsync(item.MatchNumber, item.TeamNumber);
        if (oldItem != null)
        {
            item.Id = oldItem.Id;
            item.Uuid = oldItem.Uuid;
            // AirtableId may be updated in item, don't overwrite
            if (!string.IsNullOrWhiteSpace(oldItem.AirtableId))
                item.AirtableId = oldItem.AirtableId;
            return await Database.UpdateAsync(item);
        }
        item.Uuid = Guid.NewGuid().ToString();
        return await Database.InsertAsync(item);
    }

    public async Task DeleteTeamMatchAsync(int match, int team)
    {
        await Init();
        var item = await Database.Table<TeamMatch>()
            .Where(i => i.TeamNumber == team && i.MatchNumber == match)
            .FirstOrDefaultAsync();
        if (item != null)
        {
            await Database.DeleteAsync(item);
        }
    }
}