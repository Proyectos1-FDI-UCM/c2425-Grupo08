//---------------------------------------------------------
// Componente Engine simplificado y comentado para principiantes
//---------------------------------------------------------
using UnityEngine;

public class Engine : MonoBehaviour
{
    // Tiempo necesario para reparar el motor (segundos)
    public float repairTime = 4f;

    // Mensaje mostrado al completar la reparación
    [TextArea(2, 5)]
    [SerializeField] private string repairCompleteMessage = "Generador reparado, refugio desbloqueado";

    // Referencias a otros componentes
    private GameObject player;
    private Terminal terminal;
    private GeneratorEnemySpawner enemySpawner;
    private AudioSource audioSource;
    private Animator animator;

    // Estado del motor
    private bool playerInZone = false;
    private bool isRepairing = false;
    private bool isRepaired = false;
    private float repairProgress = 0f;

    void Start()
    {
        // Buscar referencias necesarias
        player = GameObject.FindGameObjectWithTag("Player");
        terminal = GetComponentInChildren<Terminal>();
        enemySpawner = GetComponent<GeneratorEnemySpawner>();
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Si el jugador está en la zona y el motor no está reparado
        if (playerInZone && !isRepaired)
        {
            // Si se pulsa la tecla de interactuar
            if (InputManager.Instance.InteractIsPressed())
            {
                if (!isRepairing)
                {
                    StartRepair();
                }
            }
            else
            {
                if (isRepairing)
                {
                    StopRepair();
                }
            }
        }

        // Si se está reparando, aumentar el progreso
        if (isRepairing && !isRepaired)
        {
            repairProgress += Time.deltaTime / repairTime;

            if (repairProgress > 1f)
                repairProgress = 1f;

            if (terminal != null)
                terminal.UpdateRepairProgress(repairProgress);

            // Animación: velocidad según progreso
            if (animator != null)
                animator.speed = Mathf.Lerp(0.5f, 4f, repairProgress);

            if (repairProgress >= 1f)
                CompleteRepair();
        }
    }

    // Iniciar reparación
    void StartRepair()
    {
        isRepairing = true;
        if (animator != null) animator.speed = 0.5f;
        if (enemySpawner != null) enemySpawner.SetCanRespawn(true);
        if (audioSource != null) AudioManager.instance.PlaySFX(SFXType.MotorSound, audioSource, true);
        if (player != null) player.GetComponent<PlayerMovement>().SetIsRepairing(true);
    }

    // Detener reparación
    void StopRepair()
    {
        isRepairing = false;
        if (animator != null) animator.speed = 0f;
        if (enemySpawner != null) enemySpawner.SetCanRespawn(false);
        if (audioSource != null) AudioManager.instance.StopSFX(audioSource);
        if (player != null) player.GetComponent<PlayerMovement>().SetIsRepairing(false);
    }

    // Completar reparación
    void CompleteRepair()
    {
        isRepaired = true;
        isRepairing = false;
        repairProgress = 0f;
        if (animator != null) animator.speed = 0f;
        if (enemySpawner != null) enemySpawner.SetCanRespawn(false);
        if (audioSource != null) AudioManager.instance.StopSFX(audioSource);
        if (player != null) player.GetComponent<PlayerMovement>().SetIsRepairing(false);
        if (terminal != null) 
        {
            terminal.Write(repairCompleteMessage);
            terminal.SetMessage(repairCompleteMessage);
        }
        LevelManager.Instance.MotorRepaired();
    }

    // Detectar entrada del jugador
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject == player)
        {
            playerInZone = true;
        }
    }

    // Detectar salida del jugador
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject == player)
        {
            playerInZone = false;
            StopRepair();
        }
    }
}
