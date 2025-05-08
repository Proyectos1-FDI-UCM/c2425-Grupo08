//---------------------------------------------------------
// Este script se encarga de respawnear a los enemigos rape que no sean del generador después de morir
// Javier Zazo Morillo
// Beyond the Depths
// Proyectos 1 - Curso 2024-25
//---------------------------------------------------------

using System.Collections;
using UnityEngine;
// Añadir aquí el resto de directivas using


/// <summary>
/// Este script se encarga de respawnear a los enemigos rape que no sean del generador después de morir
/// </summary>
public class Respawner : MonoBehaviour
{
    // ---- ATRIBUTOS DEL INSPECTOR ----
    #region Atributos del Inspector (serialized fields)
    // Documentar cada atributo que aparece aquí.
    // El convenio de nombres de Unity recomienda que los atributos
    // públicos y de inspector se nombren en formato PascalCase
    // (palabras con primera letra mayúscula, incluida la primera letra)
    // Ejemplo: MaxHealthPoints

    [Tooltip("Tiempo mínimo que tiene que pasar para que spawnee el enemigo (con Random.Range)")]
    [SerializeField] float minRespawnTime;
    [Tooltip("Tiempo máximo que tiene que pasar para que spawnee el enemigo (sin contar si el jugador está demasiado cerca o no)")]
    [SerializeField] float maxRespawnTime;

    [SerializeField] float respawnDistance;

    [SerializeField] GameObject enemyPrefab;
    
    #endregion

    // ---- ATRIBUTOS PRIVADOS ----
    #region Atributos Privados (private fields)
    // Documentar cada atributo que aparece aquí.
    // El convenio de nombres de Unity recomienda que los atributos
    // privados se nombren en formato _camelCase (comienza con _, 
    // primera palabra en minúsculas y el resto con la 
    // primera letra en mayúsculas)
    // Ejemplo: _maxHealthPoints

    private bool canRespawn = false;
    private float respawnTime;
    private bool isDead = false;
    private GameObject player;

    #endregion

    // ---- MÉTODOS DE MONOBEHAVIOUR ----
    #region Métodos de MonoBehaviour

    #endregion

    // ---- MÉTODOS PÚBLICOS ----
    #region Métodos públicos
    // Documentar cada método que aparece aquí con ///<summary>
    // El convenio de nombres de Unity recomienda que estos métodos
    // se nombren en formato PascalCase (palabras con primera letra
    // mayúscula, incluida la primera letra)
    // Ejemplo: GetPlayerController

    /// <summary>
    /// Método que se llama cuando un enemigo muere para que se active el respawn
    /// </summary>
    public void EnemyDead(GameObject player)
    {
        if (!canRespawn)
        {
            Destroy(gameObject);
        }
        else if (!isDead)
        {
            respawnTime = Random.Range(minRespawnTime, maxRespawnTime);
            isDead = true;
            this.player = player;
        }
    }

    public void CanNotRespawn()
    {
        canRespawn = false;
    }

    #endregion

    // ---- MÉTODOS PRIVADOS ----
    #region Métodos Privados
    // Documentar cada método que aparece aquí
    // El convenio de nombres de Unity recomienda que estos métodos
    // se nombren en formato PascalCase (palabras con primera letra
    // mayúscula, incluida la primera letra)

    /// <summary>
    /// Spawnea un enemigo rape cuando el jugador esté lo suficientemente lejos y pase el tiempo de respawn
    /// </summary>
    /// <returns></returns>
    IEnumerator RespawnCooldown()
    {
        yield return new WaitForSeconds(respawnTime);

        while (player != null && isDead)
        {
            if ((player.transform.position - transform.position).magnitude > respawnDistance)
            {
                Instantiate(enemyPrefab, transform);
                isDead = false;
            }
        }
    }

    /// <summary>
    /// Dibuja en el editor el rango en el que el enemigo no puede respawnear si el jugador está dentro
    /// </summary>
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, respawnDistance);
    }

    #endregion   

} // class Respawner 
// namespace
