using AirtableApiClient;
using BertScout2025.Models;
using System.Reflection;
using System.Text;

namespace BertScout2025.Databases;

public class AirtableService
{
    // This is the Airtable base and table keys. A new base and table has to be created each year, with new keys.
    private const string AIRTABLE_BASE = "app37rfVnkSIZNl4m";
    private const string AIRTABLE_TABLE = "tblax92LZb7R9HDIz";

    // This is the Airtable token converted to Base64. This has to be done manually and inserted here.
    private const string AIRTABLE_TOKEN_BASE64 = "cGF0OGE1Mk9IVHpvRU1GUVIuZWVkYzQzMTUxMmNhMmVjM2ZkZTVhMDhhMzU4YmMxMjVlMWRhOGU0ZWI4Njk5Mzk1OTZkZmE1NzcyYjc3NTAyNw==";

    private static string AIRTABLE_TOKEN
    {
        get
        {
            var base64EncodedBytes = Convert.FromBase64String(AIRTABLE_TOKEN_BASE64);
            return Encoding.UTF8.GetString(base64EncodedBytes);
        }
    }

    public static async Task<int> AirtableSendRecords(List<TeamMatch> matches)
    {
        int NewCount = 0;
        int UpdatedCount = 0;
        List<Fields> newRecordList = [];
        List<IdFields> updatedRecordList = [];
        FieldInfo[] myFieldInfo;
        Type myType = typeof(TeamMatch);
        myFieldInfo = myType.GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);

        using AirtableBase airtableBase = new(AIRTABLE_TOKEN, AIRTABLE_BASE);

        foreach (TeamMatch match in matches)
        {
            if (match.Uuid == null) continue;
            if (string.IsNullOrEmpty(match.AirtableId))
            {
                Fields fields = new();
                foreach (FieldInfo fi in myFieldInfo)
                {
                    if (string.IsNullOrEmpty(fi.Name)) continue;
                    if (fi.Name.Contains('<') && fi.Name.Contains('>'))
                    {
                        // name is "<name>stuff", so just get the name part
                        int pos1 = fi.Name.IndexOf('<') + 1;
                        int pos2 = fi.Name.IndexOf('>');
                        string name = fi.Name[pos1..pos2];
                        // these fields are not in airtable
                        if (name.Equals("id", StringComparison.OrdinalIgnoreCase)) continue;
                        if (name.Equals("airtableid", StringComparison.OrdinalIgnoreCase)) continue;
                        if (name.Equals("changed", StringComparison.OrdinalIgnoreCase)) continue;
                        if (name.Equals("deleted", StringComparison.OrdinalIgnoreCase)) continue;
                        object? value = fi.GetValue(match);
                        if (value is bool v) // change to integers
                        {
                            value = v ? 1 : 0;
                        }
                        fields.AddField(name, value);
                    }
                }
                newRecordList.Add(fields);
            }
            else if (match.Changed)
            {
                IdFields idFields = new(match.AirtableId);
                foreach (FieldInfo fi in myFieldInfo)
                {
                    if (string.IsNullOrEmpty(fi.Name)) continue;
                    if (fi.Name.Contains('<') && fi.Name.Contains('>'))
                    {
                        // name is "<name>stuff", so just get the name part
                        int pos1 = fi.Name.IndexOf('<') + 1;
                        int pos2 = fi.Name.IndexOf('>');
                        string name = fi.Name[pos1..pos2];
                        // these fields are not in airtable
                        if (name.Equals("id", StringComparison.OrdinalIgnoreCase)) continue;
                        if (name.Equals("airtableid", StringComparison.OrdinalIgnoreCase)) continue;
                        if (name.Equals("changed", StringComparison.OrdinalIgnoreCase)) continue;
                        if (name.Equals("deleted", StringComparison.OrdinalIgnoreCase)) continue;
                        object? value = fi.GetValue(match);
                        if (value is bool v) // change to integers
                        {
                            value = v ? 1 : 0;
                        }
                        idFields.AddField(name, value);
                    }
                }
                updatedRecordList.Add(idFields);
            }

            if (newRecordList.Count > 0)
            {
                int tempCount = await AirtableSendNewRecords(airtableBase, newRecordList, matches);
                if (tempCount < 0)
                {
                    tempCount = 0; // error, don't count
                }
                NewCount += tempCount;
            }

            if (updatedRecordList.Count > 0)
            {
                int tempCount = await AirtableSendUpdatedRecords(airtableBase, updatedRecordList);
                if (tempCount < 0)
                {
                    tempCount = 0; // error, don't count
                }
                UpdatedCount += tempCount;
            }
        }

        return NewCount + UpdatedCount;
    }

    private static async Task<int> AirtableSendNewRecords(
        AirtableBase airtableBase,
        List<Fields> newRecordList,
        List<TeamMatch> matches)
    {
        AirtableCreateUpdateReplaceMultipleRecordsResponse result;
        List<Fields> sendList = [];
        int finalCount = 0;
        while (newRecordList.Count > 0)
        {
            sendList.Clear();
            do
            {
                sendList.Add(newRecordList[0]);
                newRecordList.RemoveAt(0);
            } while (newRecordList.Count > 0 && sendList.Count < 10);
            result = await airtableBase.CreateMultipleRecords(AIRTABLE_TABLE, sendList.ToArray());
            if (!result.Success)
            {
                return finalCount; // some may have sent
            }
            foreach (AirtableRecord rec in result.Records)
            {
                foreach (TeamMatch match in matches
                    .Where(x => x.Uuid == (rec.GetField("Uuid")?.ToString() ?? "")))
                {
                    match.AirtableId = rec.Id;
                    match.Changed = true;
                    finalCount++;
                }
            }
            if (newRecordList.Count > 0)
            {
                // can only send 5 batches per second - make sure that doesn't happen
                Thread.Sleep(250);
            }
        }
        return finalCount;
    }

    private static async Task<int> AirtableSendUpdatedRecords(
        AirtableBase airtableBase,
        List<IdFields> updatedRecordList)
    {
        AirtableCreateUpdateReplaceMultipleRecordsResponse result;
        List<IdFields> sendList = [];
        int finalCount = 0;
        while (updatedRecordList.Count > 0)
        {
            sendList.Clear();
            do
            {
                sendList.Add(updatedRecordList[0]);
                updatedRecordList.RemoveAt(0);
            } while (updatedRecordList.Count > 0 && sendList.Count < 10);
            result = await airtableBase.UpdateMultipleRecords(AIRTABLE_TABLE, sendList.ToArray());
            if (!result.Success)
            {
                return finalCount; // some may have sent
            }
            foreach (AirtableRecord rec in result.Records)
            {
                finalCount++;
            }
            if (updatedRecordList.Count > 0)
            {
                // can only send 5 batches per second, make sure that doesn't happen
                Thread.Sleep(250);
            }
        }
        return finalCount;
    }
}
