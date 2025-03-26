//---------------------------------------------------------
// Breve descripción del contenido del archivo
// Responsable de la creación de este archivo
// Nombre del juego
// Proyectos 1 - Curso 2024-25
//---------------------------------------------------------

using EnemyLogic;
using System.Collections;
using UnityEngine;
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

    [SerializeField] private float sonarFrequency = 3f;
    [SerializeField] private float shadowDelay = 0.5f;

    [SerializeField] private float sonarHearingDistance = 5f;
    [SerializeField] private float sonarAttackDistance = 3f;

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

    private Collider2D bodyCollider;

    private GameObject player;
    private SonarUI sonarUI;

    private bool attack = false;

    private bool isInsideRadious = false;

    private bool attackDebug = false;

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

        bodyCollider = GetComponent<Collider2D>();

        player = GameObject.FindGameObjectWithTag("Player");
        sonarUI = player.GetComponentInChildren<SonarUI>();

        nodeRoute = new NodeRoute(nodeArray); // Aviso. El array de nodos se crea al inicio, no es dinámico.
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
        if ((player.transform.position - transform.position).magnitude <= sonarHearingDistance && !isInsideRadious)
        {
            StartCoroutine(Sonar());
            isInsideRadious = true;
            sonarUI.ActivateSonarUI();                  
        }
        else if ((player.transform.position - transform.position).magnitude > sonarHearingDistance)
        {
            StopAllCoroutines();
            isInsideRadious = false;
            sonarUI.DeactivateSonarUI();                    
        }
        if ((player.transform.position - transform.position).magnitude <= sonarAttackDistance)
        {
            sonarUI.ActivatePulseUI();
        }
        else if ((player.transform.position - transform.position).magnitude > sonarAttackDistance)
        {
            sonarUI.DeactivatePulseUI();
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
            StartCoroutine(Sonar());
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

    private void Move(Vector2 direction, float speed)
    {
        transform.rotation = Quaternion.Euler(0, 0, Mathf.Rad2Deg * Mathf.Atan2(direction.y, direction.x));

        rb.velocity = direction * speed;
    }

    private IEnumerator Sonar()
    {
        yield return new WaitForSeconds(sonarFrequency);

        // Hacer el sonido
        // Hacer la animación del UI

        StartCoroutine(ShadowDelay());
    }

    private IEnumerator ShadowDelay()
    {
        attackDebug = true;

        yield return new WaitForSeconds(shadowDelay);

        // if ((player.transform.position - transform.position).magnitude < sonarAttackDistance && player.GetComponent<Rigidbody2D>().velocity.sqrMagnitude != 0)
        // {
        //     attack = true;
        // }

        attackDebug = false;

        if ((player.transform.position - transform.position).magnitude < sonarAttackDistance)
        {
            attack = true;
        }
        else
        {
            StartCoroutine(Sonar());
        }
    }

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
