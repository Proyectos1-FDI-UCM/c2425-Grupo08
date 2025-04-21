//---------------------------------------------------------
// Script que gestiona la linterna del jugador, incluyendo su rotación y el haz de luz.
// Este script permite al jugador apuntar y enfocar la linterna, así como realizar un flashazo.

// Vicente Rodriguez Casado
// Carlos Dochao Moreno
// Proyect Abyss
// Proyectos 1 - Curso 2024-25
//---------------------------------------------------------

using UnityEngine;
using UnityEngine.Rendering.Universal;

/// <summary>
/// Controla el comportamiento de la linterna del jugador, incluyendo su rotación, enfoque y flash.
/// </summary>
public class FlashLight : MonoBehaviour
{
    // ---- ATRIBUTOS DEL INSPECTOR ----
    #region Atributos del Inspector

    [Header("Input Settings")]
    [Space]
    [Tooltip("Zona muerta para joystick o ratón (entre 0 y 1)")]
    [Range(0, 1)]
    [SerializeField] private float inputDeadzone = 0.1f;
    [Tooltip("Velocidad de rotación de la linterna")]
    [Range(1, 50)]
    [SerializeField] private float aimSpeed = 15;

    [Space]
    [Header("General Settings")]
    [Space]
    [Tooltip("Difusión de la luz")]
    [Range(0, 80)]
    [SerializeField] private float lightDiffusion = 7f;
    [Tooltip("Velocidad de transición entre estados")]
    [Range(1, 25)]
    [SerializeField] private float transitionSpeed = 15;
    [Tooltip("Intensidad mínima de la luz")]
    [Range(0, 5)]
    [SerializeField] private float minIntensity = 0.5f;
    [Tooltip("Velocidad de parpadeo en cooldown")]
    [Range(1, 25)]
    [SerializeField] private float flickerSpeed = 12f;
    [Tooltip("Tiempo para volver a usar el flash (en segundos)")]
    [Range(0, 10)]
    [SerializeField] private float flashCooldown = 4f;

    [Space]
    [Header("Unfocus Settings")]
    [Space]
    [Tooltip("Ángulo de desenfoque")]
    [Range(0, 180)]
    [SerializeField] private float unfocusAngle = 70f;
    [Tooltip("Longitud en modo desenfoque")]
    [Range(0, 20)]
    [SerializeField] private float unfocusLength = 6f;
    [Tooltip("Intensidad de la luz en modo desenfoque")]
    [Range(0, 150)]
    [SerializeField] private float unfocusIntensity = 30f;

    [Space]
    [Header("Focus Settings")]
    [Space]
    [Tooltip("Ángulo de enfoque")]
    [Range(0, 180)]
    [SerializeField] private float focusAngle = 10f;
    [Tooltip("Longitud en modo enfoque")]
    [Range(0, 20)]
    [SerializeField] private float focusLength = 10f;
    [Tooltip("Intensidad de la luz en modo enfoque")]
    [Range(0, 300)]
    [SerializeField] private float focusIntensity = 180f;

    [Space]
    [Header("Flash Settings")]
    [Space]
    [Tooltip("Multiplicador de valores (ángulo, radio...) para el flash")]
    [Range(1, 10)]
    [SerializeField] private float flashMultiplier = 2f;
    [Tooltip("Apagón del flash (en segundos)")]
    [Range(0, 20)]
    [SerializeField] private float flashFalloff = 4.5f;
    [Tooltip("Tiempo de aparición del collider")]
    [Range(0, 0.1f)]
    [SerializeField] private float colliderTime = 0.02f;

    [Space]
    [Header("References (remove in future)")]
    [Space]
    [Tooltip("Sprite del jugador")]
    [SerializeField] private SpriteRenderer playerSprite; // Referencia al objeto del jugador (TODO: Quitar esto en un futuro?)

    #endregion

    // ---- ATRIBUTOS PRIVADOS ----

    #region Atributos Privados

    private Light2D flashLight; // Referencia a la luz 2D de la linterna

    private PolygonCollider2D flashCollider; // Referencia al polygon collider de la luz 2D

    private PlayerMovement player; // Referencia al script del jugador (PlayerMovement)

    private AudioSource audioSource; // Referencia al AudioSource de la linterna

    public struct LightValues // Estructura para almacenar los valores de la luz
    {
        public float intensity;
        public float angle;
        public float length;
    }

    private LightValues target; // Valores objetivo de la luz

    private bool canFlash = true; // Indica si se puede usar el flash

    private float timer; // Temporizador para el cooldown del flash

    private float tmp; // Variable temporal para guardar la velocidad de transición

    #endregion

    // ---- MÉTODOS DE MONOBEHAVIOUR ----
    #region Métodos de MonoBehaviour

