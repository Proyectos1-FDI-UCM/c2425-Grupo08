//---------------------------------------------------------
// Script que gestiona la linterna del jugador, incluyendo su rotación y el haz de luz.
// Este script permite al jugador apuntar y enfocar la linterna, así como realizar un flashazo.

// Vicente Rodriguez Casado
// Carlos Dochao Moreno
// Proyect Abyss
// Proyectos 1 - Curso 2024-25
//---------------------------------------------------------

using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

/// <summary>
/// Controla el comportamiento de la linterna del jugador, incluyendo su rotación, enfoque y flash.
/// </summary>
public class FlashLight : MonoBehaviour
{
    // ---- ATRIBUTOS DEL INSPECTOR ----
    [Range(0, 1)]
    [SerializeField] private float inputDeadzone = 0.1f; // Zona muerta para la entrada del mouse respecto a la posición del jugador

    [SerializeField] private SpriteRenderer playerSprite; // Referencia al objeto del jugador (TODO: Quitar esto en un futuro)

    // ---- ATRIBUTOS PRIVADOS ----

    private Light2D flashLight; // Referencia a la luz 2D de la linterna

    [SerializeField] private float flashIntensity = 2f;
    [SerializeField] private float flashDuration = 0.2f;
    [SerializeField] private float flashCooldown = 1f;

    private float defaultIntensity;
    private bool isFlashAvailable = true;
    private bool canFlash = true;

    // ---- MÉTODOS DE MONOBEHAVIOUR ----
    #region Métodos de MonoBehaviour

    private void Start()
    {
        // Obtener la referencia a la luz 2D de la linterna
        flashLight = GetComponentInChildren<Light2D>();
    }

    private void Update()
    {
        // Control del movimiento de la linterna con el joystick o el ratón
        AimAtInput();

        if (InputManager.Instance.FocusIsPressed())
        {
            //Focus();
        }
        else
        {
            //UnFocus();
        }
    }

    #endregion

    // ---- MÉTODOS PÚBLICOS ----
    #region Métodos públicos
    #endregion

    // ---- MÉTODOS PRIVADOS ----
    #region Métodos privados

    /// <summary>
    /// Método para manejar la entrada del ratón o joystick derecho y rotar la linterna.
    /// Este método calcula la dirección del cursor respecto al jugador y ajusta la rotación de la linterna en consecuencia.
    /// </summary>
    private void AimAtInput()
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

    private void Focus()
    {
        if (flashLight != null) {
            flashLight.pointLightInnerAngle = Mathf.Lerp(flashLight.pointLightInnerAngle, 0, Time.deltaTime * 5f);
            flashLight.pointLightOuterRadius = Mathf.Lerp(flashLight.pointLightOuterRadius, 0, Time.deltaTime * 5f);
        }
        
    }

    private void UnFocus()
    {
        if (flashLight != null)
        {
            flashLight.pointLightInnerAngle = Mathf.Lerp(flashLight.pointLightInnerAngle, 180, Time.deltaTime * 5f);
            flashLight.pointLightOuterRadius = Mathf.Lerp(flashLight.pointLightOuterRadius, 5, Time.deltaTime * 5f);
        }
    }

    private IEnumerator Flash() // Método para hacer el flash de la linterna
    {
        flashLight.intensity = flashIntensity;
        yield return new WaitForSeconds(flashDuration);
        flashLight.intensity = defaultIntensity;

        isFlashAvailable = false;
        yield return new WaitForSeconds(flashCooldown);
        canFlash = true;
    }

    #endregion

    // class Lantern 
    // namespace
}