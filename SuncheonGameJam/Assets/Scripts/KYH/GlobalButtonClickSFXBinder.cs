using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GlobalButtonClickSFXBinder : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("연결할 클릭 사운드 타입")]
    public SoundType clickSound = SoundType.Button_Click;

    [Tooltip("새 버튼을 감지해 바인딩하는 주기(초). 0이면 매 프레임 체크")]
    [Min(0f)] public float rescanInterval = 0.5f;

    [Tooltip("특정 캔버스 아래 버튼만 바인딩하고 싶다면 지정(비워두면 씬 전체)")]
    public Canvas rootCanvasFilter;

    // 이미 바인딩된 버튼(중복 방지)
    private readonly HashSet<Button> _bound = new HashSet<Button>();
    private float _timer;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Start()
    {
        BindAllInScene();
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        _bound.Clear();   // 씬 전환 시 초기화
        BindAllInScene(); // 새 씬 버튼 일괄 바인딩
    }

    void Update()
    {
        _timer += Time.unscaledDeltaTime;
        if (rescanInterval <= 0f || _timer >= rescanInterval)
        {
            _timer = 0f;
            BindAllInScene(); // 동적으로 생긴 버튼도 커버
        }
    }

    void BindAllInScene()
    {
        // Unity 6.2 (신 API): 비활성 포함 검색
        var buttons = FindObjectsByType<Button>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        foreach (var btn in buttons)
        {
            if (btn == null) continue;
            if (_bound.Contains(btn)) continue;

            // 특정 캔버스 필터가 있으면 그 하위만 바인딩
            if (rootCanvasFilter != null)
            {
                var canvas = btn.GetComponentInParent<Canvas>(includeInactive: true);
                if (canvas == null || canvas.rootCanvas != rootCanvasFilter.rootCanvas)
                    continue;
            }

            // 한 번만 붙이기
            btn.onClick.AddListener(() =>
            {
                if (btn != null && btn.interactable && SoundManager.Instance != null)
                {
                    SoundManager.Instance.Play(clickSound);
                }
            });

            _bound.Add(btn);
        }
    }
}
