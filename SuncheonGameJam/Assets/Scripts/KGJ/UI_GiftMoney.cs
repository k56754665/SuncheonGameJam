using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using System;

public class UI_GiftMoney : MonoBehaviour
{
    [SerializeField] private Image image;

    // 애니메이션 끝났을 때 호출되는 콜백
    public Action OnEffectComplete;

    public void PlayEffect(Vector3 uiTarget, float spreadRadius = 100f, float spreadTime = 0.3f, float waitTime = 0.1f, float flyTime = 0.8f)
    {
        RectTransform rt = GetComponent<RectTransform>();

        rt.localRotation = Quaternion.Euler(0, 0, UnityEngine.Random.Range(-25f, 25f));

        Vector2 dir = UnityEngine.Random.insideUnitCircle.normalized;
        Vector2 spreadPos = rt.anchoredPosition + dir * spreadRadius;

        Sequence seq = DOTween.Sequence();

        // 1. 퍼지기
        seq.Append(rt.DOAnchorPos(spreadPos, spreadTime).SetEase(Ease.OutQuad));
        // 2. 대기
        seq.AppendInterval(waitTime);

        // 3. 타겟으로 이동 (곡선 + 축소 + 페이드)
        seq.AppendCallback(() =>
        {
            Vector3 worldStart = rt.position;
            Vector3 worldTarget = uiTarget;
            Vector3 control = (worldStart + worldTarget) / 2f + Vector3.up * 200f;

            Vector3[] path = { worldStart, control, worldTarget };

            var pathTween = rt.DOPath(path, flyTime, PathType.CatmullRom).SetEase(Ease.InQuad).SetOptions(false);
            var scaleTween = rt.DOScale(Vector3.zero, flyTime).SetEase(Ease.InQuad);
            var fadeTween = image.DOFade(0f, flyTime).SetEase(Ease.InQuad);

            DOTween.Sequence()
                .Join(pathTween)
                .Join(scaleTween)
                .Join(fadeTween)
                .OnComplete(() =>
                {
                    OnEffectComplete?.Invoke(); // 콜백 실행
                    Destroy(gameObject);
                });
        });
    }
}