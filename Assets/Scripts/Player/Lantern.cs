//---------------------------------------------------------
// La linterna deberá tener un haz regulable (ancho-fino) que siga la posición del cursor/dirección
// del joystick derecho y ofrezca un ángulo de visión.

// Vicente Rodriguez Casado
// Proyect Abyss
// Proyectos 1 - Curso 2024-25
//---------------------------------------------------------

using PlayerLogic;
using System.Collections;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem;

/// <summary>
/// Antes de cada class, descripción de qué es y para qué sirve,
/// usando todas las líneas que sean necesarias.
/// </summary>
public class Lantern : MonoBehaviour
{
    // ---- ATRIBUTOS DEL INSPECTOR ----
    [SerializeField] private float beamGrowSpeed = 2f; // Velocidad a la que el haz crece
    [SerializeField] private float maxBeamLength = 5f; // Longitud máxima del haz de luz
    [SerializeField] private float minBeamWidth = 1f; // Ancho mínimo cuando se apunta (en el eje Y)
    [SerializeField] private float flashCooldown = 5f; //Cooldown del flash (linterna apagada)
    [SerializeField] private float inputDeadzone; // Umbral de movimiento para detectar si el ratón se mueve

    // ---- ATRIBUTOS PRIVADOS ----
    private Vector3 initialBeamScale; // Para guardar la escala inicial del haz de luz
    private bool isFocus = false; // Indica si el clic derecho está presionado
    private SpriteRenderer secondChildSpriteRenderer;//Sprite luz de flash
    private PolygonCollider2D secondChildPolygonCollider; //Collider de flash
    private bool isCooldownActive = false; // Indica si el cooldown de la linterna está activo
    private GameObject beamObject; // EL haz de luz
    private GameObject player; //Referencia al jugador (deberia ser el padre)

    // ---- MÉTODOS DE MONOBEHAVIOUR ----

    #region Métodos de MonoBehaviour

    private void Start()
    {
        // Adignamos el player (siendo este el primer hijo del padre de la linterna)
        player = GetComponentInParent<PlayerScript>().gameObject.transform.GetChild(0).gameObject;

        // Asignamos el hijo corrspondiente al beam (el LightBeam)
        beamObject = transform.GetChild(0).gameObject;

        // Guardamos la escala original del haz de luz cuando se inicia el juego
        initialBeamScale = beamObject.transform.localScale;

        // Obtener el segundo hijo (con SpriteRenderer y BoxCollider2D)
        Transform secondChild = transform.GetChild(1); // El segundo hijo (índice 1)
        secondChildSpriteRenderer = secondChild.GetComponent<SpriteRenderer>(); // Obtener SpriteRenderer
        secondChildPolygonCollider = secondChild.GetComponent<PolygonCollider2D>(); // Obtener BoxCollider2D

        // Desactivar inicialmente los componentes del segundo hijo
        secondChildSpriteRenderer.enabled = false; // Desactivar SpriteRenderer del segundo hijo
        secondChildPolygonCollider.enabled = false; // Desactivar BoxCollider2D del segundo hijo
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

        player.GetComponent<SpriteRenderer>().flipX = aimInput.x < 0; // Cambiar direción según si el cursor esta a izquierda o derecha

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

        // Verificar si el clic izquierdo del ratón o el RT del mando están siendo presionados
        if (InputManager.Instance.FocusIsPressed() && InputManager.Instance.FlashIsPressed())
        {
            GetComponentInParent<PlayerScript>().isLanternAimed = false;

            StartCoroutine(FlashRoutine());
            StartCoroutine(LanternCooldown());

        }
    }

    // ---- MÉTODOS DE BEAM ----

    // Corutina para aumentar el largo del haz de luz (GrowBeam)
    private IEnumerator FocusLight()
    {
        // Mientras la longitud actual del haz sea menor a la máxima permitida
        // Y el ancho actual sea mayor al mínimo permitido:
        while (beamObject.transform.localScale.x < maxBeamLength && beamObject.transform.localScale.y > minBeamWidth)
        {
            // Si se suelta el botón de crecimiento (isFocus pasa a false),
            // se termina la corutina de forma inmediata usando yield break.
            if (!isFocus)
                yield break;

            // Se incrementa la longitud (eje X) del haz de luz en función de la velocidad de crecimiento y el tiempo transcurrido.
            // Simultáneamente se reduce el ancho (eje Y) pero sin bajar de un ancho mínimo (minBeamWidth).
            beamObject.transform.localScale = new Vector3(
                beamObject.transform.localScale.x + beamGrowSpeed * Time.deltaTime,
                Mathf.Max(minBeamWidth, beamObject.transform.localScale.y - beamGrowSpeed * Time.deltaTime),
                beamObject.transform.localScale.z
            );

            // Se espera hasta el siguiente frame para continuar el ciclo.
            yield return null;
        }
    }

    private IEnumerator UnFocusLight()
    {
        // Volvemos al tamaño original gradualmente cuando se suelta el clic derecho
        while (beamObject.transform.localScale.x > initialBeamScale.x || beamObject.transform.localScale.y < initialBeamScale.y)
        {
            // Reducimos la longitud y aumentamos el ancho gradualmente hasta los valores originales
            beamObject.transform.localScale = new Vector3(
                Mathf.Max(initialBeamScale.x, beamObject.transform.localScale.x - beamGrowSpeed * Time.deltaTime),
                Mathf.Min(initialBeamScale.y, beamObject.transform.localScale.y + beamGrowSpeed * Time.deltaTime),
                beamObject.transform.localScale.z
            );

            yield return null;
        }
    }

    private IEnumerator FlashRoutine()
    {
        // Afectar al segundo hijo: activar SpriteRenderer y BoxCollider2D
        secondChildSpriteRenderer.enabled = true;
        secondChildPolygonCollider.enabled = true;
        yield return new WaitForSeconds(0.1f);
        // Desactivar SpriteRenderer y BoxCollider2D del segundo hijo
        secondChildSpriteRenderer.enabled = false;
        secondChildPolygonCollider.enabled = false;
    }

    private IEnumerator LanternCooldown()
    {
        isCooldownActive = true; // Activa el cooldown

        beamObject.SetActive(false);  // Afectar solo al primer hijo: desactivar SpriteRenderer

        yield return new WaitForSeconds(flashCooldown);

        beamObject.SetActive(true);  // Reactivar SpriteRenderer del primer hijo

        isCooldownActive = false; // Desactiva el cooldown
    }

    #endregion

    // class Lantern 
    // namespace
}