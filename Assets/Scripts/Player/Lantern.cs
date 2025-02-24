//---------------------------------------------------------
// La linterna deberá tener un haz regulable (ancho-fino) que siga la posición del cursor/dirección
// del joystick derecho y ofrezca un ángulo de visión.

// Vicente Rodriguez Casado
// Proyect Abyss
// Proyectos 1 - Curso 2024-25
//---------------------------------------------------------

using UnityEngine;
using UnityEngine.InputSystem;



/// <summary>
/// Antes de cada class, descripción de qué es y para qué sirve,
/// usando todas las líneas que sean necesarias.
/// </summary>
public class Lantern : MonoBehaviour
{
    // ---- ATRIBUTOS DEL INSPECTOR ----
    #region Atributos del Inspector (serialized fields)
    // Documentar cada atributo que aparece aquí.
    // El convenio de nombres de Unity recomienda que los atributos
    // públicos y de inspector se nombren en formato PascalCase
    // (palabras con primera letra mayúscula, incluida la primera letra)
    // Ejemplo: MaxHealthPoints

    #endregion

    // ---- ATRIBUTOS PRIVADOS ----
   [SerializeField] private Transform player;  // El jugador alrededor del cual se mueve la linterna
   [SerializeField] private float radius = 2f; // Radio del círculo sobre el que se mueve la linterna
   [SerializeField] private float rotationSpeed = 30f; // Velocidad del movimiento de la linterna en el arco

    private float currentAngle = 0f; // Ángulo actual de la linterna en el círculo
   


    // ---- MÉTODOS DE MONOBEHAVIOUR ----
    private void Update()
    {
        // 1. Obtener la posición del mouse usando el nuevo sistema de entrada
        Vector2 mousePosition = Mouse.current.position.ReadValue();

        // Convertir la posición del mouse a coordenadas del mundo
        Vector3 worldMousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
        worldMousePosition.z = 0;

        // 2. Calcular la dirección hacia el mouse desde la posición de la linterna
        Vector3 directionToMouse = worldMousePosition - transform.position;

        // 3. Calcular el ángulo deseado para que la linterna apunte hacia el mouse
        float targetAngle = Mathf.Atan2(directionToMouse.y, directionToMouse.x) * Mathf.Rad2Deg;

        // 4. Actualizar el ángulo de la linterna para que apunte instantáneamente hacia el mouse
        currentAngle = targetAngle;

        // 5. Calcular la nueva posición de la linterna en la circunferencia alrededor del jugador
        float x = Mathf.Cos(currentAngle * Mathf.Deg2Rad) * radius;
        float y = Mathf.Sin(currentAngle * Mathf.Deg2Rad) * radius;

        // 6. Actualizar la posición de la linterna
        transform.position = player.position + new Vector3(x, y, 0);

        // 7. Aplicar la rotación hacia el mouse (en la dirección calculada)
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, currentAngle));

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
    #region Métodos Privados
    // Documentar cada método que aparece aquí
    // El convenio de nombres de Unity recomienda que estos métodos
    // se nombren en formato PascalCase (palabras con primera letra
    // mayúscula, incluida la primera letra)

    #endregion

    // class Lantern 
    // namespace
}