//---------------------------------------------------------
// Gamma Controller. Modifica el gamma de la escena desde el slider
// Vicente Rodríguez Casado
// Proyect abyss
// Proyectos 1 - Curso 2024-25
//---------------------------------------------------------

using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Antes de cada class, descripción de qué es y para qué sirve,
/// usando todas las líneas que sean necesarias.
/// </summary>
public class VolumeGammaController : MonoBehaviour
{
    // ---- ATRIBUTOS PRIVADOS ----
    #region Atributos Privados (private fields)

    private Slider slider;

    #endregion

    // ---- MÉTODOS DE MONOBEHAVIOUR ----
    #region Métodos de MonoBehaviour

    void Start()
    {
       slider = GetComponent<Slider>();

       GameManager.Instance.ChangeGamma(slider.value);
    }

    #endregion

    // ---- MÉTODOS PÚBLICOS ----
    #region Métodos públicos
    
    public void ChangeGamma() 
    {
        GameManager.Instance.ChangeGamma(slider.value);
    }

    #endregion

} // class Gamma 
// namespace
