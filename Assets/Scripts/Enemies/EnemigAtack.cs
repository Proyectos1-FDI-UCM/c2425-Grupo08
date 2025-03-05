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
namespace EnemyLogic
{
    public class EnemygAtackState : EnemyState
    {
    // ---- ATRIBUTOS DEL INSPECTOR ----
    #region Atributos del Inspector (serialized fields)
    // Documentar cada atributo que aparece aquí.
    // El convenio de nombres de Unity recomienda que los atributos
    // públicos y de inspector se nombren en formato PascalCase
    // (palabras con primera letra mayúscula, incluida la primera letra)
    // Ejemplo: MaxHealthPoints

    #endregion

    //[Header("Parámetros de Ataque")]
     private float PerserSpeed = 3f;       // Velocidad de persecución
     private float rotateSpeed = 90f;       // Velocidad de giro en grados/segundo (ajústalo para suavizar)


     private bool spriteLooksUp = false;     // true si el sprite está diseñado para apuntar hacia arriba (+Y), false si apunta a la derecha (+X)



    // ---- ATRIBUTOS PRIVADOS ----
    #region Atributos Privados (private fields)
    // Documentar cada atributo que aparece aquí.
    // El convenio de nombres de Unity recomienda que los atributos
    // privados se nombren en formato _camelCase (comienza con _,
    // primera palabra en minúsculas y el resto con la
    // primera letra en mayúsculas)
    // Ejemplo: _maxHealthPoints

    #endregion

    private Rigidbody2D rb;
    private bool _isAttacking = false;

    private EnemyScript enemy;
    private GameObject enemyObject;
    public EnemygAtackState(GameObject enemyObject){
        this.enemyObject = enemyObject;
        enemy = enemyObject.GetComponent<EnemyScript>();
        this.rb = enemyObject.GetComponent<Rigidbody2D>();
    }
    /// <summary>
    /// </summary>
    public void Move(){
        if (_isAttacking && enemy.playerTransform != null)
        {
            // Calcula la dirección hacia el jugador
            Vector2 direction = (enemy.playerTransform.position - enemy.transform.position);
            Vector2 dirNormalized = direction.normalized;
            rb.velocity = dirNormalized * PerserSpeed;

            // Calcula el ángulo deseado usando Atan2
            float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            if (spriteLooksUp)
            {
                // Si el sprite está diseñado para apuntar hacia arriba, ajustamos el ángulo restando 90°
                targetAngle -= 90f;
            }

            // Interpola suavemente el ángulo actual hacia el ángulo objetivo
            float currentAngle = enemy.transform.eulerAngles.z;
            float newAngle = Mathf.MoveTowardsAngle(currentAngle, targetAngle, rotateSpeed * Time.deltaTime);
            enemy.transform.rotation = Quaternion.Euler(0f, 0f, newAngle);
        }
    }
    public void NextState(){
        enemy.state
    }

    /*private void OnTriggerEnter2D(Collider2D collision)
    {
        // 1) Si el jugador entra en la visión => activar ataque
        if (collision == enemy.PlayerCollider)
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
        else if (collision == enemy.flashCollider)
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


    }*/



    private void OnTriggerExit2D(Collider2D collision)
    {
        // Si el jugador sale del rango => desactivar ataque y reactivar patrulla
        if (collision == enemy.PlayerCollider)
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


    // Método para desactivar el collider de visión.

    }

}
