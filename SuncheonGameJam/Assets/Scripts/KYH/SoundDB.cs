using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Audio/SoundDB")]
public class SoundDB : ScriptableObject {
    public List<SoundEntry> entries;
}

[System.Serializable]
public class SoundEntry {
    public SoundType type;
    public AudioClip clip;
}