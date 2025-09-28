using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class MiniGameManager : Singleton<MiniGameManager>
{
    [FormerlySerializedAs("fishingUIRoot")]
    [Header("Fishing UI Root")]
    [SerializeField] private GameObject MiniGameCanvas;
    [SerializeField] private GameObject SuccessCanvas;
    [SerializeField] private GameObject FailCanvas;

    [Header("Fishing Components")]
    private InertiaHandleUI handleCtrl;
    private TargetNoiseMoverUI targetCtrl;
    private FishingUIController fishingCtrl;
    private MiniGameResult resultCtrl;
    private AnimalPoolData animalPoolData;
    private CharacterControl characterControl;
    
    public AnimalPoolData AnimalPoolData => animalPoolData;
    [SerializeField] private Image targetImage;
    
    [SerializeField] private AnimalStruct currentAnimal;
    
    
    protected override void Awake()
    {
        base.Awake();
        handleCtrl = FindAnyObjectByType<InertiaHandleUI>();
        targetCtrl = FindAnyObjectByType<TargetNoiseMoverUI>();
        fishingCtrl = FindAnyObjectByType<FishingUIController>();
        resultCtrl = FindAnyObjectByType<MiniGameResult>();
        animalPoolData = FindAnyObjectByType<AnimalPoolData>();
        
    }

    private void OnEnable()
    {
        EventBus.SubscribeEndMiniGame(OnEndMiniGame);
    }

    private void OnDisable()
    {
        EventBus.UnsubscribeEndMiniGame(OnEndMiniGame);
    }

    private void Start()
    {
        if (MiniGameCanvas) MiniGameCanvas.SetActive(false);
        if (handleCtrl) handleCtrl.enabled = false;
        if (targetCtrl) targetCtrl.enabled = false;
        if (fishingCtrl) fishingCtrl.enabled = false;
        if (SuccessCanvas) SuccessCanvas.SetActive(false);
        if (FailCanvas) FailCanvas.SetActive(false);
    }

    public void OnStartMiniGame()
    {
        AnimalStruct animal = animalPoolData.PickAnimal();
        if (!MiniGameCanvas) return;
        MiniGameCanvas.SetActive(true);

        if (handleCtrl) handleCtrl.enabled = true;
        if (targetCtrl)
        {
            targetCtrl.enabled = true;
            float level = 1f;
            if (animal != null)
                level = CalLevel(animal.difficulty, animal.monsterLevel);
            targetCtrl.SetLevel(level);
        }

        if (targetImage && animal != null)
            targetImage.sprite = animal.animalImage;
        if (fishingCtrl)
        {
            fishingCtrl.enabled = true;
            fishingCtrl.Begin(); // 진행도/타이머 초기화 (FishingUIController에 이미 구현됨)
        }
        characterControl = FindAnyObjectByType<CharacterControl>();
        characterControl.CanControl = false;
        SoundManager.Instance.StopBGM();
        EventBus.PublishStartMiniGame(animal);
    }

    public void OnEndMiniGame(AnimalStruct animal, bool success)
    {
        // TODO: 성공/실패 보상 처리

        if (MiniGameCanvas) MiniGameCanvas.SetActive(false);

        if (handleCtrl) handleCtrl.enabled = false;
        if (targetCtrl) targetCtrl.enabled = false;
        if (fishingCtrl) fishingCtrl.enabled = false;
        if (success)
        {
            if (SuccessCanvas) SuccessCanvas.SetActive(true);
            if (FailCanvas) FailCanvas.SetActive(false);
            if (resultCtrl) resultCtrl.SetSuccess(animal);
            MoneyManager.Instance.AddMoney(animal.Bounties[(int)animal.monsterLevel]);
            BookManager.Instance.Unlock(animal.id, animal.monsterLevel);
            
        }
        else
        {
            if (SuccessCanvas) SuccessCanvas.SetActive(false);
            if (FailCanvas) FailCanvas.SetActive(true);
        }
        if (SceneManager.GetActiveScene().name == "ReedMap")
        {
            SoundManager.Instance.PlayBGM(SoundType.BGM_Reed, true);
        }else
        {
            SoundManager.Instance.PlayBGM(SoundType.BGM_Sea, true);
        }
        
    }
    
    private float CalLevel(float difficulty, MonsterLevelType monsterLevel)
    {
        int level = (int)monsterLevel;
        return difficulty + (level - 1) * 0.2f;
    }
    
    
    public void OffResultCanvas()
    {
        if (SuccessCanvas) SuccessCanvas.SetActive(false);
        if (FailCanvas) FailCanvas.SetActive(false);
        characterControl = FindAnyObjectByType<CharacterControl>();
        characterControl.CanControl = true;
    }
}