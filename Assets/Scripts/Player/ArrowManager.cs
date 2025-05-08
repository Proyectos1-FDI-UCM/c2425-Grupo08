//---------------------------------------------------------
// Script que lleva la lógica y gestión de las flechas que apuntan a los enemigos cuando atacan.
// Miguel Ángel González López
// Beyond the Depths
// Proyectos 1 - Curso 2024-25
//---------------------------------------------------------

using UnityEngine;

/// <summary>
/// La clase ArrowManager es única y gestiona la lógica y movimiento de todas las flechas. Las flechas no gestionan su lógica de manera interna.
/// </summary>
public class ArrowManager: MonoBehaviour
{
    // ---- ATRIBUTOS DEL INSPECTOR ----
    #region Atributos del Inspector (serialized fields)
    // Número máximo de objetivos que puede asumir la lógica de las flechas
    #endregion
    [SerializeField] private int MaxObjectives = 0;
    // Referencia al prefab de la flecha
    [SerializeField] private GameObject Arrow;
    
    // ---- ATRIBUTOS PRIVADOS ----
    #region Atributos Privados (private fields)
    // Una estructura que contiene todas las flechas
    private struct _arrowStructure{
        // array de flechas del jugador
        public _arrow[] _arrows{get;set;}
        // "Tapón" del array. Indica en que posición está la última flecha añadida.
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
    }
    #endregion

    // ---- MÉTODOS PÚBLICOS ----
    #region Métodos públicos

    // Se llama desde fuera del script. Crea una flecha y actualiza la estructura de flechas. La ID de la flecha es una referencia al objeto al que apunta.
    public void CreateArrow(GameObject objective){
        if (objective != null){
            _arrowsBuffer._arrows[_arrowsBuffer._hat]._objective = objective;
            _arrowsBuffer._arrows[_arrowsBuffer._hat]._arrowObject = Instantiate(Arrow,objective.transform.position,Quaternion.identity);
            CalculateArrowPosition(objective,_arrowsBuffer._arrows[_arrowsBuffer._hat]._arrowObject);
            _arrowsBuffer._hat ++;
            }
        else Debug.Log("ERROR: trying to create an arrow with a null object");
        }
    // Se llama desde fuera del script. Borra una flecha que tiene la ID del objeto al que apunta.
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

    #endregion
    
    // ---- MÉTODOS PRIVADOS ----
    #region Métodos Privados

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

    #endregion   1
} // class ArrowManager