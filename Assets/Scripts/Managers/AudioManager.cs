using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public enum SFXType
{
    Breath,
    Walk,
    Jump,
    Fall,
    GameOver
    // Añadir más sonidos aquí según sea necesario
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private AudioClip[] sfxClips;
    [SerializeField] private AudioSource _sfxSource;

    [Range(0f, 1f)]
    [SerializeField] private float sfxVolume = 1f;

    private List<AudioSource> loopingSources = new List<AudioSource>();
    private List<SFXType> activeLoopingSFX = new List<SFXType>();

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    public void PlaySFX(SFXType type, bool loop = false)
    {
        int index = (int)type;
        if (index >= 0 && index < sfxClips.Length)
        {
            if (loop)
            {
                PlayLoopingSFX(type);
                return;
            }

            AudioSource newSource = gameObject.AddComponent<AudioSource>();
            newSource.clip = sfxClips[index];
            newSource.outputAudioMixerGroup = _sfxSource.outputAudioMixerGroup;
            newSource.volume = sfxVolume;
            newSource.Play();
        }
    }

    public void StopSFX(SFXType type)
    {
        int index = (int)type;
        foreach (AudioSource source in GetComponents<AudioSource>())
        {
            if (source.clip == sfxClips[index])
            {
                source.Stop();
            }
        }
    }

    private void PlayLoopingSFX(SFXType type)
    {
        if (activeLoopingSFX.Contains(type)) return; // Si ya está activo, no lo reproduce de nuevo

        int index = (int)type;
        if (index >= 0 && index < sfxClips.Length)
        {
            AudioSource newSource = gameObject.AddComponent<AudioSource>();
            newSource.clip = sfxClips[index];
            newSource.loop = true;
            newSource.volume = sfxVolume;
            newSource.outputAudioMixerGroup = _sfxSource.outputAudioMixerGroup;
            newSource.Play();

            loopingSources.Add(newSource);
            activeLoopingSFX.Add(type);
        }
    }

    public void StopLoopingSFX(SFXType type)
    {
        int index = (int)type;
        for (int i = loopingSources.Count - 1; i >= 0; i--)
        {
            if (loopingSources[i].clip == sfxClips[index])
            {
                loopingSources[i].Stop();
                loopingSources.RemoveAt(i);
                activeLoopingSFX.Remove(type);
                break;
            }
        }
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = volume;
        audioMixer.SetFloat("SFXVolume", Mathf.Log10(volume) * 20);
    }
}
