//---------------------------------------------------------
// Breve descripción del contenido del archivo
// Responsable de la creación de este archivo
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
public class OxigenScript : MonoBehaviour
{
    // ---- ATRIBUTOS DEL INSPECTOR ----
    #region Atributos del Inspector (serialized fields)
    // Documentar cada atributo que aparece aquí.
    // El convenio de nombres de Unity recomienda que los atributos
    // públicos y de inspector se nombren en formato PascalCase
    // (palabras con primera letra mayúscula, incluida la primera letra)
    // Ejemplo: MaxHealthPoints

    [SerializeField] private int maxOxigen; // La cantidad máxima de oxígeno que puede tener el jugador
    [SerializeField] private float oxigenDecayHealthy; // La cantidad de oxígeno que se pierde por segundo al estar en estado "sano"
    [SerializeField] private float oxigenDecayBroken; // La cantidad de oxígeno que se pierde por segundo al estar en estado "roto"
    [SerializeField] private Text oxigenText; // El texto que muestra la cantidad de oxígeno que tiene el jugador

    #endregion

    // ---- ATRIBUTOS PRIVADOS ----
    #region Atributos Privados (private fields)
    // Documentar cada atributo que aparece aquí.
    // El convenio de nombres de Unity recomienda que los atributos
    // privados se nombren en formato _camelCase (comienza con _, 
    // primera palabra en minúsculas y el resto con la 
    // primera letra en mayúsculas)
    // Ejemplo: _maxHealthPoints

    private float currentOxigen; // La cantidad actual de oxígeno que tiene el jugador
    private bool tankBroken = false; // Indica si el tanque de oxígeno está roto o no

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
        currentOxigen = maxOxigen;
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update() // Cada frame se resta oxígeno al jugador y en el caso de llegar a 0 el jugador muere
    {
        oxigenText.text = ((int)currentOxigen).ToString();

        if (tankBroken)
        {
            currentOxigen -= oxigenDecayBroken * Time.deltaTime;
        }
        else
        {
            currentOxigen -= oxigenDecayHealthy * Time.deltaTime;
        }

        if (currentOxigen <= 0)
        {
            currentOxigen = 0;
            // die
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

    public void PierceTank() // Método que se llama cuando el tanque de oxígeno recibe un impacto (si el tanque ya estaba roto, el jugador muere
    {
        if (tankBroken)
        {
            // die
        }
        else
        {
            tankBroken = true;
        }      
    }
    public void RepairTank() // Método que se llama cuando el tanque de oxígeno es reparado
    {
        tankBroken = false;
    }

    #endregion
    
    // ---- MÉTODOS PRIVADOS ----
    #region Métodos Privados
    // Documentar cada método que aparece aquí
    // El convenio de nombres de Unity recomienda que estos métodos
    // se nombren en formato PascalCase (palabras con primera letra
    // mayúscula, incluida la primera letra)

    #endregion   

} // class OxigenScript 
// namespace