    private void Start()
    {
        // Guardar la velocidad de transición original
        tmp = transitionSpeed;

        // Obtener la referencia a la luz 2D de la linterna
        flashLight = GetComponentInChildren<Light2D>();

        // Obtener la referencia al collider de la linterna
        flashCollider = GetComponentInChildren<PolygonCollider2D>();

        // Obtener la referencia al PlayerMovement del jugador
        player = GameManager.Instance.GetPlayerController().GetComponent<PlayerMovement>();

        //Obtener la referencia del audiosource de la linterna
        audioSource = GetComponent<AudioSource>();

        if (flashLight != null && flashCollider != null && player != null)
        {
            SetDefaults(target); // Configura la luz y su collider por defecto
        }

        else
        {
            if (flashLight == null)

                Debug.LogError("No se ha encontrado la luz 2D en el objeto linterna.");

            if (flashCollider == null)

                Debug.LogError("No se ha encontrado el PolygonCollider2D en el objeto linterna.");

            if (player == null)

                Debug.LogError("No se ha encontrado el script PlayerMovement en el objeto jugador.");
        }
    }

    private void Update()
    {
        if (player.GetIsDeath())
        {
            LerpValues(); // Interpolar los valores de la luz

            LightUnfocus(); // Desenfocar la linterna (default)
        }

        else
        {
            LookAtInput(); // Control del movimiento de la linterna con el joystick o el ratón

            if (flashLight != null && flashCollider != null)
            {
                LerpValues(); // Interpolar los valores de la luz

                ChangeState(); // Cambiar el estado de la linterna según la entrada del jugador
            }
        }
    }

    #endregion

    // ---- MÉTODOS PRIVADOS ----
    #region Métodos privados

    private void ChangeState()
    {
        if (canFlash)
        {
            if (InputManager.Instance.FocusIsPressed() && !player.GetIsRepairing())
            {
                if (InputManager.Instance.FlashIsPressed())

                    LightFlash(); // Activa el flash de la linterna

                else

                    LightFocus(); // Enfoca la linterna
            }

            else

                LightUnfocus(); // Desenfoca la linterna (default)
        }

        else

            LightFlicker(); // Parpadea la linterna durante el cooldown después del flash
    }

    /// <summary>
    /// Método para manejar la entrada del ratón o joystick derecho y rotar la linterna.
    /// Este método calcula la dirección del cursor respecto al jugador y ajusta la rotación de la linterna en consecuencia.
    /// </summary>
    private void LookAtInput()
    {
        if (!InputManager.Instance.IsGamepadActive()) // Si el ratón está activo, usar el ratón para rotar la linterna
        {
            // Obtener la posición del cursor en el mundo y calcular la distancia al jugador
            Vector2 cursorPos = InputManager.Instance.GetWorldCursorPos();

            // Calcular la distancia entre el cursor y el jugador
            float distanceToCursor = Vector2.Distance(cursorPos, transform.position);

            // Cambiar la direción del sprite según si el cursor esta a izquierda o derecha
            Vector2 mouseInput = cursorPos - (Vector2)transform.position;
            
            // Solo rotar si el cursor está fuera del círculo del radio inputDeadzone
            if (distanceToCursor > inputDeadzone)
            {
                float angle = Mathf.Atan2(mouseInput.y, mouseInput.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0, 0, Mathf.LerpAngle(transform.rotation.eulerAngles.z, angle, Time.deltaTime * aimSpeed)); 
            }
            
            if (Mathf.Abs(mouseInput.x) > inputDeadzone) // Rotar el sprite según la dirección del cursor (condicional para evitar bugs)

                playerSprite.flipX = mouseInput.x < 0; // Cambiar esta lógica al PlayerMovement?
        }

        else // Si el gamepad está activo, usar el joystick derecho para rotar la linterna
        {
            // Obtener la dirección del joystick derecho y calcular la distancia al jugador
            Vector2 gamepadInput = InputManager.Instance.GetRightStickInput();

            // Solo rotar si el joystick supera el radio inputDeadzone
            if (gamepadInput.magnitude > inputDeadzone)
            {
                float angle = Mathf.Atan2(gamepadInput.y, gamepadInput.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0, 0, Mathf.LerpAngle(transform.rotation.eulerAngles.z, angle, Time.deltaTime * aimSpeed));

                // Cambiar la dirección del sprite según si el cursor esta a izquierda o derecha
                playerSprite.flipX = gamepadInput.x < 0; // Cambiar esta lógica al PlayerMovement?
            }
        }
    }

