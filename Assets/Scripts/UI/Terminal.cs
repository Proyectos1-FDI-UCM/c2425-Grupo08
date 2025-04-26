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
    [SerializeField] private float timeDelay = 0.03f;
    [Tooltip("Tiempo de espera antes de empezar a escribir el mensaje")]
    [Range(0f, 2f)]
    [SerializeField] private float initPause = 0.5f;

    /*[Header("Cursor Settings")]
    [Space]
    [Tooltip("Carácter del cursor.")]
    [SerializeField] private string cursor = "▌";*/

    [Header("Message Settings")]
    [Space]
    [Tooltip("Mensaje que se mostrará en la consola.")]
    [TextArea(3, 10)]
    [SerializeField] private string message;

    [Header("Optional Settings")]
    [Space]
    [Tooltip("Objeto alternativo con el que se comprueba la colisión (por defecto Player)")]
    [SerializeField] private GameObject collisionTarget;

    public System.Action OnMessageComplete; // revisar

    #endregion

    // ---- ATRIBUTOS PRIVADOS ----
    #region Atributos Privados

    // Declaración de textos, colores y audio
    private TMP_Text textTMP;
    private AudioSource audioSource;
    //private Color textColor; // Color del texto (para controlar su alfa)

    // Contadores de tiempo
    private float init = 0f; // Contador de espera para antes de empezar a escribir. (pausa inicial)
    private float time = 0f; // Contador para la escritura progresiva

    // Indices y colores
    private float alpha; // Almacena el valor de alpha para el efecto de fade in/out
    private int index = 0; // Índice del carácter actual que se está escribiendo

    // Variables de control
    private string currentStr; // Almacena el texto actual
    private bool stop = false; // Indica si se debe detener la escritura progresiva

    // Boleana Mensaje completo: revisar
    private bool messageComplete = false;

    private string mark = "<mark=#000000fa padding=“5, 5, 5, 5”>";

    #endregion

    // ---- MÉTODOS DE MONOBEHAVIOUR ----
    #region Métodos de MonoBehaviour

    /// <summary>
    /// Inicializa el componente TMP_Text y AudioSource
    /// </summary>
    void Awake()
    {
        // Inicializa el componente TMP_Text

        textTMP = GetComponentInChildren<TMP_Text>(); // Obtiene el componente TMP_Text del objeto actual

        if (textTMP == null)

            Debug.LogError("No se ha encontrado el componente AudioSource en el objeto ni en sus hijos.");

        else // Si se encuentra el componente TMP_Text

            textTMP.alpha = 0f; // Inicializa el texto a oculto

        // Inicializa el componente AudioSource

        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
        {
            // Si no se encuentra el componente AudioSource en el objeto actual, busca en los hijos
            audioSource = GetComponentInChildren<AudioSource>();

            if (audioSource == null)

                Debug.LogError("No se ha encontrado el componente AudioSource en el objeto ni en sus hijos.");
        }
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

        if (textTMP.alpha > 0.1f)

            visible = true; // Si el texto es completamente visible

        else

            visible = false; // Si el texto es completamente invisible
        
        return visible; // Devuelve el estado de visibilidad
    }

    public bool IsMessageComplete() => messageComplete;

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
        textTMP.alpha = Mathf.MoveTowards(textTMP.alpha, alpha, fadeSpeed * Time.deltaTime);
    }

    /// <summary>
    /// Escribe el texto de forma progresiva, caracter a caracter.
    /// Si el texto es más corto que el mensaje, se añade un nuevo carácter.
    /// </summary>
    private void SlowRender()
    {
        if (index == 0) // Si el índice es 0, se reinicia el texto actual

            currentStr = ""; // Reinicia el texto actual

        // Si el texto actual es menor que el mensaje y no se ha detenido
        if (textTMP.text.Length < (mark.Length + message.Length) && index < (mark.Length + message.Length) && !stop) 
        {
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

        if (index >= message.Length && !messageComplete) // Revisar
        {
            messageComplete = true;
            OnMessageComplete?.Invoke();
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
        // Si collisionTarget está asignado, solo reacciona a ese objeto
        if (collisionTarget != null)
        {
            if (other.gameObject == collisionTarget)

                Write(message);
        }

        // Si no se comprueba la del jugador por defecto
        else if (other.GetComponent<PlayerMovement>() != null)

            Write(message);
    }

    /// <summary>
    /// Detecta la salida de la colisión con el objeto especificado y oculta el mensaje.
    /// </summary>
    /// <param name="other">El objeto con el que se ha colisionado.</param>
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