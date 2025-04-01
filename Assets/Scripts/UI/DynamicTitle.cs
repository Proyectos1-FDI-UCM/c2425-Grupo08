//---------------------------------------------------------
// Clase que maneja el título dinámico con efectos de fade
// Carlos Dochao Moreno
// Project Abyss
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

    private TMP_Text _titleText;

    private Color _targetAlpha = new Color(1f, 1f, 1f, 0f); // Color objetivo para el fade

    #endregion

    #region Métodos de MonoBehaviour

    void Start()
    {
        _titleText = GetComponent<TMP_Text>();

        if (_titleText == null)

            Debug.LogError("No se ha encontrado el componente TMP_Text en el objeto");

        else

            _titleText.color = new Color(_titleText.color.r, _titleText.color.g, _titleText.color.b, 0f);
    }

    void Update()
    {
        _titleText.color = Color.Lerp(_titleText.color, _targetAlpha, fadeSpeed * Time.deltaTime);
    }

    #endregion

    #region Métodos públicos

    /// <summary>
    /// Inicia la transición de fade in del texto
    /// </summary>
    public void FadeIn()
    {
        _targetAlpha.a = 1f;
    }

    /// <summary>
    /// Inicia la transición de fade out del texto
    /// </summary>
    public void FadeOut()
    {
        _targetAlpha.a = 0f;
    }

    public void SetTitle(string title)
    {
        if (_titleText != null)

            _titleText.text = title; // Asigna el texto al componente TMP_Text

        else

            Debug.LogError("No se han asignado los componentes de texto");
    }

    #endregion
} // class DynamicTitle
