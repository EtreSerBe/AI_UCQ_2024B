using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyFSM : BaseFSM
{
    // Necesitamos que la máquina contenga una referencia a cada estado que va a poseer.
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


    public float VisionAngle = 45.0f;
    public float VisionDistance = 10.0f;
    public GameObject PlayerGameObject;


    // private AttackState AttackStateInstance;
    private int i; 

    // Start is called before the first frame update
    public override void Start()
    {
        i = 5;
        // Antes de asignar el estado inicial, hay que crear los estados de esta FSM.
        Debug.Log(i);

        PlayerGameObject = GameObject.FindGameObjectWithTag("Player");

        PatrolStateInstance = new PatrolState(this);
        PatrolStateInstance.PlayerGameObject = PlayerGameObject;

        AlertStateInstance = new AlertState(this);

        base.Start();
    }


    // Tenemos que sobreescribir la función de GetInitialState para que no sea null.
    public override BaseState GetInitialState()
    {
        // Aquí el estado inicial debería ser Patrol State, ya que lo implementemos.
        return PatrolStateInstance;
    }
}
