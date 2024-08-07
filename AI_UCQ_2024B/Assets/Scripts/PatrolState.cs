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
    private Vector3 _initialPatrolPosition;

    private GameObject _FSMGameObjectOwner;

    private bool PlayerDetected = false;

    private float PlayerDetectedAccumulatedTime = 0.0f;

    private float AccumulatedTimeSinceLastPlayerDetected = 0.0f;

    public bool EnableDebug;

    public PatrolState()
    {
        Name = "Patrol";
    }

    public PatrolState(BaseFSM inBaseFSM) : base("Patrol", inBaseFSM)
    {

    }

    public virtual void InitializeState(BaseFSM inFSMRef, GameObject inPlayerGameObject, PatrolStateScriptableObject inStateValues, Vector3 inInitialPatrolPosition)
    {
        base.InitializeState(inFSMRef);

        _stateValues = inStateValues;
        _initialPatrolPosition = inInitialPatrolPosition;
        PlayerGameObject = inPlayerGameObject;
        // para poder acceder a la info del Patrullero, y, por ejemplo, hacer que rote cada cierto tiempo.
        _FSMGameObjectOwner = FSMRef.gameObject;
    }

    public override void OnEnter()
    {
        StartCoroutine(RotateCoroutine());
    }

    IEnumerator RotateCoroutine()
    {
        // Entra,
        // Se espera por TimeToRotate-segundos, 
        yield return new WaitForSeconds(_stateValues.TimeToRotate);

        // Y ya que se acabó el tiempo, rota al personaje.
        RotateGuard();

        // Después, decirle que la vuelva a iniciar, para que se vuelva a llamar dentro de ese mismo tiempo.
        StartCoroutine(RotateCoroutine());
    }

    void RotateGuard()
    {
        _FSMGameObjectOwner.transform.Rotate(Vector3.up, _stateValues.RotationAngle);
    }


    public override void OnUpdate()
    {
        //if(EnableDebug)
        //    Debug.Log();

        // Primero necesitamos saber si estamos detectando al jugador o no.
        PlayerDetected = TargetIsInRange();

        // Si sí se detectó, tenemos que acumular tiempo en la variable de acumular tiempo de detección.
        if (PlayerDetected)
        {
            PlayerDetectedAccumulatedTime += Time.deltaTime;

            // También, si sí lo detectamos, reseteamos este valor a 0.0f.
            // NOTA: una alternativa sería manejar estos de Accumulated como si fueran una barra que se llena y vacía.
            AccumulatedTimeSinceLastPlayerDetected += Time.deltaTime;

            // Si el tiempo acumulado es mayor al umbral que pusimos (es decir: TimeSeeingInfiltratorBeforeEnteringAlert)
            if (PlayerDetectedAccumulatedTime >= _stateValues.TimeSeeingInfiltratorBeforeEnteringAlert)
            {
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
            AccumulatedTimeSinceLastPlayerDetected += Time.deltaTime;

            // si no se detectó, y es llevamos un rato sin verlo, (es decir, si pasó cierto umbral de tiempo),
            // lo olvidamos y quitamos nuestro tiempo de detección acumulado.
            if (AccumulatedTimeSinceLastPlayerDetected >= _stateValues.TimeToForget)
            {
                PlayerDetectedAccumulatedTime = 0.0f;
            }
        }
    }

    public override void OnExit()
    {
        // Asegurarnos de detener la corrutina que hace que gire el guardia.
        StopCoroutine(RotateCoroutine());
    }

    private bool TargetIsInRange()
    {
        // Mandamos a llamar la función de la FSM, y le pasamos los valores que este estado ya contiene.
        return ((EnemyFSM)FSMRef).TargetIsInRange(PlayerGameObject.transform.position, _stateValues.VisionDistance);
    }

    // Update is called once per frame by the FSM in this case.
    //public override void Update()
    //{
    //    float h = 5.5f;

    //    int i = (int)h;

    //    float j = (float)i;

    //    Debug.Log("h: " + h + " i: " + i + " j: " + j);

    //    base.Update();

    //    Debug.Log(PlayerGameObject.name);

    //    // Aquí es 100% NECESARIO que hagamos el cast de nuestro FSMRef (que es de la clase BaseFSM) y lo 
    //    // casteemos a la clase que SÍ contiene la variable a la que queremos acceder, en este caso, AlertStateRef.
    //    FSMRef.ChangeState(((EnemyFSM)FSMRef).AlertStateRef);
    //    return;  // SIEMPRE, SIEMPRE, SIEMPREEE después del ChangeState.
    //    // SIEMPRE que se haga un ChangeState, lo ideal es hacer un return para salirnos de lo que estaba ejecutando dicho estado.
    //    //Debug.Log("Yo no me debería de imprimir porque ya no estoy en el estado de patrullaje.");
    //}
}
