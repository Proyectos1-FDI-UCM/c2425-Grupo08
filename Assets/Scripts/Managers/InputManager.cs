//---------------------------------------------------------
// Contiene el componente de InputManager
// Guillermo Jiménez Díaz, Pedro Pablo Gómez Martín
// TemplateP1
// Proyectos 1 - Curso 2024-25
//---------------------------------------------------------
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;

/// <summary>
/// Manager para la gestión del Input. Se encarga de centralizar la gestión
/// de los controles del juego. Es un singleton que sobrevive entre
/// escenas.
/// La configuración de qué controles realizan qué acciones se hace a través
/// del asset llamado InputActionSettings que está en la carpeta Settings.
///
/// A modo de ejemplo, este InputManager tiene métodos para consultar
/// el estado de dos acciones:
/// - Move: Permite acceder a un Vector2D llamado MovementVector que representa
/// el estado de la acción Move (que se puede realizar con el joystick izquierdo
/// del gamepad, con los cursores...)
/// - Fire: Se proporcionan 3 métodos (FireIsPressed, FireWasPressedThisFrame
/// y FireWasReleasedThisFrame) para conocer el estado de la acción Fire (que se
/// puede realizar con la tecla Space, con el botón Sur del gamepad...)
///
/// Dependiendo de los botones que se quieran añadir, será necesario ampliar este
/// InputManager. Para ello:
/// - Revisar lo que se hace en Init para crear nuevas acciones
/// - Añadir nuevos métodos para acceder al estado que estemos interesados
///
/// </summary>
public class InputManager : MonoBehaviour
{
    // ---- ATRIBUTOS DEL INSPECTOR ----

    #region Atributos del Inspector (serialized fields)

    // Documentar cada atributo que aparece aquí.
    // El convenio de nombres de Unity recomienda que los atributos
    // públicos y de inspector se nombren en formato PascalCase
    // (palabras con primera letra mayúscula, incluida la primera letra)
    // Ejemplo: MaxHealthPoints

    #endregion

    // ---- ATRIBUTOS PRIVADOS ----

    #region Atributos Privados (private fields)

    /// <summary>
    /// Instancia única de la clase (singleton).
    /// </summary>
    private static InputManager _instance;

    /// <summary>
    /// Controlador de las acciones del Input. Es una instancia del asset de
    /// InputAction que se puede configurar desde el editor y que está en
    /// la carpeta Settings
    /// </summary>
    private InputActionSettings _theController;

    // Del Player
    /// <summary>
    /// Acción para Fire. Si tenemos más botones tendremos que crear más
    /// acciones como esta (y crear los métodos que necesitemos para
    /// conocer el estado del botón)
    /// </summary>
    private InputAction _fire;

    private InputAction _jump;

    private InputAction _focus;

    private InputAction _flash;

    private InputAction _interact;

    // Del UI
    private InputAction _return;

    #endregion

    #region Vectores de Inputs
    /// <summary>
    /// Propiedad para acceder al vector de movimiento.
    /// Según está configurado el InputActionController, es un vector normalizado
    /// </summary>
    public Vector2 MovementVector { get; private set; }

    #endregion

    // ---- MÉTODOS DE MONOBEHAVIOUR ----

    #region Métodos de MonoBehaviour

    /// <summary>
    /// Método llamado en un momento temprano de la inicialización.
    ///
    /// En el momento de la carga, si ya hay otra instancia creada,
    /// nos destruimos (al GameObject completo)
    /// </summary>
    protected void Awake()
    {
        if (_instance != null)
        {
            // No somos la primera instancia. Se supone que somos un
            // InputManager de una escena que acaba de cargarse, pero
            // ya había otro en DontDestroyOnLoad que se ha registrado
            // como la única instancia.
            // Nos destruímos. DestroyImmediate y no Destroy para evitar
            // que se inicialicen el resto de componentes del GameObject para luego ser
            // destruídos. Esto es importante dependiendo de si hay o no más managers
            // en el GameObject.
            DestroyImmediate(this.gameObject);
        }
        else
        {
            // Somos el primer InputManager.
            // Queremos sobrevivir a cambios de escena.
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
            Init();
        }
    } // Awake

