//---------------------------------------------------------
// Reproduce sonidos de forma aleatoria en el entorno
// Carlos Dochao Moreno
// Project Abyss
// Proyectos 1 - Curso 2024-25
//---------------------------------------------------------

using UnityEngine;

/// <summary>
/// Reproduce sonidos seleccionados aleatoriamente en un intervalo de tiempo.
/// </summary>
public class RandomNoise : MonoBehaviour
{
    // ---- ATRIBUTOS DEL INSPECTOR ----
    #region Atributos del Inspector (serialized fields)

    [SerializeField]
    [Tooltip("Lista de clips de audio para reproducir aleatoriamente.")]
    private AudioClip[] audioClips;

    [SerializeField, Range(0f, 10f)]
    [Tooltip("Tiempo mínimo en mins entre reproducciones.")]
    private float minTime = 2f;

    [SerializeField, Range(0f, 10f)]
    [Tooltip("Tiempo máximo en mins entre reproducciones.")]
    private float maxTime = 5f;

    #endregion

    // ---- ATRIBUTOS PRIVADOS ----
    #region Atributos Privados (private fields)

    private AudioSource audioSource; // Componente AudioSource para reproducir sonidos

    private float timer; // Temporizador para controlar el intervalo de reproducción

    #endregion

    // ---- MÉTODOS MONOBEHAVIOUR ----
    #region Métodos MonoBehaviour

    /// <summary>
    /// Inicializa el componente AudioSource y programa la primera reproducción.
    /// </summary>
    void Start()
    {
        // Validar y ajustar los tiempos de reproducción
        maxTime = Mathf.Max(minTime, maxTime) * 60;
        minTime = Mathf.Min(minTime, maxTime) * 60;

        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)

            Debug.LogError("Falta el componente AudioSource.");

        timer = maxTime;
    }

    /// <summary>
    /// Actualiza el temporizador y reproduce un sonido si es necesario.
    /// </summary>
    void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            PlayAudioClip();

            timer = Random.Range(minTime, maxTime);
        }
    }

    #endregion

    // ---- MÉTODOS PRIVADOS ----
    #region Métodos Privados

    /// <summary>
    /// Reproduce un clip aleatorio de la lista.
    /// </summary>
    private void PlayAudioClip()
    {
        if (audioClips != null)
        {
            int index = Random.Range(0, audioClips.Length);

            if (audioClips[index] != null)

                audioSource.PlayOneShot(audioClips[index]);
            
            else

                Debug.LogWarning("Clip de audio nulo en la lista.");
        }
    }

    #endregion

} // class RandomNoise