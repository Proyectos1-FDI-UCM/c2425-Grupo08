    //---------------------------------------------------------
// La linterna deberá tener un haz regulable (ancho-fino) que siga la posición del cursor/dirección
// del joystick derecho y ofrezca un ángulo de visión.

// Vicente Rodriguez Casado
// Proyect Abyss
// Proyectos 1 - Curso 2024-25
//---------------------------------------------------------

using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.UIElements.UxmlAttributeDescription;



/// <summary>
/// Antes de cada class, descripción de qué es y para qué sirve,
/// usando todas las líneas que sean necesarias.
/// </summary>
public class LanternVicentePrototipo : MonoBehaviour
{
    // ---- ATRIBUTOS DEL INSPECTOR ----
    [SerializeField] private Transform player;  // El jugador alrededor del cual se mueve la linterna
    [SerializeField] private float radius = 2f; // Radio del círculo sobre el que se mueve la linterna
    [SerializeField] private GameObject beamObject; // El objeto rectángulo que representa el haz
    [SerializeField] private float beamGrowSpeed = 2f; // Velocidad a la que el haz crece
    [SerializeField] private float maxBeamLength = 5f; // Longitud máxima del haz de luz
    [SerializeField] private float minBeamWidth = 1; // Ancho mínimo cuando se apunta (en el eje Y)
    [SerializeField] private float flashCooldown = 5; //Cooldown del flash (linterna apagada)



    // ---- ATRIBUTOS PRIVADOS ----



    private float currentAngle = 0f; // Ángulo actual de la linterna en el círculo
    private bool isJoystickActive = false; // Variable que indica si el joystick está siendo usado
    private Vector2 lastMousePosition; // Última posición conocida del ratón
    private float mouseMoveThreshold = 0.1f; // Umbral de movimiento para detectar si el ratón se mueve
    private Vector3 initialBeamScale; // Para guardar la escala inicial del haz de luz
    private bool isRightClickPressed = false; // Indica si el clic derecho está presionado
    private SpriteRenderer firstChildSpriteRenderer; //Sprite luz de linterna
    private SpriteRenderer secondChildSpriteRenderer;//Sprite luz de flash
    private BoxCollider2D secondChildBoxCollider; //Collider de flash
    private bool isCooldownActive = false; // Indica si el cooldown de la linterna está activo

    // ---- MÉTODOS DE MONOBEHAVIOUR ----

    private void Start()
    {
        // Guardamos la escala original del haz de luz cuando se inicia el juego
        initialBeamScale = beamObject.transform.localScale;
        // Hacer invisible el cursor

        // Obtener el primer hijo (con solo SpriteRenderer)
        Transform firstChildPivot = transform.GetChild(0); // El primer hijo (el pivote vacío)
        firstChildSpriteRenderer = firstChildPivot.GetChild(0).GetComponent<SpriteRenderer>(); // Obtener SpriteRenderer del hijo dentro del pivote

        // Obtener el segundo hijo (con SpriteRenderer y BoxCollider2D)
        Transform secondChild = transform.GetChild(1); // El segundo hijo (índice 1)
        secondChildSpriteRenderer = secondChild.GetComponent<SpriteRenderer>(); // Obtener SpriteRenderer
        secondChildBoxCollider = secondChild.GetComponent<BoxCollider2D>(); // Obtener BoxCollider2D

        // Desactivar inicialmente los componentes del segundo hijo
        secondChildSpriteRenderer.enabled = false; // Desactivar SpriteRenderer del segundo hijo
        secondChildBoxCollider.enabled = false; // Desactivar BoxCollider2D del segundo hijo
    }

    private void Update()
    {
        // Control del movimiento de la linterna con el joystick o el ratón
        ControlMovement();

        // Detectar si el clic derecho está siendo presionado
        HandleBeamGrowth();


        // Verificar si se puede hacer un flash
        Flash();
    }


    // ---- MÉTODOS PÚBLICOS ----
    #region Métodos públicos
    // Documentar cada método que aparece aquí con ///<summary>
    // El convenio de nombres de Unity recomienda que estos métodos
    // se nombren en formato PascalCase (palabras con primera letra
    // mayúscula, incluida la primera letra)
    // Ejemplo: GetPlayerController

    #endregion

    // ---- MÉTODOS PRIVADOS ----

    // Método para controlar el movimiento de la linterna, usando el joystick o el ratón
    private void ControlMovement()
    {
        // Verifica si el Gamepad está conectado
        if (Gamepad.current != null)
        {
            // Lee la entrada del joystick derecho
            Vector2 joystickInput = Gamepad.current.rightStick.ReadValue();

            // Si el joystick se mueve (es decir, tiene una magnitud significativa)
            if (joystickInput.sqrMagnitude > 0.1f)
            {
                // Marcamos que el joystick está activo
                isJoystickActive = true;

                // Llama a la función que mueve la linterna con el joystick
                MoveWithJoystick(joystickInput);
            }
            else
            {
                // Si el joystick no se está moviendo, usamos el ratón
                isJoystickActive = false;
                MoveWithMouse();
            }
        }
        else
        {
            // Si no hay mando, movemos la linterna con el ratón
            MoveWithMouse();
        }
    }

