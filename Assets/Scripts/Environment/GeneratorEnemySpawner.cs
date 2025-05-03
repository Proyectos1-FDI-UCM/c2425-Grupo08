//---------------------------------------------------------
// Este script se encarga de spawnear rapes alrededor del motor mientras el jugador lo esté reparando
// Javier Zazo Morillo
// Project Abyss
// Proyectos 1 - Curso 2024-25
//---------------------------------------------------------

using System.Collections;
using UnityEngine;
// Añadir aquí el resto de directivas using


/// <summary>
/// Este script se encarga de spawnear rapes alrededor del motor mientras el jugador lo esté reparando
/// </summary>
public class GeneratorEnemySpawner : MonoBehaviour
{
    // ---- ATRIBUTOS DEL INSPECTOR ----
    #region Atributos del Inspector (serialized fields)
    // Documentar cada atributo que aparece aquí.
    // El convenio de nombres de Unity recomienda que los atributos
    // públicos y de inspector se nombren en formato PascalCase
    // (palabras con primera letra mayúscula, incluida la primera letra)
    // Ejemplo: MaxHealthPoints

    [SerializeField] float maxRespawnRadious;
    [SerializeField] float minRespawnRadious;

    /// <summary>
    /// Altura mínima a la que puede aparecer el enemigo.
    /// Importante por el posible terreno desigual 
    /// o por decisiones de diseño.
    /// NO poner mayor que maxRespawnRadious ya que no tiene sentido
    /// </summary>
    [SerializeField] float minRespawnHeight;

    [SerializeField] float minRespawnCooldown;
    [SerializeField] float maxRespawnCooldown;

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

    private float currentRespawnCooldown;
    private bool canRespawn = false;

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
        currentRespawnCooldown = Random.Range(minRespawnCooldown, maxRespawnCooldown);
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        if (canRespawn)
        {
            currentRespawnCooldown -= Time.deltaTime;

            if (currentRespawnCooldown <= 0)
            {
                currentRespawnCooldown = Random.Range(minRespawnCooldown, maxRespawnCooldown);
                RespawnEnemy();
                Debug.Log("Respawned enemy");             
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

    /// <summary>
    /// Método que activa o desactiva la capacidad de spawnear enemigos
    /// </summary>
    /// <param name="canRespawn">true para activar false para desactivar</param>
    public void SetCanRespawn(bool canRespawn)
    {
        this.canRespawn = canRespawn;
    }

    #endregion

    // ---- MÉTODOS PRIVADOS ----
    #region Métodos Privados
    // Documentar cada método que aparece aquí
    // El convenio de nombres de Unity recomienda que estos métodos
    // se nombren en formato PascalCase (palabras con primera letra
    // mayúscula, incluida la primera letra)

    /// <summary>
    /// Método que se encarga de hacer aparecer al enemigo
    /// en una posición aleatoria dentro de un rango
    /// </summary>
    private void RespawnEnemy()
    {
        Vector2 randomPosition;

        float randomModule = Random.Range(minRespawnRadious, maxRespawnRadious);
        float randomY = Random.Range(minRespawnHeight, randomModule);
        float x = Mathf.Sqrt((randomModule * randomModule) - (randomY * randomY));
        if (Random.Range(0, 2) == 0)
        {
            randomPosition = new Vector3(x, randomY, 0) + transform.position;
        }
        else
        {
            randomPosition = new Vector3(-x, randomY, 0) + transform.position;
        }

        GameObject instantiatedEnemy = Instantiate(enemyPrefab, randomPosition, Quaternion.identity);
        instantiatedEnemy.GetComponentInChildren<Enemy1PhantomAnglerfish>().SetAttack(true);
        instantiatedEnemy.GetComponent<Respawner>().CanNotRespawn();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, minRespawnRadious);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, maxRespawnRadious);
    }

    #endregion   

} // class GeneratorEnemySpawner 
// namespace
