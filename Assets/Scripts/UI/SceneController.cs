//---------------------------------------------------------
// Clase que gestiona la transición entre escenas y la activación/desactivación de objetos.
// Tomás Arévalo Almagro
// Carlos Dochao Moreno
// Beyond the Depths
// Proyectos 1 - Curso 2024-25
//---------------------------------------------------------

using UnityEngine;
using UnityEngine.Rendering.Universal;

/// <summary>
/// Clase que gestiona la transición entre escenas y la activación/desactivación de objetos.
/// </summary>
public class SceneController : MonoBehaviour
{
    // ---- ATRIBUTOS DEL INSPECTOR ----
    #region Atributos del Inspector

    [SerializeField]
    [Tooltip("Index de la escena a la que se va tras finalizar")]
    private int sceneIndex;

    [SerializeField]
    [Tooltip("Luz para gestionar (la luz que va a hacer de sol)")]
    private Light2D topLight;

    [SerializeField, Range(0f, 1f)]
    [Tooltip("mínimo de intensidad a la que llega la luz")]
    private float minIntesity;

    [SerializeField]
    [Tooltip("Se muestra entre textos para indicar como continuar")]
    private GameObject continueObject;

    [SerializeField]
    [Tooltip("Objetos para gestionar")]
    private GameObject[] objects;

    #endregion

    // ---- ATRIBUTOS PRIVADOS ----
    #region Atributos Privados

    private int current = 0; // Index actual de los objetos
    
    #endregion

    // ---- MÉTODOS DE MONOBEHAVIOUR ----
    #region Métodos de MonoBehaviour

    /// <summary>
    /// Inicializa el controlador de escena.
    /// </summary>
    void Start()
    {
        if (objects != null && objects.Length > 0)
        {
            for (int i = 0; i < objects.Length; i++)

                if (objects[i] != null)

                    objects[i].SetActive(i == 0);
        }

        //Escribir del tirón el mensaje del Terminal para continuar
        continueObject.GetComponent<Terminal>().SetMessage(continueObject.GetComponent<Terminal>().GetMessage());
        continueObject.SetActive(false);
    }

    /// <summary>
    /// Actualiza la lógica del controlador de escena.
    /// </summary>
    void Update()
    {
        // Intentar obtener el Terminal del objeto activo
        Terminal terminal = objects[current]?.GetComponentInChildren<Terminal>();

        if (InputManager.Instance.InteractWasRealeasedThisFrame())
        {
            if (!terminal.IsFinished())
            {
                    // Mostrar mensaje completo de inmediato
                    terminal.SetMessage(terminal.GetMessage());

                    return; // No continuar hasta que termine
            }    

            // Si se ha completado el texto (o no hay Terminal), avanzar
            if (current < objects.Length - 1)

                ShowNext();

            else

                GameManager.Instance.ChangeScene(sceneIndex);
        }

        continueObject.SetActive(terminal.IsFinished()); // Mostrar/Ocultar el objeto de continuar

        if (topLight.intensity > minIntesity)

            topLight.intensity -= Time.deltaTime * 0.1f;
    }

    #endregion

    // ---- MÉTODOS PRIVADOS ----
    #region Métodos Privados

    /// <summary>
    /// Muestra el siguiente objeto en la lista.
    /// </summary>
    private void ShowNext()
    {

        if (objects != null && objects.Length != 0)
        {
            if (objects[current] != null)
                objects[current].SetActive(false);

            current++;

            if (objects[current] != null)
                objects[current].SetActive(true);
        }
    }

    #endregion
}