    // Método para mover la linterna usando el joystick
    private void MoveWithJoystick(Vector2 joystickInput)
    {
        // Convertimos la entrada del joystick a un vector normalizado de dirección
        Vector3 directionToJoystick = new Vector3(joystickInput.x, joystickInput.y, 0).normalized;

        // Calculamos el ángulo de la linterna en función de la dirección del joystick
        currentAngle = Mathf.Atan2(directionToJoystick.y, directionToJoystick.x) * Mathf.Rad2Deg;

        // Calculamos la nueva posición de la linterna en un círculo alrededor del jugador
        Vector3 newPosition = player.position + directionToJoystick * radius;

        // Actualizamos la posición de la linterna
        transform.position = newPosition;

        // Aplicamos la rotación de la linterna para que apunte en la dirección del joystick
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, currentAngle));
    }

    // Método para mover la linterna usando el ratón
    private void MoveWithMouse()
    {
        // Obtenemos la posición actual del ratón en la pantalla
        Vector2 currentMousePosition = Mouse.current.position.ReadValue();

        // Comprobamos si el ratón se ha movido lo suficiente desde la última posición conocida
        if (Vector2.Distance(currentMousePosition, lastMousePosition) > mouseMoveThreshold)
        {
            // Actualizamos la última posición conocida del ratón
            lastMousePosition = currentMousePosition;

            // Convertimos la posición del ratón a coordenadas del mundo
            Vector3 worldMousePosition = Camera.main.ScreenToWorldPoint(currentMousePosition);

            // Aseguramos que la posición Z se mantenga en 0
            worldMousePosition.z = 0;

            // Calculamos la dirección desde la linterna hacia el ratón
            Vector3 directionToMouse = worldMousePosition - transform.position;

            // Si la distancia al ratón es mayor que el radio, movemos la linterna
            if (directionToMouse.magnitude > radius)
            {
                // Normalizamos la dirección para obtener un vector unitario
                directionToMouse.Normalize();

                // Calculamos el ángulo que debe tener la linterna para apuntar hacia el ratón
                currentAngle = Mathf.Atan2(directionToMouse.y, directionToMouse.x) * Mathf.Rad2Deg;

                // Calculamos la nueva posición de la linterna en un círculo alrededor del jugador
                transform.position = player.position + directionToMouse * radius;

                // Aplicamos la rotación de la linterna para que apunte en la dirección del ratón
                transform.rotation = Quaternion.Euler(new Vector3(0, 0, currentAngle));
            }
        }
    }

    // Método para manejar el crecimiento del haz de luz cuando se presiona el botón adecuado
    // Método para manejar el crecimiento del haz de luz basado en la entrada del usuario
    private void HandleBeamGrowth()
    {
        // Se comprueba que exista el dispositivo Mouse y que el botón derecho esté presionado.
        // Esto evita que se intente acceder a Mouse.current.rightButton si el Mouse no está conectado.
        bool isRightMousePressed = Mouse.current != null && Mouse.current.rightButton.isPressed;

        // Se comprueba de forma similar para el Gamepad, verificando que esté conectado y que el botón LT esté presionado.
        bool isLeftTriggerPressed = Gamepad.current != null && Gamepad.current.leftTrigger.isPressed;

        // Si cualquiera de los dos botones está presionado, consideramos que el haz debe crecer.
        if (isRightMousePressed || isLeftTriggerPressed)
        {
            // Se marca que el botón de crecimiento está activo.
            isRightClickPressed = true;

            // Si no hay cooldown activo, se inicia la corutina para aumentar el haz.
            if (!isCooldownActive)
                StartCoroutine(GrowBeam());
        }
        else
        {
            // Si no se presionan los botones, se indica que el crecimiento ya no está activo.
            isRightClickPressed = false;

            // Si no hay cooldown activo, se inicia la corutina para retraer el haz de luz.
            if (!isCooldownActive)
                StartCoroutine(RetractBeam());
        }
    }

    private void Flash() // Método para hacer el flash de la linterna
    {
        // No permitir el flash si el cooldown está activo
        if (isCooldownActive) return;
        // Verificar si el clic izquierdo del ratón o el RT del mando están siendo presionados
        if ((Mouse.current.leftButton.isPressed && isRightClickPressed) ||
             (Gamepad.current != null && Gamepad.current.rightTrigger.isPressed && Gamepad.current.leftTrigger.isPressed)) 
        {
            StartCoroutine(FlashRutine());
            StartCoroutine(LanternCooldown());

        }
    }


    // ---- MÉTODOS DE BEAM ----

    // Corutina para aumentar el tamaño del haz de luz (GrowBeam)
    private IEnumerator GrowBeam()
    {
        // Mientras la longitud actual del haz sea menor a la máxima permitida
        // Y el ancho actual sea mayor al mínimo permitido:
        while (beamObject.transform.localScale.x < maxBeamLength && beamObject.transform.localScale.y > minBeamWidth)
        {
            // Si se suelta el botón de crecimiento (isRightClickPressed pasa a false),
            // se termina la corutina de forma inmediata usando yield break.
            if (!isRightClickPressed)
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

    private IEnumerator RetractBeam()
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

    private IEnumerator FlashRutine()
    {
        // Afectar al segundo hijo: activar SpriteRenderer y BoxCollider2D
        secondChildSpriteRenderer.enabled = true;
        secondChildBoxCollider.enabled = true;
        yield return new WaitForSeconds(0.1f);
        // Desactivar SpriteRenderer y BoxCollider2D del segundo hijo
        secondChildSpriteRenderer.enabled = false;
        secondChildBoxCollider.enabled = false;
    }

    private IEnumerator LanternCooldown()
    {
        isCooldownActive = true; // Activa el cooldown

        firstChildSpriteRenderer.enabled = false;  // Afectar solo al primer hijo: desactivar SpriteRenderer

        yield return new WaitForSeconds(flashCooldown);
        firstChildSpriteRenderer.enabled = true;  // Reactivar SpriteRenderer del primer hijo

        isCooldownActive = false; // Desactiva el cooldown
    }
    // class Lantern 
    // namespace



}