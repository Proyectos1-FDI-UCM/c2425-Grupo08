//---------------------------------------------------------
// Gestiona el Menú de pausa del juego
// Vicente Rodriguez Casado
// Nombre del juego
// Proyectos 1 - Curso 2024-25
//---------------------------------------------------------

using UnityEngine;
// Añadir aquí el resto de directivas using


/// <summary>
/// Antes de cada class, descripción de qué es y para qué sirve,
/// usando todas las líneas que sean necesarias.
/// </summary>
public class PauseManager : MonoBehaviour
{
    // ---- ATRIBUTOS DEL INSPECTOR ----
    #region Atributos del Inspector (serialized fields)
    // Documentar cada atributo que aparece aquí.
    // El convenio de nombres de Unity recomienda que los atributos
    // públicos y de inspector se nombren en formato PascalCase
    // (palabras con primera letra mayúscula, incluida la primera letra)
    // Ejemplo: MaxHealthPoints

    [SerializeField] GameObject pauseMenuUI;

    #endregion

    // ---- ATRIBUTOS PRIVADOS ----
    #region Atributos Privados (private fields)
    // Documentar cada atributo que aparece aquí.
    // El convenio de nombres de Unity recomienda que los atributos
    // privados se nombren en formato _camelCase (comienza con _, 
    // primera palabra en minúsculas y el resto con la 
    // primera letra en mayúsculas)
    // Ejemplo: _maxHealthPoints

    public static bool GameIsPaused { get; private set; } = false;
    private AudioSource[] audioSources;
    private Animator[] animators;



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
    void Start()
    {

        // Guardar todos los audios y animators de la escena
        audioSources = FindObjectsOfType<AudioSource>();
        animators = FindObjectsOfType<Animator>();

        if (pauseMenuUI == null)

            Debug.LogWarning("No se ha asignado el menú de pausa en el inspector");

        else

            pauseMenuUI.SetActive(false);
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        if (InputManager.Instance.ReturnWasReleased() && pauseMenuUI != null) // Pausa el juego al pulsar ESCAPE
        {
            Debug.Log("Pausa");
            if (GameIsPaused)

            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    #endregion

    // ---- MÉTODOS PÚBLICOS ----
    #region Métodos públicos
    // Documentar cada método que aparece aquí con ///<summary>
    // El convenio de nombres de Unity recomienda que estos métodos
    // se nombren en formato PascalCase (palabras con primera letra
    // mayúscula, incluida la primera letra)
    // Ejemplo: GetPlayerController

    public void Resume()
    {
        Time.timeScale = 1f; // El juego continúa
        GameIsPaused = false;
        pauseMenuUI.SetActive(false);

        foreach (var audio in audioSources)
            audio.UnPause();

        foreach (var animator in animators)
            animator.speed = 1f;
    }

    void Pause()
    {

        GameIsPaused = true;

        Time.timeScale = 0f; // El juego se "congela"
        pauseMenuUI.SetActive(true); // Se activa el menú de pausa

        foreach (var audio in audioSources)
            audio.Pause();

        foreach (var animator in animators)
            animator.speed = 0f;
    }


    #endregion

    // ---- MÉTODOS PRIVADOS ----
    #region Métodos Privados
    // Documentar cada método que aparece aquí
    // El convenio de nombres de Unity recomienda que estos métodos
    // se nombren en formato PascalCase (palabras con primera letra
    // mayúscula, incluida la primera letra)

    #endregion

} // class PauseManager 
// namespace
