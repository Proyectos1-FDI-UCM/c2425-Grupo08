//---------------------------------------------------------
// Breve descripción del contenido del archivo
// Responsable de la creación de este archivo. El original: Javier Zazo Morillo (nuevo modificacion): Andrés Díaz Guerrero Soto (El sordo)
// Beyond the Depths
// Proyectos 1 - Curso 2024-25
//---------------------------------------------------------

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Unity.Mathematics;


/// <summary>
/// Antes de cada class, descripción de qué es y para qué sirve,
/// usando todas las líneas que sean necesarias.
/// </summary>
public class OxigenScript : MonoBehaviour
{
    // ---- ATRIBUTOS DEL INSPECTOR ----
    #region Atributos del Inspector (serialized fields)
    // Documentar cada atributo que aparece aquí.
    // El convenio de nombres de Unity recomienda que los atributos
    // públicos y de inspector se nombren en formato PascalCase
    // (palabras con primera letra mayúscula, incluida la primera letra)
    // Ejemplo: MaxHealthPoints

    [Tooltip("La cantidad máxima de oxígeno que puede tener el jugador")]
    [SerializeField] private int maxOxigen;
    [Tooltip("La cantidad de oxígeno que se pierde por segundo al estar en estado \"sano\"")]
    [SerializeField] private float oxigenDecayHealthy;
    [Tooltip("La cantidad de oxígeno que se pierde por segundo al estar en estado \"roto\"")]
    [SerializeField] private float oxigenDecayBroken;

    [SerializeField] private GameObject Bubbles;
    [SerializeField] private GameObject BubbleSpot;

    //[SerializeField] private Text oxigenText; // El texto que muestra la cantidad de oxígeno que tiene el jugador

    #endregion

    // ---- ATRIBUTOS PRIVADOS ----
    #region Atributos Privados (private fields)
    // Documentar cada atributo que aparece aquí.
    // El convenio de nombres de Unity recomienda que los atributos
    // privados se nombren en formato _camelCase (comienza con _, 
    // primera palabra en minúsculas y el resto con la 
    // primera letra en mayúsculas)
    // Ejemplo: _maxHealthPoints

    private float currentOxigen; // La cantidad actual de oxígeno que tiene el jugador
    private bool tankBroken = false; // Indica si el tanque de oxígeno está roto o no
    private AudioSource audioSource;
    private AudioSource audioSourceOxygen;

    private bool isDead = false;
    private PlayerMovement player;
    private bool immortal = false; // Indica si el jugador es inmortal o no

    private Animator animator;
    private GameObject currentBubbles;

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
        animator = GetComponentInChildren<Animator>();
        player = GameManager.Instance.GetPlayerController().GetComponent<PlayerMovement>();
        currentOxigen = maxOxigen;
        audioSource= AudioManager.instance.GetComponent<AudioSource>();
        audioSourceOxygen = gameObject.AddComponent<AudioSource>();
        AudioManager.instance.PlaySFX(SFXType.Breath, audioSource, true);
        immortal = GameManager.Instance.GetImmortal();
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update() // Cada frame se resta oxígeno al jugador y en el caso de llegar a 0 el jugador muere
    {
        if(audioSource.isPlaying == false && !isDead) // Si el audio de la respiración no se está reproduciendo, lo reproduce
        {
            AudioManager.instance.PlaySFX(SFXType.Breath, audioSource, true);
        }

        if (tankBroken)
        {
            currentOxigen -= oxigenDecayBroken * Time.deltaTime;

        }
        else
        {
            currentOxigen -= oxigenDecayHealthy * Time.deltaTime;
        }


        if (currentOxigen <= 0)
        {
            currentOxigen = 0;
            Death();
        }
     

        // Nueva integracion: Se envia el porcentaje de oxigeno al GameManager
        OxigenPercentage();
    }
    #endregion

    // ---- MÉTODOS PÚBLICOS ----
    #region Métodos públicos
    // Documentar cada método que aparece aquí con ///<summary>
    // El convenio de nombres de Unity recomienda que estos métodos
    // se nombren en formato PascalCase (palabras con primera letra
    // mayúscula, incluida la primera letra)
    // Ejemplo: GetPlayerController

    /// <summary>
    /// Método que devuelve la cantidad de oxígeno actual del jugador
    /// </summary>
    /// <param name="value">Valor que restar</param>
    public void ReduceOxygen(float value)
    {
        currentOxigen -= value;
    }

    /// <summary>
    /// Método que se llama cuando el tanque de oxígeno recibe un impacto (si el tanque ya estaba roto, el jugador muere)
    /// </summary>
    public void PierceTank()
    {
        if (tankBroken)
        {
            Death();
        }
        else
        {
            tankBroken = true;
            GameManager.Instance.UpdateTankStateGM(tankBroken);
            currentBubbles = Instantiate(Bubbles,BubbleSpot.transform.position,quaternion.identity,this.gameObject.transform);
            StartCoroutine(PlayOxygenTankSounds()); // Inicia la corutina
        }      
    }
    /// <summary>
    /// Método que se llama cuando el tanque de oxígeno es reparado (no utilizado)
    /// </summary>
    public void RepairTank()
    {
        tankBroken = false;
        GameManager.Instance.UpdateTankStateGM(tankBroken);
    }
    /// <summary>
    /// Método que devuelve el estado del tanque de oxígeno
    /// </summary>
    /// <returns></returns>
    public bool IsTankBroken() 
    {
        return tankBroken;
    }
    public void Death()
    {
        if (!immortal)
        {
            if (isDead) return; // Evitar múltiples ejecuciones

            isDead = true;
            AudioManager.instance.PlaySFX(SFXType.GameOver, audioSource);
            audioSourceOxygen.Stop();
            Destroy(currentBubbles);
            animator.SetTrigger("Death");
            player.SetIsDeath(true);

            StartCoroutine(DestroyAfterDelay());
        }
    }
    #endregion

    // ---- MÉTODOS PRIVADOS ----
    #region Métodos Privados
    // Documentar cada método que aparece aquí
    // El convenio de nombres de Unity recomienda que estos métodos
    // se nombren en formato PascalCase (palabras con primera letra
    // mayúscula, incluida la primera letra)

    private IEnumerator DestroyAfterDelay()
    {
        yield return new WaitForSeconds(5); // Tiempo despúes de la muerte
        GameManager.Instance.ChangeScene(0);
    }

    private IEnumerator PlayOxygenTankSounds()
    {
        audioSourceOxygen.volume = 1f;
        AudioManager.instance.PlaySFX(SFXType.HitOxygenTank, audioSourceOxygen);
        yield return new WaitForSeconds(1f); // Espera antes de reproducir el sonido de burbujeo
        
        AudioManager.instance.PlaySFX(SFXType.OxygenBubbles, audioSourceOxygen, true);
    }

    #endregion

    #region Integracion con UI
    /// <summary>
    /// CAlcula el porcentaje actual de oxigeno (valor 0 y 1) y lo envia al GameManager
    /// </summary>

    private void OxigenPercentage()
    {
        float percentage = currentOxigen / maxOxigen;
        GameManager.Instance.UpdateOxygenGM(percentage);
    }

    #endregion
} // class OxigenScript 
// namespace
