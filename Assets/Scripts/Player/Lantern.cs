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
public class Lantern : MonoBehaviour
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
    private bool isLTButtonPressed = false; // Indica si el botón LT del mando está presionado
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
        //Cursor.visible = false;
        // Verificar si el mando está conectado antes de intentar usarlo
        if (Gamepad.current != null)
        {
            // Leer la entrada del joystick derecho (si está siendo usado)
            Vector2 joystickInput = Gamepad.current.rightStick.ReadValue();

            // Si el joystick se está moviendo, desactivar el control del ratón
            if (joystickInput.sqrMagnitude > 0.1f)
            {
            // El joystick está siendo movido, lo que hace que el mouse se ignore.
            isJoystickActive = true; // Marcamos que el joystick está activo

            // Controlar la linterna con el joystick
            Vector3 directionToJoystick = new Vector3(joystickInput.x, joystickInput.y, 0).normalized;

            // Calcular el ángulo de la linterna basado en el movimiento del joystick
            float targetAngle = Mathf.Atan2(directionToJoystick.y, directionToJoystick.x) * Mathf.Rad2Deg;

            // Actualizar el ángulo de la linterna para que apunte en la dirección del joystick
            currentAngle = targetAngle;

            // Calcular la nueva posición de la linterna en la circunferencia alrededor del jugador
            Vector3 newPosition = player.position + directionToJoystick * radius;

            // Actualizar la posición de la linterna
            transform.position = newPosition;

            // Aplicar la rotación
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, currentAngle));

            }
        else
        {
            // Si el joystick no está en movimiento, permitir que el ratón controle la linterna
            if (isJoystickActive)
            {
                // Si el joystick estaba activo pero ya no se está moviendo, desactivamos el control del joystick
                isJoystickActive = false;
            }

            // Obtener la posición actual del mouse
            Vector2 currentMousePosition = Mouse.current.position.ReadValue();

            // Verificamos si el ratón se ha movido desde la última vez
            if (Vector2.Distance(currentMousePosition, lastMousePosition) > mouseMoveThreshold)
            {
                // El ratón se ha movido, entonces activamos el control del ratón
                isJoystickActive = false; // Desactivar el joystick

                // Actualizamos la última posición conocida del ratón
                lastMousePosition = currentMousePosition;

                // Controlar la linterna con el mouse
                Vector3 worldMousePosition = Camera.main.ScreenToWorldPoint(currentMousePosition);
                worldMousePosition.z = 0;

                Vector3 directionToMouse = worldMousePosition - transform.position;
                float distanceToMouse = directionToMouse.magnitude;

                if (distanceToMouse > radius)
                {
                    // Si el mouse está fuera del radio, movemos la linterna y actualizamos la rotación
                    directionToMouse.Normalize();
                    float targetAngle = Mathf.Atan2(directionToMouse.y, directionToMouse.x) * Mathf.Rad2Deg;

                    currentAngle = targetAngle;
                    Vector3 newPosition = player.position + directionToMouse * radius;
                    transform.position = newPosition;
                    transform.rotation = Quaternion.Euler(new Vector3(0, 0, currentAngle));
                }
            }

        }
        // Detectar si el clic derecho está siendo presionado
        if (Mouse.current.rightButton.isPressed)
        {
           
                isRightClickPressed = true;
                StartCoroutine(GrowBeam());
               

            }
        else
        {
           
                isRightClickPressed = false;
                StartCoroutine(RetractBeam());
            
        }


        // Detectar si el botón LT del mando está siendo presionado
        isLTButtonPressed = Gamepad.current.leftTrigger.isPressed;

        if (isLTButtonPressed)
        {
                    StartCoroutine(GrowBeam());
                    
            }
        else
        {
                StartCoroutine(RetractBeam());
        }
        Flash();
        }
        // Si no hay mando, controlar con el ratón
        else
        {
            // Obtener la posición actual del mouse
            Vector2 currentMousePosition = Mouse.current.position.ReadValue();

            // Verificamos si el ratón se ha movido desde la última vez
            if (Vector2.Distance(currentMousePosition, lastMousePosition) > mouseMoveThreshold)
            {
                // El ratón se ha movido, entonces activamos el control del ratón
                isJoystickActive = false; // Desactivar el joystick

                // Actualizamos la última posición conocida del ratón
                lastMousePosition = currentMousePosition;

                // Controlar la linterna con el mouse
                Vector3 worldMousePosition = Camera.main.ScreenToWorldPoint(currentMousePosition);
                worldMousePosition.z = 0;

                Vector3 directionToMouse = worldMousePosition - transform.position;
                float distanceToMouse = directionToMouse.magnitude;

                if (distanceToMouse > radius)
                {
                    // Si el mouse está fuera del radio, movemos la linterna y actualizamos la rotación
                    directionToMouse.Normalize();
                    float targetAngle = Mathf.Atan2(directionToMouse.y, directionToMouse.x) * Mathf.Rad2Deg;

                    currentAngle = targetAngle;
                    Vector3 newPosition = player.position + directionToMouse * radius;
                    transform.position = newPosition;
                    transform.rotation = Quaternion.Euler(new Vector3(0, 0, currentAngle));
                }
            }

            // Detectar si el clic derecho está siendo presionado
            if (Mouse.current.rightButton.isPressed)
            {
                
                    isRightClickPressed = true;
                    StartCoroutine(GrowBeam());
                     
            }
            else
            {
                
                    isRightClickPressed = false;
                    StartCoroutine(RetractBeam());
                
            }
            Flash();   

        }
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

    private void ControllerHandler()
    {
        // Leer la entrada del joystick derecho (si está siendo usado)
        Vector2 joystickInput = Gamepad.current.rightStick.ReadValue();

        // Si el joystick se está moviendo, desactivar el control del ratón
        if (joystickInput.sqrMagnitude > 0.1f)
        {
            // El joystick está siendo movido, lo que hace que el mouse se ignore.
            isJoystickActive = true; // Marcamos que el joystick está activo

            // Controlar la linterna con el joystick
            Vector3 directionToJoystick = new Vector3(joystickInput.x, joystickInput.y, 0).normalized;

            // Calcular el ángulo de la linterna basado en el movimiento del joystick
            float targetAngle = Mathf.Atan2(directionToJoystick.y, directionToJoystick.x) * Mathf.Rad2Deg;

            // Actualizar el ángulo de la linterna para que apunte en la dirección del joystick
            currentAngle = targetAngle;

            // Calcular la nueva posición de la linterna en la circunferencia alrededor del jugador
            Vector3 newPosition = player.position + directionToJoystick * radius;

            // Actualizar la posición de la linterna
            transform.position = newPosition;

            // Aplicar la rotación
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, currentAngle));

            // Ocultar el cursor cuando el joystick está en uso
            Cursor.visible = false;
        }
        }

    private void Flash() // Método para hacer el flash de la linterna
    {
        // No permitir el flash si el cooldown está activo
        if (isCooldownActive) return;
        // Verificar si el clic izquierdo del ratón o el RT del mando están siendo presionados
        if ((Mouse.current.leftButton.isPressed && isRightClickPressed) ||
            (Gamepad.current != null && Gamepad.current.rightTrigger.isPressed && isLTButtonPressed))
        {
            StartCoroutine(FlashRutine());
            StartCoroutine(LanternCooldown());

        }
    }


    // ---- MÉTODOS DE BEAM ----

    private IEnumerator GrowBeam()
    {
        // Aumentamos la longitud y reducimos el ancho gradualmente mientras se mantiene el clic derecho
        while (beamObject.transform.localScale.x < maxBeamLength && beamObject.transform.localScale.y > minBeamWidth)
        {
            if(isRightClickPressed == false)
            {
                StopCoroutine(GrowBeam());
            }
            // Aumentamos la longitud y reducimos el ancho gradualmente
            beamObject.transform.localScale = new Vector3(
                beamObject.transform.localScale.x + beamGrowSpeed * Time.deltaTime,
                Mathf.Max(minBeamWidth, beamObject.transform.localScale.y - beamGrowSpeed * Time.deltaTime),
                beamObject.transform.localScale.z
            );
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