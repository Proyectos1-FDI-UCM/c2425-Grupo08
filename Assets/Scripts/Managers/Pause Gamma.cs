//---------------------------------------------------------
// Breve descripción del contenido del archivo: Para poner en pausa el juego
// Responsable de la creación de este archivo: Andrés Díaz Guerrero Soto
// Nombre del juego
// Proyectos 1 - Curso 2024-25
//---------------------------------------------------------

using UnityEngine;
// Añadir aquí el resto de directivas using
using UnityEngine.EventSystems;

/// <summary>
/// Antes de cada class, descripción de qué es y para qué sirve,
/// usando todas las líneas que sean necesarias.
/// </summary>
public class PauseGamma : MonoBehaviour
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

    /// <summary>
    /// Guarda la escala de tiempo previa a la pausa para restaurarla posteriormente.
    /// </summary>
    private float _prevTimeScale = 1f;

    /// <summary>
    /// Indica si el juego está actualmente en pausa.
    /// </summary>
    private bool _isPaused = false;
    

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
        
    }
    #endregion

    // ---- MÉTODOS PÚBLICOS ----
    #region Métodos públicos
    // Documentar cada método que aparece aquí con ///<summary>
    // El convenio de nombres de Unity recomienda que estos métodos
    // se nombren en formato PascalCase (palabras con primera letra
    // mayúscula, incluida la primera letra)
    // Ejemplo: GetPlayerController

    /// <summary>
    /// Pausa o reanuda la simulación del juego.
    /// </summary>
    /// <param name="pause">True para pausar, false para reanudar.</param>
    public void SetPaused(bool pause)
    {
        if (pause == _isPaused) return;

        if (pause)
        {
            // Guardar la escala de tiempo actual y detener la simulación
            _prevTimeScale = Time.timeScale;
            Time.timeScale = 0f;

            // Silenciar el audio global
            AudioListener.pause = true;

            // Desactivar navegación de UI para evitar inputs no deseados
            EventSystem.current.sendNavigationEvents = false;
        }
        else
        {
            // Restaurar la escala de tiempo y reanudar simulación
            Time.timeScale = _prevTimeScale;
            AudioListener.pause = false;
            EventSystem.current.sendNavigationEvents = true;
        }

        _isPaused = pause;
    }

    /// <summary>
    /// Alterna el estado de pausa: si está en pausa, reanuda; si no, pausa.
    /// </summary>
    public void TogglePause()
    {
        SetPaused(!_isPaused);
    }

    #endregion

    // ---- MÉTODOS PRIVADOS ----
    #region Métodos Privados
    // Documentar cada método que aparece aquí
    // El convenio de nombres de Unity recomienda que estos métodos
    // se nombren en formato PascalCase (palabras con primera letra
    // mayúscula, incluida la primera letra)

    #endregion

} // class PauseGamma 
// namespace
