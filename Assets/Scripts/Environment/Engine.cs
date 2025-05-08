//---------------------------------------------------------
// Este archivo gestiona la lógica del motor dentro del juego, 
// incluyendo su reparación y el progreso de carga visual.
// Tomás Arévalo Almagro, Carlos Dochao Moreno
// Beyond the Depths
// Proyecto 1 - Curso 2024-25
//---------------------------------------------------------

using UnityEngine;
using System.Collections;
using UnityEngine.Rendering.Universal;

/// <summary>
/// Clase que maneja la interacción del jugador con el motor,
/// permitiendo su reparación a través de un sistema de carga progresiva.
/// </summary>
public class Engine : MonoBehaviour
{
    // ---- ATRIBUTOS DEL INSPECTOR ----
    #region Atributos del Inspector (serialized fields)

    // Tiempo total requerido para completar la carga (en segundos)
    [SerializeField] private float loadTime = 4f;
 
    [SerializeField] private float finalanimationspeed = 4f;
    [SerializeField] private float finalsoundspeed = 4f;

    // Mensaje mostrado al completar la reparación
    [TextArea(2, 5)]
    [SerializeField] private string repairCompleteMsg;

    #endregion

    // ---- ATRIBUTOS PRIVADOS ----
    #region Atributos Privados (private fields)

    // Indica si el jugador ha entrado en la zona de interacción del motor
    private bool hasEnter = false;

    // Indica si el motor ya ha sido reparado
    private bool isRepaired = false;

    // Almacena el progreso actual de la reparación del motor
    private float currentLoadProgress = 0f;

    // Referencia a la corrutina de carga para poder detenerla si es necesario
    private Coroutine loadCoroutine;

    private Terminal Console; // Referencia al componente Terminal para mostrar mensajes en la UI

    private GameObject player;
    private FlashLight flashlight;
    private Animator motorAnimator;
    private AudioSource audioSource;

    private GameObject bubbles; // Objeto de burbujas que se activa al completar la reparación
    private Light2D engineLight; // Luz del motor que parpadea durante la reparación
    private float intesity; // Intensidad de la luz del motor

    #endregion

    // ---- MÉTODOS DE MONOBEHAVIOUR ----
    #region Métodos de MonoBehaviour

    /// <summary>
    /// Start se ejecuta una vez al inicio del juego.
    /// Configura los elementos UI desactivándolos inicialmente.
    /// </summary>
    void Start()
    {
        Console = GetComponentInChildren<Terminal>(); // Busca el componente Terminal en los hijos del objeto actual

        if (Console == null) // Si no se encuentra el componente Terminal, muestra un mensaje de error

            Debug.LogError("No se ha encontrado el componente Terminal en el objeto ni en sus hijos.");

        flashlight = GameManager.Instance.GetFlashLight();
        audioSource = GetComponent<AudioSource>();
        player = GameManager.Instance.GetPlayerController();
        motorAnimator = GetComponent<Animator>();

        bubbles = GetComponentInChildren<ParticleSystem>().gameObject; // Busca el objeto de burbujas en los hijos del objeto actual

        if (bubbles != null) // Si no se encuentra el objeto de burbujas, muestra un mensaje de error
            
            bubbles.SetActive(false); // Desactiva las burbujas al inicio

        else // Si no se encuentra el objeto de burbujas, muestra un mensaje de error

            Debug.LogWarning("No se ha encontrado el objeto de burbujas en el motor.");

        engineLight = GetComponentInChildren<Light2D>(); // Busca la luz del motor en los hijos del objeto actual

        if (engineLight == null) // Si no se encuentra la luz, muestra un mensaje de error
        
            Debug.LogWarning("No se ha encontrado el objeto de burbujas en el motor.");

        else 

            intesity = engineLight.intensity; // Almacena la intensidad inicial de la luz del motor
    }

    /// <summary>
    /// Update se ejecuta en cada frame.
    /// Verifica si el jugador ha ingresado en la zona de interacción del motor
    /// y permite iniciar/detener la reparación con la tecla de interacción.
    /// </summary>
    void Update()
    {
        // Solo permite la interacción si el jugador está en la zona y el motor no ha sido reparado
        if (hasEnter && !isRepaired)
        {
            // Si el jugador mantiene presionada la tecla de interacción, comienza la carga
            if (InputManager.Instance.InteractIsPressed())
            {
                StartLoading();
            }
            // Si el jugador suelta la tecla de interacción, se detiene la carga
            if (InputManager.Instance.InteractWasRealeasedThisFrame())
            {
                StopLoading();
            }
        }

        if (engineLight != null && !isRepaired)

            engineLight.intensity = Mathf.PingPong(Time.time, 2f); // Cambia la intensidad de la luz entre 0 y 2
    }

