using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Dynamic;
using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class SteeringBehaviors : MonoBehaviour
{
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


    // Start is called before the first frame update
    void Start()
    {
        Init();

        // encontramos al GameObject de nombre infiltrator y le pedimos su componente rigidbody.
        EnemyRigidbody = GameObject.Find("Infiltrator").GetComponent<Rigidbody>();

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

        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = transform.position.z;

        bool MouseIsInRange = Senses.TargetIsInRange(mouseWorldPos);

        // Algo así deberían de poder hacer con su tarea del cono de visión.
        // bool MouseIsInVisionCone = Senses.TargetIsInVisionCone(mouseWorldPos);


        Debug.Log("El mouse sí está en rango");
        // Si ya la puede "ver" o sentir, pues ya debería poder reaccionar ante ello., En este caso, perseguirlo.

        // Vector3 SeekVector = Seek(mouseWorldPos);

        Vector3 PursuitVector = Pursuit(EnemyRigidbody.position, EnemyRigidbody.velocity);

        // Una fuerza de magnitud = Force (que es una variable de esta clase), multiplicado por la dirección deseada
        // (que es la variable DesiredDirectionNormalized que tenemos aquí arribita).

        rb.AddForce(PursuitVector * Force, ForceMode.Acceleration);

        LimitToMaxSpeed();

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

    protected Vector3 Seek(Vector3 targetPosition)
    {
        // para perseguir a alguien, nos tenemos que mover en la dirección en la que están ellos, respecto a nuestra posición.
        Vector3 desiredDirection = targetPosition - transform.position;
        // Ahorita queremos solo la dirección, entonces guardamos la normalización de dicho vector.
        Vector3 desiredDirectionNormalized = desiredDirection.normalized;
        return desiredDirectionNormalized;
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

}
