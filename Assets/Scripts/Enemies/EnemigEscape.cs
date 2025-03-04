//---------------------------------------------------------
// Breve descripción del contenido del archivo: Huir a enemigo cuando es atacado por el flash
// Responsable de la creación de este archivo: Andrés Diaz Guerrero Soto (El sordo)
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
public class EnemigEscape : MonoBehaviour
{
    // ---- ATRIBUTOS DEL INSPECTOR ----
    #region Atributos del Inspector (serialized fields)
    // Documentar cada atributo que aparece aquí.
    // El convenio de nombres de Unity recomienda que los atributos
    // públicos y de inspector se nombren en formato PascalCase
    // (palabras con primera letra mayúscula, incluida la primera letra)
    // Ejemplo: MaxHealthPoints

    #endregion
    [Header("Los valores de la huida")]
    [SerializeField] private float fleeSpeed = 5f; // La velocidad de la huida
    [SerializeField] private float rotationDelay = 2f; // Tiempo en segundos antes de aplicar la rotación en Y

    [Header("Referencia para comunicar")]
    [SerializeField] private Transform playerTransform; // referencia de transform del jugador
    [SerializeField] private SpriteRenderer spriteRenderer; // Para girar el sprite
    [SerializeField] private Collider2D flashCollider;



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
    private bool _isFleeing = false;
    
    
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
        // Si nuestro enemigo va a huir, va a ir la direccion contraria de jugador
        if (_isFleeing && playerTransform != null)
        {
            // Direccion desde el jugador hacia el enemigo
            Vector2 fleeDirection = (transform.position - playerTransform.position).normalized;

            // Asignar la velocidad de huida
            _rb.velocity = fleeDirection * fleeSpeed;

            // Conserva la orientación que tenía antes de huir
            spriteRenderer.flipX = (fleeDirection.x > 0);
        }
    }

    // ---- Detectar la Literna (Trigger) y huir ----

    private void OnTriggerEnter2D (Collider2D collision)
    {

        Debug.Log("OnTriggerEnter2D con: " + collision.gameObject.name);

        // Comparamos directamente si el collider que entró es el flashCollider
        if (collision == flashCollider)
        {
            Debug.Log("¡Collider del flash detectado!");

            // Desactivar la patrulla (si el enemigo la tiene)
            EnemyRouteScript route = GetComponent<EnemyRouteScript>();
            if (route != null)
            {
                route.enabled = false;
                Debug.Log("EnemyRouteScript desactivado.");
            }

            // Activar modo huida
            _isFleeing = true;
            Debug.Log("Modo huida activado.");

           
        }
    }

    // Método que se llama cuando el objeto deja de ser visible por la cámara
    void OnBecameInvisible()
    {
        if (_isFleeing)
        {
            Debug.Log("Enemigo huido fuera de la cámara. Destruyéndolo.");
            Destroy(gameObject);
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
    public void ActivateEscape()
    {
        _isFleeing = true;
        // Aquí se asume que el movimiento se controla en Update
        // Inicia la rutina para rotar 180° en Y después de rotationDelay segundos
        StartCoroutine(RotateYAfterDelay(rotationDelay));
    }

    private IEnumerator RotateYAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Vector3 currentEuler = transform.eulerAngles;
        currentEuler.y += 180f;
        transform.eulerAngles = currentEuler;
        Debug.Log("Rotación en Y aplicada. Nueva rotación Y: " + transform.eulerAngles.y);
    }


    // ---- MÉTODOS PRIVADOS ----
    #region Métodos Privados
    // Documentar cada método que aparece aquí
    // El convenio de nombres de Unity recomienda que estos métodos
    // se nombren en formato PascalCase (palabras con primera letra
    // mayúscula, incluida la primera letra)

    #endregion



} // class EnemigEscape 
// namespace
