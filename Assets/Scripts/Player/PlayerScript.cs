//---------------------------------------------------------
// Breve descripción del contenido del archivo
// Responsable de la creación de este archivo
// Nombre del juego
// Proyectos 1 - Curso 2024-25
//---------------------------------------------------------

using UnityEngine;
using System;
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
            joystickMaxSpeed = maxSpeed * InputManager.Instance.MovementVector.x;
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

        rb.AddForce(new Vector2(x, 0) * acceleration, ForceMode2D.Force);
        if (Math.Abs(rb.velocity.x) > Math.Abs(joystickMaxSpeed))
        {
            rb.velocity = new Vector2(joystickMaxSpeed, 0);
        }
    }
    private void StopWalking()
    {
        rb.drag = deceleration;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(gameObject.transform.position, gameObject.transform.position + new Vector3(rb.velocity.x, rb.velocity.y, 0));
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(gameObject.transform.position, gameObject.transform.position + new Vector3(InputManager.Instance.MovementVector.x, InputManager.Instance.MovementVector.y, 0));
    }

} // class PlayerScript 
// namespace
