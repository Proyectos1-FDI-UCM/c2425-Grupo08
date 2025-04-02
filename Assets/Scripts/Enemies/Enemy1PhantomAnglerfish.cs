//---------------------------------------------------------
// Este archivo se encarga del funcionamiento del enemigo 1 (rape fantasma)
// Javier Zazo Morillo
// Project abyss
// Proyectos 1 - Curso 2024-25
//---------------------------------------------------------

using System.Collections;
using UnityEngine;
// Añadir aquí el resto de directivas using


/// <summary>
/// Este archivo se encarga del funcionamiento del enemigo 1 (rape fantasma)
/// </summary>
public class Enemy1PhantomAnglerfish : MonoBehaviour
{
    // ---- ATRIBUTOS DEL INSPECTOR ----
    #region Atributos del Inspector (serialized fields)
    // Documentar cada atributo que aparece aquí.
    // El convenio de nombres de Unity recomienda que los atributos
    // públicos y de inspector se nombren en formato PascalCase
    // (palabras con primera letra mayúscula, incluida la primera letra)
    // Ejemplo: MaxHealthPoints

    [SerializeField] private float patrolSpeed;
    [SerializeField] private float attackSpeed;
    [SerializeField] private float fleeSpeed;

    [SerializeField] private float disintegrationDelay;

    [SerializeField] private float maxHearingDistance;

    #endregion

    // ---- ATRIBUTOS PRIVADOS ----
    #region Atributos Privados (private fields)
    // Documentar cada atributo que aparece aquí.
    // El convenio de nombres de Unity recomienda que los atributos
    // privados se nombren en formato _camelCase (comienza con _, 
    // primera palabra en minúsculas y el resto con la 
    // primera letra en mayúsculas)
    // Ejemplo: _maxHealthPoints

    private GameObject[] nodeArray;

    private Rigidbody2D rb;

    private GameObject player;
    private Collider2D playerCollider;
    private Collider2D flashCollider;

    private Collider2D enemyCollider;

    private bool attack = false;
    private bool flee = false;

    //Audio
    private AudioSource audioSource;
    private float patrolSoundCooldown = 0f; // Tiempo que ha pasado desde la última vez que se reprodujo el sonido
    private float patrolSoundInterval; // Intervalo aleatorio para ejecutar el sonido

    private bool attackSoundPlaying = false;
    private bool fleeSoundPlaying = false;

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
        SetNodeArray();

        // Inicializar el AudioSource
        audioSource = GetComponent<AudioSource>();

        player = GameObject.FindGameObjectWithTag("Player");

        playerCollider = player.GetComponent<Collider2D>();
        flashCollider = player.GetComponentInChildren<FlashLight>().GetComponentInChildren<Collider2D>();

        Debug.Log(playerCollider);
        Debug.Log(flashCollider);

        enemyCollider = GetComponent<Collider2D>();

        rb = GetComponent<Rigidbody2D>();      

        nodeRoute = new NodeRoute(nodeArray); // Aviso. El array de nodos se crea al inicio, no es dinámico.