    #endregion


    // ---- MÉTODOS PRIVADOS ----
    #region Métodos privados

    /// <summary>
    /// Inicia el proceso de reparación del motor activando la barra de progreso.
    /// Se asegura de que la corrutina solo se inicie si no se está ejecutando otra.
    /// </summary>
    private void StartLoading()
    {
        if (loadCoroutine == null) // Evita iniciar múltiples instancias de la corrutina
        {
            flashlight.LightUnfocus(); // Desactiva el foco de la linterna

            GetComponent<GeneratorEnemySpawner>().SetCanRespawn(true); // Permite el respawn de enemigos
            loadCoroutine = StartCoroutine(LoadProgress()); // Inicia la carga
            AudioManager.instance.PlaySFX(SFXType.MotorSound, audioSource, true); // Reproduce el sonido de reparación
        }
    }

    /// <summary>
    /// Detiene el proceso de reparación si el jugador deja de interactuar.
    /// Oculta la barra de progreso y resetea variables de control.
    /// </summary>
    private void StopLoading()
    {
        if (loadCoroutine != null)
        {
            StopCoroutine(loadCoroutine); // Detiene la corrutina de carga
            loadCoroutine = null; // Limpia la referencia
        }

        AudioManager.instance.StopSFX(audioSource); // Para el sonido de reparación

        player.GetComponent<PlayerMovement>().SetIsRepairing(false);
        GetComponent<GeneratorEnemySpawner>().SetCanRespawn(false);
    }

    /// <summary>
    /// Corrutina que maneja la carga progresiva de la reparación del motor.
    /// Aumenta el progreso de carga con el tiempo hasta completarlo.
    /// </summary>
    private IEnumerator LoadProgress()
    {
        player.GetComponent<PlayerMovement>().SetIsRepairing(true);

        if (motorAnimator != null)
            motorAnimator.speed = 0.5f;

        if (audioSource != null)
            audioSource.pitch = 0.8f; // Comienza con un pitch más bajo

        while (currentLoadProgress < 1f) // Mientras la carga no esté completa
        {
            currentLoadProgress += Time.deltaTime / loadTime; // Incrementa el progreso

            // Actualiza directamente el texto de la terminal
            Console.SetMessage("Reparando..." + Mathf.RoundToInt(currentLoadProgress * 100).ToString() + "%"); 

            if (motorAnimator != null)
            {
                float speedMultiplier = Mathf.Lerp(0.5f, finalanimationspeed, currentLoadProgress);
                motorAnimator.speed = speedMultiplier;
            }
            if (audioSource != null)
            {
                float pitchMultiplier = Mathf.Lerp(0.8f, finalsoundspeed, currentLoadProgress);
                audioSource.pitch = pitchMultiplier;
            }

            if (currentLoadProgress >= 1f) // Si la carga se completa
            {
                CompleteRepair(); // Finaliza la reparación
                yield break; // Termina la corrutina
            }
            yield return null; // Espera un frame antes de continuar
        }
    }

    /// <summary>
    /// Finaliza el proceso de reparación cuando la barra de carga llega al 100%.
    /// Oculta los elementos UI y cambia el color del motor a verde.
    /// Informa al LevelManager que el motor ha sido reparado.
    /// </summary>
    private void CompleteRepair()
    {
        isRepaired = true; // Marca el motor como reparado
        
        player.GetComponent<PlayerMovement>().SetIsRepairing(false);
        GetComponent<GeneratorEnemySpawner>().SetCanRespawn(false);

        bubbles.SetActive(true); // Activa las burbujas de reparación
        engineLight.intensity = intesity; // Restablece la intensidad de la luz del motor

        Console.Clear();
        Console.Write(repairCompleteMsg);

        // Notifica al LevelManager que el motor ha sido reparado
        LevelManager.Instance.MotorRepaired();
    }

    /// <summary>
    /// Detecta cuando el jugador entra en la zona de interacción del motor.
    /// Activa la interfaz de carga si el motor no ha sido reparado aún.
    /// </summary>
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject == player && !isRepaired) // Si el objeto que entra es el jugador y el motor no está reparado
        {
            hasEnter = true; // Permite la interacción
        }
    }

    /// <summary>
    /// Detecta cuando el jugador sale de la zona de interacción del motor.
    /// Desactiva la interfaz de carga y detiene el progreso.
    /// </summary>
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject == player) // Si el objeto que sale es el jugador
        {
            hasEnter = false; // Desactiva la interacción

            StopLoading(); // Detiene la carga si estaba en progreso
        }
    }

    #endregion

} // class Engine
