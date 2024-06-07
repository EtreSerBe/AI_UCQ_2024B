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
        MAX  // el número de elementos que tiene dicho Enum. No 
    };
    
    public SteeringBehaviorType CurrentBehavior = SteeringBehaviorType.Seek;


    // Velocidad máxima a la que nuestro personaje puede ir.
    // sirve para decirle a la aceleración que ya no incremente más la velocidad.
    // PREGUNTA: ¿De qué tipo de variable debería ser nuestra velocidad máxima y por qué?
    // float, porque únicamente necesitamos limitar la magnitud, sin importar la dirección.
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
        // Component.Destroy(rb);  // si destruyéramos el componente rigidbody aquí en el código, también
        // lo estaríamos borrando del GameObject que lo tiene asignado en el editor.
    }

    protected void Init()
    {
        rb = GetComponent<Rigidbody>();
        Senses = GetComponent<AgentSenses>();
    }



    // Update is called once per frame
    void Update()
    {
        // Si hacemos este cambio a la variable RB, se verá reflejado en el editor
        //rb.angularDrag = rb.angularDrag + 0.01f;

        MouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        MouseWorldPos.z = transform.position.z;

        bool MouseIsInRange = Senses.TargetIsInRange(MouseWorldPos);

        // Algo así deberían de poder hacer con su tarea del cono de visión.
        // bool MouseIsInVisionCone = Senses.TargetIsInVisionCone(mouseWorldPos);


        // Debug.Log("El mouse sí está en rango");
        // Si ya la puede "ver" o sentir, pues ya debería poder reaccionar ante ello., En este caso, perseguirlo.
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
                // Hacer seek a la posición del mouse en pantalla dentro del juego.
                currentSteeringDirection = Seek(MouseWorldPos);
                break;
            case SteeringBehaviorType.Arrive:
                currentSteeringDirection = Arrive(MouseWorldPos, 5.0f);
                break;
        }

        // Una fuerza de magnitud = Force (que es una variable de esta clase), multiplicado por la dirección deseada
        // (que es la variable DesiredDirectionNormalized que tenemos aquí arribita).

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
    // ¿Cómo vamos a saber que ya casi llega a su objetivo?
    // Vamos a usar una detección de rango justo como la que usamos para detectar si algo estaba en nuestro rango de vista o no.
    protected Vector3 Arrive(Vector3 targetPosition, float SlowDownRadius)
    {
        // Es lo mismo que Seek, pero incorpora este aspecto de comenzar a frenar conforme nos acercamos al objetivo.


        // para perseguir a alguien, nos tenemos que mover en la dirección en la que están ellos, respecto a nuestra posición.
        Vector3 desiredDirection = targetPosition - transform.position;
        // Ahorita queremos solo la dirección, entonces guardamos la normalización de dicho vector.
        Vector3 desiredDirectionNormalized = desiredDirection.normalized;
        float distance = Vector3.Distance(transform.position, targetPosition);
        if (InsideToleranceRadius(targetPosition))
        {
            // Entonces no hacemos nada, porque ya estamos suficientemente cerca
            return Vector3.zero;
        }
        // Nos falta limitar qué tanta magnitud va a tener.
        // Normalmente, vamos a toda velocidad, hasta que entramos al rango de bajar la velocidad.
        Vector3 desiredVelocity = desiredDirectionNormalized * MaxSpeed;
        //Checamos si ya entramos al rango de bajar la velocidad.
        if (distance < SlowDownRadius)
        {

            // si esto es verdad, sí estamos dentro del rango, y tenemos que limitar nuestra velocidad máxima.
            desiredVelocity *= distance / SlowDownRadius;
        }

        // Esto es lo que faltaba, que nos dé fuerza solo para corregir nuestra velocidad cuando es necesario.
        Vector3 steeringForce = desiredVelocity - rb.velocity;

        return steeringForce;
    }


    protected Vector3 Evade(Vector3 targetPosition, Vector3 targetCurrentVelocity)
    {
        return -Pursuit(targetPosition, targetCurrentVelocity);
    }

    protected Vector3 Pursuit(Vector3 targetPosition, Vector3 targetCurrentVelocity)
    {
        // ¿cuánto tiempo le tomaría a nuestro agente llegar a la posición del target si nuestro agente va a su
        // máxima velocidad?
        // la distancia entre nuestro agente y su objetivo, dividido entre la máxima velocidad de nuestro agente
        float timeToReachTargetPosition = (targetPosition - transform.position).magnitude / MaxSpeed;

        // Usamos ese estimado de tiempo para proyectar/predecir la posición de nuestro objetivo según su velocidad.
        // NOTA: La velocidad modifica una posición según cuánto tiempo pasó.
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

        // para perseguir a alguien, nos tenemos que mover en la dirección en la que están ellos, respecto a nuestra posición.
        Vector3 desiredDirection = targetPosition - transform.position;
        // Ahorita queremos solo la dirección, entonces guardamos la normalización de dicho vector.
        Vector3 desiredDirectionNormalized = desiredDirection.normalized;
        // Esto es lo que faltaba, que nos dé fuerza solo para corregir nuestra velocidad cuando es necesario.
        Vector3 steeringForce = desiredDirectionNormalized * MaxSpeed;
        return steeringForce - rb.velocity;
    }

    protected void LimitToMaxSpeed()
    {
        // Ahora necesitamos limitar velocidad, para que no supere a la máxima velocidad (MaxSpeed).
        // Checamos la magnitud de la velocidad.
        // si esa magnitud es mayor que la MaxSpeed, entonces hay que limitarlo
        if (rb.velocity.magnitude > MaxSpeed)
        {
            // cómo le dirían: sigues yendo en la misma dirección, pero tu magnitud es distinta.
            // Qué estamos tratando de cambiar? queremos cambiar la velocidad de nuestro rigidbody (es decir, rb.velocity)
            // la queremos limitar a que su magnitud sea la de nuestra velocidad máxima
            rb.velocity = rb.velocity.normalized * MaxSpeed;  // la misma dirección de movimiento, pero con la magnitud del límite que le ponemos (MaxSpeed)
        }
    }


    void OnDrawGizmos()
    {
        // Solamente dibujar eso si ya está asignado el EnemyRigidbody.
        if(EnemyRigidbody != null)
        {
            // Qué queremos mostrar ahorita?
            // queremos ver las cosas del algoritmo Pursuit, por ejemplo, la posición futura a la que estamos persiguiendo.
            float timeToReachTargetPosition = (EnemyRigidbody.position - transform.position).magnitude / MaxSpeed;

            // Usamos ese estimado de tiempo para proyectar/predecir la posición de nuestro objetivo según su velocidad.
            // NOTA: La velocidad modifica una posición según cuánto tiempo pasó.
            Vector3 predictedTargetPosition = EnemyRigidbody.position + EnemyRigidbody.velocity * timeToReachTargetPosition;

            Gizmos.color = UnityEngine.Color.yellow;
            Gizmos.DrawSphere(predictedTargetPosition, 1.0f);
        }

        if (rb != null)
        {
            // Hacer una línea desde nuestro agente hacia la velocidad en que se está moviendo, para ver si sí llegaría a la esfera
            // amarilla.
            Gizmos.DrawLine(transform.position, rb.velocity * 10000);
        }
        // MovementDirection = MovementDirection.normalized;
        // Gizmos.DrawLine(transform.position, transform.position + MovementDirection * 10000f);
    }

}
