//---------------------------------------------------------
// Breve descripción del contenido del archivo
// Responsable de la creación de este archivo
// Nombre del juego
// Proyectos 1 - Curso 2024-25
//---------------------------------------------------------

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[ExecuteAlways]
public class GammaManager : MonoBehaviour
{
    [Header("REFERENCIAS (¡Manuales!)")]
    [Tooltip("Arrastra aquí el Volume global con LiftGammaGain")]
    public Volume globalVolume;

    [Tooltip("Arrastra aquí el Slider (rango -1…1)")]
    public Slider gammaSlider;

    // --- Static para mantener valor durante toda la sesión ---
    private static float savedSliderValue = 0f;

    private LiftGammaGain liftGamma;

    void OnValidate()
    {
        if (gammaSlider != null)
        {
            gammaSlider.minValue = -1f;
            gammaSlider.maxValue = 1f;
        }
        if (globalVolume == null)
            Debug.LogWarning($"[{nameof(GammaManager)}] Falta asignar globalVolume en {name}", this);
        if (gammaSlider == null)
            Debug.LogWarning($"[{nameof(GammaManager)}] Falta asignar gammaSlider en {name}", this);
    }

    void Awake()
    {
        // Obtener override LiftGammaGain
        if (!globalVolume.profile.TryGet(out liftGamma))
            Debug.LogError($"[{nameof(GammaManager)}] No se encontró LiftGammaGain en {globalVolume.name}", this);

        if (gammaSlider != null)
        {
            // Rango y listener
            gammaSlider.minValue = -1f;
            gammaSlider.maxValue = 1f;
            gammaSlider.onValueChanged.RemoveAllListeners();
            gammaSlider.onValueChanged.AddListener(OnSliderChanged);

            // Restaurar valor de sesión
            gammaSlider.value = savedSliderValue;
        }
    }

    void Start()
    {
        // Aplicar el valor actual al inicializar
        if (gammaSlider != null)
            OnSliderChanged(gammaSlider.value);
    }

    private void OnSliderChanged(float v)
    {
        // Guardar para próximas escenas
        savedSliderValue = v;

        // Mapear v∈[-1,1] → gamma∈[0,2]
        float gammaValue = v + 1f;
        Debug.Log($"[GammaManager] Slider: {v:F2} → Gamma: {gammaValue:F2}", this);

        if (liftGamma != null)
        {
            liftGamma.active = true;
            liftGamma.gamma.overrideState = true;
            var g = new Vector4(gammaValue, gammaValue, gammaValue, gammaValue);
            liftGamma.gamma.value = g;
        }
    }
}
