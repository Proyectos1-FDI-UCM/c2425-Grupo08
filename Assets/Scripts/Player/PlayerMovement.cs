//---------------------------------------------------------
// Lógica del movimiento del jugador. Tiene dos switch, uno para cambiar de estado y otro para ejecutar la lógica específica de cada estado
// Miguel Ángel González López
// Project Abyss
// Proyectos 1 - Curso 2024-25
//---------------------------------------------------------

using UnityEngine;


/// <summary>
/// Clase heredara del MonoBehaviour que lleva la lógica del movimiento y estados del jugador
/// </summary>
public class PlayerMovement : MonoBehaviour
{
    // ---- ATRIBUTOS DEL INSPECTOR ----
    #region Atributos del Inspector (serialized fields)

        [Header("Walk Attributes")]
        [SerializeField]private float WalkAcceleration;
        [SerializeField]private float WalkDeceleration;
        [SerializeField]private float WalkMaxSpeed;
        [SerializeField]private float WalkDecelerationThreshold;

        [Header("Idle Attributes")]

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
 
        [Header("Aim Attributes")]

        [SerializeField]private bool debug;

    #endregion
    
    // ---- ATRIBUTOS PRIVADOS ----
    #region Atributos Privados (private fields)
        /// <summary>
        /// Enum con los estados del jugador.
        /// </summary>
        enum States {
            Idle,
            Walk,
            Fall,
            Jump,
            Aim
        }
        /// <summary>
        ///  Limitador de la velocidad máxima según el desplazamiento del joystick (desplazamiento analógico).
        /// </summary>
        private float _joystickMaxSpeed;
        /// <summary>
        /// RigidBody2D del objeto jugador.
        /// </summary>
        private Rigidbody2D _rb;
        /// <summary>
        ///  True si el jugador está apuntando, False si el jugador no está apuntando.
        /// </summary>
        private bool _isLanternAimed;
        /// <summary>
        /// Multiplicador del salto del jugador que se aplica a la aceleración vertical al saltar y que decrementa mientras se pulsa el botón de salto.
        /// </summary>
        private float _jumpMultiplier = 1;
        /// <summary>
        /// Estado del jugador. Inicializa en Idle.
        /// </summary>
        private States _state = States.Idle;

        private AudioSource audioSource;

    private Animator animator;

    /// <summary>
    /// Un bool que representa si el jugador está reparando. Tiene un setter y un getter como métodos públicos
    /// </summary>
    private bool isRepairing = false;

    #endregion

    // ---- MÉTODOS DE MONOBEHAVIOUR ----
    #region Métodos de MonoBehaviour

    /// <summary>
    /// Start is called on the frame when a script is enabled just before 
    /// any of the Update methods are called the first time.
    /// </summary>
    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        _rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        CheckState(ref _state);

        if (GetComponent<OxigenScript>().IsTankBroken())
        {
            animator.Play(_state.ToString() + "Damaged");
        }
        else
        {
            animator.Play(_state.ToString());
        }    

