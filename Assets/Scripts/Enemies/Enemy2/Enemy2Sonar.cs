//---------------------------------------------------------
// Este archivo se encarga del funcionamiento del enemigo 2 (sonar)
// Javier Zazo Morillo
// Project Abyss
// Proyectos 1 - Curso 2024-25
//---------------------------------------------------------

using EnemyLogic;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;
// Añadir aquí el resto de directivas using


/// <summary>
/// Antes de cada class, descripción de qué es y para qué sirve,
/// usando todas las líneas que sean necesarias.
/// </summary>
public class Enemy2Sonar : MonoBehaviour
{
    // ---- ATRIBUTOS DEL INSPECTOR ----
    #region Atributos del Inspector (serialized fields)
    // Documentar cada atributo que aparece aquí.
    // El convenio de nombres de Unity recomienda que los atributos
    // públicos y de inspector se nombren en formato PascalCase
    // (palabras con primera letra mayúscula, incluida la primera letra)
    // Ejemplo: MaxHealthPoints

    [SerializeField] private GameObject[] nodeArray;
    [SerializeField] private float patrolSpeed;
    [SerializeField] private float attackSpeed;

    [SerializeField] private bool debug = false;

    /// <summary>
    /// Frecuencia con la que el enemigo utiliza su sonar
    /// </summary>
    [SerializeField] private float sonarFrequency;
    /// <summary>
    /// Pequeño delay en el que el jugador aún no es detectado por el sonar a pesar de haberle llegado el pulso
    /// </summary>
    [SerializeField] private float shadowDelay;
    /// <summary>
    /// Tiempo que tarda la animación del UI del sonar en rellenar la circunferencia
    /// </summary>
    [SerializeField] private float sonarChargeTime;

    [SerializeField] private float sonarHearingDistance;
    [SerializeField] private float sonarAttackDistance;

    #endregion

    // ---- ATRIBUTOS PRIVADOS ----
    #region Atributos Privados (private fields)
    // Documentar cada atributo que aparece aquí.
    // El convenio de nombres de Unity recomienda que los atributos
    // privados se nombren en formato _camelCase (comienza con _, 
    // primera palabra en minúsculas y el resto con la 
    // primera letra en mayúsculas)
    // Ejemplo: _maxHealthPoints

    private Rigidbody2D rb;

    private GameObject player;
    private SonarUI sonarUI;

    /// <summary>
    /// Bool que se activa cuando el enemigo detecta al jugador
    /// </summary>
    private bool attack = false;

    /// <summary>
    /// Bool de control para saber si el jugador estaba ya en el radio de escucha
    /// </summary>
    private bool alreadyInsideHearingRadious;
    /// <summary>
    /// Bool de control para saber si el jugador estaba ya en el radio de ataque
    /// </summary>
    private bool alreadyInsideAttackRadious;

    /// <summary>
    /// Bool que le dice al OnDrawGizmos que represente el ataque inminente en el editor
    /// </summary>
    private bool attackDebug = false;

    /// <summary>
    /// Delay de la corrutina del cooldown del sonar
    /// </summary>
    private float sonarCooldownTime;

    private struct NodeRoute
    {
        private int nodeCont; //Contador que indica a que nodo se está moviendo
        private GameObject[] nodeArray; //Array con todos los nodos
        private Collider2D collider;

        // Constructor
        public NodeRoute(GameObject[] _nodeArray)
        {
            nodeArray = _nodeArray;
            nodeCont = 0;
            collider = nodeArray[0].GetComponent<Collider2D>();
        }
        // Pasa al siguiente nodo
        public void SetNextNode()
        {
            nodeCont++;
            nodeCont = nodeCont % nodeArray.Length; // Vuelve al 0 cuando se pasa del tamaño del array
            collider = nodeArray[nodeCont].GetComponent<Collider2D>();
        }
        // Devuelve el nodo al que se dirige
        public GameObject GetNextNode()
        {
            return nodeArray[nodeCont];
        }
        public Collider2D GetCollider()
        {
            return collider;
        }
    };

    NodeRoute nodeRoute;

    #endregion

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
        rb = GetComponent<Rigidbody2D>();

        player = GameObject.FindGameObjectWithTag("Player");
        sonarUI = player.GetComponentInChildren<SonarUI>();

        nodeRoute = new NodeRoute(nodeArray); // Aviso. El array de nodos se crea al inicio, no es dinámico.

