//---------------------------------------------------------
// Breve descripción del contenido del archivo
// Responsable de la creación de este archivo
// Nombre del juego
// Proyectos 1 - Curso 2024-25
//---------------------------------------------------------

using UnityEngine;
// Añadir aquí el resto de directivas using


/// <summary>
/// Antes de cada class, descripción de qué es y para qué sirve,
/// usando todas las líneas que sean necesarias.
/// </summary>
public class PlayerScript : MonoBehaviour
{
    // ---- ATRIBUTOS DEL INSPECTOR ----
    [SerializeField] private float maxSpeed;
    [SerializeField] private float acceleration;
    [SerializeField] private float deceleration;

    // ---- ATRIBUTOS PRIVADOS ----
    private float joystickMaxSpeed;
    private Rigidbody2D rb;

    // ---- MÉTODOS DE MONOBEHAVIOUR ----
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    private void Update()
    {
        if (InputManager.Instance.MovementVector.x != 0)
        {
            Walk(InputManager.Instance.MovementVector.x);
        }
        else StopWalking();
        
    }
    // ---- MÉTODOS PÚBLICOS ----
    #region Métodos públicos
    // Documentar cada método que aparece aquí con ///<summary>
    // El convenio de nombres de Unity recomienda que estos métodos
    // se nombren en formato PascalCase (palabras con primera letra
    // mayúscula, incluida la primera letra)
    // Ejemplo: GetPlayerController

    #endregion

    // ---- MÉTODOS PRIVADOS ----
    private void Walk(float x)
    {
        Debug.Log("X: " + x);

        rb.drag = 0;

        rb.AddForce(new Vector2(x, 0) * 5, ForceMode2D.Force);
        if (rb.velocity.magnitude > 3)
        {
            rb.velocity = rb.velocity.normalized * 3;
        }
    }
    private void StopWalking()
    {
        rb.drag = 5;
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(gameObject.transform.position, gameObject.transform.position + new Vector3(rb.velocity.x, rb.velocity.y, 0));
    }

} // class PlayerScript 
// namespace
