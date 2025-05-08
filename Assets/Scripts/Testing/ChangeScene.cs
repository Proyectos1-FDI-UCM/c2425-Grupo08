//---------------------------------------------------------
// ChangeScene es un componente de pruebas para cambiar entre
// escenas
// Guillermo Jiménez Díaz
//Modificado por: Tomás Arévalo Almagro
// TemplateP1
// Proyectos 1 - Curso 2024-25
//---------------------------------------------------------

using UnityEngine;

/// <summary>
/// Componente de pruebas que cambia a otra escena que se
/// configura desde el editor. Se usa principalmente para
/// comunicarse con el GameManager desde un botón y hacer el
/// cambio de escena
/// </summary>
public class ChangeScene : MonoBehaviour
{
    // ---- ATRIBUTOS DEL INSPECTOR ----
    #region Atributos del Inspector (serialized fields)
    
    /// <summary>
    /// Índice de la escena (en el build settings)
    /// que se cargará. 
    /// </summary>
    [SerializeField]
    private int nextScene;

    #endregion
    
    // ---- ATRIBUTOS PRIVADOS ----
    #region Atributos Privados (private fields)
    // Documentar cada atributo que aparece aquí.
    // El convenio de nombres de Unity recomienda que los atributos
    // privados se nombren en formato _camelCase (comienza con _, 
    // primera palabra en minúsculas y el resto con la 
    // primera letra en mayúsculas)
    // Ejemplo: _maxHealthPoints

    #endregion
    

    // ---- MÉTODOS PÚBLICOS ----
    #region Métodos públicos

    /// <summary>
    /// Cambia de escena haciendo uso del GameManager
    /// </summary>
    public void ChangeToNextScene()
    {
        GameManager.Instance.ChangeScene(nextScene);
    }
    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // Solo funciona dentro del editor
#else
        Application.Quit(); // Esto es lo que se usa en el build final
#endif
    }
    #endregion


} // class ChangeScene 
// namespace
