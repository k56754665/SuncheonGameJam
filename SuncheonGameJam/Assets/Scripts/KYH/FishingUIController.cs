using UnityEngine;
using UnityEngine.UI;

/// Handle(플레이어 바)과 Target(물고기)의 가로폭 겹침을 판정하고 게이지를 증감.
public class FishingUIController : MonoBehaviour
{
    public RectTransform handle;     // 플레이어 바
    public RectTransform target;     // 물고기(자율 이동)
    public Image progressFill;       // 진행도 표시(선택)

    [Header("Tuning")]
    [Tooltip("겹칠 때 초당 증가율.")]
    public float gainPerSec = 0.35f;
    [Tooltip("안 겹칠 때 초당 감소율.")]
    public float decayPerSec = 0.25f;
    [Tooltip("겹침 판정에 여유 폭(px).")]
    public float extraPadding = 0f;

    float progress;

    void OnEnable()
    {
        progress = 0f;
        UpdateUI();
    }

    void Update()
    {
        if (!handle || !target) return;

        // 같은 부모 + Pivot(0.5, 0.5) 전제
        float hx = handle.anchoredPosition.x;
        float tx = target.anchoredPosition.x;

        float halfH = handle.rect.width * 0.5f;
        float halfT = target.rect.width * 0.5f;

        // 1D 구간 겹침: |Δx| <= halfH + halfT + padding
        bool overlap = Mathf.Abs(tx - hx) <= (halfH + halfT + extraPadding);

        float rate = overlap ? gainPerSec : -decayPerSec;
        progress = Mathf.Clamp01(progress + rate * Time.deltaTime);

        UpdateUI();

        // 예시: 성공/실패 트리거
        // if (Mathf.Approximately(progress, 1f)) OnSuccess();
        // if (progress <= 0.001f) OnFail();
    }

    void UpdateUI()
    {
        if (progressFill) progressFill.fillAmount = progress;
    }
}