    /// <summary>
    /// Método llamado cuando se destruye el componente.
    /// </summary>
    protected void OnDestroy()
    {
        if (this == _instance)
        {
            // Éramos la instancia de verdad, no un clon.
            _instance = null;
        } // if somos la instancia principal
    } // OnDestroy

    #endregion

    // ---- MÉTODOS PÚBLICOS ----

    #region Métodos públicos

    /// <summary>
    /// Propiedad para acceder a la única instancia de la clase.
    /// </summary>
    public static InputManager Instance
    {
        get
        {
            Debug.Assert(_instance != null);
            return _instance;
        }
    } // Instance

    /// <summary>
    /// Devuelve cierto si la instancia del singleton está creada y
    /// falso en otro caso.
    /// Lo normal es que esté creada, pero puede ser útil durante el
    /// cierre para evitar usar el GameManager que podría haber sido
    /// destruído antes de tiempo.
    /// </summary>
    /// <returns>True si hay instancia creada.</returns>
    public static bool HasInstance()
    {
        return _instance != null;
    }

    #region Player Interactions

    public bool JumpIsPressed()
    {
        return _jump.IsPressed();
    }

    public bool FocusIsPressed()
    {
        return _focus.IsPressed();
    }

    public bool FlashIsPressed()
    {
        return _flash.IsPressed();
    }

    public bool JumpWasPressedThisFrame()
    {
        return _jump.WasPressedThisFrame();
    }

    public bool JumpWasRealeasedThisFrame()
    {
        return _jump.WasReleasedThisFrame();
    }

    public bool InteractIsPressed()
    {
        return _interact.IsPressed();
    }

    public bool InteractWasPressedThisFrame()
    {
        return _interact.WasPressedThisFrame();
    }

    public bool InteractWasRealeasedThisFrame()
    {
        return _interact.WasReleasedThisFrame();
    }

    public bool ReturnWasReleased()
    {
        return _return.WasPressedThisFrame();
    }

    // Getters para los controles del juego

    public string GetInteractKey()
    {
        string key;

        if (IsGamepadActive() && _interact.bindings.Count > 1)
        {
            // Devuelve el botón genérico por defecto
            key = _interact.bindings[1].effectivePath.Split('/').Last().ToUpper();

            // Para PlayStation
            if (IsDualShockController())

                key = "CUADRADO";

            // Para Xbox
            else if (IsXboxController())

                key = "X";

            // Para Switch Pro Controller
            else if (IsSwitchProController())

                key = "Y";
        }

        else // Si no hay gamepad o no hay binding para el gamepad, usamos el teclado

            key = _interact.bindings[0].effectivePath.Split('/').Last().ToUpper();

        return key;
    }

    public string GetJumpKey()
    {
        string key;

        if (IsGamepadActive() && _jump.bindings.Count > 1)
        {
            // Devuelve el botón genérico por defecto
            key = _jump.bindings[1].effectivePath.Split('/').Last().ToUpper();

            // Para PlayStation
            if (IsDualShockController())

                key = "X";

            // Para Xbox
            else if (IsXboxController())

                key = "A";

            // Para Switch Pro Controller
            else if (IsSwitchProController())

                key = "B";
        }

        else // Si no hay gamepad o no hay binding para el gamepad, usamos el teclado

            key = "Espacio";

        return key;
    }

    public string GetFocusKey()
    {
        string key;

        if (IsGamepadActive() && _focus.bindings.Count > 1)
        {
            // Devuelve el botón genérico por defecto
            key = _focus.bindings[1].effectivePath.Split('/').Last().ToUpper();

            // Para PlayStation
            if (IsDualShockController())

                key = "L2";

            // Para Xbox
            else if (IsXboxController())

                key = "LT";

            // Para Switch Pro Controller
            else if (IsSwitchProController())

                key = "ZL";
        }

        else // Si no hay gamepad o no hay binding para el gamepad, usamos el teclado

            key = "Click derecho";

        return key;
    }

