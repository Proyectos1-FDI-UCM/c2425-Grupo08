//---------------------------------------------------------
// Miguel Ángel González López
// Project Abyss
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

    private Terminal Console;

    private float time;
    private int keyCount;

    #endregion

    // ---- MÉTODOS DE MONOBEHAVIOUR ----
    #region Métodos de MonoBehaviour


    /// <summary>
    /// Start is called on the frame when a script is enabled just before 
    /// any of the Update methods are called the first time.
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

            Console.SetMessage("{key_jump}");
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
                Console.Hide();
                _isGrabbed = false;
                PlayerMovement.Released();
            }

            if (time < delay)
            {
                time += Time.deltaTime;

                if (time > delay/4)

                    Console.Hide();
            }

            else
            {
                Console.Show();
                //player.GetComponent<OxigenScript>().ReduceOxygen(damage);
                // Sonido de que te quitan oxígeno?
                time = 0f;
            }

            // Animación de atrapado?
        }

        else
        {
            // Animación idle
        }

        //float playerSpeed = PlayerMovement.GetComponent<Rigidbody2D>().velocity.magnitude;
        //bool inGrab = _canGrab && playerSpeed <= maxCatchVelocity && IsPlayerInside(grabRadius);
        //bool inActivate = IsPlayerInside(activateRadius);

        /*
        if (inGrab)
        {
            if (!_isGrabbed)
            {
                PlayerMovement.StartGrabbed();
                _isGrabbed = true;
                PlayerMovement.isGrabbed = true;
                PlayerMovement.kelpGrabbing = this;
            }
            _isActive = false;
        }
        else if (inActivate)
        {
            _isActive = true;
        }
        else if (_isGrabbed)
        {
            // Esperar a que Player llame a ReleasePlayer()
        }
        else
        {
            _isActive = false;
        }

        */
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
        }
    }


    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponent<PlayerMovement>() != null)
        {
            //Animación de liberación/idle
            _isGrabbed = false;
            Console.Hide();
        }
    }


    /// <summary>
    /// Compara la posición del jugador con la posición del enemigo alga y devuelve true si está dentro del radio dado.
    /// </summary>
    /// <param name="radius">Radio de la circunferencia de acción a comparar con el jugador</param>
    /// <returns></returns>
    /*private bool IsPlayerInside(float radius)
    {
        return (Player.transform.position - transform.position).magnitude <= radius;
    }*/


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
