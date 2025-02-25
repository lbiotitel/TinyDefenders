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


    [SerializeField] private AudioSource musicAudioSource;
    [SerializeField] private AudioSource[] sfxAudioSource;
    [SerializeField] private List<Sound> musicSoundList = new List<Sound>();
    [SerializeField] private List<Sound> sfxSoundList = new List<Sound>();


    public void StopAllMusicSound()
    {
        musicAudioSource.Stop();
        musicAudioSource.resource = null;
        musicAudioSource.loop = false;
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

    public void PlaySfxSound(string sfxName, bool loop = false)
    {
        for (int i = 0; i < sfxSoundList.Count; i++)
        {
            if (sfxName == sfxSoundList[i].soundName)
            {
                for (int j = 0; j < sfxAudioSource.Length; j++)
                {
                    if (!sfxAudioSource[j].isPlaying)
                    {
                        sfxAudioSource[j].resource = sfxSoundList[i].audioClip;
                        sfxAudioSource[j].loop = loop;
                        sfxAudioSource[j].Play();
                    }
                }
            }
        }
    }


    public void PlayMusicSound(string soundName,bool loop = false)
    {
        for (int i = 0; i < musicSoundList.Count; i++)
        {
            if (musicSoundList[i].soundName == soundName)
            {
                musicAudioSource.Stop();
                musicAudioSource.resource = musicSoundList[i].audioClip;
                musicAudioSource.loop = loop;
                musicAudioSource.Play(); 
            }
        }
    }

}
