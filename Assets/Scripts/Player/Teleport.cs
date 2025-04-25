//---------------------------------------------------------
// Este sirve para activar el texto de pulsa e para teleportarse al motor y teleportarse a las coordenadas dadas
// Andrés Bartolomé Clap
// Project Abyss
// Proyectos 1 - Curso 2024-25
//---------------------------------------------------------

using TMPro;
using UnityEngine;
// Añadir aquí el resto de directivas using


/// <summary>
/// Antes de cada class, descripción de qué es y para qué sirve,
/// usando todas las líneas que sean necesarias.
/// </summary>
public class Teleport : MonoBehaviour
{
    // ---- ATRIBUTOS DEL INSPECTOR ----
    #region Atributos del Inspector (serialized fields)
    [SerializeField] private Vector3 coordenadas; // Coordenadas a las que se teletransportará el jugador

    #endregion

    // ---- ATRIBUTOS PRIVADOS ----
    #region Atributos Privados (private fields)
    private GameObject player; // Referencia al jugador
    private Canvas canva; // Referencia al canvas de la UI
    private bool hasEnter = false;
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
        player = GameObject.FindGameObjectWithTag("Player"); //Se obtiene la referencia al jugador
        canva = gameObject.GetComponentInChildren<Canvas>(); //Se obtiene la referencia al canvas
        canva.gameObject.SetActive(false);  // Oculta el Canvas al inicio
        if (!GameManager.Instance.GetTeleport())
        {
            gameObject.SetActive(false); // Desactiva el objeto si el truco de teletransporte no está activado
        }
    }
    void Update()
    {
        if (hasEnter&&InputManager.Instance.InteractWasPressedThisFrame())
        {
            player.transform.position = coordenadas; // Teletransporta al jugador a las coordenadas dadas
        }
    }

    #endregion

    // ---- MÉTODOS PRIVADOS ----
    #region Métodos Privados
    /// <summary>
    /// Detecta cuando el jugador entra en la zona de interacción del teleport.
    /// Activa la interfaz de interactuar.
    /// </summary>
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject == player) // Si el objeto que entra es el jugador
        {
            hasEnter = true; // Permite la interacción
            canva.gameObject.SetActive(true); // Muestra la UI del teleport
        }
    }

    /// <summary>
    /// Detecta cuando el jugador sale de la zona de interacción del teleport.
    /// Desactiva la interfaz de interactuar.
    /// </summary>
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject == player) // Si el objeto que sale es el jugador
        {
            hasEnter = false; // Desactiva la interacción
            canva.gameObject.SetActive(false); // Oculta la UI del teleport
        }
    }

    #endregion   

} // class Teleport 
// namespace
