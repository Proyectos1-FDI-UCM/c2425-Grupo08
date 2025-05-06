//---------------------------------------------------------
// Script que maneja la transición entre escenas y la activación/desactivación de objetos.
// Carlos Dochao Moreno
// Project Abbys
// Proyectos 1 - Curso 2024-25
//---------------------------------------------------------

using UnityEngine;

/// <summary>
/// Clase que gestiona la transición entre escenas y la activación/desactivación de objetos.
/// </summary>
public class SceneController : MonoBehaviour
{
    // ---- ATRIBUTOS DEL INSPECTOR ----
    #region Atributos del Inspector (serialized fields)

    [SerializeField]
    [Tooltip("Index de la escena a la que se va tras finalizar")]
    private int sceneIndex;

    [SerializeField]
    [Tooltip("Objetos para gestionar")]
    private GameObject[] objects;

    #endregion
    
    // ---- ATRIBUTOS PRIVADOS ----
    #region Atributos Privados (private fields)

    // Índice del objeto actualmente activo
    private int current = 0;

    #endregion
    
    // ---- MÉTODOS DE MONOBEHAVIOUR ----
    #region Métodos de MonoBehaviour
    
    /// <summary>
    /// Start is called on the frame when a script is enabled just before 
    /// any of the Update methods are called the first time.
    /// </summary>
    void Start()
    {
        // Asegurarse de que solo el primer objeto esté activo al inicio
        if (objects != null && objects.Length > 0)
        {
            for (int i = 0; i < objects.Length; i++)
            {
                if (objects[i] != null)

                    objects[i].SetActive(i == 0);
            }
        }
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        // Gestionar la activación/desactivación de objetos al presionar la tecla "E"
        if (InputManager.Instance.InteractWasRealeasedThisFrame())
        {
            if (current < objects.Length - 1)

                ShowNext();

            else

                GameManager.Instance.ChangeScene(sceneIndex);
        }
    }

    #endregion
    
    // ---- MÉTODOS PRIVADOS ----
    #region Métodos Privados

    /// <summary>
    /// Activa el siguiente objeto en el array y desactiva el actual.
    /// </summary>
    private void ShowNext()
    {
        if (objects != null || objects.Length != 0)
        {
            // Desactivar el objeto actual
            if (objects[current] != null)

                objects[current].SetActive(false);

            // Incrementar el índice y asegurarse de que no se salga del rango
            current++;

            // Activar el siguiente objeto
            if (objects[current] != null)

                objects[current].SetActive(true);
        }
    }

    #endregion   

} // class SceneController