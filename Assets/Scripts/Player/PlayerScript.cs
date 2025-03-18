//---------------------------------------------------------
// Breve descripción del contenido del archivo
// Responsable de la creación de este archivo
// Nombre del juego
// Proyectos 1 - Curso 2024-25
//---------------------------------------------------------

using UnityEngine;
// Añadir aquí el resto de directivas using

namespace PlayerLogic
{

/// <summary>
/// Antes de cada class, descripción de qué es y para qué sirve,
/// usando todas las líneas que sean necesarias.
/// </summary>
public struct Movement{
    [SerializeField]public float maxSpeed {get;set;}
    [SerializeField]public float acceleration {get;set;}
    [SerializeField]public float deceleration {get;set;}
    [SerializeField]public float decelerationThreshold {get;set;}
    [SerializeField]public float jumpAcceleration {get;set;}
    [SerializeField]public float jumpMultiplierDecay {get;set;}
    public Movement(float maxSpeed, float acceleration, float deceleration, float decelerationThreshold, float jumpAcceleration, float jumpMultiplierDecay){
        this.maxSpeed = maxSpeed;
        this.acceleration = acceleration;
        this.deceleration = deceleration;
        this.decelerationThreshold = decelerationThreshold;
        this.jumpAcceleration = jumpAcceleration;
        this.jumpMultiplierDecay = jumpMultiplierDecay;
    }
}
    public class PlayerScript : MonoBehaviour
    {
        // ---- ATRIBUTOS DEL INSPECTOR ----
        #region Atributos del Inspector (serialized fields)
        // Documentar cada atributo que aparece aquí.
        // El convenio de nombres de Unity recomienda que los atributos
        // públicos y de inspector se nombren en formato PascalCase
        // (palabras con primera letra mayúscula, incluida la primera letra)
        // Ejemplo: MaxHealthPoints

        [SerializeField] public bool debug{get;set;}
        #endregion

        // ---- ATRIBUTOS PRIVADOS ----
        #region Atributos Privados (private fields)
        // Documentar cada atributo que aparece aquí.
        // El convenio de nombres de Unity recomienda que los atributos
        // privados se nombren en formato _camelCase (comienza con _,
        // primera palabra en minúsculas y el resto con la
        // primera letra en mayúsculas)
        // Ejemplo: _maxHealthPoints

        private float joystickMaxSpeed;
        public Rigidbody2D rb {get; set;} //Estos métodos son propiedades, por debajo son públicos
        public bool isLanternAimed {get; set;}
        private float jumpMultiplier = 1;
        public PlayerState State {get;set;}
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
            State = gameObject.AddComponent<PlayerIdleState>();
            State.SetPlayer(this.gameObject);
            print(GetComponent<GameObject>());
            //Debug.Log("Player Starts");
            //Debug.Log("State: " + State);
        }

        /// <summary>
        /// Update is called every frame, if the MonoBehaviour is enabled.
        /// </summary>:
        public void Update()
        {
            State.NextState();
            //Debug.Log(State);
        }
        public void FixedUpdate()
        {
            State.Move();
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

        #endregion
    }
}
