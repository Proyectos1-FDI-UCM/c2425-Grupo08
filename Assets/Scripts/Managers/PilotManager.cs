//---------------------------------------------------------
// Breve descripción del contenido del archivo: Permite mostrar un mensaje de texto para notificar al jugador 
// que su mando esta conectado o desconectado.
// Responsable de la creación de este archivo: Andrés Díaz Guerrero Soto
// Beyond the Depths
// Proyectos 1 - Curso 2024-25
//---------------------------------------------------------


using UnityEngine;

// Añadir aquí el resto de directivas using
using UnityEngine.UI;
using System.Collections;
using UnityEngine.InputSystem;
using TMPro;

/// <summary>
/// Clase que gestiona la notificación al jugador sobre el estado de conexión del mando (gamepad).
/// Muestra un mensaje en pantalla cuando el mando se conecta o desconecta, y pausa el juego mientras
/// el mensaje está visible.
/// </summary>
public class PilotManager : MonoBehaviour
{
    // ---- ATRIBUTOS DEL INSPECTOR ----
    #region Atributos del Inspector (serialized fields)
    

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

    
    // Indica si el mando estaba conectado en el último frame.
    
    private bool wasConnected;
    
    //Referencia a la corrutina que oculta el mensaje tras un retardo.
   
    private Coroutine hideCoroutine;
    
    // Valor original de Time.fixedDeltaTime para restaurarlo tras pausar el juego.
    
    private float originalFixedDeltaTime;



    #endregion

    // ---- MÉTODOS DE MONOBEHAVIOUR ----
    #region Métodos de MonoBehaviour


    /// <summary>
    /// Al iniciar el juego, se inicializan los atributos y se oculta el UI completo.
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
    /// Esta siempre actualizando y buscando si esta conectado o no el mando.
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


    #endregion

    // ---- MÉTODOS PRIVADOS ----
    #region Métodos Privados
    

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
