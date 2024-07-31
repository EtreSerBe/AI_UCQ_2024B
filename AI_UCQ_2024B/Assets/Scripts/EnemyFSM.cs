using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyFSM : BaseFSM
{
    // Necesitamos que la m�quina contenga una referencia a cada estado que va a poseer.
    private PatrolState PatrolStateInstance;

    public PatrolState PatrolStateRef
    {
        get { return PatrolStateInstance; }
    }

    private AlertState AlertStateInstance;

    public AlertState AlertStateRef
    {
        get { return AlertStateInstance; }
    }

    // Patrol state
    public float VisionAnglePatrol = 45.0f;
    public float VisionDistancePatrol = 10.0f;
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


    // Alert State
    public float VisionAngleAlert = 60.0f;
    public float VisionDistanceAlert = 15.0f;
    // Cu�nto tiempo tiene que ver al Infiltrador para pasar al estado de Alerta.
    public float TimeSeeingInfiltratorBeforeEnteringAttack = 2.0f;

    // Attack state
    public float VisionAngleAttack = 45.0f;
    public float VisionDistanceAttack = 10.0f;

    public GameObject PlayerGameObject;


    // private AttackState AttackStateInstance;
    private int i; 

    // Start is called before the first frame update
    public override void Start()
    {
        i = 5;
        // Antes de asignar el estado inicial, hay que crear los estados de esta FSM.
        Debug.Log(i);

        // Le decimos que su posici�n inicial de patrullaje es aquella en la que estaba cuando se le dio play a la escena.
        InitialPatrolPosition = transform.position;

        PlayerGameObject = GameObject.FindGameObjectWithTag("Player");

        PatrolStateInstance = this.AddComponent<PatrolState>();
        PatrolStateInstance.FSMRef = this;
        PatrolStateInstance.PlayerGameObject = PlayerGameObject;

        PatrolStateInstance.InitializeState(VisionAnglePatrol, VisionDistancePatrol, 
            TimeSeeingInfiltratorBeforeEnteringAlert, TimeToForget, RotationAngle, TimeToRotate, 
            InitialPatrolPosition);


        AlertStateInstance = new AlertState(this);

        base.Start();
    }


    // Tenemos que sobreescribir la funci�n de GetInitialState para que no sea null.
    public override BaseState GetInitialState()
    {
        // Aqu� el estado inicial deber�a ser Patrol State, ya que lo implementemos.
        return PatrolStateInstance;
    }
}
