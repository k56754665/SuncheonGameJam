using TMPro;
using UnityEngine;
using DG.Tweening;

public class MoneyPopupText : MonoBehaviour
{
    private TMP_Text _moneyText;
    private float _moveUpDistance = 50f;
    private float _duration = 1.5f;

    private Tween _moveTween;
    private Tween _fadeTween;

    private void Awake()
    {
        _moneyText = GetComponentInChildren<TMP_Text>();
        _moneyText.alpha = 0f; // 초기에는 안 보이게
    }

    /// <summary>
    /// 돈이 들어왔을 때 뜨는 효과를 실행합니다.
    /// </summary>
    public void ShowPopup(float amount)
    {
        if (_moneyText == null) return;

        // 이전 트윈 정리
        _moveTween?.Kill();
        _fadeTween?.Kill();

        // 텍스트 설정
        _moneyText.text = $"+{amount:N0}";
        _moneyText.alpha = 1f;

        RectTransform rect = _moneyText.rectTransform;

        // 항상 원래 위치에서 시작
        rect.localPosition = Vector3.zero;

        // 목표 위치
        Vector3 endPos = Vector3.up * _moveUpDistance;

        // 새 트윈 실행
        _moveTween = rect.DOLocalMove(endPos, _duration).SetEase(Ease.OutCubic);
        _fadeTween = _moneyText.DOFade(0f, _duration);
    }
}