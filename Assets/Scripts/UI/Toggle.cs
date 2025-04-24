//---------------------------------------------------------
//---------------------------------------------------------
// Gestiona el toggle cambiando el estado de easyMode y el cheat de inmortalidad
// Vicente Rodríguez Casado
// Andrés Bartolomé Clap
// Proyect abyss
// Proyectos 1 - Curso 2024-25
//---------------------------------------------------------

using UnityEngine;
using UnityEngine.Rendering.Universal;


// Añadir aquí el resto de directivas using


/// <summary>
/// Antes de cada class, descripción de qué es y para qué sirve,
/// usando todas las líneas que sean necesarias.
/// </summary>
public class Toggle : MonoBehaviour
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

    #endregion
    
   
    // ---- MÉTODOS DE MONOBEHAVIOUR ----
    #region Métodos de MonoBehaviour



    /// <summary>
    /// Start is called on the frame when a script is enabled just before 
    /// any of the Update methods are called the first time.
    /// </summary>
    void Start()
    {
       GameManager.Instance.SetEasyMode(false);
       GameManager.Instance.SetInmortal(false);
       GameManager.Instance.SetTeleport(false);
       GameManager.Instance.GetComponent<Light2D>().enabled = false;
    }


    // ---- MÉTODOS PÚBLICOS ----
    #region Métodos públicos
    // Documentar cada método que aparece aquí con ///<summary>
    // El convenio de nombres de Unity recomienda que estos métodos
    // se nombren en formato PascalCase (palabras con primera letra
    // mayúscula, incluida la primera letra)
    // Ejemplo: GetPlayerController

    public void EasyMode()
    {
        
        // Cambia el estado de easyMode
        GameManager.Instance.ToggleEasyMode();  


    }
    public void Inmortal()
    {

        // Cambia el estado del cheat de inmortalidad
        GameManager.Instance.ToggleInmortal();


    }
    public void Teleport()
    {
        // Cambia el estado del cheat de teleportarse
        GameManager.Instance.ToggleTeleport();

    }

    #endregion

    // ---- MÉTODOS PRIVADOS ----
    #region Métodos Privados
    // Documentar cada método que aparece aquí
    // El convenio de nombres de Unity recomienda que estos métodos
    // se nombren en formato PascalCase (palabras con primera letra
    // mayúscula, incluida la primera letra)

    #endregion

} // class Toggle 
  // namespace
#endregion