using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public enum SFXType
{
    Breath,
    Walk,
    Jump,
    Fall,
    GameOver,
    AttackEnemy1,
    PatrolEnemy1,
    FleeEnemy

    // Añadir más sonidos aquí según sea necesario
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private List<SFXClip> sfxClips;
    [Range(0f, 1f)] [SerializeField] private float sfxVolume = 1f;

    [System.Serializable]
    public struct SFXClip
    {
        public SFXType type;
        public AudioClip clip;
    }

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

    public void PlaySFX(SFXType type, AudioSource source, bool loop = false)
    {
        if (source == null) return;
        
        AudioClip clip = GetSFXClip(type);
        if (clip == null) return;
        
        source.clip = clip;
        source.loop = loop;
        source.volume = sfxVolume;
        source.outputAudioMixerGroup = audioMixer.FindMatchingGroups("SFX")[0];
        source.Play();
    }

    // Métodos para reproducir el sonido con la distancia al jugador
    public void PlaySFX(SFXType type, AudioSource source, Vector3 playerPosition, bool loop = false)
    {
        if (source == null) return;

        AudioClip clip = GetSFXClip(type);
        if (clip == null) return;

        source.clip = clip;
        source.loop = loop;
        source.volume = CalculateVolume(playerPosition);
        source.outputAudioMixerGroup = audioMixer.FindMatchingGroups("SFX")[0];
        source.Play();
    }

    public void StopSFX(AudioSource source)
    {
        if (source == null) return;
        
        source.Stop();
        source.clip = null;
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = volume;
        audioMixer.SetFloat("SFXVolume", Mathf.Log10(volume) * 20);
    }

    private AudioClip GetSFXClip(SFXType type)
    {
        foreach (var sfx in sfxClips)
        {
            if (sfx.type == type)
                return sfx.clip;
        }
        return null;
    }

    private float CalculateVolume(Vector3 playerPosition)
    {
        // Distancia máxima para oír el sonido es 20 unidades
        float distance = Vector3.Distance(playerPosition, transform.position);
        // La fórmula para el volumen puede variar, pero esta es un ejemplo
        float volume = Mathf.Clamp01(1 / (distance / 20));
        return volume;
    }
}


