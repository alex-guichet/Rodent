using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum AudioName
{
    SoundTrack,
    Rock_Slide,
    Cat_Meow,
    Eat_Cheese,
    Failure,
    Victory
}

public sealed class SoundManager : MonoBehaviour
{
    public static SoundManager _instance;
    [SerializeField] List<AudioSourceWithName> _audioSourceList;

    private void Awake()
    {
        if (_instance != null)
        {
            Debug.LogError("More than one SoundManager instance !");
            return;
        }
        _instance = this;
    }

    public void PlayAudio(AudioName _name)
    {
        int index = _audioSourceList.FindIndex(audio => audio._name == _name);
        _audioSourceList[index]._audioSource.Play();
    }

    public void StopAudio(AudioName _name)
    {
        int index = _audioSourceList.FindIndex(audio => audio._name == _name);
        _audioSourceList[index]._audioSource.Stop();
    }

}

[System.Serializable]
public struct AudioSourceWithName
{
    public AudioSource _audioSource;
    public AudioName _name;
}
