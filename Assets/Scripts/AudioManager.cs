using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager _instance;

    public List<AudioSource> audioSources =new List<AudioSource>();

    //Temp volue
    float volume1 =1;
    float volume2 =1;
    // BK,SFX
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public void SetVolume(float volume)
    {
        audioSources[0].volume = volume1 * GameManager.instance.sfxVolume;
        audioSources[1].volume = volume2 * GameManager.instance.sfxVolume;
    }

    public void PlaySfx(string clipName, float volume = 1.0f)
    {
        AudioClip clip = Resources.Load<AudioClip>($"Audio/{clipName}");
        volume2 = volume;
        audioSources[1].volume = volume  * GameManager.instance.sfxVolume;
        audioSources[1].PlayOneShot(clip);
    }

    public void PlaySfx(AudioClip clip, float volume = 1.0f)
    {
        volume2 = volume;
        audioSources[1].volume = volume * GameManager.instance.sfxVolume;
        audioSources[1].PlayOneShot(clip);
    }


    public void PlayBKMusic(string clipName, float volume = 1.0f)
    {
        audioSources[0].clip = Resources.Load<AudioClip>($"Audio/{clipName}");

        volume1 = volume;
        audioSources[0].volume = volume * GameManager.instance.sfxVolume;
        audioSources[0].Play();
    }

    public void PlayBKMusic(AudioClip clip,float volume = 1.0f)
    {
        audioSources[0].clip = clip;
        volume1 = volume;
        audioSources[0].volume = volume * GameManager.instance.sfxVolume;
        audioSources[0].Play();
    }

}
