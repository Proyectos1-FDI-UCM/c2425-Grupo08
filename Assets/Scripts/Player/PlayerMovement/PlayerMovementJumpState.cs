//---------------------------------------------------------
// Breve descripción del contenido del archivo
// Responsable de la creación de este archivo
// Nombre del juego
// Proyectos 1 - Curso 2024-25
//---------------------------------------------------------

using UnityEngine;
// Añadir aquí el resto de directivas using


/// <summary>
/// Antes de cada class, descripción de qué es y para qué sirve,
/// usando todas las líneas que sean necesarias.
/// </summary>
public class PlayerMovementJumpState : MonoBehaviour
{
    // ---- ATRIBUTOS DEL INSPECTOR ----
    #region Atributos del Inspector (serialized fields)
    // Documentar cada atributo que aparece aquí.
    // El convenio de nombres de Unity recomienda que los atributos
    // públicos y de inspector se nombren en formato PascalCase
    // (palabras con primera letra mayúscula, incluida la primera letra)
    // Ejemplo: MaxHealthPoints
    [SerializeField] private float maxSpeed; // El límite de velocidad del jugador
    [SerializeField] private float acceleration; // La aceleración positiva para moverse
    [SerializeField] private float deceleration; // La aceleración negativa para frenar
    [SerializeField] private float decelerationThreshold; // El límite de velocidad para parar de frenar mediante fuerzas (para evitar temblor en el jugador) SE RECOMIENDA UN MÍNIMO DE 0.15
    [SerializeField] private float jumpAcceleration; // La aceleración positiva para saltar
    [SerializeField] private float jumpMultiplierDecay; // La cantidad de decremento de la fuerza de salto por frame (1 es el valor máximo del multiplicador)
    [SerializeField] private bool debug; // Para ver los vectores de velocidad y aceleración
    #endregion

    // ---- ATRIBUTOS PRIVADOS ----
    #region Atributos Privados (private fields)
    // Documentar cada atributo que aparece aquí.
    // El convenio de nombres de Unity recomienda que los atributos
    // privados se nombren en formato _camelCase (comienza con _, 
    // primera palabra en minúsculas y el resto con la 
    // primera letra en mayúsculas)
    // Ejemplo: _maxHealthPoints
    private float joystickMaxSpeed; // El límite de velocidad con el que operará el script (en el caso del teclado no hace nada, en el caso del joystick se multiplica la velocidad máxima por el desplazamiento horizontal de este)
    private Rigidbody2D rb;
    private float jumpMultiplier = 1; // Multiplicador de la fuerza de salto para bajarla si se mantiene pulsado el botón de salto

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
        rb = GetComponent<Rigidbody2D>();
    }

    #endregion

    // ---- MÉTODOS PÚBLICOS ----
    #region Métodos públicos
    // Documentar cada método que aparece aquí con ///<summary>
    // El convenio de nombres de Unity recomienda que estos métodos
    // se nombren en formato PascalCase (palabras con primera letra
    // mayúscula, incluida la primera letra)
    // Ejemplo: GetPlayerController
    public void Move()
    {
        if (InputManager.Instance.JumpWasRealeasedThisFrame())
        {
            jumpMultiplier = 0;
        }

        if (InputManager.Instance.JumpIsPressed() && jumpMultiplier > 0)
        {
            Jump();
        }      

        if (InputManager.Instance.MovementVector.x != 0)
        {
            joystickMaxSpeed = maxSpeed * InputManager.Instance.MovementVector.x;
            Walk(InputManager.Instance.MovementVector.x);
        }
        else Decelerate(deceleration);
    }
    public void NextState()
    {
        if (rb.velocity.y < 0)
        {
            //player.State = new FallState;
        }
    }

    #endregion

    // ---- MÉTODOS PRIVADOS ----
    #region Métodos Privados
    // Documentar cada método que aparece aquí
    // El convenio de nombres de Unity recomienda que estos métodos
    // se nombren en formato PascalCase (palabras con primera letra
    // mayúscula, incluida la primera letra)
    private void Jump()
    {
        rb.AddForce(new Vector2(0, 1) * jumpAcceleration * jumpMultiplier, ForceMode2D.Impulse);
        jumpMultiplier -= jumpMultiplierDecay;
    }
    private void Walk(float x) // Mueve al jugador en la dirección indicada por el signo de x y con la velocidad máxima indicada por el valor de x
    {
        if ((x < 0 && rb.velocity.x > 0) || (x > 0 && rb.velocity.x < 0)) // Deceleración en cambio de sentido
        {
            Decelerate(deceleration);
        }
        else // Aceleración en el sentido del movimiento
        {
            rb.AddForce(new Vector2(x, 0).normalized * acceleration, ForceMode2D.Force);
        }

        if (Mathf.Abs(rb.velocity.x) > Mathf.Abs(joystickMaxSpeed)) // Limitación de la velocidad
        {
            if (joystickMaxSpeed == 1 || joystickMaxSpeed == -1)
            {
                rb.velocity = rb.velocity.normalized * Mathf.Abs(joystickMaxSpeed);
            }
            else
            {
                Decelerate(acceleration); // En el caso (nada raro) de que el joystick pase de un valor a otro más bajo del mismo signo, se frena con el valor de la aceleración
            }
        }
    }
    private void Decelerate(float decelerationValue) // Frena al jugador con la aceleración negativa indicada
    {
        if (rb.velocity.x > decelerationThreshold) // Comprobación de signo para elegir el sentido de la fuerza
        {
            rb.AddForce(new Vector2(-decelerationValue, 0), ForceMode2D.Force);
        }
        else if (rb.velocity.x < -decelerationThreshold)
        {
            rb.AddForce(new Vector2(decelerationValue, 0), ForceMode2D.Force);
        }
    }
    private void OnDrawGizmos()
    {
        if (debug)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, transform.position + new Vector3(rb.velocity.x, rb.velocity.y, 0));
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, transform.position + new Vector3(InputManager.Instance.MovementVector.x, InputManager.Instance.MovementVector.y, 0));
        }
    }

    #endregion

} // class PlayerMovementScript 
// namespace