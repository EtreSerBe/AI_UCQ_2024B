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

    [SerializeField]
    private PatrolStateScriptableObject PatrolStateValues;

    [SerializeField]
    private AlertStateScriptableObject AlertStateValues;

    // Posición de patrullaje inicial a la cual volverá después de Alert o Attack, según corresponda.
    protected Vector3 InitialPatrolPosition;
    
    public GameObject PlayerGameObject;


    // private AttackState AttackStateInstance;
    private int i; 

    // Start is called before the first frame update
    public override void Start()
    {
        i = 5;
        // Antes de asignar el estado inicial, hay que crear los estados de esta FSM.
        Debug.Log(i);

        // Le decimos que su posición inicial de patrullaje es aquella en la que estaba cuando se le dio play a la escena.
        InitialPatrolPosition = transform.position;

        PlayerGameObject = GameObject.FindGameObjectWithTag("Player");

        // Recuerden que, como necesitamos que nuestros estados hereden de MonoBehavior para que puedan usar Corrutinas, 
        // entonces tenemos que añadirlos con AddComponent, no podemos crearlos usando new como antes.
        PatrolStateInstance = this.AddComponent<PatrolState>();

        // Estas dos líneas las quité porque las metí a la función de InitializeState y
        // ya nomás paso this y PlayerGameObject como parámetros. 
        // PatrolStateInstance.FSMRef = this;
        // PatrolStateInstance.PlayerGameObject = PlayerGameObject;


        // Ahora nos basta con pasarle la referencia al Scriptable Object que tiene los valores deseados de dicho estado.
        PatrolStateInstance.InitializeState(this, PlayerGameObject, PatrolStateValues, InitialPatrolPosition);

        AlertStateInstance = this.AddComponent<AlertState>();
        AlertStateInstance.InitializeState(this, PlayerGameObject, AlertStateValues);



        base.Start();
    }


    // Tenemos que sobreescribir la función de GetInitialState para que no sea null.
    public override BaseState GetInitialState()
    {
        // Aquí el estado inicial debería ser Patrol State, ya que lo implementemos.
        return PatrolStateInstance;
    }

    public bool TargetIsInRange(Vector3 targetPosition, float VisionRange)
    {
        // Diferencia entre mi posición y la posición de mi objetivo.
        // Queremos la magnitud de esa distancia.
        float distance = (targetPosition - transform.position).magnitude;

        // Si la distancia entre la posición de mi objetivo y mi posición es mayor que mi rango de visión, entonces...
        if (distance > VisionRange)
        {
            // Entonces no podemos ver a ese objetivo.
            return false;
        }

        // Entonces sí lo podría ver.
        return true;
    }
}
