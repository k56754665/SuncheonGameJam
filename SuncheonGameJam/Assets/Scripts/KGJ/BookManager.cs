using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BookManager : MonoBehaviour
{
    private const string RESOURCES_FOLDER = "AnimalSO";
    public static BookManager Instance { get; private set; }

    private List<AnimalStruct> _allEntries = new();
    private HashSet<int> _unlockedIds = new();

    // id별 최고 레벨 기록
    private Dictionary<int, MonsterLevelType> _highestLevels = new();

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
    
    public bool IsUnlocked(int id) => _unlockedIds.Contains(id);

    /// <summary>
    /// id값을 가진 동물을 해금시키는 메서드
    /// </summary>
    public void Unlock(int id, MonsterLevelType level)
    {
        bool changed = _unlockedIds.Add(id);

        // 해금 기록

        // 최고 레벨 기록 (기존보다 높은 경우만 업데이트)
        if (!_highestLevels.ContainsKey(id) || level > _highestLevels[id])
        {
            _highestLevels[id] = level;
            changed = true;
        }

        if (changed) SaveProgress();
    }

    /// <summary>
    /// 특정 동물의 최고 레벨 반환
    /// </summary>
    public MonsterLevelType GetHighestLevel(int id)
    {
        if (_highestLevels.TryGetValue(id, out var level))
            return level;
        return MonsterLevelType.C; // 기본값
    }

    private void LoadAllData()
    {
        _allEntries.Clear();
        AnimalStruct[] loaded = Resources.LoadAll<AnimalStruct>(RESOURCES_FOLDER);

        foreach (AnimalStruct entry in loaded)
        {
            _allEntries.Add(entry);
            Debug.Log($"[BookManager] Loaded: ID={entry.id}, Name={entry.animalName}");
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
    
    private void OnGUI()
    {
        float btnWidth = 150f;
        float btnHeight = 40f;
        float padding = 10f;

        float x = Screen.width - btnWidth - padding;
        float y0 = Screen.height - (btnHeight * 2 + padding * 2);
        float y1 = Screen.height - (btnHeight + padding);

        if (GUI.Button(new Rect(x, y0, btnWidth, btnHeight), "Unlock ID 0"))
        {
            Unlock(0, MonsterLevelType.SSS);
            Debug.Log("Unlocked ID 0");
        }

        if (GUI.Button(new Rect(x, y1, btnWidth, btnHeight), "Unlock ID 1"))
        {
            Unlock(1, MonsterLevelType.A);
            Debug.Log("Unlocked ID 1");
        }
    }

    /// <summary>
    /// id 순서대로 모든 AnimalStruct를 반환하는 메서드
    /// </summary>
    public List<AnimalStruct> GetAllEntries() => new List<AnimalStruct>(_allEntries).OrderBy(e => e.id).ToList();

    [System.Serializable]
    private class SaveData
    {
        public List<int> unlockedIds;
        public List<LevelEntry> highestLevels;

        [System.Serializable]
        public class LevelEntry
        {
            public int id;
            public MonsterLevelType level;
            public LevelEntry(int id, MonsterLevelType level)
            {
                this.id = id;
                this.level = level;
            }
        }

        public SaveData(HashSet<int> set, Dictionary<int, MonsterLevelType> levels)
        {
            unlockedIds = new List<int>(set);

            highestLevels = new List<LevelEntry>();
            foreach (var kvp in levels)
                highestLevels.Add(new LevelEntry(kvp.Key, kvp.Value));
        }
    }
}
