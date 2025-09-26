using System.Collections.Generic;
using UnityEngine;

public class BookManager : MonoBehaviour
{
    private const string RESOURCES_FOLDER = "AnimalSO";
    public static BookManager Instance { get; private set; }

    private List<AnimalStruct> _allEntries = new();
    private HashSet<int> _unlockedIds = new();
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
    
    /// <summary>
    /// id값을 가진 동물의 해금 여부를 반환하는 메서드
    /// </summary>
    public bool IsUnlocked(int id) => _unlockedIds.Contains(id);

    /// <summary>
    /// id값을 가진 동물을 해금시키는 메서드
    /// </summary>
    /// <param name="id"></param>
    public void Unlock(int id)
    {
        if (_unlockedIds.Add(id))
        {
            SaveProgress();
        }
    }

    /// <summary>
    /// 특정 Resources 폴더에서 모든 AnimalStruct SO 불러오기
    /// </summary>
    private void LoadAllData()
    {
        _allEntries.Clear();
        AnimalStruct[] loaded = Resources.LoadAll<AnimalStruct>(RESOURCES_FOLDER);

        foreach (AnimalStruct entry in loaded)
        {
            _allEntries.Add(entry);
        }
    }
    
    /// <summary>
    /// PlayerPrefs에 현재 해금 상황 저장
    /// </summary>
    private void SaveProgress()
    {
        SaveData data = new SaveData(_unlockedIds);
        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(SaveKey, json);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// 현재 PlayerPrefs의 해금 상황을 불러옴
    /// </summary>
    private void LoadProgress()
    {
        _unlockedIds.Clear();
        string json = PlayerPrefs.GetString(SaveKey, "");
        if (!string.IsNullOrEmpty(json))
        {
            SaveData data = JsonUtility.FromJson<SaveData>(json);
            foreach (var id in data.unlockedIds)
                _unlockedIds.Add(id);
        }
    }

    /// <summary>
    /// 해금 여부 상관 없이 모든 SO 데이터를 리스트로 반환함
    /// </summary>
    public List<AnimalStruct> GetAllEntries() => new List<AnimalStruct>(_allEntries);
    
    [System.Serializable]
    private class SaveData
    {
        public List<int> unlockedIds;
        public SaveData(HashSet<int> set) => unlockedIds = new List<int>(set);
    }
}
