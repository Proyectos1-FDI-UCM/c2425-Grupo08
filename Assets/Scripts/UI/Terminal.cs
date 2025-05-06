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

    [Header("Debug Settings")]
    [Space]
    [Tooltip("Mostrar el terminal inmediatamente al iniciar")]
    [SerializeField] private bool skipCollider = false; // Por defecto sin skippear
    [Tooltip("Hacer que el texto se muestre parpadeando")]
    [SerializeField] private bool textBlink = false; // Por defecto apagado
    [Tooltip("Muestra el texto con un fondo negro a su alrededor")]
    [SerializeField] private bool textBackground = true; // Por defecto con fondo

    [Header("Transition Settings")]
    [Space]
    [Tooltip("Velocidad de aparición/desparición")]
    [Range(0f, 10f)]
    [SerializeField] private float fadeSpeed = 2f;

    [Header("Writing Settings")]
    [Space]
    [Tooltip("Tiempo de retardo entre caracteres")]
    [Range(0.001f, 0.05f)]
    [SerializeField] private float timeDelay = 0.03f;
    [Tooltip("Tiempo de espera antes de empezar a escribir el mensaje")]
    [Range(0f, 2f)]
    [SerializeField] private float initPause = 0.5f;

    /*[Header("Cursor Settings")]
    [Space]
    [Tooltip("Carácter del cursor.")]
    [SerializeField] private string cursor = "▌";*/

    [Space]
    [Tooltip("Mensaje que se mostrará en la consola.")]
    [TextArea(3, 10)]
    [SerializeField] private string message;

    #endregion

    // ---- ATRIBUTOS PRIVADOS ----
    #region Atributos Privados

    // Declaración de textos, colores y audio
    private CanvasGroup canvas;
    private TMP_Text textTMP;
    private AudioSource audioSource;

    // Contadores de tiempo
    private float init = 0f; // Contador de espera para antes de empezar a escribir. (pausa inicial)
    private float time = 0f; // Contador para la escritura progresiva

    // Indices y colores
    private float alpha; // Almacena el valor de alpha para el efecto de fade in/out
    private int index = 0; // Índice del carácter actual que se está escribiendo

    // Variables de control
    private string currentStr; // Almacena el texto actual
    private bool stop = false; // Indica si se debe detener la escritura progresiva

    // Añade fondo a los terminales (con una fuente fallback)
    private string mark;

    // Boleana de control para saber si el mensaje ha terminado de escribirse
    private bool end = false;

    #endregion

    // ---- MÉTODOS DE MONOBEHAVIOUR ----
    #region Métodos de MonoBehaviour

    /// <summary>
    /// Inicializa el componente TMP_Text y AudioSource
    /// </summary>
    void Start()
    {
        // Inicializa el componente CanvasGroup
        canvas = GetComponent<CanvasGroup>();

        if (canvas == null)

            Debug.LogError("No se ha encontrado el componente CanvasGroup en el objeto.");

        else // Si se encuentra el componente CanvasGroup

            canvas.alpha = 0f; // Inicializa el texto a oculto

        // Inicializa el componente TMP_Text

        textTMP = GetComponentInChildren<TMP_Text>(); // Obtiene el componente TMP_Text del objeto actual

        if (textTMP == null)

            Debug.LogError("No se ha encontrado el componente AudioSource en el objeto ni en sus hijos.");

        // Inicializa el componente AudioSource

        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
        {
            // Si no se encuentra el componente AudioSource en el objeto actual, busca en los hijos
            audioSource = GetComponentInChildren<AudioSource>();

            if (audioSource == null)

                Debug.LogError("No se ha encontrado el componente AudioSource en el objeto ni en sus hijos.");
        }

        if (skipCollider) // Skipea directamente la activación con el collider

            Write(message);

        if (textBackground) // Si se quiere que el texto tenga un fondo

            mark = "<mark=#000000fa padding=“5, 5, 5, 5”>"; // Añade el fondo al texto

        else // Si no se quiere que el texto tenga un fondo

            mark = ""; // Elimina el fondo del texto
    }

    /// <summary>
    /// Lógica básica del terminal
    /// Se encarga de la interpolación del color del texto y la escritura progresiva.
    /// </summary>
    void Update()
    {
        LerpAlpha(); // Interpolación del color del texto

        SlowRender(); // Escritura progresiva del texto

        ReplaceKey();
    }

    #endregion

    // ---- MÉTODOS PÚBLICOS ----
    #region Métodos públicos

    /// <summary>
    /// Muestra el texto de consola (se aplica un fade in)
    /// </summary>
    public void Show()
    {
        alpha = 1f;
    }

    /// <summary>
    /// Oculta el texto de consola (se aplica un fade out)
    /// </summary>
    public void Hide()
    {
        alpha = 0f;
    }

    /// <summary>
    /// Escribe el mensaje en la consola de forma progresiva.
    /// Limpia el texto actual y lo reemplaza por el nuevo mensaje.
    /// </summary>
    public void Write(string input)
    {
        Clear();

        Show();

        message = input;
    }

    /// <summary>
    /// Establece el mensaje a mostrar en la consola DIRECTAMENTE.
    /// Se le añade el prefijo por defecto al mensaje.
    /// </summary>
    /// <param name="input">Mensaje a mostrar</param>
    public void SetMessage(string input)
    {
        message = input.Trim(); // Se eliminan los espacios en blanco al principio y al final del mensaje

        ReplaceKey(); // Reemplaza las teclas de interacción en el mensaje por los valores actuales

        textTMP.text = mark + message; // Se establece el mensaje directamente en el texto
    }

    /// <summary>
    /// Limpia el texto actual y reinicia los contadores de tiempo.
    /// Se llama al iniciar un nuevo mensaje o al limpiar el texto.
    /// </summary>
    public void Clear()
    {
        ResetText();
    }

    /// <summary>
    /// Reinicia el texto y los contadores de tiempo.
    /// (SIN TESTEAR)
    /// </summary>
    public void Continue()
    {
        stop = false;
    }

    /// <summary>
    /// Detiene la escritura progresiva del texto.
    /// (SIN TESTEAR)
    /// </summary>
    public void Stop()
    {
        stop = true;
    }

    /// <summary>
    /// Devuelve el mensaje actual.
    /// </summary>
    /// <returns>Mensaje actual</returns>
    public string GetMessage()
    {
        return message;
    }

    /// <summary>
    /// Devuelve el estado de visibilidad del texto.
    /// "Visible" si el texto es completamente visible (alpha = 1f)
    /// </summary>
    /// <returns>True si es visible</returns>
    public bool GetVisibility()
    {
        bool visible;

        if (canvas.alpha > 0.1f)

            visible = true; // Si el texto es completamente visible

        else

            visible = false; // Si el texto es completamente invisible
        
        return visible; // Devuelve el estado de visibilidad
    }

    /// <summary>
    /// Devuelve el estado de escritura del texto.
    /// </summary>
    /// <returns>True si ha terminado de escribirse</returns>
    public bool IsFinished()
    {       
        return end; // Devuelve el estado de la escritura
    }

    #endregion

    // ---- MÉTODOS PRIVADOS ----
    #region Métodos Privados

    /// <summary>
    /// Resetea el texto y los contadores de tiempo.
    /// Se llama al iniciar un nuevo mensaje o al limpiar el texto.
    /// </summary>
    private void ResetText()
    {
        index = 0;
        init = 0f;
        textTMP.text = "";
    }

    /// <summary>
    /// Interpolación del color del texto para el efecto de fade in/out
    /// </summary>
    private void LerpAlpha()
    {
        canvas.alpha = Mathf.MoveTowards(canvas.alpha, alpha, fadeSpeed * Time.deltaTime);
    }

    /// <summary>
    /// Escribe el texto de forma progresiva, caracter a caracter.
    /// Si el texto es más corto que el mensaje, se añade un nuevo carácter.
    /// </summary>
    private void SlowRender()
    {
        if (index == 0) // Si el índice es 0, se reinicia el texto actual

            currentStr = ""; // Reinicia el texto actual

        // Longitud total del mensaje (incluyendo el mark)
        int totalLength = mark.Length + message.Length;

        // Si el texto actual es menor que el mensaje y no se ha detenido
        if (message != "" && textTMP.text.Length < totalLength && index < totalLength && !stop) 
        {
            end = false; // Dejamos la boleana a falso

            if (init < initPause) // Pausa inicial

                init += Time.deltaTime;

            else // Escritura del texto
            {
                time += Time.deltaTime;

                if (time > timeDelay) // Si ha pasado el tiempo de espera entre caracteres
                {
                    char currentChar = message[index]; // Carácter actual a escribir

                    currentStr += currentChar; // Añade el carácter actual al texto

                    textTMP.text = mark + currentStr; // Actualiza el texto en la consola con el prefijo

                    index++; // Aumenta el índice para el siguiente carácter

                    time = 0f; // Reinicia el contador de tiempo para la escritura

                    if (currentChar == '.' || currentChar == ':')

                        init = 0f; // Reinicia el contador de tiempo para la pausa especial

                    if (GetVisibility()) // Si el texto es visible
                        
                        audioSource.PlayOneShot(audioSource.clip); // Reproduce el sonido de escritura                    
                }
            }
        }

        else // Si el texto ha terminado de escribirse:
        {
            end = true; // Devolvemos true en la boleana de control

            if (textBlink) // Si se quiere que parpadee el texto
                 
                alpha = Mathf.PingPong(Time.time * fadeSpeed, 1f); // Parpadeo del texto
        }
    }

    /// <summary>
    /// Reemplaza las teclas de interacción en el mensaje por los valores actuales.
    /// Se utiliza para mostrar las teclas de interacción en el mensaje.
    /// </summary>
    private void ReplaceKey()
    {
        if (message.Contains("{key_"))
        {
            message = message.Replace("{key_interact}", InputManager.Instance.GetInteractKey());
            message = message.Replace("{key_jump}", InputManager.Instance.GetJumpKey());
            message = message.Replace("{key_flash}", InputManager.Instance.GetFlashKey());
            message = message.Replace("{key_focus}", InputManager.Instance.GetFocusKey());
            message = message.Replace("{key_return}", InputManager.Instance.GetReturnKey());
        }
    }

    /// <summary>
    /// Detecta la colisión con el objeto especificado y muestra el mensaje.
    /// </summary>
    /// <param name="other">El objeto con el que se ha colisionado.</param>
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<PlayerMovement>() != null)

            Write(message);
    }

    /// <summary>
    /// Detecta la salida de la colisión con el objeto especificado y oculta el mensaje.
    /// </summary>
    /// <param name="other">El objeto con el que se ha colisionado.</param>
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponent<PlayerMovement>() != null)

            Hide();
    }

    #endregion

} // class Terminal