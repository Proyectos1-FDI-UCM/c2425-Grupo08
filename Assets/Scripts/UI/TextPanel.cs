//---------------------------------------------------------
// Breve descripción del contenido del archivo
// Este archivo contiene la clase TextPanel, que representa un panel de texto en la interfaz de usuario.
// Carlos Dochao Moreno
// Proyect Abyss
// Proyectos 1 - Curso 2024-25
//---------------------------------------------------------

using TMPro;
using UnityEngine;
using System.Collections;

/// <summary>
/// Clase que representa un panel de texto en la interfaz de usuario.
/// Permite abrir y cerrar el panel, así como mostrar texto con un efecto de escritura.
/// </summary>
public class TextPanel : MonoBehaviour
{
    // ---- SINGLETON ----
    public static TextPanel Instance;

    // ---- ATRIBUTOS DEL INSPECTOR ----
    #region Atributos del Inspector

    [Header("Animation Settings")]
    [Space]
    [Range(2f, 15f)]
    [SerializeField] private float lerpSpeed = 6f; // Velocidad del Lerp

    [Range(0.01f, 1f)]
    [SerializeField] private float typingDelay = 0.01f; // Velocidad del efecto typewriter

    [Header("Text References")]
    [Space]
    [SerializeField] private TMP_Text titleChild; // Referencia al componente Title hijo
    [SerializeField] private TMP_Text textChild; // Referencia al componente Text hijo

    #endregion

    // ---- ATRIBUTOS PRIVADOS ----
    #region Atributos Privados

    private float _openPosition = 770f; // Posición abierta del panel
    private float _closedPosition = 1280f; // Posición cerrada del panel

    private Vector3 _targetPosition; // Posición objetivo del panel

    private bool visible = false; // Estado del panel (abierto o cerrado)

    private bool isTyping = false; // Estado del efecto typewriter
    
    private Coroutine typewriterCoroutine; // Referencia a la coroutine del efecto typewriter

    #endregion

    // ---- MÉTODOS DE MONOBEHAVIOUR ----
    #region Métodos de MonoBehaviour

    /// <summary>
    /// Se utiliza para implementar el patrón Singleton, asegurando que solo haya una instancia de esta clase en la escena.
    /// </summary>
    void Awake()
    {
        if (Instance != null && Instance != this)

            Destroy(this);

        else

            Instance = this;
    }

    /// <summary>
    /// Inicializa la posición del panel de texto en la posición cerrada.
    /// </summary>
    void Start()
    {
        // Establece la posición inicial del panel en la posición cerrada
        transform.position = _targetPosition = new Vector3(_closedPosition, transform.position.y, 0);
    }

    /// <summary>
    /// Se utiliza para actualizar la posición del panel de texto suavemente.
    /// </summary>
    void Update()
    {
        // Actualiza la posición del panel suavemente
        transform.position = Vector3.Lerp(transform.position, _targetPosition, lerpSpeed * Time.deltaTime);
    }

    #endregion

    // ---- MÉTODOS PÚBLICOS ----
    #region Métodos públicos

    /// <summary>
    /// Método para alternar el estado del panel de texto.
    /// Si el panel está visible, se cierra; si está cerrado, se abre.
    /// </summary>
    public void TogglePanel()
    {
        if (visible)

            ClosePanel();

        else

            OpenPanel();
    }

    /// <summary>
    /// Método para abrir el panel de texto.
    /// </summary>
    public void OpenPanel()
    {
        _targetPosition = new Vector3(_openPosition, transform.position.y, 0);

        visible = true;
    }

    /// <summary>
    /// Método para cerrar el panel de texto.
    /// </summary>
    public void ClosePanel()
    {
        _targetPosition = new Vector3(_closedPosition, transform.position.y, 0);

        visible = false;
    }

    /// <summary>
    /// Método para establecer el texto del panel.
    /// Se utiliza un efecto typewriter para mostrar el texto de forma gradual.
    /// </summary>
    /// <param name="title"></param>
    /// <param name="text"></param>
    public void SetText(string title, string text)
    {
        if (textChild != null && titleChild != null)
        {
            if (title != titleChild.text)
            {
                if (typewriterCoroutine != null)

                    StopCoroutine(typewriterCoroutine);

                titleChild.text = "";
                textChild.text = "";
                typewriterCoroutine = StartCoroutine(TypeWriterEffect(title, text));
            }
        }
        else

            Debug.LogError("No se han asignado los componentes de texto");
    }

    /// <summary>
    /// Método para comprobar si el panel está visible.
    /// </summary>
    /// <returns></returns>
    public bool IsTyping()
    {
        return isTyping;
    }

    /// <summary>
    /// Método para comprobar si el panel está visible.
    /// </summary>
    /// <returns></returns>
    public bool IsVisible()
    {
        return visible;
    }

    #endregion

    // ---- MÉTODOS PRIVADOS ----
    #region Métodos Privados

    /// <summary>
    /// Efecto typewriter para mostrar el texto de forma gradual.
    /// </summary>
    /// <param name="title"></param>
    /// <param name="text"></param>
    /// <returns></returns>
    private IEnumerator TypeWriterEffect(string title, string text)
    {
        isTyping = true;

        // Efecto typewriter para el título
        foreach (char c in title)
        {
            titleChild.text += c;
            yield return new WaitForSeconds(typingDelay);
        }

        yield return new WaitForSeconds(typingDelay * 2); // Pequeña pausa entre título y texto

        // Efecto typewriter para el texto
        foreach (char c in text)
        {
            textChild.text += c;
            yield return new WaitForSeconds(typingDelay);
        }

        isTyping = false;
    }

    #endregion

} // class TextPanel
// namespace
