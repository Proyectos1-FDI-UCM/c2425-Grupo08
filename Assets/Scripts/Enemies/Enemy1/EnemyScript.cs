//---------------------------------------------------------
// Este script es la máquina de stados que gestiona los comportamientos del jugador.
// Miguel Ángel González López
// ProjectAbyss
// Proyectos 1 - Curso 2024-25
//---------------------------------------------------------

using UnityEngine;
// Añadir aquí el resto de directivas using

    namespace EnemyLogic{

    /// <summary>
    /// Antes de cada class, descripción de qué es y para qué sirve,
    /// usando todas las líneas que sean necesarias.
    /// </summary>

    struct Movement{
        float speed;
        Movement(float speed){
            this.speed = speed;
        }
    };
    public class EnemyScript : MonoBehaviour
    {
        // ---- ATRIBUTOS DEL INSPECTOR ----
        #region Atributos del Inspector (serialized fields)
        // Documentar cada atributo que aparece aquí.
        // El convenio de nombres de Unity recomienda que los atributos
        // públicos y de inspector se nombren en formato PascalCase
        // (palabras con primera letra mayúscula, incluida la primera letra)
        // Ejemplo: MaxHealthPoints

        #endregion

        // ---- ATRIBUTOS PRIVADOS ----
        #region Atributos Privados (private fields)
        // Documentar cada atributo que aparece aquí.
        // El convenio de nombres de Unity recomienda que los atributos
        // privados se nombren en formato _camelCase (comienza con _,
        // primera palabra en minúsculas y el resto con la
        // primera letra en mayúsculas)
        // Ejemplo: _maxHealthPoints
        public EnemyState State { get; set; } // Contexto del estado del enemigo.
        public GameObject PlayerObject{get;set;}
        public Collider2D EnemyCollider {get; set;}
        public Collider2D PlayerCollider {get;set;}
        public Collider2D FlashCollider {get;set;}

        private AudioSource audioSource; // Fuente de audio para reproducir sonidos
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
            State = gameObject.AddComponent<EnemyAttackState>();
            PlayerObject = GameObject.FindGameObjectWithTag("Player"); ; // Javier esta es la cosa. No fufa.

            //Debug.Log("PlayerObject: " + PlayerObject);

            EnemyCollider = this.GetComponent<Collider2D>(); // ahora todos estos componentes los tiene el enemigo defacto, es tontería que cada estado los pida.
            PlayerCollider = PlayerObject.GetComponent<Collider2D>(); // idem
            FlashCollider = PlayerObject.GetComponentInChildren<Collider2D>(); // idem

            // Inicializamos el AudioSource
            audioSource = GetComponent<AudioSource>();

            // Reproducir sonido de patrullaje (si no está sonando)
            PlayPatrolSound();
        }

        /// <summary>
        /// Update is called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        void FixedUpdate()
        {
            State.Move();

            CalculateVolume(PlayerObject.transform.position); // Ajustar volumen en función de la distancia al jugador
        }
        void Update()
        {
            State.NextState();
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

        private void PlayPatrolSound()
        {
            // Obtener el clip de sonido de patrullaje desde el AudioManager
            AudioManager.instance.PlaySFX(SFXType.PatrolEnemy1, audioSource);  // Cambiar a SFXType adecuado para patrullaje

            // Configurar el AudioSource para que repita el sonido mientras patrulla
            audioSource.loop = true;
            audioSource.Play();
        }

        // Calcular volumen en función de la distancia entre el enemigo y el jugador
        private float CalculateVolume(Vector3 targetPosition)
        {
            float distance = Vector3.Distance(targetPosition, transform.position);
            // Cuanto más cerca esté el jugador, más fuerte será el sonido
            float volume = Mathf.Clamp01(1 - (distance / 20)); // Ajusta el divisor para que el volumen se incremente a medida que el jugador se acerque
            return volume;
        }
        #endregion

    } // class EnemyScript
    // namespace
}
