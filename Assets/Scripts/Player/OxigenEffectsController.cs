//---------------------------------------------------------
// Este script se encarga de aplicar efectos de post-procesado según el nivel de oxígeno del jugador
// Javier Zazo Morillo
// Project Abyss
// Proyectos 1 - Curso 2024-25
//---------------------------------------------------------

using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
// Añadir aquí el resto de directivas using


/// <summary>
/// Este script se encarga de aplicar efectos de post-procesado según el nivel de oxígeno del jugador
/// </summary>
public class OxigenEffectsController : MonoBehaviour
{
    // ---- ATRIBUTOS DEL INSPECTOR ----
    #region Atributos del Inspector (serialized fields)
    // Documentar cada atributo que aparece aquí.
    // El convenio de nombres de Unity recomienda que los atributos
    // públicos y de inspector se nombren en formato PascalCase
    // (palabras con primera letra mayúscula, incluida la primera letra)
    // Ejemplo: MaxHealthPoints

    [Tooltip("El tiempo que tardan los efectos en ir de un extremo al otro de su rango")]
    [SerializeField] private float oscilationSpeed;

    [Header("Post Exposure")]

    [SerializeField] private float minPostExposureValue;
    [SerializeField] private float maxPostExposureValue;
    [SerializeField] private float postExposureOscilationRange;

    [Header("Chromatic Aberration")]

    [SerializeField] private float minChromaticAberrationValue;
    [SerializeField] private float maxChromaticAberrationValue;
    [SerializeField] private float chromaticAberrationRange;

    #endregion

    // ---- ATRIBUTOS PRIVADOS ----
    #region Atributos Privados (private fields)
    // Documentar cada atributo que aparece aquí.
    // El convenio de nombres de Unity recomienda que los atributos
    // privados se nombren en formato _camelCase (comienza con _, 
    // primera palabra en minúsculas y el resto con la 
    // primera letra en mayúsculas)
    // Ejemplo: _maxHealthPoints

    private ColorAdjustments colorAdjustments;
    private ChromaticAberration chromaticAberration;

    /// <summary>
    /// El valor actual de la oscilación
    /// </summary>
    private float oscilationValue;
    /// <summary>
    /// El porcentaje con el que se aplican los efectos
    /// </summary>
    private float effectPercentage = 0;

    private float minPostExposure;
    private float maxPostExposure;

    private float minChromaticAberration;
    private float maxChromaticAberration;

    /// <summary>
    /// Si la oscilación crece o decrece
    /// </summary>
    private bool oscilationDown = true;

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
        GameManager.Instance.InitializeOxigenEffects(this);

        GetComponent<Volume>().profile.TryGet<ColorAdjustments>(out colorAdjustments);
        GetComponent<Volume>().profile.TryGet<ChromaticAberration>(out chromaticAberration);

        oscilationValue = oscilationSpeed;
    }

    private void Update()
    {
        if (effectPercentage > 0)
        {
            if (oscilationDown)
            {
                oscilationValue -= Time.deltaTime;
                colorAdjustments.postExposure.value = Mathf.Lerp(minPostExposure, maxPostExposure, Mathf.InverseLerp(0, oscilationSpeed, oscilationValue));
                chromaticAberration.intensity.value = Mathf.Lerp(maxChromaticAberration, minChromaticAberration, Mathf.InverseLerp(0, oscilationSpeed, oscilationValue));

                if (oscilationValue <= 0)
                {
                    oscilationDown = false;
                }
            }
            else
            {
                oscilationValue += Time.deltaTime;
                colorAdjustments.postExposure.value = Mathf.Lerp(minPostExposure, maxPostExposure, Mathf.InverseLerp(0, oscilationSpeed, oscilationValue));
                chromaticAberration.intensity.value = Mathf.Lerp(maxChromaticAberration, minChromaticAberration, Mathf.InverseLerp(0, oscilationSpeed, oscilationValue));

                if (oscilationValue >= oscilationSpeed)
                {
                    oscilationDown = true;
                }
            }
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

    public void AdjustOxigenEffects(float oxygenPercentage)
    {
        if (oxygenPercentage < 0.5f)
        {
            effectPercentage = -2 * oxygenPercentage + 1; // 50% oxígeno = 0% efecto, 25% oxígeno = 50% efecto, 0% oxígeno = 100% efecto

            minPostExposure = Mathf.Lerp(maxPostExposureValue - postExposureOscilationRange, minPostExposureValue, effectPercentage);
            maxPostExposure = Mathf.Lerp(maxPostExposureValue, minPostExposureValue + postExposureOscilationRange, effectPercentage);

            minChromaticAberration = Mathf.Lerp(maxChromaticAberrationValue - chromaticAberrationRange, minChromaticAberrationValue, effectPercentage);
            maxChromaticAberration = Mathf.Lerp(maxChromaticAberrationValue, minChromaticAberrationValue + chromaticAberrationRange, effectPercentage);
        }      
    }

    #endregion

    // ---- MÉTODOS PRIVADOS ----
    #region Métodos Privados
    // Documentar cada método que aparece aquí
    // El convenio de nombres de Unity recomienda que estos métodos
    // se nombren en formato PascalCase (palabras con primera letra
    // mayúscula, incluida la primera letra)

    #endregion

} // class OxigenEffectsController 
// namespace
