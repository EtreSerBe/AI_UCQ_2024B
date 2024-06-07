using System.Drawing;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class SteeringBehaviors : MonoBehaviour
{
    // Enums (Enumerations)
    // Norte sur este oeste.

    // tipo de Cabello [0]
    // tipo de cuerpo [1]
    // Clase de peleador [2]

    public enum SteeringBehaviorType
    {
        Seek = 0,
        Flee,
        Pursuit,
        Evade,
        SeekTheMouse,
        Arrive,
        MAX  // el n�mero de elementos que tiene dicho Enum. No 
    };
    
    public SteeringBehaviorType CurrentBehavior = SteeringBehaviorType.Seek;


    // Velocidad m�xima a la que nuestro personaje puede ir.
    // sirve para decirle a la aceleraci�n que ya no incremente m�s la velocidad.
    // PREGUNTA: �De qu� tipo de variable deber�a ser nuestra velocidad m�xima y por qu�?
    // float, porque �nicamente necesitamos limitar la magnitud, sin importar la direcci�n.
    public float MaxSpeed = 20.0f;

    // La magnitud de la fuerza que le vamos a aplicar al rigidbody.
    public float Force = 10.0f;

    // una referencia al Rigidbody component que tiene nuestro GameObject en el editor.
    public Rigidbody rb;
    public AgentSenses Senses;

    public Rigidbody EnemyRigidbody;

    public float ToleranceRadius = 1.0f;

    private Vector3 MouseWorldPos = Vector3.zero;

    void Awake()
    {
        Init();

        // encontramos al GameObject de nombre infiltrator y le pedimos su componente rigidbody.
        EnemyRigidbody = GameObject.Find("Infiltrator").GetComponent<Rigidbody>();
    }

    // Start is called before the first frame update
    void Start()
    {


        // rb = GetComponent<CapsuleCollider>();
        // rb = GetComponent<MeshRenderer>();
        // Component.Destroy(rb);  // si destruy�ramos el componente rigidbody aqu� en el c�digo, tambi�n
        // lo estar�amos borrando del GameObject que lo tiene asignado en el editor.
    }

    protected void Init()
    {
        rb = GetComponent<Rigidbody>();
        Senses = GetComponent<AgentSenses>();
    }



    // Update is called once per frame
    void Update()
    {
        // Si hacemos este cambio a la variable RB, se ver� reflejado en el editor
        //rb.angularDrag = rb.angularDrag + 0.01f;

        MouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        MouseWorldPos.z = transform.position.z;

        bool MouseIsInRange = Senses.TargetIsInRange(MouseWorldPos);

        // Algo as� deber�an de poder hacer con su tarea del cono de visi�n.
        // bool MouseIsInVisionCone = Senses.TargetIsInVisionCone(mouseWorldPos);


        // Debug.Log("El mouse s� est� en rango");
        // Si ya la puede "ver" o sentir, pues ya deber�a poder reaccionar ante ello., En este caso, perseguirlo.
    }

    void FixedUpdate()
    {
        Vector3 currentSteeringDirection = Vector3.zero;

        // Vector3 SeekVector = Seek(mouseWorldPos);
        switch (CurrentBehavior)
        {
            case SteeringBehaviorType.Seek:
                currentSteeringDirection = Seek(EnemyRigidbody.position);
                break;
            case SteeringBehaviorType.Flee:
                currentSteeringDirection = Flee(EnemyRigidbody.position);
                break;
            case SteeringBehaviorType.Pursuit:
                currentSteeringDirection = Pursuit(EnemyRigidbody.position, EnemyRigidbody.velocity);
                break;
            case SteeringBehaviorType.Evade:
                currentSteeringDirection = Evade(EnemyRigidbody.position, EnemyRigidbody.velocity);
                break;
            case SteeringBehaviorType.SeekTheMouse:
                // Hacer seek a la posici�n del mouse en pantalla dentro del juego.
                currentSteeringDirection = Seek(MouseWorldPos);
                break;
            case SteeringBehaviorType.Arrive:
                currentSteeringDirection = Arrive(MouseWorldPos, 5.0f);
                break;
        }

        // Una fuerza de magnitud = Force (que es una variable de esta clase), multiplicado por la direcci�n deseada
        // (que es la variable DesiredDirectionNormalized que tenemos aqu� arribita).

        Vector3 currentSteeringForce = Vector3.Min(currentSteeringDirection, currentSteeringDirection.normalized * Force);

        rb.AddForce(currentSteeringForce, ForceMode.Acceleration);

        // LimitToMaxSpeed();

    }

    bool InsideToleranceRadius(Vector3 targetPosition)
    {
        float distance = Vector3.Distance(transform.position, targetPosition);
        if (distance < ToleranceRadius)
        {
            rb.velocity = Vector3.zero;
            // Entonces no hacemos nada, porque ya estamos suficientemente cerca
            return true;
        }

        return false;
    }

    // Arrive
    // Tiene que bajar la velocidad cuando ya casi llega a su objetivo.
    // �C�mo vamos a saber que ya casi llega a su objetivo?
    // Vamos a usar una detecci�n de rango justo como la que usamos para detectar si algo estaba en nuestro rango de vista o no.
    protected Vector3 Arrive(Vector3 targetPosition, float SlowDownRadius)
    {
        // Es lo mismo que Seek, pero incorpora este aspecto de comenzar a frenar conforme nos acercamos al objetivo.


        // para perseguir a alguien, nos tenemos que mover en la direcci�n en la que est�n ellos, respecto a nuestra posici�n.
        Vector3 desiredDirection = targetPosition - transform.position;
        // Ahorita queremos solo la direcci�n, entonces guardamos la normalizaci�n de dicho vector.
        Vector3 desiredDirectionNormalized = desiredDirection.normalized;
        float distance = Vector3.Distance(transform.position, targetPosition);
        if (InsideToleranceRadius(targetPosition))
        {
            // Entonces no hacemos nada, porque ya estamos suficientemente cerca
            return Vector3.zero;
        }
        // Nos falta limitar qu� tanta magnitud va a tener.
        // Normalmente, vamos a toda velocidad, hasta que entramos al rango de bajar la velocidad.
        Vector3 desiredVelocity = desiredDirectionNormalized * MaxSpeed;
        //Checamos si ya entramos al rango de bajar la velocidad.
        if (distance < SlowDownRadius)
        {

            // si esto es verdad, s� estamos dentro del rango, y tenemos que limitar nuestra velocidad m�xima.
            desiredVelocity *= distance / SlowDownRadius;
        }

        // Esto es lo que faltaba, que nos d� fuerza solo para corregir nuestra velocidad cuando es necesario.
        Vector3 steeringForce = desiredVelocity - rb.velocity;

        return steeringForce;
    }


    protected Vector3 Evade(Vector3 targetPosition, Vector3 targetCurrentVelocity)
    {
        return -Pursuit(targetPosition, targetCurrentVelocity);
    }

    protected Vector3 Pursuit(Vector3 targetPosition, Vector3 targetCurrentVelocity)
    {
        // �cu�nto tiempo le tomar�a a nuestro agente llegar a la posici�n del target si nuestro agente va a su
        // m�xima velocidad?
        // la distancia entre nuestro agente y su objetivo, dividido entre la m�xima velocidad de nuestro agente
        float timeToReachTargetPosition = (targetPosition - transform.position).magnitude / MaxSpeed;

        // Usamos ese estimado de tiempo para proyectar/predecir la posici�n de nuestro objetivo seg�n su velocidad.
        // NOTA: La velocidad modifica una posici�n seg�n cu�nto tiempo pas�.
        Vector3 predictedTargetPosition = targetPosition + targetCurrentVelocity * timeToReachTargetPosition;
        return Seek(predictedTargetPosition);
    }

    protected Vector3 Flee(Vector3 targetPosition)
    {
        return -Seek(targetPosition);
    }

    protected Vector3 Seek(Vector3 targetPosition)
    {
        //if (InsideToleranceRadius(targetPosition))
        //{
        //    return Vector3.zero;
        //}

        // para perseguir a alguien, nos tenemos que mover en la direcci�n en la que est�n ellos, respecto a nuestra posici�n.
        Vector3 desiredDirection = targetPosition - transform.position;
        // Ahorita queremos solo la direcci�n, entonces guardamos la normalizaci�n de dicho vector.
        Vector3 desiredDirectionNormalized = desiredDirection.normalized;
        // Esto es lo que faltaba, que nos d� fuerza solo para corregir nuestra velocidad cuando es necesario.
        Vector3 steeringForce = desiredDirectionNormalized * MaxSpeed;
        return steeringForce - rb.velocity;
    }

    protected void LimitToMaxSpeed()
    {
        // Ahora necesitamos limitar velocidad, para que no supere a la m�xima velocidad (MaxSpeed).
        // Checamos la magnitud de la velocidad.
        // si esa magnitud es mayor que la MaxSpeed, entonces hay que limitarlo
        if (rb.velocity.magnitude > MaxSpeed)
        {
            // c�mo le dir�an: sigues yendo en la misma direcci�n, pero tu magnitud es distinta.
            // Qu� estamos tratando de cambiar? queremos cambiar la velocidad de nuestro rigidbody (es decir, rb.velocity)
            // la queremos limitar a que su magnitud sea la de nuestra velocidad m�xima
            rb.velocity = rb.velocity.normalized * MaxSpeed;  // la misma direcci�n de movimiento, pero con la magnitud del l�mite que le ponemos (MaxSpeed)
        }
    }


    void OnDrawGizmos()
    {
        // Solamente dibujar eso si ya est� asignado el EnemyRigidbody.
        if(EnemyRigidbody != null)
        {
            // Qu� queremos mostrar ahorita?
            // queremos ver las cosas del algoritmo Pursuit, por ejemplo, la posici�n futura a la que estamos persiguiendo.
            float timeToReachTargetPosition = (EnemyRigidbody.position - transform.position).magnitude / MaxSpeed;

            // Usamos ese estimado de tiempo para proyectar/predecir la posici�n de nuestro objetivo seg�n su velocidad.
            // NOTA: La velocidad modifica una posici�n seg�n cu�nto tiempo pas�.
            Vector3 predictedTargetPosition = EnemyRigidbody.position + EnemyRigidbody.velocity * timeToReachTargetPosition;

            Gizmos.color = UnityEngine.Color.yellow;
            Gizmos.DrawSphere(predictedTargetPosition, 1.0f);
        }

        if (rb != null)
        {
            // Hacer una l�nea desde nuestro agente hacia la velocidad en que se est� moviendo, para ver si s� llegar�a a la esfera
            // amarilla.
            Gizmos.DrawLine(transform.position, rb.velocity * 10000);
        }
        // MovementDirection = MovementDirection.normalized;
        // Gizmos.DrawLine(transform.position, transform.position + MovementDirection * 10000f);
    }

}
