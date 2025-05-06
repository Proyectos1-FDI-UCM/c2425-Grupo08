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
    [Tooltip("Objetos para gestionar")]
    private GameObject[] objects;

    #endregion

    // ---- ATRIBUTOS PRIVADOS ----
    #region Atributos Privados

    private int current = 0;

    #endregion

    // ---- MÉTODOS DE MONOBEHAVIOUR ----
    #region Métodos de MonoBehaviour

    void Start()
    {
        if (objects != null && objects.Length > 0)
        {
            for (int i = 0; i < objects.Length; i++)
                if (objects[i] != null)
                    objects[i].SetActive(i == 0);
        }
    }

    void Update()
    {
        if (InputManager.Instance.InteractWasRealeasedThisFrame())
        {
            // Intentar obtener el Terminal del objeto activo
            Terminal terminal = objects[current]?.GetComponentInChildren<Terminal>();

            if (terminal != null)
            {
                if (!terminal.IsFinished())
                {
                    // Mostrar mensaje completo de inmediato
                    terminal.SetMessage(terminal.GetMessage());
                    return; // No continuar hasta que termine
                }
            }

            // Si se ha completado el texto (o no hay Terminal), avanzar
            if (current < objects.Length - 1)
                ShowNext();
            else
                GameManager.Instance.ChangeScene(sceneIndex);
        }

        if (topLight.intensity > minIntesity)
            topLight.intensity -= Time.deltaTime * 0.1f;
    }

    #endregion

    // ---- MÉTODOS PRIVADOS ----
    #region Métodos Privados

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
