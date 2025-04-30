//---------------------------------------------------------
// Breve descripción del contenido del archivo: Permite mostrar un mensaje de texto para notificar al jugador 
// que su mando esta conectado o desconectado.
// Responsable de la creación de este archivo: Andrés Díaz Guerrero Soto
// Nombre del juego
// Proyectos 1 - Curso 2024-25
//---------------------------------------------------------


using UnityEngine;

// Añadir aquí el resto de directivas using
using UnityEngine.UI;
using System.Collections;
using UnityEngine.InputSystem;
using TMPro;

/// <summary>
/// Antes de cada class, descripción de qué es y para qué sirve,
/// usando todas las líneas que sean necesarias.
/// </summary>
public class PilotManager : MonoBehaviour
{
    // ---- ATRIBUTOS DEL INSPECTOR ----
    #region Atributos del Inspector (serialized fields)
    // Documentar cada atributo que aparece aquí.
    // El convenio de nombres de Unity recomienda que los atributos
    // públicos y de inspector se nombren en formato PascalCase
    // (palabras con primera letra mayúscula, incluida la primera letra)
    // Ejemplo: MaxHealthPoints

    [Header("UI Message")]
    [Tooltip("Texto que se muestra cuando el mando está conectado")]
    [SerializeField] private TMP_Text pilotText;

    [Header("Timming")]
    [Tooltip("Tiempo que se muestra el mensaje permanece visible")]
    [SerializeField] private float messageDuration = 2f;

    [Header("UI Container")]
    [Tooltip("GameObject raíz del UI que contiene el mensaje de piloto.")]
    [SerializeField] private GameObject pilotContainer;


    #endregion

    // ---- ATRIBUTOS PRIVADOS ----
    #region Atributos Privados (private fields)
    // Documentar cada atributo que aparece aquí.
    // El convenio de nombres de Unity recomienda que los atributos
    // privados se nombren en formato _camelCase (comienza con _, 
    // primera palabra en minúsculas y el resto con la 
    // primera letra en mayúsculas)
    // Ejemplo: _maxHealthPoints

    private bool wasConnected;
    private Coroutine hideCoroutine;
    private float originalFixedDeltaTime;



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
        if (pilotContainer == null || pilotText == null)
        {
            Debug.LogError("PilotManager: pilotContainer o pilotText no asignados en el Inspector.");
            enabled = false;
            return;
        }

        // Guardar fixedDeltaTime para restaurar después
        originalFixedDeltaTime = Time.fixedDeltaTime;

        // Inicializar estado previo y ocultar UI completo
        wasConnected = IsGamepadConnected();
        pilotContainer.SetActive(false);
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        bool isConnected = IsGamepadConnected();
        if (isConnected != wasConnected)
        {
            if (isConnected)
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.lockState = CursorLockMode.Confined;
                ShowMessage("Mando conectado");
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                ShowMessage("Mando desconectado");
            }
            wasConnected = isConnected;
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

    #endregion

    // ---- MÉTODOS PRIVADOS ----
    #region Métodos Privados
    // Documentar cada método que aparece aquí
    // El convenio de nombres de Unity recomienda que estos métodos
    // se nombren en formato PascalCase (palabras con primera letra
    // mayúscula, incluida la primera letra)

    /// <summary>
    /// Muestra el mensaje y pausa toda la lógica de juego.
    /// </summary>
    /// <param name="message">Texto a mostrar</param>
    private void ShowMessage(string message)
    {
        pilotText.text = message;
        pilotContainer.SetActive(true);

        // Pausar toda la lógica de juego
        Time.timeScale = 0f;
        Time.fixedDeltaTime = 0f;

        if (hideCoroutine != null)
            StopCoroutine(hideCoroutine);
        hideCoroutine = StartCoroutine(HideAfterDelay());
    }

    /// <summary>
    /// Oculta el UI tras un retardo en tiempo real y reanuda el juego.
    /// </summary>
    private IEnumerator HideAfterDelay()
    {
        yield return new WaitForSecondsRealtime(messageDuration);

        pilotContainer.SetActive(false);

        // Reanudar juego completo
        Time.timeScale = 1f;
        Time.fixedDeltaTime = originalFixedDeltaTime;

        hideCoroutine = null;
    }

    /// <summary>
    /// Comprueba si hay al menos un Gamepad conectado usando el nuevo Input System.
    /// </summary>
    /// <returns>True si hay al menos un Gamepad conectado; false en caso contrario.</returns>
    private bool IsGamepadConnected()
    {
        return Gamepad.all.Count > 0;
    }
    #endregion   

} // class PilotManager 
// namespace
