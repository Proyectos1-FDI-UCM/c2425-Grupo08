//---------------------------------------------------------
// Breve descripción del contenido del archivo
// Responsable de la creación de este archivo
// Nombre del juego
// Proyectos 1 - Curso 2024-25
//---------------------------------------------------------

using UnityEngine;
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
    [SerializeField] private AudioClip[] Audios; //Se crea un array para introducir los sonidos
    [SerializeField] private bool AudioOn;   //Se crea un booleano para encender y apagar el audio
    [SerializeField] private float Volume;   //Se Crea un volumen y un indice para regular el audio
    [SerializeField] private int AudioIndex;

    #endregion

    // ---- ATRIBUTOS PRIVADOS ----
    #region Atributos Privados (private fields)
    // Documentar cada atributo que aparece aquí.
    // El convenio de nombres de Unity recomienda que los atributos
    // privados se nombren en formato _camelCase (comienza con _, 
    // primera palabra en minúsculas y el resto con la 
    // primera letra en mayúsculas)
    // Ejemplo: _maxHealthPoints
    private AudioSource _audioSource;   //Se crea un audiosource que es el que hace que el sonido suene (valga la redundancia)
    private float timer; //Timer para que cuando acaba el audio empieze otra vez sin solapar. para esperar a que acabe de reproducir un audio anter de empezar otro

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
        _audioSource = GetComponent<AudioSource>();    //Se coje el componente audiosource
        timer = Audios[AudioIndex].length;         //Se inicia el timer con la longitud de la pista para pueda activarse desde el principio
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        timer+=Time.deltaTime;                                         //Un timer par que no se solapen los audios junto a la condicion de si suena o no
        if (AudioOn && Audios[AudioIndex].length <= timer)
        {
            Breath(AudioIndex, Volume);
            timer = 0f;
        }
        else if (!AudioOn)
            NoBreath();
    }
    #endregion

    // ---- MÉTODOS PÚBLICOS ----
    #region Métodos públicos
    // Documentar cada método que aparece aquí con ///<summary>
    // El convenio de nombres de Unity recomienda que estos métodos
    // se nombren en formato PascalCase (palabras con primera letra
    // mayúscula, incluida la primera letra)
    // Ejemplo: GetPlayerController

    #endregion

    // ---- MÉTODOS PRIVADOS ----
    #region Métodos Privados
    // Documentar cada método que aparece aquí
    // El convenio de nombres de Unity recomienda que estos métodos
    // se nombren en formato PascalCase (palabras con primera letra
    // mayúscula, incluida la primera letra)
    private void Breath(int index,float volume) 
    {
        _audioSource.PlayOneShot(Audios[index],volume);   //al audio source se le da un audio del array de audios y un volumen y este lo reproduce
    }
    private void NoBreath()
    {
        _audioSource.Stop();                                   //El audio para y el timer se pone de la longitud del audio para poder activar el audio
        timer = Audios[AudioIndex].length;                     //y que este no espere a que el audio que se a parado "termine de reproducirse"
    }
    #endregion   

} // class AudioManager 
// namespace
