using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    public enum UIType
    {
        None,
        Map,
        Book,
        MiniGame
    }

    public UIType CurrentUI { get; private set; } = UIType.None;

    public bool TryOpen(UIType type)
    {
        if (CurrentUI != UIType.None && CurrentUI != type)
            return false;
        
        CurrentUI = type;
        Debug.Log($"Open : {CurrentUI.ToString()}");
        return true;
    }

    public void Close(UIType type)
    {
        if (CurrentUI == type)
        {
            Debug.Log($"Close : {CurrentUI.ToString()}");
            CurrentUI = UIType.None;
        }
    }
}
