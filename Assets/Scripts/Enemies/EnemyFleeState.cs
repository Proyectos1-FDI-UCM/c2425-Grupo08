//---------------------------------------------------------
// Breve descripción del contenido del archivo: Huir a enemigo cuando es atacado por el flash
// Responsable de la creación de este archivo: Andrés Diaz Guerrero Soto (El sordo)
// Nombre del juego: Proyect Abyss
// Proyectos 1 - Curso 2024-25
//---------------------------------------------------------

// Añadir aquí el resto de directivas using
using System.Collections;
using System.Threading;
using UnityEngine;


/// <summary>
/// Antes de cada class, descripción de qué es y para qué sirve,
/// usando todas las líneas que sean necesarias.
/// </summary>
public class EnemyFleeState : MonoBehaviour
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
    [SerializeField] private float escapeDuracion = 10f; // Duración de escape (para reactivar ataque, etc.)


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
    private float _fleeStartTime;


    // ---- MÉTODOS DE MONOBEHAVIOUR ----
    #region Métodos de MonoBehaviour
void Start()
    {
        _rb = GetComponent<Rigidbody2D>();

    }

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods are called the first time.
    /// </summary>


    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        Move();
    }





    // Método que activa la huida y comienza la lógica

    #endregion




    // ---- MÉTODOS PÚBLICOS ----
    #region Métodos públicos
    // Método que activa la huida y comienza la lógica
    public void Move()
    {
        _fleeStartTime = Time.time; // Guardamos el tiempo de inicio de la huida
        //Debug.Log("Modo huida activado.");

        // Ejecutamos la duración de la huida
        StartCoroutine(FleeDuration());

        Flee();
    }
    #endregion








    // ---- MÉTODOS PRIVADOS ----
    #region Métodos Privados
    // Método que activa la huida y aplica todo el comportamiento de movimiento
    private void Flee()
    {
        // Dirección desde el jugador hacia el enemigo (huir)
        Vector2 fleeDirection = (transform.position - playerTransform.position).normalized;

        // Si ha pasado el tiempo de espera, se realiza la rotación de 180 grados en Y
        if (Time.time - _fleeStartTime >= rotationDelay)
        {
            Vector3 currentEuler = transform.eulerAngles;
            currentEuler.y += 180f;  // Giro de 180 grados en el eje Y
            transform.eulerAngles = currentEuler;
            //Debug.Log("Rotación en Y aplicada. Nueva rotación Y: " + transform.eulerAngles.y);
        }

        // Ajustamos la dirección del sprite según la huida
        if (fleeDirection.x > 0)
        {
            transform.localScale = new Vector3(1, -1, 1);
        }
        else
        {
            transform.localScale = new Vector3(1, 1, 1);
        }

        //Rotación vertical
        float angle = Mathf.Atan2(fleeDirection.y, fleeDirection.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0, 0, angle);

        // Asignamos la velocidad de huida
        _rb.velocity = fleeDirection * fleeSpeed;

        // Destruimos el objeto después de un tiempo determinado (escapeDuracion)
        if (Time.time - _fleeStartTime >= escapeDuracion)
        {
            //Debug.Log("El enemigo ha dejado de huir.");
            Destroy(gameObject);
        }
    }

    // Método para rotar el enemigo 180 grados después de un retraso
   

    // Método que destruye el enemigo después de un tiempo de huida
    private IEnumerator FleeDuration()
    {
        yield return new WaitForSeconds(escapeDuracion);
      Destroy(gameObject);
        //Debug.Log("El enemigo ha dejado de huir.");
    }

    // Documentar cada método que aparece aquí
    // El convenio de nombres de Unity recomienda que estos métodos
    // se nombren en formato PascalCase (palabras con primera letra
    // mayúscula, incluida la primera letra)

    #endregion



} // class EnemigEscape
// namespace
