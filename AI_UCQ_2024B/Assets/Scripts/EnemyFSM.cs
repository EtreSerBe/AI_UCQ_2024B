using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemyFSM : BaseFSM
{
    // Necesitamos que la máquina contenga una referencia a cada estado que va a poseer.
    private PatrolState PatrolStateInstance;
    private AlertState AlertStateInstance;

    // Estos "getters" se usan para que los estados de esta FSM puedan LEER los estados pero no modificarlos.
    public PatrolState PatrolStateRef
    {
        get { return PatrolStateInstance; }
    }


    public AlertState AlertStateRef
    {
        get { return AlertStateInstance; }
    }

    // También necesitamos que la máquina contenga una referencia a los
    // ScriptableObjects de cada estado que posee.
    [SerializeField]
    private PatrolStateScriptableObject PatrolStateValues;

    [SerializeField]
    private AlertStateScriptableObject AlertStateValues;

    // Posición de patrullaje inicial a la cual volverá después de Alert o Attack, según corresponda.
    private Vector3 _initialPatrolPosition;
    public Vector3 InitialPatrolPosition
    {
        get { return _initialPatrolPosition; }
    }

    public GameObject PlayerGameObject;

    // NavMeshAgent del dueño de esta FSM. Se usa para decirle a qué posición moverse.
    // Asignarlo (al GameObject que tenga este script de FSM) en el editor para poder obtenerlo en el start
    // y poder usarlo; si no, tronará. 
    // Los estados pueden acceder a él a través de la máquina de estados.
    private NavMeshAgent _navMeshAgentRef;
    public NavMeshAgent NavMeshAgentRef
    {
        get { return _navMeshAgentRef; }
    }

    // Al igual que con el _navMeshAgent, hay que signarlo al
    // GameObject que tenga este script de FSM en el editor para poder obtenerlo en el start
    private MeshRenderer _meshRendererRef;

    public MeshRenderer MeshRenderer
    {
        get { return _meshRendererRef; }
    }

    // Usaremos esta wall layer mask para hacer que el patrullero no pueda ver al jugador a través de las paredes.
    // La inicializamos en el Start de nuestra FSM.
    // Para que funcione, tienen que ir a "Tags & Layers", agregar una nueva layer que se llame "Wall", y
    // asignarle dicha Layer a los gameObjects en su escena que sean paredes. Les recomiendo hacer un gameObject
    // vacío que sea padre de las walls de su escenario, y hacer que ese padre tenga el Layer de wall, y aplicarle
    // el cambio de layer a todos los hijos (vean la Jerarquía de mi escena para que vean que todas mis walls
    // tienen el tag de Wall).
    private LayerMask _wallLayerMask;

    // private AttackState AttackStateInstance;

    // Start is called before the first frame update
    public override void Start()
    {
        // Aquí inicializamos esto, que usaremos con un raycast más abajo.
        // _wallLayerMask = LayerMask.GetMask("Wall");

        if (!TryGetComponent<MeshRenderer>(out _meshRendererRef))
        {
            Debug.LogError("Invalid MeshRenderer in the FSM.");
            return;
        }

        // Le decimos que su posición inicial de patrullaje es aquella en la que estaba cuando se le dio play a la escena.
        _initialPatrolPosition = transform.position;

        PlayerGameObject = GameObject.FindGameObjectWithTag("Player");
        if (PlayerGameObject == null)
        {
            Debug.LogError("Invalid PlayerGameObject in the FSM.");
            return;
        }

        // Obtenemos el NavMeshAgent que está asignado a este GameObject en el editor.
        // Al parecer, esto de TryGetComponent es la manera recomendada de checar si tiene un componente y,
        // si sí lo tiene, asignarlo (por eso usa la palabra "out" porque lo asigna en esa variable).
        if(!TryGetComponent<NavMeshAgent>(out _navMeshAgentRef))
        {
            Debug.LogError("Invalid NavMeshAgent in the FSM.");
            return;
        }

        // Antes de asignar el estado inicial, hay que crear los estados de esta FSM.


        // Recuerden que, como necesitamos que nuestros estados hereden de MonoBehavior para que puedan usar Corrutinas, 
        // entonces tenemos que añadirlos con AddComponent, no podemos crearlos usando new como antes.
        PatrolStateInstance = this.AddComponent<PatrolState>();

        // Ahora nos basta con pasarle la referencia al Scriptable Object que tiene los valores deseados de dicho estado.
        PatrolStateInstance.InitializeState(this, PlayerGameObject, PatrolStateValues, InitialPatrolPosition);

        // Hacemos lo mismo con los otros estados según corresponda.
        AlertStateInstance = this.AddComponent<AlertState>();
        AlertStateInstance.InitializeState(this, PlayerGameObject, AlertStateValues);

        // Finalmente, ya que tenemos completo el setup de la FSM, la iniciamos con la función Start de su clase padre.
        base.Start();
    }


    // Tenemos que sobreescribir la función de GetInitialState para que no sea null.
    public override BaseState GetInitialState()
    {
        // Aquí el estado inicial debería ser Patrol State, ya que lo implementemos.
        return PatrolStateInstance;
    }

    public bool TargetIsInRange(Vector3 targetPosition, float visionRange)
    {
        // Diferencia entre mi posición y la posición de mi objetivo.
        // Queremos la magnitud de esa distancia.
        float distance = (targetPosition - transform.position).magnitude;

        // Si la distancia entre la posición de mi objetivo y mi posición es mayor que mi rango de visión, entonces...
        if (distance > visionRange)
        {
            // le pongo un color de material para cuando No lo vio.
            _meshRendererRef.material.color = new Color(1, 0, 0, 1);

            // Entonces no podemos ver a ese objetivo.
            return false;
        }

        // le pongo un color de material para cuando sí lo vio.
        _meshRendererRef.material.color = new Color(1, 0, 1, 1);

        // Entonces sí lo podría ver.
        return true;
    }
}
