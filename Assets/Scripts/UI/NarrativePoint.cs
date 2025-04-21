//---------------------------------------------------------
// Este archivo contiene la clase NarrativePoint, que representa un punto narrativo en la interfaz de usuario.
// Carlos Dochao Moreno
// Proyect Abyss
// Proyectos 1 - Curso 2024-25
//---------------------------------------------------------

using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Clase que representa un punto narrativo en la interfaz de usuario.
/// Permite mostrar un texto narrativo al hacer clic en el objeto.
/// </summary>
public class NarrativePoint : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler, IPointerClickHandler
{
    // ---- ATRIBUTOS DEL INSPECTOR ----
    #region Atributos del Inspector

    [Header("Scale Settings")]
    [Space]
    [Range(1f, 3f)]
    [SerializeField] private float scaleMultiplier = 1.2f; // Factor de aumento al hacer highlight

    [Header("Text Settings")]
    [Space]
    [SerializeField] private string narrativeTitle; // Texto narrativo del punto
    [Space]
    [TextArea(3, 10)]
    [SerializeField] private string narrativeText; // Texto narrativo del punto

    #endregion

    // ---- ATRIBUTOS PRIVADOS ----
    #region Atributos Privados

    private float lerpSpeed = 10f; // Velocidad del Lerp

    private Vector3 defaultScale = Vector3.one; // Escala original del objeto
    private Vector3 targetScale;

    private GameObject textPanel; // Referencia al componente "panel de texto"
    private GameObject dynamicTitle; // Referencia al componente "panel de texto"

    #endregion

    // ---- MÉTODOS DE MONOBEHAVIOUR ----
    #region Métodos de MonoBehaviour

    /// <summary>
    /// Se utiliza para obtener la referencia al componente TextPanel y establecer la escala objetivo.
    /// </summary>
    void Awake()
    {
        textPanel = UIManager.Instance.GetTextPanel(); // Obtiene la referencia al componente Textpanel

        if (textPanel == null)

            Debug.LogError("No se ha encontrado el objeto TextPanel en la escena.");

        dynamicTitle = FindObjectOfType<DynamicTitle>()?.gameObject; // Busca el objeto DynamicTitle en la escena

        if (dynamicTitle == null)

            Debug.LogError("No se ha encontrado el objeto DynamicTitle en la escena.");

        targetScale = defaultScale; // Inicializa la escala objetivo

    }

    /// <summary>
    /// Se utiliza para actualizar la escala del objeto suavemente en cada frame.
    /// </summary>
    void Update()
    {
        // Actualiza la escala suavemente
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, lerpSpeed * Time.deltaTime);
    }

    #endregion

    // ---- MÉTODOS PÚBLICOS ----
    #region Métodos públicos

    /// <summary>
    /// Método que se llama cuando el puntero entra en el objeto. Cambia la escala del objeto.
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerEnter(PointerEventData eventData)
    {
        targetScale = defaultScale * scaleMultiplier;

        dynamicTitle.GetComponent<DynamicTitle>().SetTitle(narrativeTitle); // Cambia el texto del panel de texto
        dynamicTitle.GetComponent<DynamicTitle>().FadeIn(); // Inicia la transición de fade in del texto
    }

    /// <summary>
    /// Método que se llama cuando el puntero sale del objeto. Cambia la escala del objeto.
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerExit(PointerEventData eventData)
    {
        targetScale = defaultScale;

        dynamicTitle.GetComponent<DynamicTitle>().FadeOut(); // Inicia la transición de fade out del texto
    }

    /// <summary>
    /// Método que se llama cuando el objeto es seleccionado. Cambia la escala del objeto.
    /// </summary>
    /// <param name="eventData"></param>
    public void OnSelect(BaseEventData eventData)
    {
        targetScale = defaultScale * scaleMultiplier;

        dynamicTitle.GetComponent<DynamicTitle>().SetTitle(narrativeTitle); // Cambia el texto del panel de texto
        dynamicTitle.GetComponent<DynamicTitle>().FadeIn(); // Inicia la transición de fade in del texto
    }

    /// <summary>
    /// Método que se llama cuando el objeto es deseleccionado. Cambia la escala del objeto.
    /// </summary>
    /// <param name="eventData"></param>
    public void OnDeselect(BaseEventData eventData)
    {
        targetScale = defaultScale;

        dynamicTitle.GetComponent<DynamicTitle>().FadeOut(); // Inicia la transición de fade out del texto
    }

    /// <summary>
    /// Método que se llama cuando el objeto es clicado. Cambia el texto del panel de texto.
    /// Si el panel de texto está visible, lo oculta; si no, lo muestra.
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerClick(PointerEventData eventData)
    {
        if (textPanel != null)
        {
            if (textPanel.GetComponent<TextPanel>().IsVisible())

                textPanel.GetComponent<TextPanel>().SetText(narrativeTitle, narrativeText);

            else
            {
                textPanel.GetComponent<TextPanel>().SetText(narrativeTitle, narrativeText);

                textPanel.GetComponent<TextPanel>().TogglePanel();
            }
        }
    }

    #endregion

    // ---- MÉTODOS PRIVADOS ----
    #region Métodos Privados
    #endregion

} // class NarrativePoint