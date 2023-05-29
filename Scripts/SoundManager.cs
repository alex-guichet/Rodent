using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;


public enum AudioName
{
    SoundTrack,
    RockSlide,
    CatMeow,
    EatCheese,
    Failure,
    Victory
}

public sealed class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;
    
    [SerializeField] List<AudioSourceWithName> audioSourceList;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("More than one SoundManager instance !");
            return;
        }
        Instance = this;
    }

    public void PlayAudio(AudioName name)
    {
        int index = audioSourceList.FindIndex(audio => audio.name == name);
        audioSourceList[index].audioSource.Play();
    }

    public void StopAudio(AudioName name)
    {
        int index = audioSourceList.FindIndex(audio => audio.name == name);
        audioSourceList[index].audioSource.Stop();
    }

}

[System.Serializable]
public struct AudioSourceWithName
{
    public AudioSource audioSource;
    public AudioName name;
}
