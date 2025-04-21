//---------------------------------------------------------
// Clase Terminal.cs
// Carlos Dochao Moreno
// Project Abyss
// Proyectos 1 - Curso 2024-25
//---------------------------------------------------------

using UnityEngine;
using TMPro;

/// <summary>
/// Clase que gestiona un terminal de consola con efectos de fade in/out y escritura progresiva.
/// </summary>
public class Terminal : MonoBehaviour
{
    // ---- ATRIBUTOS DEL INSPECTOR ----
    #region Atributos del Inspector

    [Header("Transition Settings")]
    [Space]
    [Tooltip("Velocidad de aparición/desparición")]
    [Range(0f, 10f)]
    [SerializeField] private float fadeSpeed = 2f;

    [Header("Writing Settings")]
    [Space]
    [Tooltip("Tiempo de retardo entre caracteres")]
    [Range(0.001f, 0.05f)]
    [SerializeField] private float typeDelay = 0.03f;
    [Tooltip("Tiempo de espera antes de empezar a escribir el mensaje")]
    [Range(0f, 2f)]
    [SerializeField] private float typePause = 0.5f;

    /*[Header("Cursor Settings")]
    [Space]
    [Tooltip("Carácter del cursor.")]
    [SerializeField] private string cursor = "▌";*/

    [Header("Message Settings")]
    [Space]
    [Tooltip("Prefijo que se mostrará antes del mensaje.")]
    [SerializeField] private string prefix;
    [Tooltip("Mensaje que se mostrará en la consola.")]
    [TextArea(3, 10)]
    [SerializeField] private string message;

    [Header("Optional Settings")]
    [Space]
    [Tooltip("Objeto alternativo con el que se comprueba la colisión (por defecto Player)")]
    [SerializeField] private GameObject collisionTarget;

    #endregion

    // ---- ATRIBUTOS PRIVADOS ----
    #region Atributos Privados

    private TMP_Text textTMP;
    private Color target = new Color(1f, 1f, 1f, 0f);

    private float writeTimer = 0f;
    private int writeIndex = 0;
    private string currentMessage = "";
    private bool isWriting = false;
    private string currentText = "";
    private float initTimer = 0f;
    private bool charPause = false;
    private float charPauseTimer = 0f;

    #endregion

    // ---- MÉTODOS DE MONOBEHAVIOUR ----
    #region Métodos de MonoBehaviour

    /// <summary>
    /// Inicializa el componente TMP_Text
    /// </summary>
    private void Awake()
    {
        // Inicializa el componente TMP_Text
        textTMP = GetComponent<TMP_Text>();

        if (textTMP == null)
        {
            // Si no se encuentra el componente TMP_Text en el objeto actual, busca en los hijos
            textTMP = GetComponentInChildren<TMP_Text>();

            if (textTMP == null)

                Debug.LogError("No se ha encontrado el componente TMP_Text en el objeto ni en sus hijos.");
        }

        else // Si se encuentra el componente TMP_Text

            textTMP.color = target; // Inicializa el color del texto a oculto
    }

    /// <summary>
    /// Lógica básica del terminal
    /// Se encarga de la interpolación del color del texto y la escritura progresiva.
    /// </summary>
    private void Update()
    {
        LerpAlpha(); // Interpolación del color del texto

        TextRender(); // Escritura progresiva del texto
    }

    #endregion

    // ---- MÉTODOS PÚBLICOS ----
    #region Métodos públicos

    /// <summary>
    /// Inicia la transición de fade in del texto de consola
    /// </summary>
    public void Show()
    {
        target.a = 1f;
    }

    /// <summary>
    /// Inicia la transición de fade out del texto de consola
    /// </summary>
    public void Hide()
    {
        target.a = 0f;
    }

    public void Write(string message)
    {
        Show();
        currentMessage = message;
        writeIndex = 0;
        writeTimer = 0f;
        isWriting = true;
        currentText = "";
        initTimer = 0f;
    }

    public void Clear()
    {
        writeIndex = 0;
        isWriting = false;
        textTMP.text = "";
    }

    public string GetMessage()
    {
        return message;
    }

    public void SetMessage(string newMessage)
    {
        message = newMessage;
    }

    public void SetPrefix(string newPrefix)
    {
        prefix = newPrefix;
    }

    #endregion

    // ---- MÉTODOS PRIVADOS ----
    #region Métodos Privados

    /// <summary>
    /// Interpolación del color del texto para el efecto de fade in/out
    /// </summary>
    private void LerpAlpha()
    {
        Color current = textTMP.color;
        current.a = Mathf.MoveTowards(current.a, target.a, fadeSpeed * Time.deltaTime);
        textTMP.color = current;
    }

    private void TextRender()
    {
        bool debeActualizarTexto = true;

        if (isWriting)
        {
            // Pausa inicial antes de escribir
            if (initTimer < typePause)
            {
                initTimer += Time.deltaTime;
                textTMP.text = "";
                debeActualizarTexto = false;
            }
            else if (charPause)
            {
                charPauseTimer += Time.deltaTime;
                if (charPauseTimer < typePause)
                {
                    textTMP.text = currentText;
                    debeActualizarTexto = false;
                }
                else
                {
                    charPause = false;
                    charPauseTimer = 0f;
                    // Avanzar el índice para escribir el carácter tras la pausa
                    writeIndex++;
                }
            }
            
            if (debeActualizarTexto)
            {
                // Escritura progresiva del mensaje
                writeTimer += Time.deltaTime;
                bool pausaDetectada = false;
                while (writeTimer >= typeDelay && writeIndex < currentMessage.Length && !pausaDetectada)
                {
                    // Si el siguiente carácter es '.' o ':' y no estamos ya en pausa especial
                    char nextChar = currentMessage[writeIndex];
                    if ((nextChar == '.' || nextChar == ':') && !charPause)
                    {
                        charPause = true;
                        charPauseTimer = 0f;
                        pausaDetectada = true;
                    }
                    else
                    {
                        writeTimer -= typeDelay;
                        writeIndex++;
                    }
                }
                currentText = currentMessage.Substring(0, writeIndex);
                if (writeIndex >= currentMessage.Length)
                    isWriting = false;
            }
        }

        if (!isWriting && currentText == "")
        {
            // Al terminar de escribir, aseguramos que currentText solo contenga el texto
            currentText = currentMessage;
        }

        textTMP.text = currentText;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Si collisionTarget está asignado, solo reacciona a ese objeto
        if (collisionTarget != null)
        {
            if (other.gameObject == collisionTarget)

                Write(prefix + message);
        }

        // Si no se comprueba la del jugador por defecto
        else if (other.GetComponent<PlayerMovement>() != null)

            Write(prefix + message);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (collisionTarget != null)
        {
            if (other.gameObject == collisionTarget)

                Hide();
        }
        else if (other.GetComponent<PlayerMovement>() != null)

            Hide();
    }

    #endregion

} // class Terminal