    /// <summary>
    /// Método para interpolar los valores de la linterna entre el estado actual y el objetivo.
    /// Este método ajusta la intensidad, el ángulo y la longitud de la luz de la linterna de forma suave.
    /// </summary>
    private void LerpValues()
    {
        flashLight.intensity = Mathf.Lerp(flashLight.intensity, target.intensity, Time.deltaTime * transitionSpeed);
        flashLight.pointLightInnerAngle = Mathf.Lerp(flashLight.pointLightInnerAngle, target.angle, Time.deltaTime * transitionSpeed);
        flashLight.pointLightOuterAngle = flashLight.pointLightInnerAngle + lightDiffusion; // Se interpola dependiente de innerAngle
        flashLight.pointLightOuterRadius = Mathf.Lerp(flashLight.pointLightOuterRadius, target.length, Time.deltaTime * transitionSpeed);
    }

    #endregion

    // ---- MÉTODOS PUBLICOS ----
    #region Métodos públicos

    /// <summary>
    /// Método para establecer los valores por defecto de la linterna.
    /// Este método establece la intensidad y los ángulos de la luz de la linterna, así como el estado de su collider.
    /// </summary>
    public void SetDefaults(LightValues target)
    {
        // Establecer los valores por defecto de la linterna (unfocus)
        flashLight.intensity = unfocusIntensity;
        flashLight.pointLightInnerAngle = unfocusAngle;
        flashLight.pointLightOuterAngle = unfocusAngle + lightDiffusion;
        flashLight.pointLightOuterRadius = unfocusLength;

        // Establecer los valores por defecto del struct LightValues target (unfocus)
        target.intensity = unfocusIntensity;
        target.angle = unfocusAngle;
        target.length = unfocusLength;

        // Establecer el estado por defecto del flash-collider (desactivado)
        flashCollider.enabled = false;
    }

    /// <summary>
    /// Método para establecer los valores de la linterna en modo desenfoque.
    /// Este método establece la intensidad, el ángulo y la longitud de la luz de la linterna en modo desenfoque.
    /// </summary>
    public void LightUnfocus()
    {
        target.intensity = unfocusIntensity;
        target.angle = unfocusAngle;
        target.length = unfocusLength;
    }

    /// <summary>
    /// Método para establecer los valores de la linterna en modo enfoque.
    /// Este método establece la intensidad, el ángulo y la longitud de la luz de la linterna en modo enfoque.
    /// </summary>
    public void LightFocus()
    {
        target.intensity = focusIntensity;
        target.angle = focusAngle;
        target.length = focusLength;
    }

    /// <summary>
    /// Método para activar el flash de la linterna.
    /// Este método establece la intensidad, el ángulo y la longitud de la luz de la linterna en modo flash.
    /// </summary>
    public void LightFlash()
    {
        // Aplicar intensidad máxima inmediatamente
        flashLight.intensity = focusIntensity * flashMultiplier;
        flashLight.pointLightInnerAngle = focusAngle * flashMultiplier;
        flashLight.pointLightOuterAngle = flashLight.pointLightInnerAngle + lightDiffusion;
        flashLight.pointLightOuterRadius = focusLength * flashMultiplier;
        AudioManager.instance.PlaySFX(SFXType.FlashLight, audioSource); // Reproducir el sonido del flash

        // Establecer el estado del flash-collider (activado)
        flashCollider.enabled = true;

        // Establecer la disponibilidad del flash
        canFlash = false;
        player.SetcanFlash(canFlash);

        // Reducir la velocidad de transición para después del flash
        transitionSpeed = flashFalloff;

        // Reiniciar el temporizador para el cooldown del flash
        timer = 0f;
    }

    /// <summary>
    /// Método para manejar el parpadeo de la linterna durante el cooldown del flash.
    /// Este método alterna la intensidad de la luz entre un valor mínimo y cero, y ajusta el estado del collider.
    /// </summary>
    public void LightFlicker()
    {
        // Incrementa el temporizador con el tiempo transcurrido desde el último frame
        timer += Time.deltaTime;

        // Desactiva el collider tras un breve periodo para que el flash no sea permanente
        if (timer > colliderTime)

            flashCollider.enabled = false;

        // Si ha pasado el tiempo de cooldown, permite volver a usar el flash y restablece valores
        if (timer > flashCooldown)
        {
            canFlash = true;
            player.SetcanFlash(canFlash);
            timer = 0f;
            transitionSpeed = tmp;
        }

        // Hace parpadear la luz alternando la intensidad entre un valor mínimo
        if (Mathf.Sin(timer * flickerSpeed) > 0)

            target.intensity = minIntensity;

        else // Y uno aún menor

            target.intensity = minIntensity / 3f;

        // Mantiene el ángulo y longitud en modo desenfoque durante el parpadeo
        target.angle = unfocusAngle;
        target.length = unfocusLength;
    }

    #endregion
    
} // class FlashLight 