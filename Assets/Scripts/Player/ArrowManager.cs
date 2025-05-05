//---------------------------------------------------------
// Script que lleva la lógica y gestión de las flechas que apuntan a los enemigos cuando atacan.
// Miguel Ángel González López
// Project Abyss
// Proyectos 1 - Curso 2024-25
//---------------------------------------------------------

using UnityEngine;

/// <summary>
/// Antes de cada class, descripción de qué es y para qué sirve,
/// usando todas las líneas que sean necesarias.
/// </summary>
public class ArrowManager: MonoBehaviour
{
    // ---- ATRIBUTOS DEL INSPECTOR ----
    #region Atributos del Inspector (serialized fields)
    // Documentar cada atributo que aparece aquí.
    // El convenio de nombres de Unity recomienda que los atributos
    // públicos y de inspector se nombren en formato PascalCase
    // (palabras con primera letra mayúscula, incluida la primera letra)
    // Ejemplo: MaxHealthPoints
    #endregion
    [SerializeField] private int MaxObjectives = 0;
    [SerializeField] private GameObject Arrow;
    //[SerializeField] GameObject test;
    
    // ---- ATRIBUTOS PRIVADOS ----
    #region Atributos Privados (private fields)
    // Documentar cada atributo que aparece aquí.
    // El convenio de nombres de Unity recomienda que los atributos
    // privados se nombren en formato _camelCase (comienza con _, 
    // primera palabra en minúsculas y el resto con la 
    // primera letra en mayúsculas)
    // Ejemplo: _maxHealthPoints
    private struct _arrowStructure{
        public _arrow[] _arrows{get;set;}
        public byte _hat{get;set;}
    };
    private struct _arrow{
        //Uso un Transform y no un Vector3 ya que es necesario que sea de tipo referencia. El Transform es un objeto
        //por lo que por defecto es por referencia. En esta version de Unity no se pueden hacer tipos referencia.
        public GameObject _objective{get;set;}
        public GameObject _arrowObject{get;set;}
    };
    
    _arrowStructure _arrows;

    _arrowStructure _arrowsBuffer;

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
        // Reserva espacio para el suficiente número de flechas
        _arrowsBuffer._arrows = new _arrow[MaxObjectives];
        _arrowsBuffer._hat = 0;
        if (_arrowsBuffer._arrows.Length == 0){
            Debug.Log("ERROR: No se ha establecido un tamaño para el array de flechas");
        }
            
    }
    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        UpdateArrows();
        //Debug.Log(_arrowsBuffer._hat);
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
    public void CreateArrow(GameObject objective){
        if (objective != null){
            _arrowsBuffer._arrows[_arrowsBuffer._hat]._objective = objective;
            _arrowsBuffer._arrows[_arrowsBuffer._hat]._arrowObject = Instantiate(Arrow,objective.transform.position,Quaternion.identity);
            CalculateArrowPosition(objective,_arrowsBuffer._arrows[_arrowsBuffer._hat]._arrowObject);
            _arrowsBuffer._hat ++;
            }
        else Debug.Log("ERROR: trying to create an arrow with a null object");
        }
    public void DeleteArrow(GameObject arrowToDelete){
        bool arrowFound = false;
        int i = 0;
        if (_arrowsBuffer._hat != 0){
            while (i <_arrowsBuffer._arrows.Length && !arrowFound){
                if (_arrowsBuffer._arrows[i]._objective==arrowToDelete){
                    Destroy(_arrowsBuffer._arrows[i]._arrowObject);
                    _arrowsBuffer._arrows[i] = _arrowsBuffer._arrows[_arrowsBuffer._hat];
                    _arrowsBuffer._hat--;
                    arrowFound = true;
                }
                i++;
            }
            if (!arrowFound){
                Debug.Log("ERROR: trying to remove a non-existent arrow");;
            }
        }

    }
    
    // ---- MÉTODOS PRIVADOS ----
    #region Métodos Privados
    // Documentar cada método que aparece aquí
    // El convenio de nombres de Unity recomienda que estos métodos
    // se nombren en formato PascalCase (palabras con primera letra
    // mayúscula, incluida la primera letra)

    #endregion   
    /// <summary>
    /// Itera sobre el array de flechas y actualiza sus estados
    /// </summary>
    private void UpdateArrows (){
        if (_arrowsBuffer._hat != 0){
            for(int i = 0; i<_arrowsBuffer._hat;i++ ){
                CalculateArrowPosition(_arrowsBuffer._arrows[i]._objective,_arrowsBuffer._arrows[i]._arrowObject);                     
            }
        }
    }
    /// <summary>
    /// Calcula la dirección de la flecha según la posición del objetivo y la mueve al lugar indicado.
    /// </summary>
    /// <param name="objective"></param>
    /// <param name="arrow"></param>
    private void CalculateArrowPosition(GameObject objective, GameObject arrow) {
        if (objective && arrow != null){
            Vector2 objectivePos = objective.transform.position;
            Vector2 relativePos = objectivePos - (Vector2)transform.position;

            float angle = Mathf.Atan2(-relativePos.x, relativePos.y) * Mathf.Rad2Deg;

            arrow.transform.SetPositionAndRotation(transform.position, Quaternion.Euler(0, 0, angle));
        }
    }

} // class ArrowManager