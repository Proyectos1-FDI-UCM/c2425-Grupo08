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
public class DeathBarrier : MonoBehaviour
{   
    // ---- MÉTODOS PRIVADOS ----
    #region Métodos Privados
    /// <summary>
    /// Detecta cuando el jugador entra en la zona de interacción del motor.
    /// Activa la interfaz de carga si el motor no ha sido reparado aún.
    /// </summary>
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<PlayerMovement>() != null)

            other.GetComponent<OxigenScript>().Death();
    }

    #endregion   

} // class DeathBarrier 
// namespace
