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
            Debug.Log("a");
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

            GetComponentInChildren<SpriteRenderer>().flipY = direction.y > 0;

            _rb.velocity = direction * PerserSpeed;
        }
        //}
        override public void NextState()
        {
            // Define las condiciones para pasar al siguiente estado
            if (flashed)
            {
                //enemyScript.State = new EnemyFleeState(this.enemyObject);
            }
        }
    }
}
