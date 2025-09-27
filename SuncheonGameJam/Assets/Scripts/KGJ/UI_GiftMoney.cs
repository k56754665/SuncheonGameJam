using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class UI_GiftMoney : MonoBehaviour
{
    [SerializeField] private Image image; // 돈 이미지 (필수: CanvasRenderer 있어야 DOFade 가능)

    public void PlayEffect(Vector3 uiTarget, float spreadRadius = 100f, float spreadTime = 0.3f, float waitTime = 0.1f, float flyTime = 0.8f)
    {
        RectTransform rt = GetComponent<RectTransform>();

        // --- 생성 시 살짝 랜덤 회전 ---
        rt.localRotation = Quaternion.Euler(0, 0, Random.Range(-25f, 25f));

        // --- 퍼질 위치 계산 ---
        Vector2 dir = Random.insideUnitCircle.normalized;
        Vector2 spreadPos = rt.anchoredPosition + dir * spreadRadius;

        Sequence seq = DOTween.Sequence();

        // 1. 중앙 → 퍼진 위치
        seq.Append(rt.DOAnchorPos(spreadPos, spreadTime).SetEase(Ease.OutQuad));

        // 2. 퍼진 자리에서 잠시 대기
        seq.AppendInterval(waitTime);

        // 3. 퍼진 자리에서 UI 위치로 곡선 이동 + 축소 + 페이드아웃
        seq.AppendCallback(() =>
        {
            Vector3 worldStart = rt.position;    // 퍼진 후 실제 위치
            Vector3 worldTarget = uiTarget;
            Vector3 control = (worldStart + worldTarget) / 2f + Vector3.up * 200f;

            Vector3[] path = { worldStart, control, worldTarget };

            // 경로 이동
            var pathTween = rt.DOPath(path, flyTime, PathType.CatmullRom)
                .SetEase(Ease.InQuad)
                .SetOptions(false);

            // 크기 축소
            var scaleTween = rt.DOScale(Vector3.zero, flyTime)
                .SetEase(Ease.InQuad);

            // 알파 페이드아웃 (Image 필요)
            var fadeTween = image.DOFade(0f, flyTime)
                .SetEase(Ease.InQuad);

            DOTween.Sequence()
                .Join(pathTween)
                .Join(scaleTween)
                .Join(fadeTween)
                .OnComplete(() => Destroy(gameObject));
        });
    }
}