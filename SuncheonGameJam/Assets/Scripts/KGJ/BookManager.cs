using System.Collections.Generic;
using UnityEngine;

public class BookManager : Singleton<BookManager>
{
    private const string RESOURCES_FOLDER = "AnimalSO";

    private List<AnimalStruct> _allEntries = new();
    private HashSet<string> _unlockedIds = new(); // int → string
    private Dictionary<string, MonsterLevelType> _highestLevels = new(); // int → string

    private const string SaveKey = "BookUnlockProgress";

    protected override void Awake()
    {
        base.Awake();
        
        LoadAllData();
        LoadProgress();
    }

    public bool IsUnlocked(string id) => _unlockedIds.Contains(id);

    /// <summary>
    /// 미니게임 성공시 잡은 개체의 id와 등급을 넣어주면 해금됩니다.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="level"></param>
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

    /// <summary>
    /// 해금된 개체의 id를 넣으면 최고 등급 기록을 반환합니다.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
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
