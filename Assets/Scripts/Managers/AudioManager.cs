//---------------------------------------------------------
// Breve descripción del contenido del archivo
// Responsable de la creación de este archivo
// Nombre del juego
// Proyectos 1 - Curso 2024-25
//---------------------------------------------------------

using System.Collections.Generic;
using UnityEngine;
    using UnityEngine.Audio;
// Añadir aquí el resto de directivas using


/// <summary>
/// Antes de cada class, descripción de qué es y para qué sirve,
/// usando todas las líneas que sean necesarias.
/// </summary>
    public class AudioManager : MonoBehaviour
    {
        // ---- ATRIBUTOS DEL INSPECTOR ----
        #region Atributos del Inspector (serialized fields)
        // Documentar cada atributo que aparece aquí.
        // El convenio de nombres de Unity recomienda que los atributos
        // públicos y de inspector se nombren en formato PascalCase
        // (palabras con primera letra mayúscula, incluida la primera letra)
        // Ejemplo: MaxHealthPoints
        public static AudioManager instance;
        [SerializeField] private AudioMixer audioMixer;
        [SerializeField] private AudioClip[] sfxClips; //Se crea un array para introducir los sonidos
   

        #endregion

        // ---- ATRIBUTOS PRIVADOS ----
        #region Atributos Privados (private fields)
        // Documentar cada atributo que aparece aquí.
        // El convenio de nombres de Unity recomienda que los atributos
        // privados se nombren en formato _camelCase (comienza con _, 
        // primera palabra en minúsculas y el resto con la 
        // primera letra en mayúsculas)
        // Ejemplo: _maxHealthPoints
        [SerializeField] public AudioSource _sfxSource;   //Se crea un audiosource que es el que hace que el sonido suene (valga la redundancia)
        private float timer; //Timer para que cuando acaba el audio empieze otra vez sin solapar. para esperar a que acabe de reproducir un audio anter de empezar otro
       [Range(0f, 1f)]
       [SerializeField] private float sfxVolume = 1f;
    #endregion
  
    // ---- MÉTODOS DE MONOBEHAVIOUR ----
    #region Métodos de MonoBehaviour

    // Por defecto están los típicos (Update y Start) pero:
    // - Hay que añadir todos los que sean necesarios
    // - Hay que borrar los que no se usen 

    /// <summary>
    /// Start is called on the frame when a script is enabled just before 
    /// any of the Update methods are called the first time.
    /// </summary>
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
            }    //Se coje el componente audiosource

        }

        /// <summary>
        /// Update is called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        void Update()
        {
            timer+=Time.deltaTime;                                         //Un timer par que no se solapen los audios junto a la condicion de si suena o no
            PlayLoopingSFX(0);
        }
        #endregion

        // ---- MÉTODOS PÚBLICOS ----
        #region Métodos públicos
        // Documentar cada método que aparece aquí con ///<summary>
        // El convenio de nombres de Unity recomienda que estos métodos
        // se nombren en formato PascalCase (palabras con primera letra
        // mayúscula, incluida la primera letra)
        // Ejemplo: GetPlayerController

        public void PlaySFX(int index)
        {
            if (index >= 0 && index < sfxClips.Length)
            {
                // Crear un nuevo AudioSource para cada sonido
                AudioSource newSource = gameObject.AddComponent<AudioSource>();
                newSource.clip = sfxClips[index];
                newSource.outputAudioMixerGroup = _sfxSource.outputAudioMixerGroup;
                newSource.volume = sfxVolume;
                newSource.Play();

                // Destruir el AudioSource después de que termine el sonido
                Destroy(newSource, newSource.clip.length);
            }
        }

        public void StopSFX(int index)
        {
            if (index == 1000) // Si el índice es 1000, detener todos los SFX
            {
                foreach (AudioSource source in GetComponents<AudioSource>())
                {
                
                        Destroy(source);
                
                }
            }
            else
            {
                // Detener solo un sonido específico
                foreach (AudioSource source in GetComponents<AudioSource>())
                {
                    if (source.clip == sfxClips[index])
                    {
                        source.Stop();
                        Destroy(source);
                    }
                }
            }
        }
    private Dictionary<int, AudioSource> loopingSources = new Dictionary<int, AudioSource>();

    public void PlayLoopingSFX(int index)
    {
        if (index >= 0 && index < sfxClips.Length)
        {
            // Comprobar si ya hay un sonido en bucle con este índice
            if (loopingSources.ContainsKey(index))
            {
                return; // No hacer nada si ya se está reproduciendo
            }

            // Crear un nuevo AudioSource y configurarlo
            AudioSource newSource = gameObject.AddComponent<AudioSource>();
            newSource.clip = sfxClips[index];
            newSource.loop = true;
            newSource.volume = sfxVolume;
            newSource.outputAudioMixerGroup = _sfxSource.outputAudioMixerGroup;
            newSource.Play();

            // Guardar en el diccionario
            loopingSources[index] = newSource;
        }
    }

    public void StopLoopingSFX(int index)
    {
        if (loopingSources.ContainsKey(index))
        {
            AudioSource source = loopingSources[index];

            if (source != null)
            {
                source.Stop();
                Destroy(source);
            }

            loopingSources.Remove(index);
        }
    }

    public void SetSFXVolume(float volume)
       {
          sfxVolume = volume;
          audioMixer.SetFloat("SFXVolume", Mathf.Log10(volume) * 20); // Convertir a dB
        }
    #endregion

    // ---- MÉTODOS PRIVADOS ----
    #region Métodos Privados
    // Documentar cada método que aparece aquí
    // El convenio de nombres de Unity recomienda que estos métodos
    // se nombren en formato PascalCase (palabras con primera letra
    // mayúscula, incluida la primera letra)


} // class AudioManager 
      // namespace
#endregion
