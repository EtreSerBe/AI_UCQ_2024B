using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolState : BaseState
{
    // GameObject que es el Player al cual nuestro patrullero debe detectar.
    public GameObject PlayerGameObject;

    // Referencia al Scriptable Object que contiene varios valores que describen cómo se debe comportar este estado.
    // Por ejemplo, VisionAngle, VisionDistance, TimeToForget, RotationAngle, etc.
    // Todas ellas se accederán a través de "_stateValues." + el nombre de la variable deseada. Ejemplo: _stateValues.TimeToRotate
    private PatrolStateScriptableObject _stateValues;

    // Posición de patrullaje inicial a la cual volverá después de Alert o Attack, según corresponda.
    // private Vector3 _initialPatrolPosition;

    private GameObject _FSMGameObjectOwner;

    private bool _playerDetected = false;

    private float _playerDetectedAccumulatedTime = 0.0f;

    private float _accumulatedTimeSinceLastPlayerDetected = 0.0f;

    public bool EnableDebug;

    private Coroutine _rotationCoroutine;

    public PatrolState()
    {
        Name = "Patrol";
    }

    public PatrolState(BaseFSM inBaseFSM) : base("Patrol", inBaseFSM)
    {

    }

    public virtual void InitializeState(BaseFSM inFSMRef, GameObject inPlayerGameObject, PatrolStateScriptableObject inStateValues, Vector3 inInitialPatrolPosition)
    {
        Name = "PatrolState";

        base.InitializeState(inFSMRef);

        _stateValues = inStateValues;
        PlayerGameObject = inPlayerGameObject;
        // para poder acceder a la info del Patrullero, y, por ejemplo, hacer que rote cada cierto tiempo.
        _FSMGameObjectOwner = FSMRef.gameObject;
    }

    public override void OnEnter()
    {
        base.OnEnter(); // lo usamos para imprimir OnEnter del estado de Patrol

        // Obtenemos una referencia a la co-rutina para poder detenerla apropiadamente.
        _rotationCoroutine = StartCoroutine(RotateCoroutine());

        _playerDetectedAccumulatedTime = 0.0f;
        _accumulatedTimeSinceLastPlayerDetected = 0.0f;
    }

    IEnumerator RotateCoroutine()
    {
        // Entra,
        // Se espera por TimeToRotate-segundos, 
        yield return new WaitForSeconds(_stateValues.TimeToRotate);

        // Y ya que se acabó el tiempo, rota al personaje.
        RotateGuard();

        // Después, decirle que la vuelva a iniciar, para que se vuelva a llamar dentro de ese mismo tiempo.
        _rotationCoroutine = StartCoroutine(RotateCoroutine());
    }

    void RotateGuard()
    {
        Debug.Log("Rotando al guardia.");
        _FSMGameObjectOwner.transform.Rotate(Vector3.up, _stateValues.RotationAngle);
    }


    public override void OnUpdate()
    {
        //if(EnableDebug)
        //    Debug.Log();

        // Primero necesitamos saber si estamos detectando al jugador o no.
        _playerDetected = TargetIsInRange();

        // Si sí se detectó, tenemos que acumular tiempo en la variable de acumular tiempo de detección.
        if (_playerDetected)
        {
            // Primero, quitamos el tiempo acumulado de no ver al player, porque lo acabamos de ver.
            _accumulatedTimeSinceLastPlayerDetected = 0.0f;

            _playerDetectedAccumulatedTime += Time.deltaTime;

            // También, si sí lo detectamos, reseteamos este valor a 0.0f.
            // NOTA: una alternativa sería manejar estos de Accumulated como si fueran una barra que se llena y vacía.
            _accumulatedTimeSinceLastPlayerDetected += Time.deltaTime;

            // Si el tiempo acumulado es mayor al umbral que pusimos (es decir: TimeSeeingInfiltratorBeforeEnteringAlert)
            if (_playerDetectedAccumulatedTime >= _stateValues.TimeSeeingInfiltratorBeforeEnteringAlert)
            {
                Debug.Log("Vi suficiente tiempo al player, pasaré al estado de Alerta.");
                // Entonces pasamos al estado de alerta.
                // Necesitamos castear de EnemyFSM para poder decirle a cuál estado queremos pasar.
                FSMRef.ChangeState(((EnemyFSM)FSMRef).AlertStateRef);

                // como hacemos un cambio de estado, SIEMPRE hacemos nuestro return;
                return;
            }
        }
        else
        {
            // si no lo detectamos, acumulamos tiempo desde la última vez que lo vimos.
            _accumulatedTimeSinceLastPlayerDetected += Time.deltaTime;

            // si no se detectó, y es llevamos un rato sin verlo, (es decir, si pasó cierto umbral de tiempo),
            // lo olvidamos y quitamos nuestro tiempo de detección acumulado.
            if (_accumulatedTimeSinceLastPlayerDetected >= _stateValues.TimeToForget)
            {
                Debug.Log("Ya pasé mucho tiempo sin ver al player, me olvidaré de él.");
                _playerDetectedAccumulatedTime = 0.0f;
            }
        }
    }

    public override void OnExit()
    {
        // Asegurarnos de detener la co-rutina que hace que gire el guardia. Para ello, le pasamos la referencia que guardamos.
        StopCoroutine(_rotationCoroutine);

        base.OnExit();
    }

    private bool TargetIsInRange()
    {
        // Mandamos a llamar la función de la FSM, y le pasamos los valores que este estado ya contiene.
        return ((EnemyFSM)FSMRef).TargetIsInRange(PlayerGameObject.transform.position, _stateValues.VisionDistance);
    }

    // Update is called once per frame by the FSM in this case.
    //public override void Update()
    //{

    //    // Aquí es 100% NECESARIO que hagamos el cast de nuestro FSMRef (que es de la clase BaseFSM) y lo 
    //    // casteemos a la clase que SÍ contiene la variable a la que queremos acceder, en este caso, AlertStateRef.
    //    FSMRef.ChangeState(((EnemyFSM)FSMRef).AlertStateRef);
    //    return;  // SIEMPRE, SIEMPRE, SIEMPREEE después del ChangeState.
    //    // SIEMPRE que se haga un ChangeState, lo ideal es hacer un return para salirnos de lo que estaba ejecutando dicho estado.
    //    //Debug.Log("Yo no me debería de imprimir porque ya no estoy en el estado de patrullaje.");
    //}
}
