using UnityEngine;

/// 입력을 힘으로 변환하여 1D 관성 이동(얼음길 느낌)으로 Handle을 좌우 이동.
/// Rigidbody 없이 반-암시적 오일러 통합.
/// 경계 관통 방지: 서브스텝 + 반사(restitution) + 경계 안쪽 재배치(epsilon).
public class InertiaHandleUI : MonoBehaviour
{
    public RectTransform track;
    public RectTransform handle;

    [Header("Input")]
    [Tooltip("체크 시 마우스 좌/우 버튼으로 제어, 해제 시 Horizontal 축 사용")]
    public bool useMouseButtons = true;
    [Tooltip("왼쪽 버튼을 오른쪽으로, 오른쪽 버튼을 왼쪽으로 바꾸고 싶을 때 체크")]
    public bool invertMouseMapping = false;

    [Header("Motion")]
    [Tooltip("질량: 클수록 같은 힘에 덜 가속.")]
    public float mass = 1.2f;
    [Tooltip("마찰/감쇠(초당 비율). 낮을수록 더 미끄러짐.")]
    public float damping = 0.75f;
    [Tooltip("입력(좌/우)을 힘으로 변환하는 계수.")]
    public float inputForce = 14f;
    [Tooltip("벽 스프링 강성(넘어서려 할 때 되돌림 경향).")]
    public float wallStiffness = 40f;
    [Tooltip("벽에 닿을 때 추가 감쇠 계수(0.5~1).")]
    [Range(0.5f, 1.0f)] public float wallDampingFactor = 0.9f;
    [Tooltip("속도 상한(px/s).")]
    public float maxSpeed = 1200f;

    [Header("Bounds")]
    [Tooltip("0이면 자동(트랙-핸들 너비).")]
    public float halfRangeOverride = 0f;

    [Header("Collision/Bounce")]
    [Tooltip("탄성 반사(1=완전탄성, 0=무반발).")]
    [Range(0f, 1f)] public float restitution = 0.8f;
    [Tooltip("충돌 시 경계 내부로 보정하는 여유(px).")]
    public float boundaryEpsilon = 0.5f;
    [Tooltip("벽 근처에서 서브스텝 최대 횟수.")]
    public int maxSubstepsNearWall = 4;

    // 상태
    float x, v, halfRange;

    // 고정 시뮬 스텝(안정화)
    const float simDt = 1f / 120f;
    float accumulator = 0f;

    void OnEnable()
    {
        ComputeRange();
        x = 0f; v = 0f;
        Apply();
    }

    void Update()
    {
        if (!track || !handle) return;
        if (halfRange <= 0f) ComputeRange();

        float axis = ReadAxis();
        accumulator += Time.deltaTime;

        while (accumulator >= simDt)
        {
            Step(axis, simDt);
            accumulator -= simDt;
        }

        Apply();
    }

    /// <summary>
    /// 입력을 -1..+1로 환산.
    /// useMouseButtons=true: 마우스 좌/우 버튼을 각각 -1/+1로 매핑(둘 다 누르면 0).
    /// useMouseButtons=false: 기존 Horizontal 축 사용.
    /// </summary>
    float ReadAxis()
    {
        if (useMouseButtons)
        {
            bool left  = Input.GetMouseButton(0); // LMB
            bool right = Input.GetMouseButton(1); // RMB

            if (left && right) return 0f;

            float axis = 0f;
            // 기본: 왼쪽=-1, 오른쪽=+1
            if (!invertMouseMapping)
            {
                if (left)  axis -= 1f;
                if (right) axis += 1f;
            }
            else
            {
                // 반전: 왼쪽=+1, 오른쪽=-1
                if (left)  axis += 1f;
                if (right) axis -= 1f;
            }
            return axis;
        }
        else
        {
            return Input.GetAxisRaw("Horizontal");
        }
    }

    void Step(float axis, float dt)
    {
        // 1) 입력 → 힘 → 가속
        float force = axis * inputForce;
        float a = force / Mathf.Max(0.0001f, mass);

        // 2) 마찰 감쇠(선형 근사)
        v *= Mathf.Clamp01(1f - damping * dt);

        // 3) 이번 프레임 변위가 경계까지 남은 거리보다 크면 서브스텝 증가
        int substeps = 1;
        float distR = halfRange - x;     // 오른쪽 경계까지
        float distL = x + halfRange;     // 왼쪽 경계까지 (양수로 해석)
        float remain = Mathf.Max(0f, Mathf.Min(Mathf.Abs(distR), Mathf.Abs(distL)));
        float disp   = Mathf.Abs(v * dt);
        if (disp > remain * 0.5f)
        {
            substeps = Mathf.Clamp(
                Mathf.CeilToInt(disp / Mathf.Max(1f, remain * 0.5f)),
                1, maxSubstepsNearWall
            );
        }

        float h = dt / substeps;
        for (int i = 0; i < substeps; i++)
        {
            // 4) 벽 스프링(넘어서려 할 때 되돌림 경향 + 벽 감쇠)
            if (x > halfRange)
            {
                a += -(x - halfRange) * wallStiffness / Mathf.Max(0.0001f, mass);
                v *= wallDampingFactor;
            }
            else if (x < -halfRange)
            {
                a += -(-halfRange - x) * wallStiffness / Mathf.Max(0.0001f, mass);
                v *= wallDampingFactor;
            }

            // 5) 반-암시적 오일러
            v = Mathf.Clamp(v + a * h, -maxSpeed, maxSpeed);
            float newX = x + v * h;

            // 6) 하드 충돌 처리: 반사 + 경계 안쪽으로 재배치(ε)
            if (newX > halfRange)
            {
                newX = halfRange - boundaryEpsilon;   // 살짝 안쪽
                v = -v * restitution * wallDampingFactor; // 반사 + 감쇠
            }
            else if (newX < -halfRange)
            {
                newX = -halfRange + boundaryEpsilon;
                v = -v * restitution * wallDampingFactor;
            }

            x = newX;
        }

        // 7) 최종 안전클램프(드문 수치 폭주 대비)
        x = Mathf.Clamp(x, -halfRange - 2f, halfRange + 2f);
    }

    void ComputeRange()
    {
        float trackW  = track.rect.width;
        float handleW = handle.rect.width;
        halfRange = (halfRangeOverride > 0f)
            ? halfRangeOverride
            : Mathf.Max(0f, 0.5f * (trackW - handleW)) - 1f; // 1px 여유
    }

    void Apply()
    {
        var p = handle.anchoredPosition;
        p.x = x;
        handle.anchoredPosition = p;
    }

    // 외부용 프로퍼티(선택)
    public float CurrentX => x;
    public float HalfRange => halfRange;
    public float CurrentV => v;
}