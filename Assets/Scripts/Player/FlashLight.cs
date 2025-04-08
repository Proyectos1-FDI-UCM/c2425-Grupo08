//---------------------------------------------------------
// Script que gestiona la linterna del jugador, incluyendo su rotación y el haz de luz.
// Este script permite al jugador apuntar y enfocar la linterna, así como realizar un flashazo.

// Vicente Rodriguez Casado
// Carlos Dochao Moreno
// Proyect Abyss
// Proyectos 1 - Curso 2024-25
//---------------------------------------------------------

using UnityEngine;
using System.Collections;
using UnityEngine.Rendering.Universal;
using UnityEditorInternal;

/// <summary>
/// Controla el comportamiento de la linterna del jugador, incluyendo su rotación, enfoque y flash.
/// </summary>
public class FlashLight : MonoBehaviour
{
    // ---- ATRIBUTOS DEL INSPECTOR ----
    [Header("Configuración de Entrada")]
    [Space]
    [Tooltip("Zona muerta para joystick o ratón (entre 0 y 1)")]
    [Range(0, 1)]
    [SerializeField] private float inputDeadzone = 0.1f;
    [Tooltip("Velocidad de rotación de la linterna")]
    [Range(1, 50)]
    [SerializeField] private int lookSpeed = 15;

    [Space]
    [Header("Configuración General")]
    [Space]
    [Tooltip("Color inicial de la luz")]
    [SerializeField] private Color32 lightColor = new Color32(0x00, 0x23, 0x4B, 0xFF);
    [Tooltip("Velocidad de transición de la linterna")]
    [Range(1, 50)]
    [SerializeField] private int transitionSpeed = 15;
    [Tooltip("Difusión de la luz")]
    [Range(0, 100)]
    [SerializeField] private float lightDiffusion = 10f;
    [Tooltip("Intensidad mínima de la luz")]
    [Range(0, 5)]
    [SerializeField] private float minIntensity = 0.5f;

    [Space]
    [Header("Configuración Desenfoque")]
    [Space]
    [Tooltip("Radio de desenfoque")]
    [Range(1, 180)]
    [SerializeField] private float unfocusRadius = 70f;
    [Tooltip("Longitud en modo desenfoque")]
    [Range(0, 20)]
    [SerializeField] private float unfocusLength = 6f;
    [Tooltip("Intensidad de la luz en modo desenfoque")]
    [Range(1, 150)]
    [SerializeField] private float unfocusIntensity = 30f;

    [Space]
    [Header("Configuración Enfoque")]
    [Space]
    [Tooltip("Radio de enfoque")]
    [Range(1, 180)]
    [SerializeField] private float focusRadius = 10f;
    [Tooltip("Longitud en modo enfoque")]
    [Range(0, 20)]
    [SerializeField] private float focusLength = 10f;
    [Tooltip("Intensidad de la luz en modo enfoque")]
    [Range(1, 300)]
    [SerializeField] private float focusIntensity = 180f;

    [Space]
    [Header("Configuración Flash")]
    [Space]
    [Tooltip("Color del flash")]
    [SerializeField] private Color flashColor = Color.white;
    [Tooltip("Intensidad del flash")]
    [Range(1, 2000)]
    [SerializeField] private float flashIntensity = 1000f;
    [Tooltip("Tiempo de recarga del flash")]
    [Range(0, 10)]
    [SerializeField] private float flashCooldown = 4f;

    [Space]
    [Header("Referencias")]
    [Space]
    [Tooltip("Sprite del jugador")]
    [SerializeField] private SpriteRenderer playerSprite; // Referencia al objeto del jugador (TODO: Quitar esto en un futuro?)

    // ---- ATRIBUTOS PRIVADOS ----

    /// <summary>
    /// Enum con los estados de la linterna.
    /// </summary>
    enum State { Unfocus, Focus, Flash, Cooldown };

    private State _state = State.Unfocus; // Estado inicial de la linterna

    private Light2D flashLight; // Referencia a la luz 2D de la linterna
    private PolygonCollider2D flashCollider; // Referencia al polygon collider de la luz 2D

    private bool isFlashAvailable = true;
    private float flashTimer;
    private float flashColliderTimer = 0f;

    private struct LightValues
    {
        public float intensity;
        public float innerAngle;
        public float outerAngle;
        public float outerRadius;
        public Color color;
    }

