//---------------------------------------------------------
// Breve descripción del contenido del archivo
// Responsable de la creación de este archivo
// Nombre del juego
// Proyectos 1 - Curso 2024-25
//---------------------------------------------------------
using System;
using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;
using UnityEngine.UI;
// Añadir aquí el resto de directivas using


/// <summary>
/// Antes de cada class, descripción de qué es y para qué sirve,
/// usando todas las líneas que sean necesarias.
/// </summary>
public class Motor : MonoBehaviour
{
    // ---- ATRIBUTOS DEL INSPECTOR ----
    #region Atributos del Inspector (serialized fields)
    // Documentar cada atributo que aparece aquí.
    // El convenio de nombres de Unity recomienda que los atributos
    // públicos y de inspector se nombren en formato PascalCase
    // (palabras con primera letra mayúscula, incluida la primera letra)
    // Ejemplo: MaxHealthPoints
    [SerializeField] private Canvas canva;
    [SerializeField] private Slider progressBar; // Barra de carga
    [SerializeField] private float loadTime = 4f;
    [SerializeField] private SpriteRenderer motorSprite;

    #endregion

    // ---- ATRIBUTOS PRIVADOS ----
    #region Atributos Privados (private fields)
    // Documentar cada atributo que aparece aquí.
    // El convenio de nombres de Unity recomienda que los atributos
    // privados se nombren en formato _camelCase (comienza con _, 
    // primera palabra en minúsculas y el resto con la 
    // primera letra en mayúsculas)
    // Ejemplo: _maxHealthPoints
    private bool hasEnter = false;
    private bool isRepaired = false;    
    private float currentLoadProgress = 0f; // Progreso guardado
    private Coroutine loadCoroutine;
    private bool noise = false;
    
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
        canva.gameObject.SetActive(false);
        progressBar.gameObject.SetActive(false);
        progressBar.value = currentLoadProgress;
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        if (hasEnter && !isRepaired)
        {
            if (InputManager.Instance.InteractIsPressed())
            {
                StartLoading();
            }
            if (InputManager.Instance.InteractWasRealeasedThisFrame())
            {
                StopLoading();
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
    #region Métodos privados
    // Documentar cada método que aparece aquí
    // El convenio de nombres de Unity recomienda que estos métodos
    // se nombren en formato PascalCase (palabras con primera letra
    // mayúscula, incluida la primera letra)
    private void StartLoading()
    {
        if (loadCoroutine == null) // Evitar que se inicie múltiples veces
        {
            progressBar.gameObject.SetActive(true);
            loadCoroutine = StartCoroutine(LoadProgress());
        }
    }

    private void StopLoading()
    {
        if (loadCoroutine != null)
        {
            StopCoroutine(loadCoroutine);
            loadCoroutine = null;
        }
        progressBar.gameObject.SetActive(false);
        noise = false;
    }

    private IEnumerator LoadProgress()
    {
        while (currentLoadProgress < 1f)
        {
            currentLoadProgress += Time.deltaTime / loadTime;
            progressBar.value = currentLoadProgress;
            noise = true;
            if (currentLoadProgress >= 1f)
            {
                CompleteRepair();
                yield break; // Salimos de la corrutina al completar la carga
            }
            yield return null;
        }
    }
    private void CompleteRepair()
    {
        isRepaired = true;
        progressBar.gameObject.SetActive(false);
        canva.gameObject.SetActive(false);
        noise = false;
        // Cambia el color del motor a verde
        if (motorSprite != null)
        {
            motorSprite.color = Color.green;
        }

        // Llama a LevelManager para registrar el motor reparado
        LevelManager.Instance.MotorRepaired();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isRepaired)
        {
            hasEnter = true;
            canva.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            hasEnter = false;
            canva.gameObject.SetActive(false);
            StopLoading();
        }
    }

    #endregion

} // class DoorScript 
// namespace
