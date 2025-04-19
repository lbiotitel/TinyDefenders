using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public struct Sound
{
    public string soundName;
    public AudioClip audioClip;
}

public enum SFXTYPE
{
    PROJECTILE,
    UI,
    EXTRA
}

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance = null;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            InitAudioSources();
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }


    [SerializeField] private AudioSource backgroundAudioSource;
    [SerializeField] private AudioSource[] sfxAudioSource;
    [SerializeField] private List<Sound> backgroundSoundList = new List<Sound>();
    [SerializeField] private List<Sound> projectileSfxSoundList = new List<Sound>();
    [SerializeField] private List<Sound> uiSfxSoundList = new List<Sound>();
    [SerializeField] private List<Sound> extraSfxSoundList = new List<Sound>();
    [SerializeField] private float fadeOutTime;


    private void InitAudioSources()
    {
        AudioSource[] audioSources = GetComponents<AudioSource>();
        backgroundAudioSource = audioSources[0];

        sfxAudioSource = new AudioSource[audioSources.Length - 1];

        int sfxAudioSourceIndex = 0;
        for (int i = 0; i < audioSources.Length; i++)
        {
            if (audioSources[i] != backgroundAudioSource)
            {
                sfxAudioSource[sfxAudioSourceIndex++] = audioSources[i];
            }
        }

        LoadAudioClips();
    }

    private void LoadAudioClips()
    {
        LoadAudioClip(backgroundSoundList, Path.bgmSoundPath);
        LoadAudioClip(projectileSfxSoundList, Path.projectilSFXPath);
        LoadAudioClip(uiSfxSoundList, Path.uiSFXPath);
        LoadAudioClip(extraSfxSoundList, Path.extraSFXPath);
    }

    private void LoadAudioClip(List<Sound> audioClipList,string path)
    {
        AudioClip[] audioClips = Resources.LoadAll<AudioClip>(path);

        foreach (var audioClip in audioClips)
        {
            Sound sound = new Sound();
            sound.audioClip = audioClip;
            sound.soundName = audioClip.name;

            audioClipList.Add(sound);
        } 
    }

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
            case SFXTYPE.PROJECTILE:
                soundList = projectileSfxSoundList;
                break;
            case SFXTYPE.UI:
                soundList = uiSfxSoundList;
                break;
            case SFXTYPE.EXTRA:
                soundList = extraSfxSoundList;
                break;
        }

        bool find = false;

        for (int i = 0; i < soundList.Count; i++)
        {
            if (sfxName == soundList[i].soundName)
            {
                find = true;
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

        //if(!find)Debug.LogError("등록 되지않은 사운드 효과");
    }


    public void PlayMusicSound(string soundName, bool loop = false)
    {
        for (int i = 0; i < backgroundSoundList.Count; i++)
        {
            if (backgroundSoundList[i].soundName == soundName)
            {
                backgroundAudioSource.Stop();
                backgroundAudioSource.resource = backgroundSoundList[i].audioClip;
                backgroundAudioSource.loop = loop;
                backgroundAudioSource.Play();
            }
        }
    }

    public void FadeOutSound()
    {
        StartCoroutine(FadeOutSoundCoroutine());
    }

    private IEnumerator FadeOutSoundCoroutine()
    {
        float prevBackgroundVolume = backgroundAudioSource.volume;
        float[] prevSfxVolume = new float[sfxAudioSource.Length];

        for(int i = 0;  i < prevSfxVolume.Length; i++) prevSfxVolume[i] = sfxAudioSource[i].volume;

        float elapsedTime = 0;
        float percent = 0;

        while(elapsedTime < 1f)
        {
            elapsedTime += Time.deltaTime;
            percent = elapsedTime / fadeOutTime;

            backgroundAudioSource.volume = Mathf.Lerp(backgroundAudioSource.volume, 0, percent);
            backgroundAudioSource.pitch = Mathf.Lerp(backgroundAudioSource.pitch, 0, percent);
            for (int i = 0; i < sfxAudioSource.Length; i++) sfxAudioSource[i].volume = Mathf.Lerp(sfxAudioSource[i].volume,0, percent);
            for (int i = 0; i < sfxAudioSource.Length; i++) sfxAudioSource[i].pitch = Mathf.Lerp(sfxAudioSource[i].pitch, 0, percent);

            yield return null;
        }

        StopAllMusicSound();
        StopAllSfxSound();

        backgroundAudioSource.volume = prevBackgroundVolume;
        backgroundAudioSource.pitch = 1f;
        for (int i = 0; i < prevSfxVolume.Length; i++)
        {
            sfxAudioSource[i].volume = prevSfxVolume[i];
            sfxAudioSource[i].pitch = 1f;
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
