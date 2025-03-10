//---------------------------------------------------------
// Breve descripción del contenido del archivo: Es la funcion para que el oxigeno baje poco a poco
// Responsable de la creación de este archivo: Andrés Díaz Guerrero Soto (El sordo)
// Nombre del juego
// Proyectos 1 - Curso 2024-25
//---------------------------------------------------------

using UnityEngine;
using UnityEngine.UI;
// Añadir aquí el resto de directivas using


/// <summary>
/// Antes de cada class, descripción de qué es y para qué sirve,
/// usando todas las líneas que sean necesarias.
/// </summary>

/// <summary>
/// Se encarga de gestionar la interfaz de usuario (HUD) Incluyendo el
/// medidor de oxigeno, que se muestre la pantalla
/// </summary>

public class UiManager : MonoBehaviour
{
    // ---- ATRIBUTOS DEL INSPECTOR ----
    #region Atributos del Inspector (serialized fields)
    // Documentar cada atributo que aparece aquí.
    // El convenio de nombres de Unity recomienda que los atributos
    // públicos y de inspector se nombren en formato PascalCase
    // (palabras con primera letra mayúscula, incluida la primera letra)
    // Ejemplo: MaxHealthPoints

    #endregion

    // Referencia de imagen de la barrade oxigeno (lo usare un circulo)
    [SerializeField] private Image OxygenCircle;




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

    // Por defecto están los típicos (Update y Start) pero:
    // - Hay que añadir todos los que sean necesarios
    // - Hay que borrar los que no se usen 

    /// <summary>
    /// Start is called on the frame when a script is enabled just before 
    /// any of the Update methods are called the first time.
    /// </summary>
    void Start()
    {
        if (OxygenCircle == null)
        {
            // Inicializa el fillamount a 1, para representar que esta 100%
            OxygenCircle.fillAmount = 1f;
        }
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

    #endregion
    
    /// <summary>
    /// Atualiza el medidor de oxigeno haceindo que el circulo se vacie gradualmente
    /// </summary>
    /// <param name="oxygenPorcentaje">Porcentaje de oxigeno (entre 0 y 1)</param>

    public void UpdateOxygenUI(float OxigenPercentage)
    {
        if (OxygenCircle == null) return;
        // Atualiza el fillAmaunt del componetne de la imagen
        OxygenCircle.fillAmount = OxigenPercentage;
    }

    // ---- MÉTODOS PRIVADOS ----
    #region Métodos privados
    // Documentar cada método que aparece aquí
    // El convenio de nombres de Unity recomienda que estos métodos
    // se nombren en formato PascalCase (palabras con primera letra
    // mayúscula, incluida la primera letra)

    #endregion   

} // class UiManager 
// namespace