    private LightValues currentValues;
    private LightValues targetValues;

    // ---- MÉTODOS DE MONOBEHAVIOUR ----
    #region Métodos de MonoBehaviour

    private void Start()
    {
        // Obtener la referencia a la luz 2D de la linterna
        flashLight = GetComponentInChildren<Light2D>();

        // Obtener la referencia al collider de la linterna
        flashCollider = GetComponentInChildren<PolygonCollider2D>();

        if (flashLight != null && flashCollider != null)
        {
            SetDefaults(); // Configura la luz y su collider
            currentValues = GetCurrentLightValues();
            targetValues = currentValues;
        }
        else
        {
            if (flashLight == null)
                Debug.LogError("No se ha encontrado la luz 2D en el objeto linterna.");

            if (flashCollider == null)
                Debug.LogError("No se ha encontrado el PolygonCollider2D en el objeto linterna.");
        }
    }

    private void Update()
    {
        LookAtInput(); // Control del movimiento de la linterna con el joystick o el ratón

        // Actualizar cooldown
        if (!isFlashAvailable)
        {
            flashTimer -= Time.deltaTime;

            if (flashTimer <= 0f)
            {
                isFlashAvailable = true;
                flashTimer = 0f;
            }
        }

        // Controlar duración del flashCollider
        if (flashCollider.enabled)
        {
            flashColliderTimer -= Time.deltaTime;
            if (flashColliderTimer <= 0f)
            {
                flashCollider.enabled = false;
            }
        }

        if (flashLight != null && flashCollider != null)
        {
            LerpValues(); // Interpolar los valores de la luz

            ChangeState(); // Cambiar el estado de la linterna según la entrada del jugador
        }

        switch (_state) // Cambiar el estado de la linterna según la entrada del jugador
        {
            case State.Unfocus:

                LightUnfocus();

                break;

            case State.Focus:

                LightFocus();

                break;

            case State.Flash:

                LightFlash();

                break;

            case State.Cooldown:

                LightFlicker();

                break;
        }
    }

    #endregion

    // ---- MÉTODOS PRIVADOS ----
    #region Métodos privados

    private void ChangeState()
    {
        if (isFlashAvailable)
        {
            if (InputManager.Instance.FocusIsPressed())
            {
                if (InputManager.Instance.FlashIsPressed())

                    _state = State.Flash; // Cambiar el estado a Flash

                else

                    _state = State.Focus; // Cambiar el estado a Focus
            }

            else

                _state = State.Unfocus; // Cambiar el estado a Unfocus
        }

        else

            _state = State.Cooldown; // Cambiar el estado a Cooldown
    }

    /// <summary>
    /// Método para manejar la entrada del ratón o joystick derecho y rotar la linterna.
    /// Este método calcula la dirección del cursor respecto al jugador y ajusta la rotación de la linterna en consecuencia.
    /// </summary>
    private void LookAtInput()
    {
        Vector2 aimInput = ((Vector2)Camera.main.ScreenToWorldPoint(InputManager.Instance.AimVector) - (Vector2)transform.position).normalized;

        // Cambiar direción según si el cursor esta a izquierda o derecha
        playerSprite.flipX = aimInput.x < 0; //TODO: Cambiar esta lógica al PlayerController

        if (aimInput.magnitude > inputDeadzone) // Para que no haya movimientos raros cerca del pivote
        {
            float angle = Mathf.Atan2(aimInput.y, aimInput.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, Mathf.LerpAngle(transform.rotation.eulerAngles.z, angle, Time.deltaTime * lookSpeed));
        }
    }

    /// <summary>
    /// Método para establecer los valores por defecto de la linterna.
    /// Este método establece la intensidad, el color y los ángulos de la luz de la linterna.
    /// </summary>
    private void SetDefaults()
    {
        targetValues = new LightValues
        {
            intensity = unfocusIntensity,
            innerAngle = unfocusRadius,
            outerAngle = unfocusRadius + lightDiffusion,
            outerRadius = unfocusLength,
            color = lightColor
        };

        currentValues = targetValues;

        flashLight.color = lightColor;
        flashLight.intensity = unfocusIntensity;
        flashLight.pointLightInnerAngle = unfocusRadius;
        flashLight.pointLightOuterAngle = unfocusRadius + lightDiffusion;
        flashLight.pointLightOuterRadius = unfocusLength;
        flashCollider.enabled = false;
    }

