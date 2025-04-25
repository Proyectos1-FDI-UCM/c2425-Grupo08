//---------------------------------------------------------
// Script que se le añade a las puertas para cambiar de escena
// Tomás Arévalo Almagro
// Proyectos 1 - Curso 2024-25
//---------------------------------------------------------

using UnityEngine;

/// <summary>
/// Antes de cada class, descripción de qué es y para qué sirve,
/// usando todas las líneas que sean necesarias.
/// </summary>
public class Door : MonoBehaviour
{
    // ---- ATRIBUTOS DEL INSPECTOR ----
    #region Atributos del Inspector (serialized fields)

    [SerializeField] private int sceneIndex;
    // Índice de la escena a la que se cambiará al interactuar con la puerta.
    #endregion

    // ---- ATRIBUTOS PRIVADOS ----
    #region Atributos Privados (private fields)
    // Documentar cada atributo que aparece aquí.
    // El convenio de nombres de Unity recomienda que los atributos
    // privados se nombren en formato _camelCase (comienza con _, 
    // primera palabra en minúsculas y el resto con la 
    // primera letra en mayúsculas)
    // Ejemplo: _maxHealthPoints
    private bool levelCompleted = false;
    // Indica si el jugador ha completado el nivel actual.

    private bool hasEnter = false;
    // Indica si el jugador ha entrado en el área de colisión de la puerta.

    private Terminal Console;
    // Referencia a la consola del juego para mostrar mensajes.

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
        Console = GetComponent<Terminal>();

        if (Console == null)

            Debug.LogError("No se ha encontrado la consola en la puerta.");

        else

            Console.SetMessage("Estado del refugio...Deshabilitado\n\nDebes reparar el generador para entrar.");

        // Oculta el mensaje al principio
        levelCompleted = LevelManager.Instance.LevelCompleted();
        // Consulta si se ha completado el nivel

    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        if (hasEnter && levelCompleted && InputManager.Instance.InteractWasPressedThisFrame())
        {
            GameManager.Instance.ChangeScene(sceneIndex);
            // Cambiar de escena si se cumple todo
        }

        if (levelCompleted && Console != null)
        {
            Console.SetMessage($"Estado del refugio...Habilitado!\n\nPresiona {InputManager.Instance.GetInteractKey()} para entrar...");
            // Muestra el mensaje de que se puede entrar al refugio
        }
    }

    #endregion

    // ---- MÉTODOS PRIVADOS ----
    #region Métodos privados
    // Documentar cada método que aparece aquí
    // El convenio de nombres de Unity recomienda que estos métodos
    // se nombren en formato PascalCase (palabras con primera letra
    // mayúscula, incluida la primera letra)

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<PlayerMovement>() != null)
        {
            hasEnter = true;
            levelCompleted = LevelManager.Instance.LevelCompleted(); // Actualiza estado

        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponent<PlayerMovement>() != null)
        {
            hasEnter = false; 
        }
    }
    
    #endregion

} // class Door