        animator.speed = _rb.velocity.magnitude / 10;
    }
    public void FixedUpdate()
    {
        switch (_state){
        case States.Idle:
        break;
        case States.Walk:
            if (InputManager.Instance.MovementVector.x != 0)
            {
                _joystickMaxSpeed = WalkMaxSpeed * InputManager.Instance.MovementVector.x;
                WalkWalk(InputManager.Instance.MovementVector.x);
            }
            else WalkDecelerate(WalkDeceleration);
        break;
        case States.Jump:
            if (InputManager.Instance.JumpWasRealeasedThisFrame())
            {
                _jumpMultiplier = 0;
            }
    
            if (InputManager.Instance.JumpIsPressed() && _jumpMultiplier > 0)
            {
                Jump();
            }
    
            if (InputManager.Instance.MovementVector.x != 0)
            {
                _joystickMaxSpeed = JumpMaxSpeed * InputManager.Instance.MovementVector.x;
                JumpWalk(InputManager.Instance.MovementVector.x);
            }
            else JumpDecelerate(JumpDeceleration);
        break;
        case States.Fall:
            if (InputManager.Instance.MovementVector.x != 0)
            {
                _joystickMaxSpeed = FallMaxSpeed * InputManager.Instance.MovementVector.x;
                FallWalk(InputManager.Instance.MovementVector.x);
            }
            else FallDecelerate(FallDeceleration);
            _jumpMultiplier = 1;
        break;
        case States.Aim:
        break;
        }
    }

    #endregion

    // ---- MÉTODOS PÚBLICOS ----
    #region Métodos públicos

    public void SetIsRepairing(bool isRepairing)
    {
        this.isRepairing = isRepairing;
    }
    public bool GetIsRepairing()
    {
        return isRepairing;
    }

    #endregion
    
    // ---- MÉTODOS PRIVADOS ----
    #region Métodos privados
    /// <summary>
    /// Comprueba los parámetros actuales del jugador y cambia de estado dependiendo de ellos. El estado cambia en _state.
    /// </summary>
    /// <param name="_state">Estado pasado por referencia del jugador.</param>
        private void CheckState(ref States _state){
            //Debug.Log(_state);
            
            //Debug.Log(_rb.velocity.y);
            //Debug.Log(_jumpMultiplier);
            switch (_state){
                case States.Idle:

                if (InputManager.Instance.MovementVector.x != 0)
                {
                    _state = States.Walk;
                    AudioManager.instance.PlaySFX(SFXType.Walk, audioSource, true);

                }
                else if (_rb.velocity.y < 0.1)
 
                    {
                        _state = States.Fall;
                    }
                    else if (InputManager.Instance.JumpWasPressedThisFrame())
                    {

                        _state = States.Jump;
                        AudioManager.instance.PlaySFX(SFXType.Jump, audioSource);

                    }
                else if (_isLanternAimed)
                    {
                        _state = States.Aim;
                    }
 
                break;
                case States.Walk:
                    if (_rb.velocity == new Vector2(0, 0))
                    {
                        _state = States.Idle; 
                        AudioManager.instance.StopSFX(audioSource);
         
                    }
                    else if (_rb.velocity.y < 0.1)
                    {
                        _state = States.Fall;
                    }
                    else if (InputManager.Instance.JumpWasPressedThisFrame())
                    {
                        _state = States.Jump;
                        AudioManager.instance.StopSFX(audioSource);
                        AudioManager.instance.PlaySFX(SFXType.Jump, audioSource);


                }
                else if (_isLanternAimed)
                    {
                        _state = States.Aim;
                    }
                break;
                case States.Jump:
                    if (_rb.velocity.y < 0.1){
                        _state = States.Fall;
                    }
                break;
                case States.Fall:
                    if (_rb.velocity.y == 0){
                        AudioManager.instance.PlaySFX(SFXType.Fall, audioSource);
                        if (_rb.velocity.x == 0){
                            _state = States.Idle;
                        }
                        else{
                            AudioManager.instance.PlaySFX(SFXType.Walk, audioSource, true);
                            _state = States.Walk;
                        }
                    }
                break;
                case States.Aim:
                    if (!_isLanternAimed){
                        if (InputManager.Instance.MovementVector.x == 0){
                            _state = States.Idle;
                        }
                        else{
                            _state = States.Walk;
                        }
                    }
                break;
            
            }
        }

        /// <summary>
        /// Método que se encarga de la lógica de mover al jugador.
        /// </summary>
        /// <param name="x">Velocidad en eje x</param>
        private void WalkWalk(float x) // Mueve al jugador en la dirección indicada por el signo de x y con la velocidad máxima indicada por el valor de x
        {
            if ((x < 0 && _rb.velocity.x > 0) || (x > 0 && _rb.velocity.x < 0)) // Deceleración en cambio de sentido
            {
                WalkDecelerate(WalkDeceleration);
            }
            else // Aceleración en el sentido del movimiento
            {
            //Debug.Log(WalkAcceleration);

            //Debug.Log("moviendose");
                _rb.AddForce(new Vector2(x, 0).normalized * WalkAcceleration, ForceMode2D.Force);
            }

            if (Mathf.Abs(_rb.velocity.x) > Mathf.Abs(_joystickMaxSpeed)) // Limitación de la velocidad
            {
                if (_joystickMaxSpeed == 1 || _joystickMaxSpeed == -1)
                {
                    _rb.velocity = _rb.velocity.normalized * Mathf.Abs(_joystickMaxSpeed);
                }
                else
                {
                    WalkDecelerate(WalkAcceleration); // En el caso (nada raro) de que el joystick pase de un valor a otro más bajo del mismo signo, se frena con el valor de la aceleración
                }
            }

        }
        /// <summary>
        /// Método que decelera al jugador cuando deja de moverse.
        /// </summary>
        /// <param name="decelerationValue">Valor de deceleración del jugador al dejar de moverse</param>
        private void WalkDecelerate(float decelerationValue) // Frena al jugador con la aceleración negativa indicada
        {
            if (_rb.velocity.x > WalkDecelerationThreshold) // Comprobación de signo para elegir el sentido de la fuerza
            {
                _rb.AddForce(new Vector2(-decelerationValue, 0), ForceMode2D.Force);
            }
           else if (_rb.velocity.x < -WalkDecelerationThreshold)
            {
                _rb.AddForce(new Vector2(decelerationValue, 0), ForceMode2D.Force);
            }
        }

        /// <summary>
        /// Método que se encarga de la lógica de las físicas cuando el jugador salta.
        /// </summary>
        private void Jump()
        {
            _rb.AddForce(new Vector2(0, 1) * JumpJumpAcceleration * _jumpMultiplier, ForceMode2D.Impulse);
            _jumpMultiplier -= JumpJumpMultiplierDecay;
        }
        /// <summary>
        /// Método que se encarga de la lógica del movimiento en el eje x cuando el jugador está saltando
        /// </summary>
        /// <param name="x">Velocidad en el eje x</param>
        private void JumpWalk(float x) // Mueve al jugador en la dirección indicada por el signo de x y con la velocidad máxima indicada por el valor de x
        {
            if ((x < 0 && _rb.velocity.x > 0) || (x > 0 && _rb.velocity.x < 0)) // Deceleración en cambio de sentido
            {
                JumpDecelerate(JumpDeceleration);
            }
            else // Aceleración en el sentido del movimiento
            {
                _rb.AddForce(new Vector2(x, 0).normalized * JumpAcceleration, ForceMode2D.Force);
            }

            if (Mathf.Abs(_rb.velocity.x) > Mathf.Abs(_joystickMaxSpeed)) // Limitación de la velocidad
            {
                if (_joystickMaxSpeed == 1 || _joystickMaxSpeed == -1)
                {
                    _rb.velocity = _rb.velocity.normalized * Mathf.Abs(_joystickMaxSpeed);
                }
                else
                {
                    JumpDecelerate(JumpAcceleration); // En el caso (nada raro) de que el joystick pase de un valor a otro más bajo del mismo signo, se frena con el valor de la aceleración
                }
            }
        }
        /// <summary>
        /// Reduce el impulso del salto.
        /// </summary>
        /// <param name="decelerationValue">Valor de deceleración del jugador</param>
        private void JumpDecelerate(float decelerationValue) // Frena al jugador con la aceleración negativa indicada
        {
            if (_rb.velocity.x > JumpDecelerationThreshold) // Comprobación de signo para elegir el sentido de la fuerza
            {
                _rb.AddForce(new Vector2(-decelerationValue, 0), ForceMode2D.Force);
            }
            else if (_rb.velocity.x < - JumpDecelerationThreshold)
            {
                _rb.AddForce(new Vector2(decelerationValue, 0), ForceMode2D.Force);
            }
        }
        /// <summary>
        /// Método que se encarga de la lógica del movimiento del jugador en el eje x cuando está en estado de caida.
        /// </summary>
        /// <param name="x">Velocidad en el eje x</param>
        private void FallWalk(float x) // Mueve al jugador en la dirección indicada por el signo de x y con la velocidad máxima indicada por el valor de x
        {
            if ((x < 0 && _rb.velocity.x > 0) || (x > 0 && _rb.velocity.x < 0)) // Deceleración en cambio de sentido
            {
                FallDecelerate(FallDeceleration);
            }
            else // Aceleración en el sentido del movimiento
            {
                _rb.AddForce(new Vector2(x, 0).normalized * FallAcceleration, ForceMode2D.Force);
            }

            if (Mathf.Abs(_rb.velocity.x) > Mathf.Abs(_joystickMaxSpeed)) // Limitación de la velocidad
            {
                if (_joystickMaxSpeed == 1 || _joystickMaxSpeed == -1)
                {
                    _rb.velocity = _rb.velocity.normalized * Mathf.Abs(_joystickMaxSpeed);
                }
                else
                {
                    FallDecelerate(FallAcceleration); // En el caso (nada raro) de que el joystick pase de un valor a otro más bajo del mismo signo, se frena con el valor de la aceleración
                }
            }
    }
    /// <summary>
    /// Método que se encarga de decelerar al jugador durante la caida.
    /// </summary>
    /// <param name="FallDecelerationValue">Valor de deceleración</param>
    private void FallDecelerate(float FallDecelerationValue) // Frena al jugador con la aceleración negativa indicada
    {
        if (_rb.velocity.x > FallDecelerationThreshold) // Comprobación de signo para elegir el sentido de la fuerza
        {
            _rb.AddForce(new Vector2(-FallDecelerationValue, 0), ForceMode2D.Force);
        }
        else if (_rb.velocity.x < -FallDecelerationThreshold)
        {
            _rb.AddForce(new Vector2(FallDecelerationValue, 0), ForceMode2D.Force);
        }
    }
    #endregion   

} // class PlayerMovement 
// namespace
