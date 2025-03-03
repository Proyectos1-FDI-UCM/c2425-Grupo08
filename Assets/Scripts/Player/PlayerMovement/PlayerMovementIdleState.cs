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
namespace PlayerLogic
{
class PlayerIdleState : PlayerState{
// ---- ATRIBUTOS PRIVADOS ----
    #region Atributos Privados (private fields)
    // Documentar cada atributo que aparece aquí.
    // El convenio de nombres de Unity recomienda que los atributos
    // privados se nombren en formato _camelCase (comienza con _,
    // primera palabra en minúsculas y el resto con la
    // primera letra en mayúsculas)
    // Ejemplo: _maxHealthPoints
    private PlayerScript player;
    #endregion
    public PlayerIdleState(PlayerScript playerObject){
        this.player = playerObject.GetComponent<PlayerScript>();
    }


    // ---- MÉTODOS PÚBLICOS ----
    #region Métodos públicos
    // Documentar cada método que aparece aquí con ///<summary>
    // El convenio de nombres de Unity recomienda que estos métodos
    // se nombren en formato PascalCase (palabras con primera letra
    // mayúscula, incluida la primera letra)
    // Ejemplo: GetPlayerController
    public void Move()
    {

    }
    public void NextState() {
        if (InputManager.Instance.MovementVector.x != 0)
        {
            //player.State = new WalkState;
            player.State = new PlayerIdleState(player); //texto de ejemplo, porfa cambiar por un estado real que cuando he creado esto no había
        }
        else if (player.rb.velocity.y < 0)

        {
            //player.State = new FallState;
        }
        else if (InputManager.Instance.JumpIsPressed())
        {
            //player.State = new JumpState;
        }
        // else if () // Aim
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
