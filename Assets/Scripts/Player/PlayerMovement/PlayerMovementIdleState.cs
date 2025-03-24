//---------------------------------------------------------
// Breve descripción del contenido del archivo
// Responsable de la creación de este archivo
// Nombre del juego
// Proyectos 1 - Curso 2024-25
//---------------------------------------------------------

using UnityEditorInternal;
using UnityEngine;
// Añadir aquí el resto de directivas using


/// <summary>
/// Antes de cada class, descripción de qué es y para qué sirve,
/// usando todas las líneas que sean necesarias.
/// </summary>
namespace PlayerLogic
{
    class PlayerIdleState : PlayerState {
    // ---- ATRIBUTOS PRIVADOS ----
        #region Atributos Privados (private fields)
        // Documentar cada atributo que aparece aquí.
        // El convenio de nombres de Unity recomienda que los atributos
        // privados se nombren en formato _camelCase (comienza con _,
        // primera palabra en minúsculas y el resto con la
        // primera letra en mayúsculas)
        // Ejemplo: _maxHealthPoints
        #endregion
        private PlayerScript player;
        public GameObject playerObject;

        //private PlayerScript player;
        //private GameObject playerObject;
        private Rigidbody2D rb;
        public PlayerIdleState(GameObject playerObject){
            player = playerObject.GetComponent<PlayerScript>();
            rb = playerObject.GetComponent<Rigidbody2D>();
        }


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
        override public void SetPlayer(GameObject player){
            playerObject = player;
        }
        override public void Move()
        {
        }
         override public void NextState() {

            //Debug.Log(playerObject);
            //Debug.Log(player);

            if (InputManager.Instance.MovementVector.x != 0)
            {
                //player.State = new WalkState;
                player.State = gameObject.AddComponent<PlayerWalkState>();
                AudioManager.Instance.PlaySFX(SFXType.Walk, true);

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
                AudioManager.Instance.PlaySFX(SFXType.Jump);
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

        #endregion
    }
}
