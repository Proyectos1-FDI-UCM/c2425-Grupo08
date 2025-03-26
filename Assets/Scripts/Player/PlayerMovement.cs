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
public class PlayerMovement : MonoBehaviour
{
    // ---- ATRIBUTOS DEL INSPECTOR ----
    #region Atributos del Inspector (serialized fields)
    // Documentar cada atributo que aparece aquí.
    // El convenio de nombres de Unity recomienda que los atributos
    // públicos y de inspector se nombren en formato PascalCase
    // (palabras con primera letra mayúscula, incluida la primera letra)
    // Ejemplo: MaxHealthPoints
        [Header("Walk Attributes")]
        [SerializeField]private float WalkAcceleration;
        [SerializeField]private float WalkDeceleration;
        [SerializeField]private float WalkMaxSpeed;
        [SerializeField]private float WalkDecelerationThreshold;
        [SerializeField]private float WalkJumpAcceleration;
        [SerializeField]private float WalkJumpMultiplierDecay;

        [Header("Idle Attributes")]
        [SerializeField]private float IdleAcceleration;
        [SerializeField]private float IdleDeceleration;
        [SerializeField]private float IdleMaxSpeed;
        [SerializeField]private float IdleDecelerationThreshold;
        [SerializeField]private float IdleJumpAcceleration;
        [SerializeField]private float IdleJumpMultiplierDecay;

        [Header("Jump Attributes")]
        [SerializeField]private float JumpAcceleration;
        [SerializeField]private float JumpDeceleration;
        [SerializeField]private float JumpMaxSpeed;
        [SerializeField]private float JumpDecelerationThreshold;
        [SerializeField]private float JumpJumpAcceleration;
        [SerializeField]private float JumpJumpMultiplierDecay;

        [Header("Fall Attributes")]
        [SerializeField]private float FallAcceleration;
        [SerializeField]private float FallDeceleration;
        [SerializeField]private float FallMaxSpeed;
        [SerializeField]private float FallDecelerationThreshold;
        [SerializeField]private float FallJumpAcceleration;
        [SerializeField]private float FallJumpMultiplierDecay;
 
        [Header("Aim Attributes")]
        [SerializeField]private float AimAcceleration;
        [SerializeField]private float AimDeceleration;
        [SerializeField]private float AimMaxSpeed;
        [SerializeField]private float AimDecelerationThreshold;
        [SerializeField]private float AimJumpAcceleration;
        [SerializeField]private float AimJumpMultiplierDecay;

        [SerializeField]private bool debug;

    #endregion
    
