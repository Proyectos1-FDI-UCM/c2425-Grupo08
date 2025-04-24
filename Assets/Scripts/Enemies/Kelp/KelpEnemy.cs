//---------------------------------------------------------
// // Miguel Ángel González López
// Project Abyss
// Proyectos 1 - Curso 2024-25
//---------------------------------------------------------

using UnityEngine;
// Añadir aquí el resto de directivas using


/// <summary>
/// Clase del enemigo alga. Tiene toda la lógica del alga, pero el movimiento lo ejecuta el jugador.
/// </summary>
public class KelpEnemy : MonoBehaviour
{
    // ---- ATRIBUTOS DEL INSPECTOR ----
    #region Atributos del Inspector (serialized fields)
    [SerializeField] private float activateRadius = 10f;
    [SerializeField] private float grabRadius = 5f;
    [SerializeField] private PlayerMovement Player;
    #endregion
    
    // ---- ATRIBUTOS PRIVADOS ----
    #region Atributos Privados (private fields)
    private bool _isGrabbed = false;
    private bool _isActive = false;
    #endregion
    
    // ---- MÉTODOS DE MONOBEHAVIOUR ----
    #region Métodos de MonoBehaviour
    
    
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
            Player.isGrabbed = true;
            Player.kelpGrabbing = this;
        }
        else if (IsPlayerInside(activateRadius)){
            _isActive = true;
            _isGrabbed = false;
            Player.isGrabbed =false;
        }
        else {
            _isActive = false;
            _isGrabbed = false;
        }
        
    }
    #endregion

    // ---- MÉTODOS PÚBLICOS ----
    #region Métodos públicos
    /// <summary>
    /// Método externo que avisa de que el jugador ya no está siendo agarrado.
    /// </summary>
    public void ReleasePlayer(){
        _isGrabbed = false;
    }
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
            Gizmos.DrawSphere(transform.position,0.1f);
        }
        else if (_isActive){
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(transform.position,0.1f);
        }
        else{
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(transform.position,0.1f);
        }
    }
} // class KelpEnemy 
// namespace
