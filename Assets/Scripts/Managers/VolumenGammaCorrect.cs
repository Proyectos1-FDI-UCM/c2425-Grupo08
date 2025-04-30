//---------------------------------------------------------
// Breve descripción del contenido del archivo: Controla el Gamma
// Responsable de la creación de este archivo: Andrés Díaz Guerrero Soto
// Nombre del juego
// Proyectos 1 - Curso 2024-25
//---------------------------------------------------------

using UnityEngine;
// Añadir aquí el resto de directivas using
using UnityEngine.UI;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.InputSystem;

/// <summary>
/// Antes de cada class, descripción de qué es y para qué sirve,
/// usando todas las líneas que sean necesarias.
/// </summary>
public class VolumenGammaCorrect : MonoBehaviour
{
    // ---- ATRIBUTOS DEL INSPECTOR ----
    #region Atributos del Inspector (serialized fields)
    // Documentar cada atributo que aparece aquí.
    // El convenio de nombres de Unity recomienda que los atributos
    // públicos y de inspector se nombren en formato PascalCase
    // (palabras con primera letra mayúscula, incluida la primera letra)
    // Ejemplo: MaxHealthPoints

    [Header("Referencias")]
    [SerializeField] private Light2D globalLight = null;
    [SerializeField] private Volume globalVolume = null;
    [Header("Sliders UI")]
    [SerializeField] private Slider intensitySlider = null;
    [SerializeField] private Slider exposureSlider = null;
    [SerializeField] private Slider contrastSlider = null;
    [Header("Buttons UI")]
    [SerializeField] private Button mainMenuButton = null;
    [SerializeField] private Button continueButton = null;


    #endregion

    // ---- ATRIBUTOS PRIVADOS ----
    #region Atributos Privados (private fields)
    // Documentar cada atributo que aparece aquí.
    // El convenio de nombres de Unity recomienda que los atributos
    // privados se nombren en formato _camelCase (comienza con _, 
    // primera palabra en minúsculas y el resto con la 
    // primera letra en mayúsculas)
    // Ejemplo: _maxHealthPoints

    private ColorAdjustments colorAdjust;

    // Claves para PlayerPrefs
    public static float CurrentIntensity = 0.1f;
    public static float CurrentExposure = 0f;
    public static float CurrentContrast = 0f;

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

    private void OnEnable()
    {
        InputSystem.onDeviceChange += OnGamepadChange;
    }
    private void OnDisable()
    {
        InputSystem.onDeviceChange -= OnGamepadChange;
    }

    void Start()
    {
        if (!ValidateReferences()) return;

        if (!globalVolume.profile.TryGet<ColorAdjustments>(out colorAdjust))
        {
            Debug.LogError("VolumenGammaCorrect: falta override ColorAdjustments en el Profile.");
            enabled = false;
            return;
        }

        // Configurar rangos de sliders
        intensitySlider.minValue = 0f;
        intensitySlider.maxValue = 0.2f;
        exposureSlider.minValue = -5f;
        exposureSlider.maxValue = 5f;
        contrastSlider.minValue = -100f;
        contrastSlider.maxValue = 100f;

        // Inicializar UI con valores estáticos actuales
        intensitySlider.value = CurrentIntensity;
        exposureSlider.value = CurrentExposure;
        contrastSlider.value = CurrentContrast;

        ApplyValues();

        // Suscribir callbacks
        intensitySlider.onValueChanged.AddListener(OnIntensityChanged);
        exposureSlider.onValueChanged.AddListener(OnExposureChanged);
        contrastSlider.onValueChanged.AddListener(OnContrastChanged);

        // Navegación explícita para gamepad
        SetupNavigation();

        // Seleccionar primer slider
        intensitySlider.Select();
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

    // ---- MÉTODOS PRIVADOS ----
    #region Métodos Privados
    // Documentar cada método que aparece aquí
    // El convenio de nombres de Unity recomienda que estos métodos
    // se nombren en formato PascalCase (palabras con primera letra
    // mayúscula, incluida la primera letra)

    private bool ValidateReferences()
    {
        if (globalLight == null || globalVolume == null || intensitySlider == null ||
            exposureSlider == null || contrastSlider == null || mainMenuButton == null || continueButton == null)
        {
            Debug.LogError("VolumenGammaCorrect: faltan referencias en el inspector.");
            enabled = false;
            return false;
        }
        return true;
    }

    private void ApplyValues()
    {
        globalLight.intensity = CurrentIntensity;
        colorAdjust.postExposure.value = CurrentExposure;
        colorAdjust.contrast.value = CurrentContrast;
    }

    private void OnGamepadChange(InputDevice device, InputDeviceChange change)
    {
        if (device is Gamepad && (change == InputDeviceChange.Added || change == InputDeviceChange.Removed))
        {
            SetupNavigation();
        }
    }

    /// <summary>
    /// Define navegación por gamepad entre sliders y botones.
    /// </summary>
    private void SetupNavigation()
    {
        var exp = Navigation.Mode.Explicit;

        var nav0 = intensitySlider.navigation; nav0.mode = exp;
        nav0.selectOnDown = exposureSlider; nav0.selectOnUp = continueButton;
        intensitySlider.navigation = nav0;

        var nav1 = exposureSlider.navigation; nav1.mode = exp;
        nav1.selectOnDown = contrastSlider; nav1.selectOnUp = intensitySlider;
        exposureSlider.navigation = nav1;

        var nav2 = contrastSlider.navigation; nav2.mode = exp;
        nav2.selectOnDown = mainMenuButton; nav2.selectOnUp = exposureSlider;
        contrastSlider.navigation = nav2;

        var nav3 = mainMenuButton.navigation; nav3.mode = exp;
        nav3.selectOnDown = continueButton; nav3.selectOnUp = contrastSlider;
        mainMenuButton.navigation = nav3;

        var nav4 = continueButton.navigation; nav4.mode = exp;
        nav4.selectOnDown = intensitySlider; nav4.selectOnUp = mainMenuButton;
        continueButton.navigation = nav4;
    }

    private void OnIntensityChanged(float v)
    {
        CurrentIntensity = v;
        globalLight.intensity = v;
    }
    private void OnExposureChanged(float v)
    {
        CurrentExposure = v;
        colorAdjust.postExposure.value = v;
    }
    private void OnContrastChanged(float v)
    {
        CurrentContrast = v;
        colorAdjust.contrast.value = v;
    }

    #endregion   

} // class VolumenGammaCorrect 
// namespace
