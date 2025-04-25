//---------------------------------------------------------
// // Miguel Ángel González López
// Project Abyss
// Proyectos 1 - Curso 2024-25
//---------------------------------------------------------

using UnityEngine;
// Añadir aquí el resto de directivas using


/// <summary>
/// Clase del enemigo alga. Tiene toda la lógica del alga, pero el movimiento lo ejecuta el jugador.
/// </summary>
public class KelpEnemy : MonoBehaviour
{
    // ---- ATRIBUTOS DEL INSPECTOR ----
    #region Atributos del Inspector (serialized fields)
    [SerializeField] private float activateRadius = 10f;    // Radio para activar
    [SerializeField] private float grabRadius = 5f;         // Radio para atrapar
    [SerializeField] private float maxCatchVelocity = 5f;   // Vel. máx. para que atrape
    [SerializeField] private float grabCooldown = 3f;  // Segundos antes de poder atrapar de nuevo
    [SerializeField] private PlayerMovement Player;         // Referencia al jugador
    #endregion

    // ---- ATRIBUTOS PRIVADOS ----
    #region Atributos Privados (private fields)
    private bool _isGrabbed = false;
    private bool _isActive = false;
    private float _cooldownTimer = 0f;
    private bool _canGrab = true;
    #endregion

    // ---- MÉTODOS DE MONOBEHAVIOUR ----
    #region Métodos de MonoBehaviour


    /// <summary>
    /// Start is called on the frame when a script is enabled just before 
    /// any of the Update methods are called the first time.
    /// </summary>
    void Start()
    {
        
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        // Gestionar cooldown tras liberación
        if (!_canGrab)
        {
            _cooldownTimer -= Time.deltaTime;
            if (_cooldownTimer <= 0f) _canGrab = true;
        }

        float playerSpeed = Player.GetComponent<Rigidbody2D>().velocity.magnitude;
        bool inGrab = _canGrab && playerSpeed <= maxCatchVelocity && IsPlayerInside(grabRadius);
        bool inActivate = IsPlayerInside(activateRadius);

        if (inGrab)
        {
            if (!_isGrabbed)
            {
                Player.StartGrabbed();
                _isGrabbed = true;
                Player.isGrabbed = true;
                Player.kelpGrabbing = this;
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
        Player.isGrabbed = false;
        _canGrab = false;
        _cooldownTimer = grabCooldown;
    }
    #endregion

    // ---- MÉTODOS PRIVADOS ----
    #region Métodos Privados
    /// <summary>
    /// Compara la posición del jugador con la posición del enemigo alga y devuelve true si está dentro del radio dado.
    /// </summary>
    /// <param name="radius">Radio de la circunferencia de acción a comparar con el jugador</param>
    /// <returns></returns>
    private bool IsPlayerInside(float radius)
    {
        return (Player.transform.position - transform.position).magnitude <= radius;
    }


    private void OnDrawGizmos()
    {
        // Dibuja radios de activación y agarre
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, activateRadius);
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
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(transform.position, 0.1f);
        }
    }


    #endregion
} // class KelpEnemy 
// namespace
