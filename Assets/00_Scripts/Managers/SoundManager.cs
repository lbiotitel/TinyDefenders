using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


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


    [SerializeField] private AudioSource audioSource;
    [SerializeField] private List<Sound> soundList = new List<Sound>();


    public void StopAllSound()
    {
        audioSource.Stop();
    }

    public void PlaySound(string soundName,bool loop = false)
    {
        for (int i = 0; i < soundList.Count; i++)
        {
            if (soundList[i].soundName == soundName)
            {
                audioSource.Stop();
                audioSource.resource = soundList[i].audioClip;
                audioSource.loop = loop;
                audioSource.Play(); 
            }
        }
    }

}
