//---------------------------------------------------------
// Breve descripción del contenido del archivo: Es la funcion para que el oxigeno baje poco a poco
// Responsable de la creación de este archivo: Andrés Díaz Guerrero Soto (El sordo)
// Beyond the Depths
// Proyectos 1 - Curso 2024-25
//---------------------------------------------------------

using UnityEngine;
using UnityEngine.UI;

// Añadir aquí el resto de directivas using



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
    

    // Referencia al panel de texto
    private GameObject textPanel;

    #endregion

    // Referencia de imagen de la barrade oxigeno (lo usare un circulo)
    [SerializeField] private RectTransform OxygenCircle;

    // Posición más baja del líquido del oxígeno (cuando el oxígeno es 0)
    [SerializeField] private float minLiquidPosition;

    // Posición más alta del líquido del oxígeno (cuando el oxígeno es máximo)
    [SerializeField] private float maxLiquidPosition;

    // Referencia a la imagen del tanque de oxígeno
    [SerializeField] private Sprite oxygenTankSprite;

    // Referencia a la imagen del tanque de oxígeno roto
    [SerializeField] private Sprite brokenOxygenTankSprite;

    // Referencia a la imagen del tanque de oxígeno
    [SerializeField] private Image oxygenTank;


    // ---- ATRIBUTOS PRIVADOS ----
    #region Atributos Privados (private fields)


    #endregion

    // ---- MÉTODOS DE MONOBEHAVIOUR ----
    #region Métodos de MonoBehaviour



    /// <summary>
    /// Método llamado al iniciar el script. 
    /// Inicializa el medidor de oxígeno y registra la UI en el GameManager.
    /// </summary>
    void Start()
    {
        if (OxygenCircle != null)
        {
            // Configura el nivel inicial del oxígeno al máximo.
            OxygenCircle.anchoredPosition = new Vector2(OxygenCircle.anchoredPosition.x, maxLiquidPosition);
        }

        // Se pasa al GameMAnager para evitar asignaciones manuales

        if (GameManager.Instance != null)
        {
            GameManager.Instance.InitializeUI(this);
        }
    }

    /// <summary>
    /// Método llamado al despertar el script. Configura el patrón Singleton para esta clase.
    /// </summary>
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
        if (OxygenCircle != null)
        {
            // Interpolamos entre la posición mínima y máxima
            float newY = Mathf.Lerp(minLiquidPosition, maxLiquidPosition, OxigenPercentage);
            OxygenCircle.anchoredPosition = new Vector2(OxygenCircle.anchoredPosition.x, newY);
        }       
    }
    public void UpdateTankState(bool isTankBroken)
    {
        if (OxygenCircle != null)
        {
            if (isTankBroken)
            {
                oxygenTank.sprite = brokenOxygenTankSprite;
            }
            else
            {
                oxygenTank.sprite = oxygenTankSprite;
            }
        }
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
