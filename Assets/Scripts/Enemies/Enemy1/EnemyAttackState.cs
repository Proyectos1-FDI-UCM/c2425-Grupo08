//---------------------------------------------------------
// Breve descripción del contenido del archivo
// Responsable de la creación de este archivo
// Nombre del juego
// Proyectos 1 - Curso 2024-25
//---------------------------------------------------------

using EnemyLogic;
using UnityEngine;

namespace EnemyLogic
{
    class EnemyAttackState : EnemyState
    {
        // Atributos privados de inspector

        [SerializeField] private float PerserSpeed = 3f; // Velocidad de persecución
        [SerializeField] private float rotateSpeed = 90f; // Velocidad de giro en grados/segundo (ajústalo para suavizar)
        [SerializeField] private bool spriteLooksUp = false; // true si el sprite está diseñado para apuntar hacia arriba (+Y), false si apunta a la derecha (+X)

        // Atributos privados
        private GameObject enemyObject;
        private EnemyScript enemyScript;

        private Rigidbody2D _rb;

        private Collider2D bodyCollider;

        private GameObject player;
        private Collider2D playerCollider;
        private Collider2D flashCollider;

        private bool flashed = false;

        //Audio
        private AudioSource audioSource; // Fuente de audio para reproducir sonidos

        public void Start()
        {
            this.enemyScript = GetComponentInParent<EnemyScript>();
            this.enemyObject = enemyScript.gameObject;
            //ERROR AQUÍ
            //this.bodyCollider = GetComponent<Collider2D>(); //ERROR
            this._rb = enemyObject.GetComponent<Rigidbody2D>();
            this.bodyCollider = enemyScript.EnemyCollider;
            this.playerCollider = enemyScript.PlayerCollider;
            this.flashCollider = enemyScript.FlashCollider;
            this.player =  enemyScript.PlayerObject;
            //this.playerCollider = player.GetComponent<Collider2D>(); //error
            //this.flashCollider = player.GetComponentInChildren<Collider2D>(); //error

            // Inicializa la fuente de audio
            audioSource = GetComponent<AudioSource>();

            // Reproducir el sonido de ataque si no está sonando
            PlayAttackSound();

        }

        // MonoBehaviour

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.GetComponent<Collider2D>() == flashCollider)
            {
                flashed = true;
            }
        }
        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.GetComponent<Collider2D>() == playerCollider)
            {
                collision.gameObject.GetComponent<OxigenScript>().PierceTank();
                Destroy(gameObject);
            }
        }

        // Métodos públicos
        override public void Move()
        {
            Vector2 direction = (player.transform.position - transform.position).normalized;

            transform.rotation = Quaternion.Euler(0, 0, Mathf.Rad2Deg * Mathf.Atan2(direction.y, direction.x));

            _rb.velocity = direction * PerserSpeed;

            // Ajustar volumen en función de la distancia al jugador
            audioSource.volume = CalculateVolume(player.transform.position);
        }
        
        
        override public void NextState()
        {
            // Define las condiciones para pasar al siguiente estado
            if (flashed) 
            {
                //enemyScript.State = new EnemyFleeState();
            }
        }

        // Métodos privados

        private void PlayAttackSound()
        {
            // Obtener el clip de sonido de ataque desde el AudioManager
            AudioManager.instance.PlaySFX(SFXType.AttackEnemy1, audioSource); // Cambiar a SFXType adecuado para ataque
         
               // Ajustar volumen en función de la distancia al jugador
                audioSource.volume = CalculateVolume(player.transform.position);

                // Configurar el AudioSource para que repita el sonido mientras esté en estado de ataque
                audioSource.loop = false; // O ajustarlo como necesites
                audioSource.Play();
            
        }

        // Calcular volumen en función de la distancia
        private float CalculateVolume(Vector3 targetPosition)
        {
            float distance = Vector3.Distance(targetPosition, transform.position);
            float volume = Mathf.Clamp01(1 - (distance / 15));  // Ajusta el divisor para que el volumen disminuya a la distancia que prefieras
            return volume;
        }
    }
}