    // ---- ATRIBUTOS PRIVADOS ----
    #region Atributos Privados (private fields)
    // Documentar cada atributo que aparece aquí.
    // El convenio de nombres de Unity recomienda que los atributos
    // privados se nombren en formato _camelCase (comienza con _, 
    // primera palabra en minúsculas y el resto con la 
    // primera letra en mayúsculas)
    // Ejemplo: _maxHealthPoints
        enum States {
            Idle,
            Walk,
            Fall,
            Jump,
            Aim
        }
        private float joystickMaxSpeed;
        private Rigidbody2D rb;
        private bool isLanternAimed;
        private float jumpMultiplier = 1;
        private States state = States.Idle;



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

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        CheckState(ref state);
    }
    public void FixedUpdate()
    {
        switch (state){
        case States.Idle:
        break;
        case States.Walk:
            if (InputManager.Instance.MovementVector.x != 0)
            {
                joystickMaxSpeed = WalkMaxSpeed * InputManager.Instance.MovementVector.x;
                WalkWalk(InputManager.Instance.MovementVector.x);
            }
            else WalkDecelerate(WalkDeceleration);
        break;
        case States.Jump:
            if (InputManager.Instance.JumpWasRealeasedThisFrame())
            {
                jumpMultiplier = 0;
            }
    
            if (InputManager.Instance.JumpIsPressed() && jumpMultiplier > 0)
            {
                JumpJump();
            }
    
            if (InputManager.Instance.MovementVector.x != 0)
            {
                joystickMaxSpeed = JumpMaxSpeed * InputManager.Instance.MovementVector.x;
                JumpWalk(InputManager.Instance.MovementVector.x);
            }
            else JumpDecelerate(JumpDeceleration);
        break;
        case States.Fall:
            if (InputManager.Instance.MovementVector.x != 0)
            {
                joystickMaxSpeed = FallMaxSpeed * InputManager.Instance.MovementVector.x;
                FallWalk(InputManager.Instance.MovementVector.x);
            }
            else FallDecelerate(FallDeceleration);
        break;
        case States.Aim:
        break;
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
    #region Métodos privados
    // Documentar cada método que aparece aquí
    // El convenio de nombres de Unity recomienda que estos métodos
    // se nombren en formato PascalCase (palabras con primera letra
    // mayúscula, incluida la primera letra)

        private void CheckState(ref States state){

            switch (state){
                case States.Idle:

                    if (InputManager.Instance.MovementVector.x != 0)
                    {
                        state = States.Walk;
                        AudioManager.instance.PlayLoopingSFX(1);

                    }
                    else if (rb.velocity.y < 0)

                    {
                        state = States.Fall;
                    }
                    else if (InputManager.Instance.JumpWasPressedThisFrame())
                    {

                        state = States.Jump;
                        AudioManager.instance.PlaySFX(2);
                    }
                    else if (isLanternAimed)
                    {
                        state = States.Aim;
                    }
 
                break;
                case States.Walk:
                    if (rb.velocity == new Vector2(0, 0))
                    {
                        state = States.Idle; 
                        AudioManager.instance.StopLoopingSFX(1);
         
                    }
                    else if (rb.velocity.y < 0)
                    {
                        state = States.Fall;
                    }
                    else if (InputManager.Instance.JumpWasPressedThisFrame())
                    {
                        state = States.Jump;
                        AudioManager.instance.PlaySFX(2);
                        AudioManager.instance.StopLoopingSFX(1);
         
                    }
                    else if (isLanternAimed)
                    {
                        state = States.Aim;
                    }
                break;
                case States.Jump:
                    if (rb.velocity.y <0){
                        state = States.Fall;
                    }
                break;
                case States.Fall:
                    if (rb.velocity.y == 0){
                        AudioManager.instance.PlaySFX(3);
                        if (rb.velocity.x == 0){
                            state = States.Idle;
                        }
                        else{
                            AudioManager.instance.PlayLoopingSFX(1);
                            state = States.Walk;
                        }
                    }
                break;
                case States.Aim:
                    if (!isLanternAimed){
                        if (InputManager.Instance.MovementVector.x == 0){
                            state = States.Idle;
                        }
                        else{
                            state = States.Walk;
                        }
                    }
                break;
            
            }
        }


        private void WalkWalk(float x) // Mueve al jugador en la dirección indicada por el signo de x y con la velocidad máxima indicada por el valor de x
        {
            if ((x < 0 && rb.velocity.x > 0) || (x > 0 && rb.velocity.x < 0)) // Deceleración en cambio de sentido
            {
                WalkDecelerate(WalkDeceleration);
            }
            else // Aceleración en el sentido del movimiento
            {
            Debug.Log(WalkAcceleration);

            Debug.Log("moviendose");
                rb.AddForce(new Vector2(x, 0).normalized * WalkAcceleration, ForceMode2D.Force);
                //rb.AddForce(new Vector2(100,100));
            }

            if (Mathf.Abs(rb.velocity.x) > Mathf.Abs(joystickMaxSpeed)) // Limitación de la velocidad
            {
                if (joystickMaxSpeed == 1 || joystickMaxSpeed == -1)
                {
                    rb.velocity = rb.velocity.normalized * Mathf.Abs(joystickMaxSpeed);
                }
                else
                {
                    WalkDecelerate(WalkAcceleration); // En el caso (nada raro) de que el joystick pase de un valor a otro más bajo del mismo signo, se frena con el valor de la aceleración
                }
            }

        }
        private void WalkDecelerate(float decelerationValue) // Frena al jugador con la aceleración negativa indicada
        {
            if (rb.velocity.x > WalkDecelerationThreshold) // Comprobación de signo para elegir el sentido de la fuerza
            {
                rb.AddForce(new Vector2(-decelerationValue, 0), ForceMode2D.Force);
            }
           else if (rb.velocity.x < -WalkDecelerationThreshold)
            {
                rb.AddForce(new Vector2(decelerationValue, 0), ForceMode2D.Force);
            }
        }

        private void JumpJump()
        {
            rb.AddForce(new Vector2(0, 1) * JumpJumpAcceleration * jumpMultiplier, ForceMode2D.Impulse);
            jumpMultiplier -= JumpJumpMultiplierDecay;
        }
        private void JumpWalk(float x) // Mueve al jugador en la dirección indicada por el signo de x y con la velocidad máxima indicada por el valor de x
        {
            if ((x < 0 && rb.velocity.x > 0) || (x > 0 && rb.velocity.x < 0)) // Deceleración en cambio de sentido
            {
                JumpDecelerate(JumpDeceleration);
            }
            else // Aceleración en el sentido del movimiento
            {
                rb.AddForce(new Vector2(x, 0).normalized * JumpAcceleration, ForceMode2D.Force);
            }

            if (Mathf.Abs(rb.velocity.x) > Mathf.Abs(joystickMaxSpeed)) // Limitación de la velocidad
            {
                if (joystickMaxSpeed == 1 || joystickMaxSpeed == -1)
                {
                    rb.velocity = rb.velocity.normalized * Mathf.Abs(joystickMaxSpeed);
                }
                else
                {
                    JumpDecelerate(JumpAcceleration); // En el caso (nada raro) de que el joystick pase de un valor a otro más bajo del mismo signo, se frena con el valor de la aceleración
                }
            }
        }
        private void JumpDecelerate(float decelerationValue) // Frena al jugador con la aceleración negativa indicada
        {
            if (rb.velocity.x > JumpDecelerationThreshold) // Comprobación de signo para elegir el sentido de la fuerza
            {
                rb.AddForce(new Vector2(-decelerationValue, 0), ForceMode2D.Force);
            }
            else if (rb.velocity.x < - JumpDecelerationThreshold)
            {
                rb.AddForce(new Vector2(decelerationValue, 0), ForceMode2D.Force);
            }
        }
        private void FallWalk(float x) // Mueve al jugador en la dirección indicada por el signo de x y con la velocidad máxima indicada por el valor de x
        {
            if ((x < 0 && rb.velocity.x > 0) || (x > 0 && rb.velocity.x < 0)) // Deceleración en cambio de sentido
            {
                FallDecelerate(FallDeceleration);
            }
            else // Aceleración en el sentido del movimiento
            {
                rb.AddForce(new Vector2(x, 0).normalized * FallAcceleration, ForceMode2D.Force);
            }

            if (Mathf.Abs(rb.velocity.x) > Mathf.Abs(joystickMaxSpeed)) // Limitación de la velocidad
            {
                if (joystickMaxSpeed == 1 || joystickMaxSpeed == -1)
                {
                    rb.velocity = rb.velocity.normalized * Mathf.Abs(joystickMaxSpeed);
                }
                else
                {
                    FallDecelerate(FallAcceleration); // En el caso (nada raro) de que el joystick pase de un valor a otro más bajo del mismo signo, se frena con el valor de la aceleración
                }
            }
    }
    private void FallDecelerate(float FallDecelerationValue) // Frena al jugador con la aceleración negativa indicada
    {
        if (rb.velocity.x > FallDecelerationThreshold) // Comprobación de signo para elegir el sentido de la fuerza
        {
            rb.AddForce(new Vector2(-FallDecelerationValue, 0), ForceMode2D.Force);
        }
        else if (rb.velocity.x < -FallDecelerationThreshold)
        {
            rb.AddForce(new Vector2(FallDecelerationValue, 0), ForceMode2D.Force);
        }
    }
    #endregion   

} // class PlayerMovement 
// namespace