        sonarCooldownTime = sonarFrequency - sonarChargeTime - shadowDelay;
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void FixedUpdate()
    {
        if (attack)
        {
            Move((player.transform.position - transform.position).normalized, attackSpeed);
        }
        else
        {
            Move((nodeRoute.GetNextNode().transform.position - transform.position).normalized, patrolSpeed);
        }    
    }
    void Update()
    {
        if (IsInsideHearingRadious())
        {
            if (!alreadyInsideHearingRadious)
            {
                StartCoroutine(SonarCooldown());
                sonarUI.ActivateSonarUI();
                alreadyInsideHearingRadious = true;
            }                           
        }
        else if (alreadyInsideHearingRadious)
        {
            StopAllCoroutines();        
            sonarUI.DeactivateSonarUI();
            alreadyInsideHearingRadious = false;
        }
        if (IsInsideAttackRadious())
        {
            if (!alreadyInsideAttackRadious)
            {
                sonarUI.ActivatePulseUI();
                alreadyInsideAttackRadious = true;
            }          
        }
        else if (alreadyInsideAttackRadious)
        {
            sonarUI.DeactivatePulseUI();
            alreadyInsideAttackRadious = false;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision == nodeRoute.GetCollider())
        {
            nodeRoute.SetNextNode();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject == player && attack)
        {
            // player.GetComponent<OxigenScript>().Kill();
            attack = false;
            StartCoroutine(SonarCooldown());
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
    #region Métodos privados
    // Documentar cada método que aparece aquí
    // El convenio de nombres de Unity recomienda que estos métodos
    // se nombren en formato PascalCase (palabras con primera letra
    // mayúscula, incluida la primera letra)

    /// <summary>
    /// Mueve al enemigo según su estado (patrulla o ataque) representado en los parámetros que le llegan
    /// </summary>
    /// <param name="direction">La dirección a la que se tiene que mover</param>
    /// <param name="speed">La velocidad a la que se tiene que mover</param>
    private void Move(Vector2 direction, float speed)
    {
        transform.rotation = Quaternion.Euler(0, 0, Mathf.Rad2Deg * Mathf.Atan2(direction.y, direction.x));

        rb.velocity = direction * speed;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <returns>Si se está dentro del radio de escucha</returns>
    private bool IsInsideHearingRadious()
    {
        return (player.transform.position - transform.position).magnitude <= sonarHearingDistance;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <returns>Si se está dentro del radio de ataque</returns>
    private bool IsInsideAttackRadious()
    {
        return (player.transform.position - transform.position).magnitude <= sonarAttackDistance;
    }
    /// <summary>
    /// Cooldown del sonar (su frecuencia menos el tiempo de carga y menos el delay del final)
    /// </summary>
    /// <returns></returns>
    private IEnumerator SonarCooldown()
    {
        yield return new WaitForSeconds(sonarCooldownTime);

        // Hacer el sonido
        // Hacer la animación del UI

        StartCoroutine(SonarCharge());
    }
    /// <summary>
    /// Tiempo que tarda la animación del UI en llegar a su máxima escala
    /// </summary>
    /// <returns></returns>
    private IEnumerator SonarCharge()
    {
        sonarUI.PlayAnimation();

        yield return new WaitForSeconds(sonarChargeTime);

        StartCoroutine(ShadowDelay());
    }
    /// <summary>
    /// Pequeño delay para dar tiempo de reacción al jugador (el sonido del sonar tendría que llegar al jugador justo antes de este delay)
    /// </summary>
    /// <returns></returns>
    private IEnumerator ShadowDelay()
    {
        attackDebug = true;

        yield return new WaitForSeconds(shadowDelay);

        if ((IsInsideAttackRadious()) &&
        (InputManager.Instance.MovementVector.x != 0 || player.GetComponent<PlayerMovement>().GetIsRepairing())) // Hace falta cambiar el interact por un bool de si se está reparando el motor
        {
            attack = true;
        }

        attackDebug = false;

        if (IsInsideAttackRadious())
        {
            attack = true;
        }
        else
        {
            StartCoroutine(SonarCooldown());
        }
    }
    /// <summary>
    /// Método que dibuja el debug en el editor 
    /// </summary>
    private void OnDrawGizmos()
    {
        if (debug)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, sonarHearingDistance);
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, sonarAttackDistance);

            if (attackDebug)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(transform.position, player.transform.position);
            }
        }
    }
}
    #endregion   

// class Enemy2Sonar 
// namespace
