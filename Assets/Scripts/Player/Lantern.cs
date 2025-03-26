//---------------------------------------------------------
// La linterna deberá tener un haz regulable (ancho-fino) que siga la posición del cursor/dirección
// del joystick derecho y ofrezca un ángulo de visión.

// Vicente Rodriguez Casado
// Proyect Abyss
// Proyectos 1 - Curso 2024-25
//---------------------------------------------------------


using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

/// <summary>
/// Antes de cada class, descripción de qué es y para qué sirve,
/// usando todas las líneas que sean necesarias.
/// </summary>
public class Lantern : MonoBehaviour
{
    // ---- ATRIBUTOS DEL INSPECTOR ----
    [SerializeField] private float beamGrowSpeed; // Velocidad a la que el haz crece
    [SerializeField] private float maxBeamLength; // Valor objetivo para pointLightOuterAngle al enfocar
    [SerializeField] private float minBeamWidth; // Valor objetivo para pointLightInnerAngle al enfocar
    [SerializeField] private float flashCooldown; // Cooldown del flash (linterna apagada)
    [SerializeField] private float inputDeadzone; // Umbral de movimiento para detectar si el ratón se mueve
    [SerializeField] private GameObject playerSprite; // Sprite del jugador
    [SerializeField] private float focusedBeamLengthMultiplier = 1.5f;
    [SerializeField] private float flashIntensityIncrease = 1f;

    // ---- ATRIBUTOS PRIVADOS ----
    private bool isFocus = false; // Indica si el clic derecho está presionado
    private bool isCooldownActive = false; // Indica si el cooldown de la linterna está activo
    private GameObject beamObject; // Objeto que contiene la luz

    // Nuevas variables para Light2D
    private Light2D lanternLight;
    private float initialInnerAngle;
    private float initialOuterAngle;
    private float initialOuterRadius; // nuevo campo para almacenar el radio original

    // ---- MÉTODOS DE MONOBEHAVIOUR ----

    #region Métodos de MonoBehaviour

    private void Start()
    {
        // Adignamos el player (siendo este el primer hijo del padre de la linterna)

        // Asignamos el hijo corrspondiente al beam (el LightBeam)
        beamObject = transform.GetChild(0).gameObject;

        // Obtener Light2D del beamObject
        lanternLight = beamObject.GetComponent<Light2D>();
        initialOuterAngle = lanternLight.pointLightOuterAngle;
        initialInnerAngle = lanternLight.pointLightInnerAngle;
        initialOuterRadius = lanternLight.pointLightOuterRadius; // asignación del radio original
    }

    private void Update()
    {
        // Control del movimiento de la linterna con el joystick o el ratón
        AimAtInput();
        // Detectar si el clic derecho está siendo presionado
        HandleBeamGrowth();
        // Verificar si se puede hacer un flash
        Flash();
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

    // Método para apuntar la linterna hacia el ratón o joystick
    private void AimAtInput()
    {
        Vector2 aimInput = ((Vector2)Camera.main.ScreenToWorldPoint(InputManager.Instance.AimVector) - (Vector2)transform.position).normalized;

        //Vector2 aimInput = InputManager.Instance.AimVector;

        playerSprite.GetComponent<SpriteRenderer>().flipX = aimInput.x < 0; // Cambiar direción según si el cursor esta a izquierda o derecha

        if (aimInput.magnitude > inputDeadzone) // Para que no haya movimientos raros cerca del pivote
        {
            float angle = Mathf.Atan2(aimInput.y, aimInput.x) * Mathf.Rad2Deg;

            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }

    // Método para manejar el haz de luz basado en la entrada del usuario
    private void HandleBeamGrowth()
    {
        if (InputManager.Instance.FocusIsPressed())
        {
            isFocus = true;

            if (!isCooldownActive)
            {
                GetComponentInParent<PlayerScript>().isLanternAimed = true;
                StartCoroutine(FocusLight());
            }
        }
        else
        {
            GetComponentInParent<PlayerScript>().isLanternAimed = false;

            // Si no se presionan los botones, se indica que el crecimiento ya no está activo.
            isFocus = false;

            // Si no hay cooldown activo, se inicia la corutina para retraer el haz de luz.
            if (!isCooldownActive)
                StartCoroutine(UnFocusLight());
        }
    }

    private void Flash() // Método para hacer el flash de la linterna
    {
        // No permitir el flash si el cooldown está activo
        if (isCooldownActive) return;

        // Verificar si el clic derecho del ratón o el RT del mando están siendo presionados
        if (InputManager.Instance.FocusIsPressed() && InputManager.Instance.FlashIsPressed())
        {
            GetComponentInParent<PlayerScript>().isLanternAimed = false;

            StartCoroutine(FlashRoutine());
            // Se elimina el llamado al cooldown

        }
    }

    // ---- MÉTODOS DE BEAM ----

    // Actualizada FocusLight: primero aumenta el outer radius y luego reduce el inner angle
    private IEnumerator FocusLight()
    {
        float targetRadius = initialOuterRadius * focusedBeamLengthMultiplier;

        while (lanternLight.pointLightOuterRadius < targetRadius)
        {
            if (!isFocus)
                yield break;
            lanternLight.pointLightOuterRadius += beamGrowSpeed * Time.deltaTime;
            yield return null;
        }

        while (lanternLight.pointLightInnerAngle > minBeamWidth)
        {
            if (!isFocus)
                yield break;
            lanternLight.pointLightInnerAngle -= beamGrowSpeed * Time.deltaTime;
            yield return null;
        }
    }

    // Actualizada UnFocusLight: primero restaura el inner angle y luego el outer radius a sus valores originales
    private IEnumerator UnFocusLight()
    {
        // Restaurar inner angle
        while (lanternLight.pointLightInnerAngle < initialInnerAngle)
        {
            lanternLight.pointLightInnerAngle += beamGrowSpeed * Time.deltaTime;
            yield return null;
        }
        // Luego restaurar el outer radius
        while (lanternLight.pointLightOuterRadius > initialOuterRadius)
        {
            lanternLight.pointLightOuterRadius -= beamGrowSpeed * Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator FlashRoutine()
    {
        isCooldownActive = true;
        float originalIntensity = lanternLight.intensity; 

        lanternLight.intensity = flashIntensityIncrease;// Flashazo
        yield return new WaitForSeconds(0.1f);
       
        lanternLight.intensity = 0.5f; // Apaga la linterna o se atenúa la intensidad
        yield return new WaitForSeconds(flashCooldown);

        lanternLight.intensity = originalIntensity;
        isCooldownActive = false;
    }

    #endregion

    // class Lantern 
    // namespace
}