//---------------------------------------------------------
// Gamma Controller. Modifica el gamma de la escena desde el slider
// Vicente Rodríguez Casado
// Proyect abyss
// Proyectos 1 - Curso 2024-25
//---------------------------------------------------------

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;



/// <summary>
/// Antes de cada class, descripción de qué es y para qué sirve,
/// usando todas las líneas que sean necesarias.
/// </summary>
public class VolumeGammaController : MonoBehaviour
{
    // ---- ATRIBUTOS DEL INSPECTOR ----
    #region Atributos del Inspector (serialized fields)


    #endregion

    // ---- ATRIBUTOS PRIVADOS ----
    #region Atributos Privados (private fields)
    [Header("Referencias")]
    [Tooltip("El Volume que contiene el Lift Gamma Gain")]
    [SerializeField] private Volume postProcessingVolume;

    [Tooltip("El Slider UI que controlará el gamma")]
    [SerializeField] private Slider gammaSlider;

    // Aquí guardaremos la instancia del override
    private LiftGammaGain liftGammaGain;

    #endregion

    // ---- MÉTODOS DE MONOBEHAVIOUR ----
    #region Métodos de MonoBehaviour

    void Start()
    {
       gammaSlider = this.GetComponent<Slider>();
       postProcessingVolume = GameManager.Instance.GetComponent<Volume>();

        // Obtenemos el override desde el perfil
        if (postProcessingVolume.profile.TryGet<LiftGammaGain>(out liftGammaGain))
        {
            // Asegurarnos de que el override está activo
            liftGammaGain.active = true;

            // Inicializar el slider con el valor actual de Gamma
            float currentGamma = liftGammaGain.gamma.value.x;
            gammaSlider.minValue = -0.5f;
            gammaSlider.maxValue = 1.0f;
            gammaSlider.value = currentGamma;


            // Suscribirse al evento de cambio
            gammaSlider.onValueChanged.AddListener(OnGammaSliderChanged);
        }
        else
        {
            Debug.LogError("No se encontró LiftGammaGain en el Volume Profile.");
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

    #endregion

    // ---- MÉTODOS PRIVADOS ----
    #region Métodos Privados
    /// <summary>
    /// Manejador del evento OnValueChanged del slider.
    /// Actualiza el valor de gamma en el override LiftGammaGain.
    /// </summary>
    private void OnGammaSliderChanged(float newValue)
    {
        // Ajusta el valor del parámetro gamma en el override
        Vector4 gammaVec = new Vector4(newValue, newValue, newValue, newValue);
        liftGammaGain.gamma.value = gammaVec;
    }

 
    #endregion

} // class Gamma 
// namespace
