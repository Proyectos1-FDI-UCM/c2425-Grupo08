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
using System.Collections.Generic;
using UnityEngine.Rendering.Universal;

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
    [SerializeField] private float inputDeadzone = 0.1f; // Zona muerta para el joystick o ratón

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
    [Range(1, 1000)]
    [SerializeField] private float flashIntensity = 500f;
    [Tooltip("Duración del flash")]
    [Range(0, 0.1f)]
    [SerializeField] private float flashDuration = 1f;
    [Tooltip("Tiempo de recarga del flash")]
    [Range(0, 10)]
    [SerializeField] private float flashCooldown = 4f;

    [Space]
    [Header("Referencias")]
    [Space]
    [Tooltip("Sprite del jugador")]
    [SerializeField] private SpriteRenderer playerSprite; // Referencia al objeto del jugador (TODO: Quitar esto en un futuro)

    // ---- ATRIBUTOS PRIVADOS ----
    private Light2D flashLight; // Referencia a la luz 2D de la linterna
    private PolygonCollider2D flashCollider; // Referencia al polygon collider de la luz 2D

    private bool isFlashAvailable = true;

    // ---- MÉTODOS DE MONOBEHAVIOUR ----
    #region Métodos de MonoBehaviour

    private void Start()
    {
        // Obtener la referencia a la luz 2D de la linterna
        flashLight = GetComponentInChildren<Light2D>();

        // Obtener la referencia a la luz 2D de la linterna
        flashCollider = GetComponentInChildren<PolygonCollider2D>();

        if (flashLight != null && flashCollider != null)

            SetDefaults(); // Configura la luz y su collider

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

        if (flashLight != null && flashCollider != null)
        {
            if (isFlashAvailable)
            {
                if (InputManager.Instance.FocusIsPressed())
                {
                    if (InputManager.Instance.FlashIsPressed())

                        StartCoroutine(LightFlash());

                    else

                        LightFocus();
                }

                else

                    LightUnFocus();
            }

            else

                StartCoroutine(LightFlicker());
        }
    }

    #endregion

    // ---- MÉTODOS PRIVADOS ----
    #region Métodos privados

    /// <summary>
    /// Método para manejar la entrada del ratón o joystick derecho y rotar la linterna.
    /// Este método calcula la dirección del cursor respecto al jugador y ajusta la rotación de la linterna en consecuencia.
    /// </summary>
    private void LookAtInput()
    {
        Vector2 aimInput = ((Vector2)Camera.main.ScreenToWorldPoint(InputManager.Instance.AimVector) - (Vector2)transform.position).normalized;
        //Vector2 aimInput = InputManager.Instance.AimVector;

        // Cambiar direción según si el cursor esta a izquierda o derecha
        playerSprite.flipX = aimInput.x < 0; //TODO: Cambiar esta lógica al PlayerController

        if (aimInput.magnitude > inputDeadzone) // Para que no haya movimientos raros cerca del pivote
        {
            float angle = Mathf.Atan2(aimInput.y, aimInput.x) * Mathf.Rad2Deg;

            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }

    /// <summary>
    /// Método para establecer los valores por defecto de la linterna.
    /// Este método establece la intensidad, el color y los ángulos de la luz de la linterna.
    /// </summary>
    private void SetDefaults()
    {
        flashLight.color = lightColor; // Establecer el color de la luz inicial
        flashLight.intensity = unfocusIntensity; // Establecer la intensidad por defecto de la luz
        flashLight.pointLightInnerAngle = unfocusRadius; // Establecer el ángulo interno por defecto de la luz
        flashLight.pointLightOuterAngle = unfocusRadius + lightDiffusion; // Establecer el ángulo externo por defecto de la luz
        flashLight.pointLightOuterRadius = unfocusLength; // Establecer el radio externo por defecto de la luz

        SetCollider(); // Ajustar el collider de la luz a su tamaño en modo focus
    }

    /// <summary>
    /// Método para establecer el collider de la linterna.
    /// Este método calcula los puntos del collider en función de la longitud y el ángulo de la luz de la linterna.
    /// </summary>
    private void SetCollider()
    {
        flashCollider.enabled = false; // Desactivar el collider al inicio

        float outerAngle = focusRadius + lightDiffusion;
        float halfOuterAngle = outerAngle * 0.5f;

        // Los dos puntos en los extremos del cono (asumiendo que la linterna apunta a la derecha)
        Vector2 point1 = Quaternion.Euler(0, 0, halfOuterAngle) * Vector2.right * focusLength;
        Vector2 point2 = Quaternion.Euler(0, 0, -halfOuterAngle) * Vector2.right * focusLength;

        // Asignamos los 3 puntos: origen y los dos extremos
        flashCollider.points = new Vector2[] { Vector2.zero, point1, point2 };
    }

    /// <summary>
    /// Método para desactivar el enfoque de la linterna.
    /// Este método ajusta la intensidad, el radio y los ángulos de la luz de la linterna para simular un efecto de desenfoque.
    /// </summary>
    private void LightUnFocus()
    {
        flashLight.pointLightInnerAngle = Mathf.Lerp(flashLight.pointLightInnerAngle, unfocusRadius, Time.deltaTime * transitionSpeed);
        flashLight.pointLightOuterAngle = Mathf.Lerp(flashLight.pointLightOuterAngle, unfocusRadius + lightDiffusion, Time.deltaTime * transitionSpeed);

        flashLight.intensity = Mathf.Lerp(flashLight.intensity, unfocusIntensity, Time.deltaTime * transitionSpeed);

        flashLight.pointLightOuterRadius = Mathf.Lerp(flashLight.pointLightOuterRadius, unfocusLength, Time.deltaTime * transitionSpeed);
    }

    /// <summary>
    /// Método para activar el enfoque de la linterna.
    /// Este método ajusta la intensidad, el radio y los ángulos de la luz de la linterna para simular un efecto de enfoque.
    /// </summary>
    private void LightFocus()
    {
        flashLight.pointLightInnerAngle = Mathf.Lerp(flashLight.pointLightInnerAngle, focusRadius, Time.deltaTime * transitionSpeed);
        flashLight.pointLightOuterAngle = Mathf.Lerp(flashLight.pointLightOuterAngle, focusRadius + lightDiffusion, Time.deltaTime * transitionSpeed);

        flashLight.intensity = Mathf.Lerp(flashLight.intensity, focusIntensity, Time.deltaTime * transitionSpeed);

        flashLight.pointLightOuterRadius = Mathf.Lerp(flashLight.pointLightOuterRadius, focusLength, Time.deltaTime * transitionSpeed);
    }

    /// <summary>
    /// Método para realizar un flashazo con la linterna.
    /// Este método ajusta la intensidad, el color y los ángulos de la luz de la linterna para simular un efecto de flash.
    /// </summary>
    private IEnumerator LightFlash()
    {
        // Cambiar el color y la intensidad a los valores del flash
        flashLight.color = flashColor;
        flashLight.intensity = flashIntensity;
        flashLight.pointLightOuterRadius = focusLength * 2f;
        flashLight.pointLightInnerAngle = focusRadius * 2f;

        isFlashAvailable = false; // Desactivar el flash hasta que termine la animación

        flashCollider.enabled = true; // Activar el collider para que la luz pueda colisionar con los objetos

        yield return new WaitForSeconds(flashDuration);

        flashCollider.enabled = false; // Desactivar el collider después del flash

        SetDefaults(); /// Volver a los valores por defecto de la linterna

        // Esperar el tiempo de recarga del flash
        yield return new WaitForSeconds(flashCooldown);
        isFlashAvailable = true;
    }

    /// <summary>
    /// Método para simular un parpadeo de la linterna.
    /// Este método ajusta la intensidad y el radio de la luz de la linterna para simular un efecto de parpadeo.
    /// </summary>
    private IEnumerator LightFlicker()
    {
        flashLight.intensity = minIntensity;
        flashLight.pointLightOuterRadius = unfocusLength * 0.3f;
        yield return new WaitForSeconds(1f);

        flashLight.intensity = 0f;
        flashLight.pointLightOuterRadius = unfocusLength * 0.1f;
        yield return new WaitForSeconds(0.5f);
    }

    #endregion

    // class FlashLight 
    // namespace
}