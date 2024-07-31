using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolState : BaseState
{
    public GameObject PlayerGameObject;

    public float VisionAngle = 45.0f;
    public float VisionDistance = 10.0f;
    // Cu�nto tiempo tiene que ver al Infiltrador para pasar al estado de Alerta.
    // como referencia: https://www.youtube.com/clip/UgkxIfuLTVvptjElLkcgE3VMJmSU6qCytLei
    public float TimeSeeingInfiltratorBeforeEnteringAlert = 2.0f;
    // Cu�nto tiempo tiene que pasar sin ver al infiltrador antes de olvidarlo
    // (es decir, si ya no lo ha visto en tanto tiempo, entonces se relaja y borra el tiempo
    // que se hab�a acumulado en TimeSeeingInfiltratorBeforeEnteringAlert).
    public float TimeToForget = 5.0f;
    // Qu� tantos grados gira en su posici�n este enemigo cuando est� patrullando.
    public float RotationAngle = 45.0f;
    // Cada cu�nto tiempo va a rotar el patrullero en su posici�n.
    public float TimeToRotate = 5.0f;

    // Posici�n de patrullaje inicial a la cual volver� despu�s de Alert o Attack, seg�n corresponda.
    protected Vector3 InitialPatrolPosition;

    private GameObject FSMGameObjectOwner;

    public PatrolState()
    {
        Name = "Patrol";
    }

    public PatrolState(BaseFSM inBaseFSM) : base("Patrol", inBaseFSM)
    {

    }

    public void InitializeState(float in_VisionAngle, float in_VisionDistance,
        float in_TimeSeeingInfiltratorBeforeEnteringAlert, float in_TimeToForget, float in_RotationAgle,
        float in_TimeToRotate, Vector3 in_InitialPatrolPosition)
    {
        VisionAngle = in_VisionAngle;
        VisionDistance = in_VisionDistance;
        TimeSeeingInfiltratorBeforeEnteringAlert = in_TimeSeeingInfiltratorBeforeEnteringAlert;
        TimeToForget = in_TimeToForget;
        RotationAngle = in_RotationAgle;
        TimeToRotate = in_TimeToRotate;
        InitialPatrolPosition = in_InitialPatrolPosition;
        // para poder acceder a la info del Patrullero, y, por ejemplo, hacer que rote cada cierto tiempo.
        FSMGameObjectOwner = FSMRef.gameObject;  
    }

    public override void Enter()
    {
        StartCoroutine(RotateCoroutine());
    }

    IEnumerator RotateCoroutine()
    {
        // Entra,
        // Se espera por TimeToRotate-segundos, 
        yield return new WaitForSeconds(TimeToRotate);

        // Y ya que se acab� el tiempo, rota al personaje.
        RotateGuard();

        // Despu�s, decirle que la vuelva a iniciar, para que se vuelva a llamar dentro de ese mismo tiempo.
        StartCoroutine(RotateCoroutine());
    }

    void RotateGuard()
    {
        FSMGameObjectOwner.transform.Rotate(Vector3.up, RotationAngle);
    }


    public override void Update()
    {

    }

    public override void Exit()
    {
        // Asegurarnos de detener la corrutina que hace que gire el guardia.
        StopCoroutine(RotateCoroutine());
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

    //    // Aqu� es 100% NECESARIO que hagamos el cast de nuestro FSMRef (que es de la clase BaseFSM) y lo 
    //    // casteemos a la clase que S� contiene la variable a la que queremos acceder, en este caso, AlertStateRef.
    //    FSMRef.ChangeState(((EnemyFSM)FSMRef).AlertStateRef);
    //    return;  // SIEMPRE, SIEMPRE, SIEMPREEE despu�s del ChangeState.
    //    // SIEMPRE que se haga un ChangeState, lo ideal es hacer un return para salirnos de lo que estaba ejecutando dicho estado.
    //    //Debug.Log("Yo no me deber�a de imprimir porque ya no estoy en el estado de patrullaje.");
    //}
}