        // Inicializa el intervalo de tiempo aleatorio para el ataque
        patrolSoundInterval = Random.Range(10f, 20f); //Rango entre 10 y 20 segundos 
    }

    private void Update()
    {
        audioSource.volume = CalculateVolume(player.transform.position);
    }

    void FixedUpdate()
    {
        if (flee)
        {
            Move((transform.position - player.transform.position).normalized, fleeSpeed);
            if (!fleeSoundPlaying)
            {
                PlayFleeSound();
                fleeSoundPlaying = true;
            }
        } 
        else if (attack)
        {
            Move((player.transform.position - transform.position).normalized, attackSpeed);
            if (!attackSoundPlaying)
            {
                PlayAttackSound();
                attackSoundPlaying = true;
            }
        }
        else
        {
            Move((nodeRoute.GetNextNode().transform.position - transform.position).normalized, patrolSpeed);

            // Incrementar el temporizador del sonido de patrullaje
            patrolSoundCooldown += Time.fixedDeltaTime;
            audioSource.volume = CalculateVolume(player.transform.position);

            // Comprobar si ha pasado suficiente tiempo para reproducir el sonido de patrullaje
            if (patrolSoundCooldown >= patrolSoundInterval)
            {
      
                PlayPatrolSound();
                patrolSoundCooldown = 0f; // Reiniciar el temporizador
                patrolSoundInterval = Random.Range(10f, 20f); // Establecer un nuevo intervalo aleatorio entre 10 y 20 segundos
            }
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

    /// <summary>
    /// Método que se llama desde el Awake de EnemyNodes para pasarle los nodos al enemigo
    /// </summary>
    /// <param name="nodes"></param>
    public void SetNodeArray()
    {
        nodeArray = GetComponentsInParent<Transform>()[1].GetComponentInChildren<EnemyNodes>().GetNodeArray();
    }
    public void SetAttack(bool attack)
    {
        this.attack = attack;
    }

    #endregion

    // ---- MÉTODOS PRIVADOS ----
    #region Métodos Privados
    // Documentar cada método que aparece aquí
    // El convenio de nombres de Unity recomienda que estos métodos
    // se nombren en formato PascalCase (palabras con primera letra
    // mayúscula, incluida la primera letra)

    private void Move(Vector2 direction, float speed)
    {
        transform.rotation = Quaternion.Euler(0, 0, Mathf.Rad2Deg * Mathf.Atan2(direction.y, direction.x));

        rb.velocity = direction * speed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("COLISIÓN CON: " + collision.gameObject);
        if (enemyCollider.IsTouching(nodeRoute.GetCollider()))
        {
            nodeRoute.SetNextNode();
        }
        else if (enemyCollider.IsTouching(flashCollider))
        {
            flee = true;
            Debug.Log("Huida");
            Debug.Log(collision.gameObject);
            StartCoroutine(DisintegrationDelay());
        }
        else if (collision.gameObject.GetComponent<Collider2D>() == playerCollider)
        {
            attack = true;
            Debug.Log("Ataque");
            Debug.Log(collision.gameObject);
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<Collider2D>() == playerCollider)
        {
            collision.gameObject.GetComponent<OxigenScript>().PierceTank();
            GetComponentInParent<Respawner>().EnemyDead(player);
            Destroy(gameObject);
        }
    }

    IEnumerator DisintegrationDelay()
    {
        yield return new WaitForSeconds(disintegrationDelay);
        GetComponentInParent<Respawner>().EnemyDead(player);
        Destroy(gameObject);
    }

    private void PlayFleeSound()
    {
        // Obtener el clip de sonido de huida desde el AudioManager
        AudioManager.instance.PlaySFX(SFXType.FleeEnemy1, audioSource);  // Cambiar a SFXType adecuado para huida

        // Configurar el AudioSource para que repita el sonido mientras esté en estado de huida
        audioSource.loop = false;
        audioSource.Play();
    }

    private void PlayAttackSound()
    {
        // Obtener el clip de sonido de ataque desde el AudioManager
        AudioManager.instance.PlaySFX(SFXType.AttackEnemy1, audioSource); // Cambiar a SFXType adecuado para ataque

        // Ajustar volumen en función de la distancia al jugador
        //audioSource.volume = CalculateVolume(player.transform.position);

        // Configurar el AudioSource para que repita el sonido mientras esté en estado de ataque
        audioSource.loop = false; // O ajustarlo como necesites
        audioSource.Play();
    }

    private void PlayPatrolSound()
    {
        // Obtener el clip de sonido de patrulla desde el AudioManager
        AudioManager.instance.PlaySFX(SFXType.PatrolEnemy1, audioSource); // Cambiar a SFXType adecuado para patrullar

        // Ajustar volumen en función de la distancia al jugador
        //audioSource.volume = CalculateVolume(player.transform.position);

        // Configurar el AudioSource para que repita el sonido mientras esté en estado de patrulla
        audioSource.loop = false; // O ajustarlo como necesites
        audioSource.Play();
        Debug.Log("Sonido de patrulla reproducido");
    }

    private float CalculateVolume(Vector3 targetPosition)
    {
        float distance = Vector3.Distance(targetPosition, transform.position);
        float volume = Mathf.Clamp01(1 - (distance / maxHearingDistance));  // Ajusta el divisor para que el volumen disminuya a la distancia que prefieras
        return volume;
    }

    #endregion   

} // class Enemy1PhantomAnglerfish 
// namespace
