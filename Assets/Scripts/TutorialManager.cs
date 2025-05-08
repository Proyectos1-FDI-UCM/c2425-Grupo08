//---------------------------------------------------------
// Básicament activa el sonar indicator y lo quita a los 3 segundos 
// Tomás Arévalo Almagro
// Beyond the Depths
// Proyectos 1 - Curso 2024-25
//---------------------------------------------------------

using System.Collections;
using UnityEngine;
// Añadir aquí el resto de directivas using


/// <summary>
/// Antes de cada class, descripción de qué es y para qué sirve,
/// usando todas las líneas que sean necesarias.
/// </summary>
public class TutorialManager : MonoBehaviour
{
    // ---- ATRIBUTOS DEL INSPECTOR ----
    #region Atributos del Inspector (serialized fields)
    [SerializeField] GameObject SonarIndicator1;


    #endregion

    // ---- ATRIBUTOS PRIVADOS ----
    #region Atributos Privados (private fields)
    private bool fadingIn = false;
    private bool fadingOut = false;
    private float fadeDuration = 2f;
    private float fadeTimer = 0f;

    #endregion

    // ---- MÉTODOS DE MONOBEHAVIOUR ----
    #region Métodos de MonoBehaviour

    // Por defecto están los típicos (Update y Start) pero:
    // - Hay que añadir todos los que sean necesarios
    // - Hay que borrar los que no se usen 

    /// <summary>
    /// Start is called on the frame when a script is enabled just before 
    /// any of the Update methods are called the first time.
    /// </summary>
    void Start()
    {
        SonarIndicator1.SetActive(false);  
    }
    /// <summary>
    /// Update controla frame a frame el efecto de aparecer y desvanecerse.
    /// </summary>
    void Update()
    {
        if (fadingIn)
        {
            fadeTimer += Time.deltaTime;
            float alpha = Mathf.Clamp01(fadeTimer / fadeDuration);
            SetAlpha(SonarIndicator1, alpha);

            if (fadeTimer >= fadeDuration)
            {
                fadingIn = false;
                fadeTimer = 0f;
                Invoke(nameof(StartFadeOut), 1f); // espera 1 segundo antes de desvanecer
            }
        }
        else if (fadingOut)
        {
            fadeTimer += Time.deltaTime;
            float alpha = Mathf.Clamp01(1f - (fadeTimer / fadeDuration));
            SetAlpha(SonarIndicator1, alpha);

            if (fadeTimer >= fadeDuration)
            {
                fadingOut = false;
                SonarIndicator1.SetActive(false);
            }
        }
    }

    #endregion

    // ---- MÉTODOS PÚBLICOS ----
    #region Métodos públicos
    // Documentar cada método que aparece aquí con ///<summary>
    // El convenio de nombres de Unity recomienda que estos métodos
    // se nombren en formato PascalCase (palabras con primera letra
    // mayúscula, incluida la primera letra)
    // Ejemplo: GetPlayerController

    #endregion

    // ---- MÉTODOS PRIVADOS ----
    #region Métodos Privados
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            StartSonarFade();
        }
    }
    private IEnumerator ShowSonarIndicator()
    {
        SonarIndicator1.SetActive(true);
        yield return new WaitForSeconds(3f);
        SonarIndicator1.SetActive(false);
    }
    /// <summary>
    /// Inicia el proceso de aparecer.
    /// </summary>
    private void StartSonarFade()
    {
        SonarIndicator1.SetActive(true);
        fadeTimer = 0f;
        fadingIn = true;
        fadingOut = false;
    }

    /// <summary>
    /// Inicia el proceso de desaparecer.
    /// </summary>
    private void StartFadeOut()
    {
        fadeTimer = 0f;
        fadingOut = true;
    }
    /// <summary>
    /// Aplica una opacidad determinada al objeto y sus hijos.
    /// </summary>
    private void SetAlpha(GameObject obj, float alpha)
    {
        SpriteRenderer[] renderers = obj.GetComponentsInChildren<SpriteRenderer>();
        foreach (var renderer in renderers)
        {
            if (renderer != null)
            {
                Color color = renderer.color;
                color.a = alpha;
                renderer.color = color;
            }
        }
    }

    #endregion   

} // class TutorialManager 
// namespace
