using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;


[System.Serializable]
public struct Sound
{
    public string soundName;
    public AudioClip audioClip;
}

public enum SFXTYPE { 
    PROJECTILESOUND,
    UISOUND
}

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance = null;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }


    [SerializeField] private AudioSource backgroundAudioSource;
    [SerializeField] private AudioSource[] sfxAudioSource;
    [SerializeField] private List<Sound> musicSoundList = new List<Sound>();
    [SerializeField] private List<Sound> projectileSfxSoundList = new List<Sound>();
    [SerializeField] private List<Sound> uiSfxSoundList = new List<Sound>();


    public void StopAllMusicSound()
    {
        backgroundAudioSource.Stop();
        backgroundAudioSource.resource = null;
        backgroundAudioSource.loop = false;
    }

    public void StopAllSfxSound()
    {
        for (int i = 0; i < sfxAudioSource.Length; i++)
        {
            if (sfxAudioSource[i].isPlaying)
            {
                sfxAudioSource[i].Stop();
                sfxAudioSource[i].resource = null;
                sfxAudioSource[i].loop = false;
            }
        }
    }

    public void PlaySfxSound(string sfxName, SFXTYPE sfxType, bool loop = false)
    {
        List<Sound> soundList = null;

        switch (sfxType)
        {
            case SFXTYPE.PROJECTILESOUND:
                soundList = projectileSfxSoundList;
                break;
            case SFXTYPE.UISOUND:
                soundList = uiSfxSoundList;
                break;  
        }

        for (int i = 0; i < soundList.Count; i++)
        {
            if (sfxName == soundList[i].soundName)
            {
                for (int j = 0; j < sfxAudioSource.Length; j++)
                {
                    if (!sfxAudioSource[j].isPlaying)
                    {
                        sfxAudioSource[j].PlayOneShot(soundList[i].audioClip);
                        return;
                    }
                }
            }
        }

        Debug.LogError("등록 되지않은 사운드 효과");
    }


    public void PlayMusicSound(string soundName,bool loop = false)
    {
        for (int i = 0; i < musicSoundList.Count; i++)
        {
            if (musicSoundList[i].soundName == soundName)
            {
                backgroundAudioSource.Stop();
                backgroundAudioSource.resource = musicSoundList[i].audioClip;
                backgroundAudioSource.loop = loop;
                backgroundAudioSource.Play(); 
            }
        }
    }


    public void SetBackgroundVolume(float volume)
    {
        backgroundAudioSource.volume = volume;
    }

    public void SetSfxVolume(float volume)
    {
        for (int i = 0; i < sfxAudioSource.Length; i++)
        {
            sfxAudioSource[i].volume = volume;
        }
    }

}
