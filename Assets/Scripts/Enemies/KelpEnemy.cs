//---------------------------------------------------------
// Miguel Ángel González López
// Beyond the Depths
// Proyectos 1 - Curso 2024-25
//---------------------------------------------------------

using UnityEngine;

/// <summary>
/// Clase del enemigo alga. Tiene toda la lógica del alga, pero el movimiento lo ejecuta el jugador.
/// </summary>
public class KelpEnemy : MonoBehaviour
{
    // ---- ATRIBUTOS DEL INSPECTOR ----
    #region Atributos del Inspector (serialized fields)
    //[SerializeField] private float activateRadius = 10f;    // Radio para activar
    [SerializeField] private float grabRadius = 5f;         // Radio para atrapar
    //[SerializeField] private float damage = 1f;   // Daño que hace al jugador al atraparlo
    [SerializeField] private int keyPresses = 5;  // Numero de pulsaciones de tecla para liberarse
    [SerializeField] private float delay = 1f; // Tiempo para quitar oxígeno (y mostrar el texto)
    #endregion

    // ---- ATRIBUTOS PRIVADOS ----
    #region Atributos Privados (private fields)
    private bool _isGrabbed = false;
    private bool _isActive = false;
    //private float _cooldownTimer = 0f;
    private bool _canGrab = true;


    private PlayerMovement PlayerMovement;

    private GameObject player;

    // Componente de la terminal
    private Terminal Console;

    // Contador de pulsaciones de tecla
    private int keyCount;

    #endregion

    // ---- MÉTODOS DE MONOBEHAVIOUR ----
    #region Métodos de MonoBehaviour

    /// <summary>
    /// Se asignan referencias a los componentes necesarios.
    /// </summary>
    void Start()
    {
        // Inicializa el jugador
        player = GameManager.Instance.GetPlayerController();

        if (player == null)

            Debug.LogError("No se ha encontrado el jugador");

        // Inicializa el componente de movimiento del jugador
        PlayerMovement = player.GetComponent<PlayerMovement>();

        if (PlayerMovement == null)

            Debug.LogError("No se ha encontrado el jugador");

        Console = GetComponentInChildren<Terminal>();

        if (Console == null)

            Debug.LogError("No se ha encontrado el componente Terminal en el GameObject.");

        else
        {
            Console.gameObject.SetActive(false);
            Console.SetMessage("Pulsa {key_jump}");
        }
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        if (_isGrabbed)
        {
            if (InputManager.Instance.JumpWasRealeasedThisFrame())
            {
                keyCount++;
            }

            if (keyCount >= keyPresses)
            {
                Console.gameObject.SetActive(false);
                _isGrabbed = false;
                PlayerMovement.Released();
            }

            // Animación de atrapado?
        }

        else
        {
            // Animación idle
        }

    }

    #endregion

    // ---- MÉTODOS PÚBLICOS ----
    #region Métodos públicos

    /// <summary>
    /// Método externo que avisa de que el jugador ya no está siendo agarrado.
    /// </summary>
    public void ReleasePlayer()
    {
        _isGrabbed = false;
        PlayerMovement.isGrabbed = false;
        _canGrab = false;
       // _cooldownTimer = grabCooldown;
    }
    #endregion

    // ---- MÉTODOS PRIVADOS ----
    #region Métodos Privados

    private void GrabPlayer()
    {
        if (_canGrab && PlayerMovement != null)
        {
            _isGrabbed = true;
            PlayerMovement.Grabbed();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<PlayerMovement>() != null)
        {
            keyCount = 0;
            PlayerMovement.kelpGrabbing = this;
            GrabPlayer();
            Console.gameObject.SetActive(true);
        }
    }


    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponent<PlayerMovement>() != null)
        {
            //Animación de liberación/idle
            _isGrabbed = false;
            Console.gameObject.SetActive(false);
        }
    }


    private void OnDrawGizmos()
    { 
        // Dibuja radios de activación y agarre
        Gizmos.color = Color.green;
        //Gizmos.DrawWireSphere(transform.position, activateRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, grabRadius);

        // Indica estado usando gizmos
        if (_isGrabbed)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(transform.position, 0.1f);
        }
        else if (_isActive)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(transform.position, 0.1f);
        }
        else
        {
            //Gizmos.color = Color.green;
            //Gizmos.DrawSphere(transform.position, 0.1f);
        }
        
    }

    #endregion
} // class AlgaEnemy
