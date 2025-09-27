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

    [Header("Difficulty")]
    [Tooltip("현재 난이도 레벨(1=기본). SetLevel()로 변경 권장.")]
    [SerializeField] private float currentLevel = 1f;

    float halfRange;
    float seed;
    float x, v;

    // --- 원본(기본) 값 백업: SetLevel이 여러 번 호출되어도 기준은 항상 '기본값' ---
    float baseNoiseScale, baseSpeed, baseEdgeBias, baseMaxJerk, baseMaxSpeed;

    void Awake()
    {
        baseNoiseScale = noiseScale;
        baseSpeed      = speed;
        baseEdgeBias   = edgeBias;
        baseMaxJerk    = maxJerk;
        baseMaxSpeed   = maxSpeed;
    }

    void OnEnable()
    {
        seed = Random.value * 1000f;
        ComputeRange();
        x = 0f; v = 0f;

        // 재활성화 시에도 현재 레벨 반영(Inspector에서 바꿨을 수도 있으니)
        ApplyLevel(currentLevel);

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
        float desired    = n * halfRange * 0.95f; // 가장자리 여유 5%
        float centerPull = -x * edgeBias;

        // 가속 근사 및 제한
        float a  = (desired - x) * 8f + centerPull;
        float dt = Time.deltaTime;
        float dv = Mathf.Clamp(a * dt, -maxJerk * dt, maxJerk * dt);
        v = Mathf.Clamp(v + dv, -maxSpeed, maxSpeed);

        x = Mathf.Clamp(x + v * dt, -halfRange, halfRange);
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

    /// <summary>
    /// 난이도 설정(1 = 기본). 1보다 크면 어려움, 1보다 작으면 쉬움.
    /// 내부 여러 파라미터를 일관되게 스케일링합니다.
    /// </summary>
    public void SetLevel(float level)
    {
        currentLevel = Mathf.Max(0.1f, level); // 0 이하 방지
        ApplyLevel(currentLevel);
    }

    /// <summary>
    /// 실제 파라미터 스케일링 로직(기본값 대비 상대 스케일).
    /// - speed:       ~ level^1.0 (선형)
    /// - noiseScale:  ~ level^0.5 (완만)
    /// - maxJerk:     ~ level^1.2 (조금 공격적)
    /// - maxSpeed:    ~ level^1.1
    /// - edgeBias:    ~ level^-0.7 (레벨↑일수록 중앙 복원력 약화 → 난이도↑)
    /// </summary>
    void ApplyLevel(float level)
    {
        // 승수(지수)는 체감 난이도 튜닝 포인트
        float s_speed      = Mathf.Pow(level, 1.0f);
        float s_noiseScale = Mathf.Pow(level, 0.5f);
        float s_maxJerk    = Mathf.Pow(level, 1.2f);
        float s_maxSpeed   = Mathf.Pow(level, 1.1f);
        float s_edgeBias   = Mathf.Pow(level, -0.7f);

        speed      = baseSpeed      * s_speed;
        noiseScale = baseNoiseScale * s_noiseScale;
        maxJerk    = baseMaxJerk    * s_maxJerk;
        maxSpeed   = baseMaxSpeed   * s_maxSpeed;

        // edgeBias는 [0,1] 범위 권장 → 클램프
        edgeBias   = Mathf.Clamp01(baseEdgeBias * s_edgeBias);

        // 너무 과격한 값들에 대한 안전 클램프(선택적)
        speed      = Mathf.Max(0.05f, speed);
        noiseScale = Mathf.Max(0.05f, noiseScale);
        maxJerk    = Mathf.Max(50f,   maxJerk);
        maxSpeed   = Mathf.Max(100f,  maxSpeed);
    }
}