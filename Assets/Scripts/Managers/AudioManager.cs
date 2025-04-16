//---------------------------------------------------------
// AudioManager que controla los sonidos, los almacena, los reproduce y los para
// Tomás Arévalo Almagro
// Project Abyss
// Proyectos 1 - Curso 2024-25
//---------------------------------------------------------

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

// Enum que define los diferentes tipos de efectos de sonido disponibles en el juego
public enum SFXType
{
    Breath,
    Walk,
    Jump,
    Fall,
    GameOver,
    AttackEnemy1,
    PatrolEnemy1,
    FleeEnemy1,
    Sonar1,
    Sonar2,
    SonarAttack,
    FlashLight,
    MotorSound
    // Se pueden añadir más sonidos según sea necesario
}
/// <summary>
/// Clase heredara del MonoBehaviour que lleva la lógica del movimiento y estados del jugador
/// </summary>
public class AudioManager : MonoBehaviour
{
    // Instancia estática para implementar el patrón Singleton
    public static AudioManager instance;
    #region Atributos del Inspector (serialized fields)
    // Referencia al AudioMixer para controlar el volumen de los efectos de sonido
    [SerializeField] private AudioMixer audioMixer;

    // Lista de clips de sonido asociados a los tipos de SFX
    [SerializeField] private List<SFXClip> sfxClips;

    // Control del volumen general de los efectos de sonido, con un rango entre 0 y 1
    [Range(0f, 1f)][SerializeField] private float sfxVolume = 1f;

    // Estructura que asocia un tipo de sonido con su correspondiente clip de audio
    #endregion

    // ---- ATRIBUTOS PRIVADOS ----
    #region Atributos Privados (private fields)

    // Audiosource del audio manager para poder superponer la respiración con las demás acciones del personaje
    private AudioSource audiosource;
    #endregion
    [System.Serializable]
    public struct SFXClip
    {
        public SFXType type;  // Tipo de sonido
        public AudioClip clip; // Clip de audio asociado
    }
    // ---- MÉTODOS DE MONOBEHAVIOUR ----
    #region Métodos de MonoBehaviour
    /// <summary>
    /// Start is called on the frame when a script is enabled just before 
    /// any of the Update methods are called the first time.
    /// </summary>

    void Awake()
    {
        audiosource = GetComponent<AudioSource>();
        // Implementación del Singleton: Si no hay una instancia previa, esta se convierte en la única
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject); // Si ya existe una instancia, se destruye esta duplicada
            return;
        }
    }
    #endregion

    // ---- MÉTODOS PÚBLICOS ----
    #region Métodos públicos

    public float CalculateVolume(Vector3 targetPosition, int maxHearingDistance)
    {
        float distance = Vector3.Distance(targetPosition, transform.position);
        float volume = Mathf.Clamp01(1 - (distance / maxHearingDistance));  // Ajusta el divisor para que el volumen disminuya a la distancia que prefieras
        return volume;
    }

    // Método para reproducir un efecto de sonido en un AudioSource específico
    public void PlaySFX(SFXType type, AudioSource source, bool loop = false)
    {
        if (source == null) return; // Si el AudioSource es nulo, no se hace nada
        AudioClip clip = GetSFXClip(type); // Obtiene el clip de sonido correspondiente
        if (clip == null) return; // Si el clip no se encuentra, no se reproduce nada

        // Se establece que todo sea en bucle puesto que los sonidos que no solo se reproducen una vez por el PlayOneShoot
        source.loop = true;
        source.volume = sfxVolume; // Aplica el volumen general de los SFX
        source.outputAudioMixerGroup = audioMixer.FindMatchingGroups("SFX")[0]; // Asigna el grupo de mezcla del AudioMixer
        source.clip = clip; // Asigna el clip al AudioSource

        if (loop) //Dependiendo del tipo de sonido se reproduce de una forma u otra
        {
            source.clip = clip; // Asigna el clip al AudioSource 
            source.Play(); // Reproduce el sonido
        }
        else
            source.PlayOneShot(clip);
    }

    // Método para detener un sonido en reproducción
    public void StopSFX(AudioSource source)
    {
        if (source == null) return; // Si el AudioSource es nulo, no se hace nada

        source.Stop(); // Detiene la reproducción del sonido
        source.clip = null; // Elimina el clip asignado al AudioSource
    }

    // Método para ajustar el volumen de los efectos de sonido
    public void SetSFXVolume(float volume)
    {
        sfxVolume = volume; // Actualiza la variable del volumen general
        // Convierte el volumen a escala logarítmica y lo aplica al AudioMixer
        audioMixer.SetFloat("SFXVolume", Mathf.Log10(volume) * 20);
    }


    #endregion   

    // ---- MÉTODOS PRIVADOS ----
    #region Métodos Privados
    // Método privado para obtener el clip de sonido correspondiente a un tipo de SFX
    private AudioClip GetSFXClip(SFXType type)
    {
        foreach (var sfx in sfxClips)
        {
            if (sfx.type == type) // Si el tipo coincide, devuelve el clip asociado
                return sfx.clip;
        }
        return null; // Si no se encuentra, devuelve null
    }
    // Método privado para calcular el volumen en función de la distancia al jugador
    /*private float CalculateVolume(Vector3 playerPosition)
    {
        // Calcula la distancia entre el jugador y la fuente del sonido
        float distance = Vector3.Distance(playerPosition, transform.position);
        // Ajusta el volumen en base a la distancia, asegurando que nunca sea menor que 0
        float volume = Mathf.Clamp01(1 / (distance / 20));
        return volume;
    }*/

    
    #endregion
}
