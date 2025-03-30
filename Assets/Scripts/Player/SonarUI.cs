//---------------------------------------------------------
// Este archivo se encarga del funcionamiento del UI del jugador, concretamente de la parte del sonar
// Javier Zazo Morillo
// Project Abyss
// Proyectos 1 - Curso 2024-25
//---------------------------------------------------------

using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
// Añadir aquí el resto de directivas using


/// <summary>
/// Antes de cada class, descripción de qué es y para qué sirve,
/// usando todas las líneas que sean necesarias.
/// </summary>
public class SonarUI : MonoBehaviour
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

    private Animation sonarIndicatorAnimation;
    private Animation pulseIndicatorAnimation;

    private Animation[] indicatorsAnimation;

    private SpriteRenderer pulseIndicator;

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
        indicatorsAnimation = GetComponentsInChildren<Animation>();

        sonarIndicatorAnimation = indicatorsAnimation[0];       
        pulseIndicatorAnimation = indicatorsAnimation[1];

        pulseIndicator = GameObject.Find("pulseIndicator").GetComponent<SpriteRenderer>();
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
    /// Activa la parte de la circunferencia del UI
    /// </summary>
    public void ActivateSonarUI()
    {
        sonarIndicatorAnimation.Play("SonarUIAppear");
    }
    /// <summary>
    /// Desactiva la parte de la circunferencia del UI
    /// </summary>
    public void DeactivateSonarUI() 
    {
        sonarIndicatorAnimation.Play("SonarUIDisappear");
    }
    /// <summary>
    /// Activa la parte del pulso del UI
    /// </summary>
    public void ActivatePulseUI()
    {
        pulseIndicator.enabled = true;
        Debug.Log("activado");
    }
    /// <summary>
    /// Desactiva la parte del pulso del UI
    /// </summary>
    public void DeactivatePulseUI()
    {
        pulseIndicator.enabled = false;
        Debug.Log("desactivado");
    }
    /// <summary>
    /// Ejecuta la animación del pulso del UI
    /// </summary>
    public void PlayAnimation()
    {
        pulseIndicatorAnimation.Play();
    }

    #endregion

    // ---- MÉTODOS PRIVADOS ----
    #region Métodos privados
    // Documentar cada método que aparece aquí
    // El convenio de nombres de Unity recomienda que estos métodos
    // se nombren en formato PascalCase (palabras con primera letra
    // mayúscula, incluida la primera letra)

    #endregion

} // class SonarUI 
// namespace
