//---------------------------------------------------------
// Breve descripción del contenido del archivo
// Responsable de la creación de este archivo
// Nombre del juego
// Proyectos 1 - Curso 2024-25
//---------------------------------------------------------


using UnityEngine;
// Añadir aquí el resto de directivas using


/// <summary>
/// Antes de cada class, descripción de qué es y para qué sirve,
/// usando todas las líneas que sean necesarias.
/// </summary>
public class EnemyRouteScript : MonoBehaviour
{
    // ---- ATRIBUTOS DEL INSPECTOR ----
    #region Atributos del Inspector (serialized fields)
    // Documentar cada atributo que aparece aquí.
    // El convenio de nombres de Unity recomienda que los atributos
    // públicos y de inspector se nombren en formato PascalCase
    // (palabras con primera letra mayúscula, incluida la primera letra)
    // Ejemplo: MaxHealthPoints

    [SerializeField] private GameObject[] NodeArray;
    [SerializeField] private float Speed;
    [SerializeField] private bool Debug = false;

    #endregion
    
    // ---- ATRIBUTOS PRIVADOS ----
    #region Atributos Privados (private fields)
    // Documentar cada atributo que aparece aquí.
    // El convenio de nombres de Unity recomienda que los atributos
    // privados se nombren en formato _camelCase (comienza con _, 
    // primera palabra en minúsculas y el resto con la 
    // primera letra en mayúsculas)
    // Ejemplo: _maxHealthPoints

    Rigidbody2D rb;
    SpriteRenderer spriteRenderer;

    private struct NodeRoute{
        private int nodeCont; //Contador que indica a que nodo se está moviendo 
        private GameObject[] nodeArray; //Array con todos los nodos
        private Collider2D collider;

        // Constructor
        public NodeRoute(GameObject[] _nodeArray){
            nodeArray = _nodeArray;
            nodeCont = 0;
            collider = nodeArray[0].GetComponent<Collider2D>();
        }
        // Pasa al siguiente nodo 
        public void SetNextNode(){
            nodeCont ++;
            nodeCont = nodeCont % nodeArray.Length ; // Vuelve al 0 cuando se pasa del tamaño del array
            collider = nodeArray[nodeCont].GetComponent<Collider2D>();
        }
        // Devuelve el nodo al que se dirige 
        public GameObject GetNextNode(){
            return nodeArray[nodeCont];
        }
        public Collider2D GetCollider(){
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
        nodeRoute = new NodeRoute(NodeArray); // Aviso. El array de nodos se crea al inicio, no es dinámico.
        MoveEnemy();

        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
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

    private void MoveEnemy(){
        rb.velocity = Vector3.Normalize(nodeRoute.GetNextNode().transform.position - transform.position) * Speed;

        if (rb.velocity.x > 0)
        {
            transform.localScale = new Vector3(1, -1, 1);
        }
        else
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision == nodeRoute.GetCollider()){
            nodeRoute.SetNextNode();
            MoveEnemy();
        }
    }

    #endregion

    void OnDrawGizmos()
    {
        if (Debug) {
        Gizmos.color = Color.green;
        foreach(GameObject go in NodeArray){
            Gizmos.DrawSphere(go.transform.position,0.5f);
        }
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position,0.5f);    
        } 
    }
} // class EnemieRouteScript 
// namespace
