//---------------------------------------------------------
// Gamma Controller. Modifica el gamma de la escena desde el slider.
// Vicente Rodríguez Casado
// Beyond the Depths
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

    private Slider slider; // Referencia al slider de gamma

    #endregion

    // ---- MÉTODOS DE MONOBEHAVIOUR ----
    #region Métodos de MonoBehaviour

    void Start() // Guardamos la referencia al slider
    {
        slider = GetComponent<Slider>();

       slider.value = GameManager.Instance.GetGamma();
    }

    #endregion

    // ---- MÉTODOS PÚBLICOS ----
    #region Métodos públicos
    
    public void ChangeGamma() // Cambia el gamma de la escena
    {
        GameManager.Instance.ChangeGamma(slider.value);
    }

    #endregion

} // class Gamma 
// namespace
