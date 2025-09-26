using UnityEngine;

/// 노이즈로 좌우 이동하는 Target(물고기).
/// Track 중앙 기준 1D(anchoredPosition.x)에서만 움직입니다.
public class TargetNoiseMoverUI : MonoBehaviour
{
    public RectTransform track;
    public RectTransform target;

    [Header("Noise Motion")]
    [Tooltip("시간 스케일(Perlin 입력).")]
    public float noiseScale = 0.6f;
    [Tooltip("진행 속도 배율.")]
    public float speed = 1.0f;
    [Tooltip("가장자리에서 중앙 쪽으로 끌어당기는 비율(0~1).")]
    public float edgeBias = 0.2f;
    [Tooltip("틱 당 속도 변화 제한(px/s).")]
    public float maxJerk = 1600f;
    [Tooltip("속도 상한(px/s).")]
    public float maxSpeed = 1200f;

    [Header("Range")]
    [Tooltip("0이면 자동 계산(트랙-타깃 너비 기준).")]
    public float halfRangeOverride = 0f;

    float halfRange;
    float seed;
    float x, v;

    void OnEnable()
    {
        seed = Random.value * 1000f;
        ComputeRange();
        x = 0f; v = 0f;
        Apply();
    }

    void Update()
    {
        if (!track || !target) return;
        if (halfRange <= 0f) ComputeRange();

        float t = Time.time * speed;
        // Perlin: 0~1 → -1~1
        float n = Mathf.PerlinNoise(seed, t * noiseScale) * 2f - 1f;

        // 목표(노이즈 기반) + 중앙 복원력
        float desired = n * halfRange * 0.95f; // 가장자리 여유 5%
        float centerPull = -x * edgeBias;

        // 가속 근사 및 제한
        float a = (desired - x) * 8f + centerPull;
        float dv = Mathf.Clamp(a * Time.deltaTime, -maxJerk * Time.deltaTime, maxJerk * Time.deltaTime);
        v = Mathf.Clamp(v + dv, -maxSpeed, maxSpeed);

        x = Mathf.Clamp(x + v * Time.deltaTime, -halfRange, halfRange);
        Apply();
    }

    void ComputeRange()
    {
        float trackW = track.rect.width;
        float tgtW   = target.rect.width;
        halfRange = (halfRangeOverride > 0f)
            ? halfRangeOverride
            : Mathf.Max(0f, 0.5f * (trackW - tgtW)) - 1f; // 1px 여유
    }

    void Apply()
    {
        var p = target.anchoredPosition;
        p.x = x;
        target.anchoredPosition = p;
    }
}