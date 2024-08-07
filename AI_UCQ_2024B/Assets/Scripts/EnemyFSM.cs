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

    [SerializeField]
    private PatrolStateScriptableObject PatrolStateValues;

    [SerializeField]
    private AlertStateScriptableObject AlertStateValues;

    // Posici�n de patrullaje inicial a la cual volver� despu�s de Alert o Attack, seg�n corresponda.
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

        // Le decimos que su posici�n inicial de patrullaje es aquella en la que estaba cuando se le dio play a la escena.
        InitialPatrolPosition = transform.position;

        PlayerGameObject = GameObject.FindGameObjectWithTag("Player");

        // Recuerden que, como necesitamos que nuestros estados hereden de MonoBehavior para que puedan usar Corrutinas, 
        // entonces tenemos que a�adirlos con AddComponent, no podemos crearlos usando new como antes.
        PatrolStateInstance = this.AddComponent<PatrolState>();

        // Estas dos l�neas las quit� porque las met� a la funci�n de InitializeState y
        // ya nom�s paso this y PlayerGameObject como par�metros. 
        // PatrolStateInstance.FSMRef = this;
        // PatrolStateInstance.PlayerGameObject = PlayerGameObject;


        // Ahora nos basta con pasarle la referencia al Scriptable Object que tiene los valores deseados de dicho estado.
        PatrolStateInstance.InitializeState(this, PlayerGameObject, PatrolStateValues, InitialPatrolPosition);

        AlertStateInstance = this.AddComponent<AlertState>();
        AlertStateInstance.InitializeState(this, PlayerGameObject, AlertStateValues);



        base.Start();
    }


    // Tenemos que sobreescribir la funci�n de GetInitialState para que no sea null.
    public override BaseState GetInitialState()
    {
        // Aqu� el estado inicial deber�a ser Patrol State, ya que lo implementemos.
        return PatrolStateInstance;
    }

    public bool TargetIsInRange(Vector3 targetPosition, float VisionRange)
    {
        // Diferencia entre mi posici�n y la posici�n de mi objetivo.
        // Queremos la magnitud de esa distancia.
        float distance = (targetPosition - transform.position).magnitude;

        // Si la distancia entre la posici�n de mi objetivo y mi posici�n es mayor que mi rango de visi�n, entonces...
        if (distance > VisionRange)
        {
            // Entonces no podemos ver a ese objetivo.
            return false;
        }

        // Entonces s� lo podr�a ver.
        return true;
    }
}
