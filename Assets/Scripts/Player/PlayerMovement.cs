//---------------------------------------------------------
// Lógica del movimiento del jugador. Tiene dos switch, uno para cambiar de estado 
// y otro para ejecutar la lógica específica de cada estado
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
    [SerializeField] private float WalkAcceleration;
    [SerializeField] private float WalkDeceleration;
    [SerializeField] private float WalkMaxSpeed;
    [SerializeField] private float WalkDecelerationThreshold;

    [Header("Idle Attributes")]

    [Header("Jump Attributes")]
    [SerializeField] private float JumpAcceleration;
    [SerializeField] private float JumpDeceleration;
    [SerializeField] private float JumpMaxSpeed;
    [SerializeField] private float JumpDecelerationThreshold;
    [SerializeField] private float JumpJumpAcceleration;
    [SerializeField] private float JumpJumpMultiplierDecay;

    [Header("Fall Attributes")]
    [SerializeField] private float FallAcceleration;
    [SerializeField] private float FallDeceleration;
    [SerializeField] private float FallMaxSpeed;
    [SerializeField] private float FallDecelerationThreshold;

    [Header("Aim Attributes")]
    [SerializeField] private float AimAcceleration;
    [SerializeField] private float AimDeceleration;
    [SerializeField] private float AimMaxSpeed;
    [SerializeField] private float AimDecelerationThreshold;

    [Space]
    [SerializeField] private bool debug;
    [SerializeField] private float KelpForce = 1f;

    #endregion

    // ---- ATRIBUTOS PRIVADOS ----
    #region Atributos Privados (private fields)
    /// <summary>
    /// Enum con los estados del jugador.
    /// </summary>
    enum States
    {
        Idle,
        Walk,
        Fall,
        Jump,
        Aim
    }
    /// <summary>
    /// Limitador de la velocidad máxima según el desplazamiento del joystick (desplazamiento analógico).
    /// </summary>
    private float _joystickMaxSpeed;
    /// <summary>
    /// RigidBody2D del objeto jugador.
    /// </summary>
    private Rigidbody2D _rb;
    /// <summary>
    /// Multiplicador del salto del jugador que se aplica a la aceleración vertical al saltar y que decrementa mientras se pulsa el botón de salto.
    /// </summary>
    private float _jumpMultiplier = 1;
    /// <summary>
    /// Estado del jugador. Inicializa en Idle.
    /// </summary>
    private States _state = States.Idle;
    /// <summary>
    /// Referencia al componente de sonido del jugador.
    /// </summary>
    private AudioSource audioSource;
    /// <summary>
    /// El jugador está o no está siendo agarrado por un alga
    /// </summary>
    public bool isGrabbed { get; set; }
    /// <summary>
    /// Referencia al alga que está agarrando al jugador
    /// </summary>
    public KelpEnemy kelpGrabbing { get; set; }
    /// <summary>
    /// Referencia al componente de animación del jugador.
    /// </summary>
    private Animator animator;
    /// <summary>
    /// El jugador puede o no flashear.
    /// </summary>
    private bool canFlash = true;
    /// <summary>
    /// Un bool que representa si el jugador está reparando. Tiene un setter y un getter como métodos públicos.
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

        if (animator.GetCurrentAnimatorClipInfo(0)[0].clip.name == "IdleDamaged" ||
            animator.GetCurrentAnimatorClipInfo(0)[0].clip.name == "Idle" ||
            animator.GetCurrentAnimatorClipInfo(0)[0].clip.name == "death")
        {
            animator.speed = 1;
        }
        else
        {
            animator.speed = _rb.velocity.magnitude / 5;
        }
    }
    public void FixedUpdate()
    {
        switch (_state)
        {
            case States.Idle:
                break;
            case States.Walk:
                if (InputManager.Instance.MovementVector.x != 0)
                {
                    // Se asigna _joystickMaxSpeed como WalkMaxSpeed, sin modificar el signo.
                    _joystickMaxSpeed = WalkMaxSpeed;
                    WalkWalk(InputManager.Instance.MovementVector.x);
                }
                else
                    WalkDecelerate(WalkDeceleration);
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
                    _joystickMaxSpeed = JumpMaxSpeed;
                    JumpWalk(InputManager.Instance.MovementVector.x);
                }
                else
                    JumpDecelerate(JumpDeceleration);
                break;
            case States.Fall:
                if (InputManager.Instance.MovementVector.x != 0)
                {
                    _joystickMaxSpeed = FallMaxSpeed;
                    FallWalk(InputManager.Instance.MovementVector.x);
                }
                else
                    FallDecelerate(FallDeceleration);
                _jumpMultiplier = 1;
                break;
            case States.Aim:
                if (InputManager.Instance.MovementVector.x != 0)
                {
                    _joystickMaxSpeed = AimMaxSpeed;
                    AimWalk(InputManager.Instance.MovementVector.x);
                }
                else
                    AimDecelerate(AimDeceleration);
                break;
        }
        if (IsBeingGrabbed())
        {
            GrabbedMovement();
        }
    }

    #endregion

    // ---- MÉTODOS PÚBLICOS ----
    #region Métodos públicos

    public void SetIsRepairing(bool isRepairing)
    {
        this.isRepairing = isRepairing;
    }
    public void SetcanFlash(bool canFlash)
    {
        this.canFlash = canFlash;
    }
    public bool GetIsRepairing()
    {
        return isRepairing;
    }

    #endregion

    // ---- MÉTODOS PRIVADOS ----
    #region Métodos privados

    private void AnimationState(States state)
    {
        switch (state)
        {
            case States.Idle:
                animator.SetInteger("State", 0);
                break;
            case States.Walk:
                animator.SetInteger("State", 1);
                break;
            case States.Jump:
                animator.SetInteger("State", 2);
                break;
            case States.Fall:
                animator.SetInteger("State", 3);
                break;
        }
    }

    /// <summary>
    /// Comprueba los parámetros actuales del jugador y cambia de estado dependiendo de ellos.
    /// El estado cambia en _state.
    /// </summary>
    /// <param name="_state">Estado pasado por referencia del jugador.</param>
    private void CheckState(ref States _state)
    {
        switch (_state)
        {
            case States.Idle:
                if (InputManager.Instance.MovementVector.x != 0)
                {
                    _state = States.Walk;
                    AudioManager.instance.PlaySFX(SFXType.Walk, audioSource, true);
                    AnimationState(_state);
                }
                else if (_rb.velocity.y < -0.1f)
                {
                    _state = States.Fall;
                    AnimationState(_state);
                }
                else if (InputManager.Instance.JumpWasPressedThisFrame())
                {
                    _state = States.Jump;
                    AudioManager.instance.PlaySFX(SFXType.Jump, audioSource);
                    AnimationState(_state);
                }
                else if (IsAiming() && canFlash)
                {
                    _state = States.Aim;
                }
                break;
            case States.Walk:
                // Se mantiene Walk mientras haya input horizontal, sin forzar Idle por poca velocidad.
                if (InputManager.Instance.MovementVector.x == 0)
                {
                    _state = States.Idle;
                    AudioManager.instance.StopSFX(audioSource);
                    AnimationState(_state);
                }
                else if (_rb.velocity.y < -0.1f)
                {
                    _state = States.Fall;
                    AnimationState(_state);
                }
                else if (InputManager.Instance.JumpWasPressedThisFrame())
                {
                    _state = States.Jump;
                    AudioManager.instance.StopSFX(audioSource);
                    AudioManager.instance.PlaySFX(SFXType.Jump, audioSource);
                    AnimationState(_state);
                }
                else if (IsAiming() && canFlash)
                {
                    _state = States.Aim;
                }
                break;
            case States.Jump:
                if (_rb.velocity.y < -0.1f)
                {
                    _state = States.Fall;
                    AnimationState(_state);
                }
                break;
            case States.Fall:
                if (_rb.velocity.y >= -0.1f)
                {
                    AudioManager.instance.PlaySFX(SFXType.Fall, audioSource);
                    if (_rb.velocity.x == 0f)
                    {
                        _state = States.Idle;
                    }
                    else
                    {
                        AudioManager.instance.PlaySFX(SFXType.Walk, audioSource, true);
                        _state = States.Walk;
                    }
                    AnimationState(_state);
                }
                break;
            case States.Aim:
                if (!IsAiming() || !canFlash)
                {
                    if (InputManager.Instance.MovementVector.x == 0)
                    {
                        _state = States.Idle;
                    }
                    else
                    {
                        _state = States.Walk;
                    }
                    AnimationState(_state);
                }
                /*else if (_rb.velocity.y < -0.1f)
                {
                    _state = States.Fall;
                    AnimationState(_state);
                }
                else if (InputManager.Instance.JumpWasPressedThisFrame())
                {
                    _state = States.Jump;
                    AudioManager.instance.StopSFX(audioSource);
                    AudioManager.instance.PlaySFX(SFXType.Jump, audioSource);
                    AnimationState(_state);
                }*/
                break;
        }
    }

    /// <summary>
    /// Método que se encarga de la lógica de mover al jugador.
    /// </summary>
    /// <param name="x">Velocidad en eje x</param>
    private void WalkWalk(float x)
    {
        // Si existe conflicto entre la dirección del input y la velocidad actual, restablecemos la velocidad
        if (x > 0 && _rb.velocity.x < 0)
        {
            _rb.velocity = new Vector2(0, _rb.velocity.y);
        }
        else if (x < 0 && _rb.velocity.x > 0)
        {
            _rb.velocity = new Vector2(0, _rb.velocity.y);
        }
            _rb.AddForce(new Vector2(x, 0).normalized * WalkAcceleration, ForceMode2D.Force);

        if (Mathf.Abs(_rb.velocity.x) > Mathf.Abs(_joystickMaxSpeed))
        {
            if (_joystickMaxSpeed == 1 || _joystickMaxSpeed == -1)
            {
                _rb.velocity = _rb.velocity.normalized * Mathf.Abs(_joystickMaxSpeed);
            }
            else
            {
                WalkDecelerate(WalkAcceleration);
            }
        }
    }

    /// <summary>
    /// Método que decelera al jugador cuando deja de moverse.
    /// </summary>
    /// <param name="decelerationValue">Valor de deceleración del jugador al dejar de moverse</param>
    private void WalkDecelerate(float decelerationValue)
    {
        if (_rb.velocity.x > WalkDecelerationThreshold)
        {
            _rb.AddForce(new Vector2(-decelerationValue, 0), ForceMode2D.Force);
        }
        else if (_rb.velocity.x < -WalkDecelerationThreshold)
        {
            _rb.AddForce(new Vector2(decelerationValue, 0), ForceMode2D.Force);
        }
    }

    /// <summary>
    /// Método que se encarga de la lógica de mover al jugador.
    /// </summary>
    /// <param name="x">Velocidad en eje x</param>
    private void AimWalk(float x)
    {
        // Si existe conflicto entre la dirección del input y la velocidad actual, restablecemos la velocidad
        if (x > 0 && _rb.velocity.x < 0)
        {
            _rb.velocity = new Vector2(0, _rb.velocity.y);
        }
        else if (x < 0 && _rb.velocity.x > 0)
        {
            _rb.velocity = new Vector2(0, _rb.velocity.y);
        }
        _rb.AddForce(new Vector2(x, 0).normalized * AimAcceleration, ForceMode2D.Force);

        if (Mathf.Abs(_rb.velocity.x) > Mathf.Abs(_joystickMaxSpeed))
        {
            if (_joystickMaxSpeed == 1 || _joystickMaxSpeed == -1)
            {
                _rb.velocity = _rb.velocity.normalized * Mathf.Abs(_joystickMaxSpeed);
            }
            else
            {
                AimDecelerate(AimAcceleration);
            }
        }
    }

    /// <summary>
    /// Método que decelera al jugador cuando deja de moverse si esta apuntando.
    /// </summary>
    /// <param name="decelerationValue">Valor de deceleración del jugador al dejar de moverse</param>
    private void AimDecelerate(float decelerationValue)
    {
        if (_rb.velocity.x > AimDecelerationThreshold)
        {
            _rb.AddForce(new Vector2(-decelerationValue, 0), ForceMode2D.Force);
        }
        else if (_rb.velocity.x < -AimDecelerationThreshold)
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
    /// Método que se encarga de la lógica del movimiento en el eje x cuando el jugador está saltando.
    /// </summary>
    /// <param name="x">Velocidad en el eje x</param>
    private void JumpWalk(float x)
    {
        // Si existe conflicto entre la dirección del input y la velocidad actual, restablecemos la velocidad.
        if (x > 0 && _rb.velocity.x < 0)
        {
            _rb.velocity = new Vector2(0, _rb.velocity.y);
        }
        else if (x < 0 && _rb.velocity.x > 0)
        {
            _rb.velocity = new Vector2(0, _rb.velocity.y);
        }

        _rb.AddForce(new Vector2(x, 0).normalized * JumpAcceleration, ForceMode2D.Force);

        if (Mathf.Abs(_rb.velocity.x) > Mathf.Abs(_joystickMaxSpeed))
        {
            if (_joystickMaxSpeed == 1 || _joystickMaxSpeed == -1)
            {
                _rb.velocity = _rb.velocity.normalized * Mathf.Abs(_joystickMaxSpeed);
            }
            else
            {
                JumpDecelerate(JumpAcceleration);
            }
        }
    }

    /// <summary>
    /// Reduce el impulso del salto.
    /// </summary>
    /// <param name="decelerationValue">Valor de deceleración del jugador</param>
    private void JumpDecelerate(float decelerationValue)
    {
        if (_rb.velocity.x > JumpDecelerationThreshold)
        {
            _rb.AddForce(new Vector2(-decelerationValue, 0), ForceMode2D.Force);
        }
        else if (_rb.velocity.x < -JumpDecelerationThreshold)
        {
            _rb.AddForce(new Vector2(decelerationValue, 0), ForceMode2D.Force);
        }
    }

    /// <summary>
    /// Método que se encarga de la lógica del movimiento del jugador en el eje x cuando está en estado de caida.
    /// </summary>
    /// <param name="x">Velocidad en el eje x</param>
    private void FallWalk(float x)
    {
        if ((x < 0 && _rb.velocity.x > 0) || (x > 0 && _rb.velocity.x < 0))
        {
            FallDecelerate(FallDeceleration);
        }
        else
        {
            _rb.AddForce(new Vector2(x, 0).normalized * FallAcceleration, ForceMode2D.Force);
        }

        if (Mathf.Abs(_rb.velocity.x) > Mathf.Abs(_joystickMaxSpeed))
        {
            if (_joystickMaxSpeed == 1 || _joystickMaxSpeed == -1)
            {
                _rb.velocity = _rb.velocity.normalized * Mathf.Abs(_joystickMaxSpeed);
            }
            else
            {
                FallDecelerate(FallAcceleration);
            }
        }
    }

    /// <summary>
    /// Método que se encarga de decelerar al jugador durante la caida.
    /// </summary>
    /// <param name="FallDecelerationValue">Valor de deceleración</param>
    private void FallDecelerate(float FallDecelerationValue)
    {
        if (_rb.velocity.x > FallDecelerationThreshold)
        {
            _rb.AddForce(new Vector2(-FallDecelerationValue, 0), ForceMode2D.Force);
        }
        else if (_rb.velocity.x < -FallDecelerationThreshold)
        {
            _rb.AddForce(new Vector2(FallDecelerationValue, 0), ForceMode2D.Force);
        }
    }

    /// <summary>
    /// Aplica una fuerza al jugador hacia las coordenadas del alga cuando este está siendo agarrado.
    /// </summary>
    private void GrabbedMovement()
    {
        _rb.AddForce((kelpGrabbing.transform.position - transform.position) * KelpForce);
    }
    /// <summary>
    /// Avisa al enemigo alga que tenía agarrado al jugador de que este se ha escapado.
    /// </summary>
    private void ReleaseKelp()
    {
        if (kelpGrabbing != null)
        {
            kelpGrabbing.ReleasePlayer();
        }
    }
    /// <summary>
    /// Devuelve true si el jugador está siendo agarrado por un alga.
    /// </summary>
    /// <returns></returns>
    private bool IsBeingGrabbed()
    {
        if (kelpGrabbing != null && isGrabbed)
        {
            return true;
        }
        return false;
    }
    private bool IsAiming()
    {
        return InputManager.Instance.FocusIsPressed();
    }
    #endregion   
} // class PlayerMovement
// namespace
