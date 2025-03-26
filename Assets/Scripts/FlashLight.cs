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

/// <summary>
/// Controla el comportamiento de la linterna del jugador, incluyendo su rotación, enfoque y flash.
/// </summary>
public class FlashLight : MonoBehaviour
{
    // ---- ATRIBUTOS DEL INSPECTOR ----
    [Header("Input Settings")][Space]
    [Range(0, 1)][SerializeField] private float inputDeadzone = 0.1f; // Deadzone para el joystick o ratón

    [Space][Header("General Settings")][Space]
    [SerializeField] private Color32 lightColor = new Color32(0x00, 0x23, 0x4B, 0xFF);
    [Range(1, 50)][SerializeField] private int transitionSpeed = 15;
    [Range(0, 100)][SerializeField] private float lightDiffusion = 10f;
    [Range(0, 5)][SerializeField] private float minIntensity = 0.5f;

    [Space][Header("UnFocus Settings")][Space]
    [Range(1, 180)][SerializeField] private float unfocusRadius = 70f;
    [Range(0, 20)][SerializeField] private float unfocusLength = 6f;
    [Range(1, 150)][SerializeField] private float unfocusIntensity = 30f;

    [Space][Header("Focus Settings")][Space]
    [Range(1, 180)][SerializeField] private float focusRadius = 10f;
    [Range(0, 20)][SerializeField] private float focusLength = 10f;
    [Range(1, 300)][SerializeField] private float focusIntensity = 180f;

    [Space][Header("Flash Settings")][Space]
    [SerializeField] private Color flashColor = Color.white;
    [Range(1, 1000)][SerializeField] private float flashIntensity = 500f;
    [Range(0, 0.1f)][SerializeField] private float flashDuration = 1f;
    [Range(0, 10)][SerializeField] private float flashCooldown = 4f;
    
    [Space][Header("References")][Space]
    [SerializeField] private SpriteRenderer playerSprite; // Referencia al objeto del jugador (TODO: Quitar esto en un futuro)

    // ---- ATRIBUTOS PRIVADOS ----
    private Light2D flashLight; // Referencia a la luz 2D de la linterna
    private bool isFlashAvailable = true;

    // ---- MÉTODOS DE MONOBEHAVIOUR ----
    #region Métodos de MonoBehaviour

    private void Start()
    {
        // Obtener la referencia a la luz 2D de la linterna
        flashLight = GetComponentInChildren<Light2D>();

        if (flashLight != null)
        {
            flashLight.color = lightColor; // Establecer el color de la luz inicial
            flashLight.intensity = unfocusIntensity; // Establecer la intensidad por defecto de la luz
            flashLight.pointLightInnerAngle = unfocusRadius; // Establecer el ángulo interno por defecto de la luz
            flashLight.pointLightOuterAngle = unfocusRadius + lightDiffusion; // Establecer el ángulo externo por defecto de la luz
            flashLight.pointLightOuterRadius = unfocusLength; // Establecer el radio externo por defecto de la luz
        }

        else

            Debug.LogError("No se ha encontrado la luz 2D en el objeto linterna.");
    }

    private void Update()
    {
        LookAtInput(); // Control del movimiento de la linterna con el joystick o el ratón

        if (flashLight != null)
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

    private void LightUnFocus()
    {
        flashLight.pointLightInnerAngle = Mathf.Lerp(flashLight.pointLightInnerAngle, unfocusRadius, Time.deltaTime * transitionSpeed);
        flashLight.pointLightOuterAngle = Mathf.Lerp(flashLight.pointLightOuterAngle, unfocusRadius + lightDiffusion, Time.deltaTime * transitionSpeed);

        flashLight.intensity = Mathf.Lerp(flashLight.intensity, unfocusIntensity, Time.deltaTime * transitionSpeed);

        flashLight.pointLightOuterRadius = Mathf.Lerp(flashLight.pointLightOuterRadius, unfocusLength, Time.deltaTime * transitionSpeed);
    }

    private void LightFocus()
    {
        flashLight.pointLightInnerAngle = Mathf.Lerp(flashLight.pointLightInnerAngle, focusRadius, Time.deltaTime * transitionSpeed);
        flashLight.pointLightOuterAngle = Mathf.Lerp(flashLight.pointLightOuterAngle, focusRadius + lightDiffusion, Time.deltaTime * transitionSpeed);

        flashLight.intensity = Mathf.Lerp(flashLight.intensity, focusIntensity, Time.deltaTime * transitionSpeed);

        flashLight.pointLightOuterRadius = Mathf.Lerp(flashLight.pointLightOuterRadius, focusLength, Time.deltaTime * transitionSpeed);
    }

    private IEnumerator LightFlash()
    {
        // Initial flash setup
        flashLight.color = flashColor;
        flashLight.intensity = flashIntensity;
        flashLight.pointLightOuterRadius = focusLength * 2f;
        flashLight.pointLightInnerAngle = focusRadius * 2f;

        // Disable flash availability
        isFlashAvailable = false;

        // Hold flash effect briefly
        yield return new WaitForSeconds(flashDuration);

        // Gradually reduce intensity
        float elapsedTime = 0f;
        float fadeDuration = 0.5f;
        float startIntensity = flashIntensity;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / fadeDuration;
            flashLight.intensity = Mathf.Lerp(startIntensity, 0f, t);
            yield return null;
        }

        // Reset to normal values
        flashLight.color = lightColor;
        flashLight.intensity = unfocusIntensity;
        flashLight.pointLightInnerAngle = unfocusRadius;
        flashLight.pointLightOuterAngle = unfocusRadius + lightDiffusion;

        // Cooldown period
        yield return new WaitForSeconds(flashCooldown);
        isFlashAvailable = true;
    }

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

    // class Lantern 
    // namespace
}