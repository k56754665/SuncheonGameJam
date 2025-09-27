using System.Collections.Generic;
using UnityEngine;

public class BookManager : Singleton<BookManager>
{
    private const string RESOURCES_FOLDER = "AnimalSO";

    private List<AnimalStruct> _allEntries = new();
    private HashSet<string> _unlockedIds = new();
    private Dictionary<string, MonsterLevelType> _highestLevels = new();

    protected override void Awake()
    {
        base.Awake();
        LoadAllData();
    }

    public bool IsUnlocked(string id) => _unlockedIds.Contains(id);

    /// <summary>
    /// 미니게임 성공시 잡은 개체의 id와 등급을 넣어주면 해금됩니다.
    /// </summary>
    public void Unlock(string id, MonsterLevelType level)
    {
        _unlockedIds.Add(id);

        if (!_highestLevels.ContainsKey(id) || level > _highestLevels[id])
        {
            _highestLevels[id] = level;
        }
    }

    /// <summary>
    /// 해금된 개체의 id를 넣으면 최고 등급 기록을 반환합니다.
    /// </summary>
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

    [ContextMenu("Unlock All")]
    public void UnlockAll()
    {
        foreach (var entry in _allEntries)
        {
            // 최고 등급으로 해금
            Unlock(entry.id, MonsterLevelType.SSS);
        }

        Debug.Log($"전체 해금 완료: {_allEntries.Count}개");
    }
    
    public List<AnimalStruct> GetAllEntries() 
        => new List<AnimalStruct>(_allEntries);
}