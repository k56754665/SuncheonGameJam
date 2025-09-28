using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterControl : MonoBehaviour
{
    CharacterController controller;
    Vector3 dir; 

    [Header("이동 설정")]
    public float speed = 5.0f;
    public float jumpPower = 7.5f;

    [Header("시점 설정")]
    public float rotationSpeed = 3.0f; // 마우스 감도
    public float verticalLookLimit = 80.0f; 
    private float rotationX = 0;
    private Camera mainCamera;

    public float walkingBobbingSpeed = 14f; // 걷기 속도에 따른 흔들림 빈도
    public float bobbingAmount = 0.05f;    // 흔들림의 최대 폭 (진폭)

    private float defaultPosY = 0;
    private float timer = 0;
    private EnvironmentLife targetPortal = null;
    Vector3 initPos;
    private int sceneIndex;//0 갈대, 1 바다

    public float digCool = 0.5f;
    private bool digCheck = false;
    
    private bool _canControl = true;
    public bool CanControl
    {
        get => _canControl;
        set
        {
            _canControl = value;
            if (_canControl)
            {
                // 입력 켜기
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else
            {
                // 입력 끄기
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                // 이동 멈추기
                dir = Vector3.zero;
            }
        }
    }

    void Start()
   {
        initPos = transform.position;
        controller = GetComponent<CharacterController>();
        
        // 씬에서 메인 카메라를 찾아 저장합니다.
        mainCamera = Camera.main; 

        // 🚨 시점 조작을 위해 마우스 커서를 숨기고 잠급니다.
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
   
        defaultPosY = mainCamera.transform.localPosition.y;

        CanControl = true;
        Debug.Log(SceneManager.GetActiveScene().name);
        if( SceneManager.GetActiveScene().name == "ReedMap")
        {
            sceneIndex = 0;
            SoundManager.Instance.PlayBGM(SoundType.BGM_Reed, true);
        }else if( SceneManager.GetActiveScene().name == "SeaMap")
        {
            sceneIndex = 1;
            SoundManager.Instance.PlayBGM(SoundType.BGM_Sea, true);
        }
   }
   void OnDestroy()
   {
        SoundManager.Instance.StopBGM();
   }

   
   void OnDisable()
   {
      // 🚨 게임이 끝날 때 마우스 커서를 해제합니다.
      Cursor.lockState = CursorLockMode.None;
      Cursor.visible = true;
   }
    private void OnTriggerEnter(Collider collider)
    {
        Debug.Log("캐릭터와 충돌 " + collider.gameObject.name);
        if(LayerMask.LayerToName(collider.gameObject.layer) == "Environment")
        {
            EnvironmentMove eMove = collider.transform.GetComponent<EnvironmentMove>();
            eMove.MoveStart();
        }
        if(LayerMask.LayerToName(collider.gameObject.layer) == "Portal")
        {
            targetPortal = collider.transform.GetComponent<EnvironmentLife>();
            targetPortal.fingerIcon.SetActive(true);
            Debug.Log("포탈 접촉");//잡은 포탈 있으면 거리체크
        }
        if(LayerMask.LayerToName(collider.gameObject.layer) == "Safe")
        {
            controller.enabled = false;
            transform.position = initPos + new Vector3(0,1,0); 
            Debug.Log(initPos);
            // 2. 🚨 누적된 중력 성분(dir.y)을 초기화하여 낙하를 방지
            dir.y = 0f; 
            controller.enabled = true;
            Debug.Log("제자리 이동 완료: ");
        }
        
    }
    IEnumerator DigCoolTime()
    {
        digCheck = true;
        yield return new WaitForSeconds(digCool);
        digCheck = false;
    }
    private void OnTriggerExit(Collider collider) {
        if(LayerMask.LayerToName(collider.gameObject.layer) == "Portal")
        {
            targetPortal.fingerIcon.SetActive(false);
            targetPortal = null;
            Debug.Log("포탈 해제");
        }
    }
   void Update()
   {
       if (!CanControl)
           return;
       
    // 1. 마우스 입력으로 캐릭터 회전 처리
    float mouseX = Input.GetAxis("Mouse X") * rotationSpeed;
    
    // 캐릭터 자체를 수평으로 회전시킵니다.
    transform.Rotate(Vector3.up * mouseX); 
    float mouseY = Input.GetAxis("Mouse Y") * rotationSpeed;
    rotationX -= mouseY; 
    rotationX = Mathf.Clamp(rotationX, -verticalLookLimit, verticalLookLimit);

    // 4. 회전 적용
    // 카메라의 로컬 X축(transform.localRotation)을 기준으로 회전시킵니다.
    mainCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);

    // 2. 중력 적용 (매 프레임)
    dir.y += Physics.gravity.y * Time.deltaTime; 

    // 3. 캐릭터가 지면에 있는 경우
    if (controller.isGrounded)
    {         
        var h = Input.GetAxis("Horizontal");
        var v = Input.GetAxis("Vertical");

        // 🚨 캐릭터의 방향이 아닌, 카메라의 방향을 기준으로 이동 벡터를 계산합니다.
        // 마우스로 캐릭터가 회전하므로, 카메라 회전과 캐릭터 회전을 일치시키는 것이 일반적입니다.

        // 현재 캐릭터의 앞(forward) 방향과 오른쪽(right) 방향을 사용합니다.
        Vector3 forward = transform.forward;
        Vector3 right = transform.right;

        // Y축 중력 성분을 제외한 순수 이동 방향을 계산합니다.
        Vector3 moveDirection = (forward * v) + (right * h);
        dir.x = moveDirection.x * speed;
        dir.z = moveDirection.z * speed;
         
         // 4. 점프 처리
        if (Input.GetKeyDown(KeyCode.Space))
            dir.y = jumpPower;
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if(digCheck == false)
            {
                StartCoroutine(DigCoolTime());
            }
            else
            {
                Debug.Log("딜레이 중");
                return;
            }
            if(targetPortal != null)
            {
                Debug.Log("공격");
                targetPortal.Damage();
                if(targetPortal.health <= 0)
                {
                     MiniGameManager.Instance.OnStartMiniGame();
                     targetPortal = null;
                }
                
            }
            else
            {
                Debug.Log("근처에 포탈이 없습니다");
            }
        }
      }
      
      // 5. 캐릭터 이동
      controller.Move(dir * Time.deltaTime);

      // 1. 캐릭터가 움직이고 있는지 확인 (예: 키보드 입력이 있을 때)
        if (Mathf.Abs(Input.GetAxis("Horizontal")) > 0.1f || Mathf.Abs(Input.GetAxis("Vertical")) > 0.1f)
        {
            
            if (!controller.isGrounded) return;
            if(SoundManager.Instance.SFXLOOPINGISPLAYING() == false)
            {
                if(sceneIndex == 0)
                {
                    SoundManager.Instance.PlaySFXLoop(SoundType.ReedWalk);
                }else
                {
                    SoundManager.Instance.PlaySFXLoop(SoundType.SeaWalk);
                }
               
            }
            // 2. 타이머를 증가시킵니다.
            timer += Time.deltaTime * walkingBobbingSpeed;

            // 3. 사인파를 이용하여 Y축 위치를 계산합니다.
            // Mathf.Sin() 함수는 주기적으로 -1과 1 사이의 값을 반환합니다.
            float newPosY = defaultPosY + Mathf.Sin(timer) * bobbingAmount;

            // 4. 카메라 위치를 업데이트합니다.
            mainCamera.transform.localPosition = new Vector3(
                mainCamera.transform.localPosition.x,
                newPosY,
                mainCamera.transform.localPosition.z);
        }
        else
        {
            if(SoundManager.Instance.SFXLOOPINGISPLAYING() == true)
            {
                SoundManager.Instance.StopSFXLoop();           
            }
            // 멈춰있을 때는 카메라를 기본 위치로 부드럽게 복귀시킵니다.
            timer = 0;
            mainCamera.transform.localPosition = Vector3.Lerp(mainCamera.transform.localPosition, 
                new Vector3(mainCamera.transform.localPosition.x, defaultPosY, mainCamera.transform.localPosition.z), 
                Time.deltaTime * walkingBobbingSpeed);
        }
   }
}