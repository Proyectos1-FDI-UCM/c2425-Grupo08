//---------------------------------------------------------
// Breve descripción del contenido del archivo: Es la funcion para que el oxigeno baje poco a poco
// Responsable de la creación de este archivo: Andrés Díaz Guerrero Soto (El sordo)
// Nombre del juego
// Proyectos 1 - Curso 2024-25
//---------------------------------------------------------

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VFX;
// Añadir aquí el resto de directivas using


/// <summary>
/// Antes de cada class, descripción de qué es y para qué sirve,
/// usando todas las líneas que sean necesarias.
/// </summary>

/// <summary>
/// Se encarga de gestionar la interfaz de usuario (HUD) Incluyendo el
/// medidor de oxigeno, que se muestre la pantalla
/// </summary>

public class UIManager : MonoBehaviour
{
    /// <summary>
    /// Instancia unica de UiManager
    /// </summary>
    
    public static UIManager Instance { get; private set; }


    // ---- ATRIBUTOS DEL INSPECTOR ----
    #region Atributos del Inspector (serialized fields)
    // Documentar cada atributo que aparece aquí.
    // El convenio de nombres de Unity recomienda que los atributos
    // públicos y de inspector se nombren en formato PascalCase
    // (palabras con primera letra mayúscula, incluida la primera letra)
    // Ejemplo: MaxHealthPoints

    // Referencia al panel de texto
    private GameObject textPanel;

    #endregion

    // Referencia de imagen de la barrade oxigeno (lo usare un circulo)
    [SerializeField] private RectTransform OxygenCircle;

    // Tamaño maximo cuando el oxigeno esta el 100%
    [SerializeField] private float MaxCircleSize = 100f;

    // Tamaño mimido cuando el oxigeno esta al 0%
    [SerializeField] private float MinCircleSize = 0f;


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
        if (OxygenCircle != null)
        {
            OxygenCircle.sizeDelta = new Vector2(MaxCircleSize, MaxCircleSize);
        }

        // Se pasa al GameMAnager para evitar asignaciones manuales

        if (GameManager.Instance != null)
        {
            GameManager.Instance.InitializeUI(this);
        }
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
           
        }
        else
        {
            Instance = this;
        }
    }


    #endregion

    // ---- MÉTODOS PÚBLICOS ----
    #region Métodos públicos

    public GameObject GetTextPanel()
    {
        textPanel = FindObjectOfType<TextPanel>()?.gameObject;

        return textPanel;
    }

    #endregion

    /// <summary>
    /// Atualiza el medidor de oxigeno haceindo que el circulo se vacie gradualmente
    /// </summary>
    /// <param name="oxygenPorcentaje">Porcentaje de oxigeno (entre 0 y 1)</param>

    public void UpdateOxygenUI(float OxigenPercentage)
    {
        if (OxygenCircle == null) return;
        
        // Interpolamos entre tamaño minimo y maximo
        float newSize = Mathf.Lerp(MinCircleSize, MaxCircleSize, OxigenPercentage);
        OxygenCircle.sizeDelta = new Vector2(newSize, newSize);
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
