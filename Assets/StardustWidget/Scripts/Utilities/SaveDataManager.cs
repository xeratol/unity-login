// Used to save scripts to json that implement ISaveable
public static class SaveDataManager
{
    public static void SaveJsonData(ISaveable saveable)
    {
        if (FileManager.WriteToFile(saveable.FileNameToUseForData(), saveable.ToJson()))
        {
            // Debug.Log("Save successful");
        }
    }

    public static void LoadJsonData(ISaveable saveable)
    {
        if (FileManager.LoadFromFile(saveable.FileNameToUseForData(), out var json))
        {
            saveable.LoadFromJson(json);
        }
    }
}

public interface ISaveable
{
    string ToJson();
    void LoadFromJson(string a_Json);
    string FileNameToUseForData();
}