    private LightValues GetCurrentLightValues()
    {
        return new LightValues
        {
            intensity = flashLight.intensity,
            innerAngle = flashLight.pointLightInnerAngle,
            outerAngle = flashLight.pointLightOuterAngle,
            outerRadius = flashLight.pointLightOuterRadius,
            color = flashLight.color
        };
    }

    private void LerpValues()
    {
        float currentTransitionSpeed = _state == State.Cooldown ? transitionSpeed * 0.3f : transitionSpeed;

        currentValues.intensity = Mathf.Lerp(currentValues.intensity, targetValues.intensity, Time.deltaTime * currentTransitionSpeed);
        currentValues.innerAngle = Mathf.Lerp(currentValues.innerAngle, targetValues.innerAngle, Time.deltaTime * currentTransitionSpeed);
        currentValues.outerAngle = Mathf.Lerp(currentValues.outerAngle, targetValues.outerAngle, Time.deltaTime * currentTransitionSpeed);
        currentValues.outerRadius = Mathf.Lerp(currentValues.outerRadius, targetValues.outerRadius, Time.deltaTime * currentTransitionSpeed);

        flashLight.intensity = currentValues.intensity;
        flashLight.pointLightInnerAngle = currentValues.innerAngle;
        flashLight.pointLightOuterAngle = currentValues.outerAngle;
        flashLight.pointLightOuterRadius = currentValues.outerRadius;
        flashLight.color = targetValues.color;
    }

    private void LightUnfocus()
    {
        targetValues.intensity = unfocusIntensity;
        targetValues.innerAngle = unfocusRadius;
        targetValues.outerAngle = unfocusRadius + lightDiffusion;
        targetValues.outerRadius = unfocusLength;
        targetValues.color = lightColor;

        flashCollider.enabled = false;
    }

    private void LightFocus()
    {
        targetValues.intensity = focusIntensity;
        targetValues.innerAngle = focusRadius;
        targetValues.outerAngle = focusRadius + lightDiffusion;
        targetValues.outerRadius = focusLength;
        targetValues.color = lightColor;

        flashCollider.enabled = false;
    }

    private void LightFlash()
    {
        // Aplicar intensidad máxima inmediatamente
        flashLight.intensity = flashIntensity;    // Aplicar directamente al flashLight
        flashLight.pointLightInnerAngle = focusRadius * 2.5f;
        flashLight.pointLightOuterAngle = (focusRadius * 2.5f) + (lightDiffusion * 2f);
        flashLight.pointLightOuterRadius = focusLength * 2.5f;
        flashLight.color = Color.white;

        // Actualizar valores objetivo para la transición posterior
        targetValues.intensity = flashIntensity;
        targetValues.innerAngle = focusRadius * 2.5f;
        targetValues.outerAngle = (focusRadius * 2.5f) + (lightDiffusion * 2f);
        targetValues.outerRadius = focusLength * 2.5f;
        targetValues.color = Color.white;

        // Actualizar valores actuales para evitar interpolación
        currentValues = targetValues;

        flashCollider.enabled = true;
        flashColliderTimer = 0.001f; // activar solo 1 ms

        isFlashAvailable = false;
        flashTimer = flashCooldown;
        _state = State.Cooldown;
    }

    private void LightFlicker()
    {
        float normalizedTime = flashTimer / flashCooldown;
        float flickerSpeed = 12f;

        // Mantener la linterna apagada durante el primer segundo después del flash
        if (normalizedTime > 0.75f)
        {
            targetValues.intensity = 0f;
        }

        else
        {
            bool isFlickerOn = Mathf.Sin(Time.time * flickerSpeed) > Mathf.Lerp(-0.2f, 0.8f, normalizedTime);
            targetValues.intensity = isFlickerOn ? minIntensity : 0f;
        }

        targetValues.innerAngle = unfocusRadius;
        targetValues.outerAngle = unfocusRadius + lightDiffusion;
        targetValues.outerRadius = unfocusLength;
        targetValues.color = lightColor;

        flashCollider.enabled = false;
    }

    #endregion

    // class FlashLight 
    // namespace
}