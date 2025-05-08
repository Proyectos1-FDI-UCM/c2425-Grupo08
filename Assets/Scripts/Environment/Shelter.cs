//---------------------------------------------------------
// Script que se le añade a las puertas para cambiar de escena
// Tomás Arévalo Almagro
// Beyond the Depths
// Proyectos 1 - Curso 2024-25
//---------------------------------------------------------

using UnityEngine;
using UnityEngine.Rendering.Universal;

/// <summary>
/// Antes de cada class, descripción de qué es y para qué sirve,
/// usando todas las líneas que sean necesarias.
/// </summary>
public class Shelter : MonoBehaviour
{
    // ---- ATRIBUTOS DEL INSPECTOR ----
    #region Atributos del Inspector (serialized fields)

    [SerializeField] private int sceneIndex;
    // Índice de la escena a la que se cambiará al interactuar con la puerta.

    [SerializeField] private Sprite shelterSprite;
    // Esto seguro que se puede hacer de mejor manera

    #endregion

    // ---- ATRIBUTOS PRIVADOS ----
    #region Atributos Privados (private fields)
    private bool levelCompleted = false;
    // Indica si el jugador ha completado el nivel actual.

    private bool hasEnter = false;
    // Indica si el jugador ha entrado en el área de colisión de la puerta.

    private Terminal Console;
    // Referencia a la consola del juego para mostrar mensajes.

    private GameObject shelterLights; // Referencia a las luces del refugio.

    #endregion

    // ---- MÉTODOS DE MONOBEHAVIOUR ----
    #region Métodos de MonoBehaviour

    /// <summary>
    /// Start is called on the frame when a script is enabled just before 
    /// any of the Update methods are called the first time.
    /// </summary>
    void Start()
    {
        Console = GetComponentInChildren<Terminal>();

        if (Console == null)

            Debug.LogError("No se ha encontrado el terminal en el refugio");

        else

            Console.SetMessage("Status: \nInhabilitado\n\nGeneradores reparados: 0/1");

        levelCompleted = LevelManager.Instance.LevelCompleted();
        // Consulta si se ha completado el nivel

        // Establece las luces
        shelterLights = GetComponentInChildren<Light2D>().transform.parent.gameObject;

        if (shelterLights == null)

            Debug.LogError("No se han encontrado las luces del refugio");

        else
            
            // Desactiva las luces al inicio
            shelterLights.SetActive(false);

    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        if (levelCompleted)
        {
            // Si se ha completado el nivel, se activa la consola y las luces
            shelterLights.SetActive(true);

            // Pasar al siguiente frame si se ha completado el nivel
            GetComponent<SpriteRenderer>().sprite = shelterSprite;

            if (hasEnter && InputManager.Instance.InteractWasPressedThisFrame()) 

                GameManager.Instance.ChangeScene(sceneIndex);
                // Cambiar de escena si se cumple todo

        }

        else 
        {
            // Si no se ha completado el nivel, se desactiva la consola y las luces
            shelterLights.SetActive(false);
        }

        levelCompleted = LevelManager.Instance.LevelCompleted(); // Actualiza estado
        
    }

    #endregion

    // ---- MÉTODOS PRIVADOS ----
    #region Métodos privados

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<PlayerMovement>() != null)
        {
            hasEnter = true;

            if (levelCompleted && Console != null)
        {
            Console.Write("Status: \nHabilitado\n\nPulsa {key_interact} para entrar al refugio");
            // Muestra el mensaje de que se puede entrar al refugio
        }

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

} // class Shelter
