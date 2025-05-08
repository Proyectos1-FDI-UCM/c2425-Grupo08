//---------------------------------------------------------
// Clase que maneja el título dinámico con efectos de fade
// Carlos Dochao Moreno
// Beyond the Depths
// Proyectos 1 - Curso 2024-25
//---------------------------------------------------------

using UnityEngine;
using TMPro;

/// <summary>
/// Clase que gestiona un título dinámico con efectos de fade in/out
/// usando TextMeshPro y interpolación lineal (Lerp).
/// </summary>
public class DynamicTitle : MonoBehaviour
{
    #region Atributos del Inspector

    [Range (0f, 10f)][SerializeField] private float fadeSpeed = 2f;

    #endregion

    #region Atributos Privados

    private TMP_Text titleText;

    private Color targetAlpha = new Color(1f, 1f, 1f, 0f); // Color objetivo para el fade

    #endregion

    // ---- MÉTODOS DE MONOBEHAVIOUR ----
    #region Métodos de MonoBehaviour

    void Awake()
    {
        titleText = GetComponent<TMP_Text>();

        if (titleText == null)

            Debug.LogError("No se ha encontrado el componente TMP_Text en el objeto");

        else

            titleText.color = new Color(titleText.color.r, titleText.color.g, titleText.color.b, 0f);
    }

    void Update()
    {
        Color textColor = titleText.color;

        textColor.a = Mathf.MoveTowards(textColor.a, targetAlpha.a, fadeSpeed * Time.deltaTime);
        
        titleText.color = textColor;
    }

    #endregion

    // ---- MÉTODOS PÚBLICOS ----
    #region Métodos públicos

    /// <summary>
    /// Inicia la transición de fade in del texto
    /// </summary>
    public void FadeIn()
    {
        targetAlpha.a = 1f;
    }

    /// <summary>
    /// Inicia la transición de fade out del texto
    /// </summary>
    public void FadeOut()
    {
        targetAlpha.a = 0f;
    }

    /// <summary>
    /// Cambia el texto del título dinámico
    /// </summary>
    /// <param name="title">Nuevo texto del título</param>
    public void SetTitle(string title)
    {
        if (titleText != null)

            titleText.text = title; // Asigna el texto al componente TMP_Text

        else

            Debug.LogError("No se han asignado los componentes de texto");
    }

    #endregion
    
} // class DynamicTitle