//---------------------------------------------------------
// 
// Miguel Ángel González López
// Project Abyss
// Proyectos 1 - Curso 2024-25
//---------------------------------------------------------

using UnityEngine;
// Añadir aquí el resto de directivas using


/// <summary>
/// Antes de cada class, descripción de qué es y para qué sirve,
/// usando todas las líneas que sean necesarias.
/// </summary>
public class KelpEnemy : MonoBehaviour
{
    // ---- ATRIBUTOS DEL INSPECTOR ----
    #region Atributos del Inspector (serialized fields)
    // Documentar cada atributo que aparece aquí.
    // El convenio de nombres de Unity recomienda que los atributos
    // públicos y de inspector se nombren en formato PascalCase
    // (palabras con primera letra mayúscula, incluida la primera letra)
    // Ejemplo: MaxHealthPoints
    [SerializeField] private float activateRadius = 10f;
    [SerializeField] private float grabRadius = 5f;
    [SerializeField] private GameObject Player;
    #endregion
    
    // ---- ATRIBUTOS PRIVADOS ----
    #region Atributos Privados (private fields)
    // Documentar cada atributo que aparece aquí.
    // El convenio de nombres de Unity recomienda que los atributos
    // privados se nombren en formato _camelCase (comienza con _, 
    // primera palabra en minúsculas y el resto con la 
    // primera letra en mayúsculas)
    // Ejemplo: _maxHealthPoints
    private bool _isGrabbed = false;
    private bool _isActive = false;
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
        
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        if (IsPlayerInside(grabRadius)){
            _isGrabbed = true;
        }
        else if (IsPlayerInside(activateRadius)){
            _isActive = true;
            _isGrabbed = false;
        }
        else {
            _isActive = false;
            _isGrabbed = false;
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

    #endregion
    
    // ---- MÉTODOS PRIVADOS ----
    #region Métodos Privados
    /// <summary>
    /// Compara la posición del jugador con la posición del enemigo alga y devuelve true si está dentro del radio dado.
    /// </summary>
    /// <param name="radius">Radio de la circunferencia de acción a comparar con el jugador</param>
    /// <returns></returns>
    private bool IsPlayerInside(float radius){
        return ((Player.transform.position-transform.position).magnitude <= radius); 
    }
    #endregion   

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, activateRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position,grabRadius);
        if (_isGrabbed){
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(transform.position,1f);
        }
        else if (_isActive){
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(transform.position,1f);
        }
        else{
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(transform.position,1f);
        }
    }

} // class KelpEnemy 
// namespace
