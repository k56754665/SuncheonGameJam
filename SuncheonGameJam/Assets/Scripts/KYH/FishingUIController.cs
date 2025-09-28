using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

/// Handle(플레이어 바)과 Target(물고기)의 가로폭 겹침을 판정하고
/// 진행도를 관리하며, 성공/실패 시 EventBus.RaiseFishingEnd(bool) 발행.
public class FishingUIController : MonoBehaviour
{
    [Header("Refs")]
    public RectTransform handle;     // 플레이어 바
    public RectTransform target;     // 물고기(자율 이동)
    public Image progressFill;       // 진행도 표시(선택)
    public AnimalStruct currentAnimal; // 현재 낚시 대상

    [Header("Progress Tuning")]
    [Tooltip("겹칠 때 초당 증가율.")]
    public float gainPerSec = 0.15f;
    [Tooltip("안 겹칠 때 초당 감소율.")]
    public float decayPerSec = 0.2f;
    [Tooltip("겹침 판정에 여유 폭(px).")]
    public float extraPadding = 0f;
    [Tooltip("성공으로 간주할 진행도(0~1).")]
    [Range(0.1f, 1f)] public float successThreshold = 1f;

    [Header("Fail Conditions")]
    [Tooltip("타임아웃(초). 0이면 비활성화.")]
    public float timeLimitSeconds = 15f;
    [Tooltip("게이지가 바닥이면 실패로 볼지 여부.")]
    public bool failWhenDrained = true;

    // 내부 상태
    float progress;
    float elapsed;
    bool isRunning;

    // 🔹 추가: 겹침 전이 감지용
    bool _wasOverlap;

    /// <summary>
    /// 외부(매니저)에서 시작 호출: 타이머/게이지 초기화 후 러닝 시작.
    /// </summary>
    public void Begin()
    {
        progress = 0.3f;
        elapsed  = 0f;
        isRunning = true;
        _wasOverlap = false;
        StopCatchLoopSFX(); // 안전 초기화
        UpdateUI();
    }

    /// <summary>
    /// 외부(필요 시)에서 강제 종료. 이벤트는 발행하지 않음.
    /// </summary>
    public void StopImmediate()
    {
        isRunning = false;
        StopCatchLoopSFX();
    }
    
    private void SetAnimal(AnimalStruct animal)
    {
        currentAnimal = animal;
    }

    private void OnEnable()
    {
        // 에디터에서 활성화만으로도 안전하게 초기화 상태 보장
        if (!isRunning)
        {
            progress = 0f;
            elapsed  = 0f;
            _wasOverlap = false;
            UpdateUI();
            StopCatchLoopSFX();
        }
        EventBus.SubscribeStartMiniGame(SetAnimal);
    }
    
    private void OnDisable()
    {
        EventBus.UnsubscribeStartMiniGame(SetAnimal);
        StopCatchLoopSFX();
    }

    private void Update()
    {
        if (!isRunning) return;
        if (!handle || !target) return;

        elapsed += Time.deltaTime;

        // 같은 부모 + Pivot(0.5, 0.5) 전제
        float hx = handle.anchoredPosition.x;
        float tx = target.anchoredPosition.x;

        float halfH = handle.rect.width * 0.5f;
        float halfT = target.rect.width * 0.5f;

        // 1D 구간 겹침: |Δx| <= halfH + halfT + padding
        bool overlap = Mathf.Abs(tx - hx) <= (halfH + halfT + extraPadding);

        // 🔹 겹침 전이 감지 → 루프 SFX on/off
        if (overlap && !_wasOverlap)
        {
            StartCatchLoopSFX();
        }
        else if (!overlap && _wasOverlap)
        {
            StopCatchLoopSFX();
        }
        _wasOverlap = overlap;

        float rate = overlap ? gainPerSec : -decayPerSec;
        progress = Mathf.Clamp01(progress + rate * Time.deltaTime);

        UpdateUI();

        // 성공 판정
        if (progress >= successThreshold)
        {
            isRunning = false;
            StopCatchLoopSFX(); // 종료 시 정지
            EventBus.PublishEndMiniGame(currentAnimal,true);
            return;
        }

        // 실패 판정(타임아웃/게이지 바닥)
        if ((timeLimitSeconds > 0f && elapsed >= timeLimitSeconds) ||
            (failWhenDrained && progress <= 0.0001f))
        {
            isRunning = false;
            StopCatchLoopSFX(); // 종료 시 정지
            EventBus.PublishEndMiniGame(null,false);
            return;
        }
    }

    private void UpdateUI()
    {
        if (progressFill) progressFill.fillAmount = progress;
    }

    // ---- 루프 SFX 헬퍼 ----
    void StartCatchLoopSFX()
    {
        if (SoundManager.Instance != null)
            SoundManager.Instance.PlaySFXLoop(SoundType.CatchingAnimal);
    }

    void StopCatchLoopSFX()
    {
        if (SoundManager.Instance != null)
            SoundManager.Instance.StopSFXLoop();
    }
}
