//---------------------------------------------------------
// Breve descripción del contenido del archivo: Es la funcion de que el enemigo abadone su trabajo y empieza a cazar
// Responsable de la creación de este archivo: Andres Diaz Guerrero Soto (El sordo)
// Nombre del juego: Proyect Abyss
// Proyectos 1 - Curso 2024-25
//---------------------------------------------------------

using System.Collections;
using UnityEngine;
// Añadir aquí el resto de directivas using


/// <summary>
/// Antes de cada class, descripción de qué es y para qué sirve,
/// usando todas las líneas que sean necesarias.
/// </summary>
public class EnemigAtack : MonoBehaviour
{
    // ---- ATRIBUTOS DEL INSPECTOR ----
    #region Atributos del Inspector (serialized fields)
    // Documentar cada atributo que aparece aquí.
    // El convenio de nombres de Unity recomienda que los atributos
    // públicos y de inspector se nombren en formato PascalCase
    // (palabras con primera letra mayúscula, incluida la primera letra)
    // Ejemplo: MaxHealthPoints

    #endregion

    [Header("Parámetros de Ataque")]
    [SerializeField] private float PerserSpeed = 3f;       // Velocidad de persecución
    [SerializeField] private float rotateSpeed = 90f;       // Velocidad de giro en grados/segundo (ajústalo para suavizar)

    [Header("Referencias")]
    [SerializeField] private Transform playerTransform;    // Referencia al jugador
    [SerializeField] private SpriteRenderer spriteRenderer;  // Para visualización

    [Header("Colliders Externos")]
    [SerializeField] private Collider2D PlayerCollider;      // Collider que detecta al jugador
    [SerializeField] private Collider2D flashCollider;       // Collider del flash
    [SerializeField] private Collider2D flashCollider2;       // Collider del flash

    [Header("Colliders Internos (enemigo)")]
    [SerializeField] private Collider2D EneVisionCollider;   // Collider de visión del enemigo
    [SerializeField] private Collider2D EneBodycollider;     // Collider del cuerpo del enemigo

    [Header("Ajuste de Orientación")]
    [SerializeField] private bool spriteLooksUp = false;     // true si el sprite está diseñado para apuntar hacia arriba (+Y), false si apunta a la derecha (+X)



    // ---- ATRIBUTOS PRIVADOS ----
    #region Atributos Privados (private fields)
    // Documentar cada atributo que aparece aquí.
    // El convenio de nombres de Unity recomienda que los atributos
    // privados se nombren en formato _camelCase (comienza con _, 
    // primera palabra en minúsculas y el resto con la 
    // primera letra en mayúsculas)
    // Ejemplo: _maxHealthPoints

    #endregion

    private Rigidbody2D _rb;
    private bool _isAttacking = false;

    // ---- MÉTODOS DE MONOBEHAVIOUR ----
    #region Métodos de MonoBehaviour

    // Por defecto están los típicos (Update y Start) pero:
    // - Hay que añadir todos los que sean necesarios
    // - Hay que borrar los que no se usen 

    /// <summary>
    /// Start is called on the frame when a script is enabled just before 
    /// any of the Update methods are called the first time.
    /// </summary>
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        if (_isAttacking && playerTransform != null)
        {
            // Calcula la dirección hacia el jugador
            Vector2 direction = (playerTransform.position - transform.position);
            Vector2 dirNormalized = direction.normalized;
            _rb.velocity = dirNormalized * PerserSpeed;

            // Calcula el ángulo deseado usando Atan2
            float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            if (spriteLooksUp)
            {
                // Si el sprite está diseñado para apuntar hacia arriba, ajustamos el ángulo restando 90°
                targetAngle -= 90f;
            }

            // Interpola suavemente el ángulo actual hacia el ángulo objetivo
            float currentAngle = transform.eulerAngles.z;
            float newAngle = Mathf.MoveTowardsAngle(currentAngle, targetAngle, rotateSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Euler(0f, 0f, newAngle);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 1) Si el jugador entra en la visión => activar ataque
        if (collision == PlayerCollider)
        {
            Debug.Log("Jugador detectado => atacar");
            _isAttacking = true;

            // Desactiva la patrulla (EnemyRouteScript), si existe
            EnemyRouteScript route = GetComponent<EnemyRouteScript>();
            if (route != null)
            {
                route.enabled = false;
                Debug.Log("EnemyRouteScript desactivado.");
            }
        }
        // 2) Si el flash choca => verificar si toca el cuerpo
        else if (collision == flashCollider)
        {
            // Usamos IsTouching para confirmar que el flash choca con el cuerpo, no con la visión
            if (EneBodycollider != null && EneBodycollider.IsTouching(flashCollider))
            {
                Debug.Log("El flash iluminó el cuerpo => desactivar ataque y apagar visión.");
                _isAttacking = false;
                DisableVisionCollider();

                // (Opcional) deshabilitar este script para que no retome la persecución
                this.enabled = false;
            }
        }

        else if (collision == flashCollider2)
        {
            // Usamos IsTouching para confirmar que el flash choca con el cuerpo, no con la visión
            if (EneBodycollider != null && EneBodycollider.IsTouching(flashCollider2))
            {
                Debug.Log("El flash iluminó el cuerpo => desactivar ataque y apagar visión.");
                _isAttacking = false;
                DisableVisionCollider();

                // (Opcional) deshabilitar este script para que no retome la persecución
                this.enabled = false;
            }
        }
    }



    private void OnTriggerExit2D(Collider2D collision)
    {
        // Si el jugador sale del rango => desactivar ataque y reactivar patrulla
        if (collision == PlayerCollider)
        {
            Debug.Log("Jugador salió del rango => fin de ataque");
            _isAttacking = false;

            EnemyRouteScript route = GetComponent<EnemyRouteScript>();
            if (route != null)
            {
                route.enabled = true;
                Debug.Log("EnemyRouteScript reactivado.");
            }

            // Deshabilita temporalmente este script por 3 segundos
            StartCoroutine(DisableAtackTemporary(3f));
        }
    }

    // Método para deshabilitar el collider de visión (para que no detecte al jugador mientras huye)
    private IEnumerator DisableAtackTemporary(float seconds)
    {
        // Deshabilitar este script
        this.enabled = false;
        yield return new WaitForSeconds(seconds);
        // Volver a habilitar
        this.enabled = true;
        Debug.Log("Ataque reactivado tras " + seconds + " seg");
    }
    
    // Método para desactivar el collider de visión.
    public void DisableVisionCollider()
    {
        if (EneVisionCollider != null)
        {
            EneVisionCollider.enabled = false;
            Debug.Log("Collider de visión deshabilitado.");
        }
    }

    // Método para reactivar el collider de visión (puedes llamarlo desde otro sistema si es necesario).
    public void EnableVisionCollider()
    {
        if (EneVisionCollider != null)
        {
            EneVisionCollider.enabled = true;
            Debug.Log("Collider de visión reactivado.");
        }
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
    #region Métodos Privados
    // Documentar cada método que aparece aquí
    // El convenio de nombres de Unity recomienda que estos métodos
    // se nombren en formato PascalCase (palabras con primera letra
    // mayúscula, incluida la primera letra)

    #endregion

} // class EnemigAtack 
// namespace