    public string GetFlashKey()
    {
        string key;

        if (IsGamepadActive() && _flash.bindings.Count > 1)
        {
            // Devuelve el botón genérico por defecto
            key = _flash.bindings[1].effectivePath.Split('/').Last().ToUpper();

            // Para PlayStation
            if (IsDualShockController())

                key = "R2";

            // Para Xbox
            else if (IsXboxController())

                key = "RT";

            // Para Switch Pro Controller
            else if (IsSwitchProController())

                key = "ZR";
        }

        else // Si no hay gamepad o no hay binding para el gamepad, usamos el teclado

            key = "Click izquierdo";

        return key;
    }

    public string GetReturnKey()
    {
        string key;

        if (IsGamepadActive() && _return.bindings.Count > 1)
        {
            // Devuelve el botón genérico por defecto
            key = _return.bindings[1].effectivePath.Split('/').Last().ToUpper();

            // Para PlayStation
            if (IsDualShockController())

                key = "OPTIONS";

            // Para Xbox
            else if (IsXboxController())

                key = "START";

            // Para Switch Pro Controller
            else if (IsSwitchProController())

                key = "+/-";
        }

        else // Si no hay gamepad o no hay binding para el gamepad, usamos el teclado

            key = "Esc";

        return key;
    }

    #endregion

    /// <summary>
    /// Método que devuelve la posición del cursor en la pantalla.
    /// Se puede usar para saber dónde está el ratón en el mundo.
    /// </summary>
    /// <returns>Posición del mouse en el mundo</returns>
    public Vector2 GetWorldCursorPos()
    {
        return Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
    }

    /// <summary>
    /// Método que devuelve las coordenadas de movimiento del joystick derecho del gamepad.
    /// Se puede usar para saber dónde apunta el joystick derecho de un mando.
    /// </summary>
    /// <returns>Vector compuesto de coordenadas del joystick derecho</returns>
    public Vector2 GetRightStickInput()
    {
        return Gamepad.current.rightStick.ReadValue();
    }

    /// <summary>
    /// Método que devuelve si el gamepad está activo o no.
    /// Se puede usar para saber si el gamepad está conectado y activo.
    /// </summary>
    /// <returns>Devuelve true si hay un mando conectado.</returns>
    public bool IsGamepadActive()
    {
        return Gamepad.current != null;
    }

    #endregion

    // ---- MÉTODOS PRIVADOS ----

    #region Métodos Privados

    /// <summary>
    /// Dispara la inicialización.
    /// </summary>
    private void Init()
    {
        // Creamos el controlador del input y activamos los controles del jugador
        _theController = new InputActionSettings();
        _theController.Player.Enable();
        _theController.UI.Enable();

        // Cacheamos la acción de movimiento
        InputAction movement = _theController.Player.Move;
        // Para el movimiento, actualizamos el vector de movimiento usando
        // el método OnMove
        movement.performed += OnMove;
        movement.canceled += OnMove;

        // Cacheamos la acción de salto
        _jump = _theController.Player.Jump;

        // Controles para la linterna (apuntado y flasheado)
        _focus = _theController.Player.Focus;

        _flash = _theController.Player.Flash;

        _interact = _theController.Player.Interact;

        // Cacheamos la acción de Volver para los menús
        _return = _theController.UI.Return;
    }

    /// <summary>
    /// Método que es llamado por el controlador de input cuando se producen
    /// eventos de movimiento (relacionados con la acción Move)
    /// </summary>
    /// <param name="context">Información sobre el evento de movimiento</param>
    private void OnMove(InputAction.CallbackContext context)
    {
        MovementVector = context.ReadValue<Vector2>();
    }

    /// <summary>
    /// Método que devuelve si se está usando un dualshock
    /// </summary>
    private bool IsDualShockController()
    {
        return Gamepad.current is UnityEngine.InputSystem.DualShock.DualShockGamepad;
    }

    /// <summary>
    /// Método que devuelve si se está usando un mando de xbox
    /// </summary>
    private bool IsXboxController()
    {
        return Gamepad.current is UnityEngine.InputSystem.XInput.XInputController;
    }

    /// <summary>
    /// Método que devuelve si se está usando mando de switch
    /// </summary>
    private bool IsSwitchProController()
    {
        return Gamepad.current is UnityEngine.InputSystem.Switch.SwitchProControllerHID;
    }





    #endregion

} // class InputManager