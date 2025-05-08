//---------------------------------------------------------
// Breve descripción del contenido del archivo
// La funcion principal de la camara para el jugador
// Responsable de la creación de este archivo: Andrés Díaz Guerrero Soto (El sordo)
// Beyond the Depths
// Proyectos 1 - Curso 2024-25
//---------------------------------------------------------

using UnityEngine;
// Añadir aquí el resto de directivas using


/// <summary>
/// Antes de cada class, descripción de qué es y para qué sirve,
/// usando todas las líneas que sean necesarias.
/// </summary>
public class MasterCamera : MonoBehaviour
{
    // ---- ATRIBUTOS DEL INSPECTOR ----

    /// <summary>
    /// El referencia para que la camara pueda tener referencia de jugador
    /// </summary> 

    [SerializeField]
    private Transform PlayerTransform;

    // Permite poner la camara 1.5 arriba de jugador para que no sea en el centro

    [SerializeField]
    private Vector2 CameraOffset = new Vector2(0f, 1.5f);

    // Limite minimo de eje X en la camara que no va a sobre pasar

    [SerializeField]
    private float MinX = -10f;

    // Limite maximo de eje X

    [SerializeField]
    private float MaxX = 10f;

    // Limite minimo de eje Y

    [SerializeField]
    private float MinY = -5f;

    // Limite maximo de eje Y

    [SerializeField]
    private float MaxY = 5f;

    // ---- ATRIBUTOS PRIVADOS ----
    #region Atributos Privados (private fields)
    // Documentar cada atributo que aparece aquí.
    // El convenio de nombres de Unity recomienda que los atributos
    // privados se nombren en formato _camelCase (comienza con _, 
    // primera palabra en minúsculas y el resto con la 
    // primera letra en mayúsculas)
    // Ejemplo: _maxHealthPoints

    #endregion

    // Referencia de componetne de la camara para forzar el aspect

    private Camera _camera;


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
        
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        
    }
    #endregion

    // Aqui es donde fuerza el aspect de la camara y que siempre se actualizen

    void start()
    {
        _camera = GetComponent<Camera>();
        if ( _camera != null )
        {
            // forzar el aspecto 16:9
            _camera.aspect = 16f / 9f;
        }
    }

    // Metodo para que siempre compruebe si esta sigueindo el jugador usando el metodo LAteUpdate

    void LateUpdate()
    {
        if (PlayerTransform == null) return;

        // La posición deseada de la cámara es la posición del jugador más el offset.
        Vector3 newPosition = new Vector3(
            PlayerTransform.position.x + CameraOffset.x,
            PlayerTransform.position.y + CameraOffset.y,
            transform.position.z
        );

        // Aplicar clamping para limitar la posición de la cámara.
        newPosition.x = Mathf.Clamp(newPosition.x, MinX, MaxX);
        newPosition.y = Mathf.Clamp(newPosition.y, MinY, MaxY);

        // Asignar la nueva posición a la cámara.
        transform.position = newPosition;
    }

    // ---- MÉTODOS PÚBLICOS ----
    #region Métodos públicos
   

    #endregion

    // ---- MÉTODOS PRIVADOS ----
    #region Métodos Privados
    

    #endregion

} // class MasterCamera 
// class MasterCamera
