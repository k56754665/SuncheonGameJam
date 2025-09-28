using UnityEngine;

public class TitleSoundControl : MonoBehaviour
{
     void Start()
    {
        if(SoundManager.Instance.BGMISPLAYING() == false)
        {
            SoundManager.Instance.PlayBGM(SoundType.TitleBGM, true);
        }else
        {
            if (SoundManager.Instance.WhatisBGMPlaying() == "Main_Title")
            {
                Debug.Log("타이틀 BGM 재생중");
                //아무것도 안함
            }
            else
            {
                Debug.Log("타이틀 BGM 재생할게");
                SoundManager.Instance.PlayBGM(SoundType.TitleBGM, true);
            }
        }
        SoundManager.Instance.StopSFX();
        SoundManager.Instance.StopSFXLoop();
            
    }
}
