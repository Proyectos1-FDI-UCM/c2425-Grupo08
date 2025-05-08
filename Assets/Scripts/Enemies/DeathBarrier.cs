//---------------------------------------------------------
// Mata al jugador al entrar en la zona de interacción
// Carlos Dochao Moreno
// Beyond the Depths
// Proyectos 1 - Curso 2024-25
//---------------------------------------------------------

using UnityEngine;

/// <summary>
/// Componente que representa una barrera de muerte en el juego.
/// </summary>
public class DeathBarrier : MonoBehaviour
{   
    // ---- MÉTODOS PRIVADOS ----
    #region Métodos Privados
    
    /// <summary>
    /// Detecta cuando el jugador entra en la zona de interacción.
    /// Si el jugador entra, se llama al método Death() del script OxigenScript.
    /// </summary>
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<PlayerMovement>() != null)

            other.GetComponent<OxigenScript>().Death();
    }

    #endregion   

} // class DeathBarrier