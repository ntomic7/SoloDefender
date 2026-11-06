using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using System.Collections.Generic;

public class SoundSystem: MonoBehaviour
{
    public static SoundSystem instance;

    [SerializeField] AudioMixer audioMixer;
    public AudioSource musicSource;

    float sfxVolume = 0.6f;
    float musicVolume = 0.097f;

    Music currentMusic = Music.Main;

    [SerializeField] List<AudioSource> audioSources;
    int audioIndex = 0;

    private void Awake()
    {
        if (instance == null) instance = this;


    }


    public void PlaySound(Sound sound)
    {
        //GameObject soundGameObject = new GameObject("Sound");
        //AudioSource audioSource = soundGameObject.AddComponent<AudioSource>();

        AudioSource audioSource = audioSources[audioIndex];
        audioSource.volume = sfxVolume;
        audioSource.PlayOneShot(GetSoundAudioClip(sound));

        audioIndex++;
        if (audioIndex >= 9) audioIndex = 0;
    }
    public void ChangeMusic(Music music)
    {
        musicSource.clip = GetMusicAudioClip(music);
        musicSource.Play();

        currentMusic = music;
    }

    AudioClip GetSoundAudioClip(Sound sound)
    {
        foreach(SoundAudioClip soundAudioClip in soundAudioClips)
        {
            if (soundAudioClip.sound == sound) return soundAudioClip.audioClip;
        }
        return null;
    }
    AudioClip GetMusicAudioClip(Music music)
    {
        foreach(MusicAudioClip musicAudioClip in musicAudioClips)
        {
            if(musicAudioClip.sound == music) return musicAudioClip.audioClip;
        }
        return null;
    }

    public Music GetCurrentBGMusic()
    {
        return currentMusic;
    }


    public void SetMusicVolume(float level)
    {
        float dB;
        musicVolume = level;

        if (level <= 0.001f) 
        {
            dB = -80f;
        }
        else
        {
            dB = Mathf.Log10(level) * 20f;
            dB = Mathf.Clamp(dB, -80f, 0f);
        }

        audioMixer.SetFloat("MusicVolume", dB);
    }
    public void SetSFXVolume(Slider slider)
    {
        sfxVolume = slider.value;
    }

    public float GetSFXVolume()
    {
        return sfxVolume;
    }
    
    public float GetMusicVolume()
    {
        return musicVolume;
    }

    public enum Sound
    {
        Attack,
        Spell,
        Potion,
        EnemyDeath,
        Death,
        Build,
        Purchase,
        Error,
        LevelUp,
        Win,
        Alarm
    }
    public enum Music
    {
        Main,
        Battle,
        Boss,
        Death
    }

    public SoundAudioClip[] soundAudioClips;
    public MusicAudioClip[] musicAudioClips;

    [Serializable]
    public class SoundAudioClip
    {
        public Sound sound;
        public AudioClip audioClip;
    }
    [Serializable]
    public class MusicAudioClip
    {
        public Music sound;
        public AudioClip audioClip;
    }
}
