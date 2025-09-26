using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BookManager : MonoBehaviour
{
    private const string RESOURCES_FOLDER = "AnimalSO";
    public static BookManager Instance { get; private set; }

    private List<AnimalStruct> _allEntries = new();
    private HashSet<string> _unlockedIds = new(); // int → string
    private Dictionary<string, MonsterLevelType> _highestLevels = new(); // int → string

    private const string SaveKey = "BookUnlockProgress";

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        LoadAllData();
        LoadProgress();
    }

    public bool IsUnlocked(string id) => _unlockedIds.Contains(id);

    public void Unlock(string id, MonsterLevelType level)
    {
        bool changed = _unlockedIds.Add(id);

        if (!_highestLevels.ContainsKey(id) || level > _highestLevels[id])
        {
            _highestLevels[id] = level;
            changed = true;
        }

        if (changed) SaveProgress();
    }

    public MonsterLevelType GetHighestLevel(string id)
    {
        if (_highestLevels.TryGetValue(id, out var level))
            return level;
        return MonsterLevelType.C;
    }

    private void LoadAllData()
    {
        _allEntries.Clear();
        AnimalStruct[] loaded = Resources.LoadAll<AnimalStruct>(RESOURCES_FOLDER);

        foreach (AnimalStruct entry in loaded)
        {
            _allEntries.Add(entry);
        }
    }

    private void SaveProgress()
    {
        SaveData data = new SaveData(_unlockedIds, _highestLevels);
        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(SaveKey, json);
        PlayerPrefs.Save();
    }

    private void LoadProgress()
    {
        _unlockedIds.Clear();
        _highestLevels.Clear();

        string json = PlayerPrefs.GetString(SaveKey, "");
        if (!string.IsNullOrEmpty(json))
        {
            SaveData data = JsonUtility.FromJson<SaveData>(json);

            foreach (var id in data.unlockedIds)
                _unlockedIds.Add(id);

            foreach (var pair in data.highestLevels)
                _highestLevels[pair.id] = pair.level;
        }
    }

    [ContextMenu("Reset Save Data")]
    public void ResetSaveData()
    {
        PlayerPrefs.DeleteKey(SaveKey);
        _unlockedIds.Clear();
        _highestLevels.Clear();
    }

    public List<AnimalStruct> GetAllEntries() 
        => new List<AnimalStruct>(_allEntries);

    [System.Serializable]
    private class SaveData
    {
        public List<string> unlockedIds; // int → string
        public List<LevelEntry> highestLevels;

        [System.Serializable]
        public class LevelEntry
        {
            public string id; // int → string
            public MonsterLevelType level;
            public LevelEntry(string id, MonsterLevelType level)
            {
                this.id = id;
                this.level = level;
            }
        }

        public SaveData(HashSet<string> set, Dictionary<string, MonsterLevelType> levels)
        {
            unlockedIds = new List<string>(set);

            highestLevels = new List<LevelEntry>();
            foreach (var kvp in levels)
                highestLevels.Add(new LevelEntry(kvp.Key, kvp.Value));
        }
    }
}
