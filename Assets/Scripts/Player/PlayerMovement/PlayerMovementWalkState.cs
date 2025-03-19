//---------------------------------------------------------
// Breve descripción del contenido del archivo
// Responsable de la creación de este archivo
// Nombre del juego
// Proyectos 1 - Curso 2024-25
//---------------------------------------------------------

using EnemyLogic;
using PlayerLogic;
using UnityEngine;
// Añadir aquí el resto de directivas using


/// <summary>
/// Antes de cada class, descripción de qué es y para qué sirve,
/// usando todas las líneas que sean necesarias.
/// </summary>
namespace PlayerLogic
{
    class PlayerWalkState : PlayerState
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
        private float joystickMaxSpeed; // El límite de velocidad con el que operará el script (en el caso del teclado no hace nada, en el caso del joystick se multiplica la velocidad máxima por el desplazamiento horizontal de este)
        [SerializeField]private float maxSpeed= 2f;
        [SerializeField]private float acceleration= 4f;
        [SerializeField]private float deceleration= 5f;
        [SerializeField]private float decelerationThreshold= 0.2f; // Unity pone esto a 0 al crear el estado.



        private PlayerScript player;
        private GameObject playerObject;
        private Rigidbody2D rb;
        public PlayerWalkState(GameObject playerObject){
            this.playerObject = playerObject;
            player = playerObject.GetComponent<PlayerScript>();
            rb = playerObject.GetComponent<Rigidbody2D>();
        }

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

        #endregion

        // ---- MÉTODOS PÚBLICOS ----
        #region Métodos públicos
        // Documentar cada método que aparece aquí con ///<summary>
        // El convenio de nombres de Unity recomienda que estos métodos
        // se nombren en formato PascalCase (palabras con primera letra
        // mayúscula, incluida la primera letra)
        // Ejemplo: GetPlayerController
        void Start()
        {
                playerObject = gameObject;
                player = playerObject.GetComponent<PlayerScript>();
                rb = playerObject.GetComponent<Rigidbody2D>();
        }
        override public void Move()
        {
            if (InputManager.Instance.MovementVector.x != 0)
            {
                joystickMaxSpeed = this.maxSpeed * InputManager.Instance.MovementVector.x;
                Walk(InputManager.Instance.MovementVector.x);
            }
            else Decelerate(deceleration);
        }
           override public void SetPlayer(GameObject player){
            playerObject = player;
        }
        override public void NextState()
        {
            //Debug.Log("State Walk");


            if (player.rb.velocity == new Vector2(0, 0))
            {
                //player.State = new IdleState;
                player.State = gameObject.AddComponent<PlayerIdleState>();
                AudioManager.instance.StopLoopingSFX(1);

            }
            else if (player.rb.velocity.y < 0)
            {
                //player.State = new FallState;
                player.State = gameObject.AddComponent<PlayerFallState>();
            }
            else if (InputManager.Instance.JumpWasPressedThisFrame())
            {
                //player.State = new JumpState;
                player.State = gameObject.AddComponent<PlayerJumpState>();
                AudioManager.instance.PlaySFX(2);
                AudioManager.instance.StopLoopingSFX(1);

            }
            else if (player.isLanternAimed)
            {
                player.State = gameObject.AddComponent<PlayerAimState>();
            }
        }

        #endregion

        // ---- MÉTODOS PRIVADOS ----
        #region Métodos Privados
        // Documentar cada método que aparece aquí
        // El convenio de nombres de Unity recomienda que estos métodos
        // se nombren en formato PascalCase (palabras con primera letra
        // mayúscula, incluida la primera letra)
        private void Walk(float x) // Mueve al jugador en la dirección indicada por el signo de x y con la velocidad máxima indicada por el valor de x
        {
            if ((x < 0 && player.rb.velocity.x > 0) || (x > 0 && player.rb.velocity.x < 0)) // Deceleración en cambio de sentido
            {
                Decelerate(deceleration);
            }
            else // Aceleración en el sentido del movimiento
            {
            //Debug.Log("moviendose");
            Debug.Log(acceleration);

                rb.AddForce(new Vector2(x, 0).normalized * acceleration, ForceMode2D.Force);
                //rb.AddForce(new Vector2(100,100));
            }

            if (Mathf.Abs(player.rb.velocity.x) > Mathf.Abs(joystickMaxSpeed)) // Limitación de la velocidad
            {
                if (joystickMaxSpeed == 1 || joystickMaxSpeed == -1)
                {
                    rb.velocity = player.rb.velocity.normalized * Mathf.Abs(joystickMaxSpeed);
                }
                else
                {
                    Decelerate(acceleration); // En el caso (nada raro) de que el joystick pase de un valor a otro más bajo del mismo signo, se frena con el valor de la aceleración
                }
            }

        }
        private void Decelerate(float decelerationValue) // Frena al jugador con la aceleración negativa indicada
        {
            if (player.rb.velocity.x > decelerationThreshold) // Comprobación de signo para elegir el sentido de la fuerza
            {
                player.rb.AddForce(new Vector2(-decelerationValue, 0), ForceMode2D.Force);
            }
           else if (player.rb.velocity.x < -decelerationThreshold)
            {
                player.rb.AddForce(new Vector2(decelerationValue, 0), ForceMode2D.Force);
            }
        }
        private void OnDrawGizmos()
        {
/*            if (player.debug)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(player.transform.position, player.transform.position + new Vector3(player.rb.velocity.x, player.rb.velocity.y, 0));
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(player.transform.position, player.transform.position + new Vector3(InputManager.Instance.MovementVector.x, InputManager.Instance.MovementVector.y, 0));
            }*/
        }

        #endregion

    }
}
