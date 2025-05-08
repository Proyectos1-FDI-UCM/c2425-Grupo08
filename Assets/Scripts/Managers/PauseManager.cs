//---------------------------------------------------------
// Gestiona el Menú de pausa del juego
// Vicente Rodriguez Casado
// Beyond the Depths
// Proyectos 1 - Curso 2024-25
//---------------------------------------------------------

using UnityEngine;
// Añadir aquí el resto de directivas using


/// <summary>
/// Gestiona el menu de pausa del juego.
/// </summary>
public class PauseManager : MonoBehaviour
{
    // ---- ATRIBUTOS DEL INSPECTOR ----
    #region Atributos del Inspector (serialized fields)
    

    [SerializeField] GameObject pauseMenuUI; // El menú de pausa

    #endregion

    // ---- ATRIBUTOS PRIVADOS ----
    #region Atributos Privados (private fields)

    public static bool GameIsPaused { get; private set; } = false; // Indica si el juego está en pausa o no
    private AudioSource[] audioSources; // Todas las fuentes de audios de la escena
    private Animator[] animators; // Todos los animators de la escena



    #endregion

    // ---- MÉTODOS DE MONOBEHAVIOUR ----
    #region Métodos de MonoBehaviour
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
        if (InputManager.Instance.ReturnWasReleased())
        {
            if (pauseMenuUI != null)
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

            else 
            {
                GameManager.Instance.ChangeScene(0);
            }
        }

    }

    #endregion

    // ---- MÉTODOS PÚBLICOS ----
    #region Métodos públicos
    
    public void Resume() // Reanuda el juego
    {
        Time.timeScale = 1f; // El juego continúa
        GameIsPaused = false;
        pauseMenuUI.SetActive(false);

        foreach (var audio in audioSources)
            audio.UnPause();

        foreach (var animator in animators)
            animator.speed = 1f;
    }

    void Pause() // Pausa el juego
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
