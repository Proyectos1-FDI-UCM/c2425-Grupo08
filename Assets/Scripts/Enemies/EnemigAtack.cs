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

    [Header("Los parametros de los ataques")]
    [SerializeField] private float PerserSpeed = 3f; // Velocidad de la persecucion
    [SerializeField] private float flashEscapeSpeed = 4f; // Velocidad de huida al recibir flash


    [Header("Los referencias")]
    [SerializeField] private Transform playerTransform; // El jugador
    [SerializeField] private SpriteRenderer spriteRenderer; // Para "mirar" al jugador 

    [Header("Referencia de collider Externo")]
    [SerializeField] private Collider2D PlayerCollider; // El collider de jugador
    [SerializeField] private Collider2D flashCollider; // El collider de flash

    [Header("Referencia de collider Interno")]
    [SerializeField] private Collider2D EneVisionCollider; // El trigger que detecte al jugador
    [SerializeField] private Collider2D EneBodycollider; // El cuerpo de enemigo

    [Header("Ajuste de rotacion")]
    [SerializeField] private bool spriteLooksUo = false; // Si el spirte apunta arriba, se pone true
    // si apunta a la derecha se pone false

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
    private bool _isAtack = false; // Activa la persecucion
    
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
        
        if (_isAtack && playerTransform != null)
        {
            // Persigue el jugador
            Vector2 direction = (playerTransform.position - transform.position).normalized;
            _rb.velocity = direction * PerserSpeed;

            // Cacluca el angulo de rotacion
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            if (spriteLooksUo)
            {
                // Si el sprite mira hacia arriba
                angle -= 90f;
            }


            //Rota el transform 
            transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }
        


    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        // Si la colision es con el jugador se activa la persecucion
        if (collision == PlayerCollider)
        {
            Debug.Log("Jugador detectado, a atacar");
            _isAtack = true;

            EnemyRouteScript route = GetComponent<EnemyRouteScript>();
            if (route != null)
            {
                route.enabled = false;
            }
        }

        // Si la colision collisiona el flash huye
        else if (collision == flashCollider)
        {
            // Verifica que el flash toque el cuerpo de enemigo
            if (EneBodycollider.bounds.Contains(collision.bounds.center))
            {

                Debug.Log("el flash lo ha iluminado");
                EnemigEscape escapeScript = GetComponent<EnemigEscape>();
                if (escapeScript != null)
                {
                    // llama el metodo de script de escape
                    escapeScript.ActivateEscape();
                    
                }
                
                _isAtack = false;
                this.enabled = false;

            }
        }

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // Si el jugador sale del rango de visión, se detiene el ataque
        if (collision == PlayerCollider)
        {
            Debug.Log("Jugador salió del rango de visión.");
            _isAtack = false;
            
                //Reactiva la patrulla
                EnemyRouteScript route = GetComponent<EnemyRouteScript>();
                if (route != null)
                {
                    route.enabled = true;
                }

                // Deshabilitar el script por 3 segundo 
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
