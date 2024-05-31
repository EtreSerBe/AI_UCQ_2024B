using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;



// Este script es para darle (simular) a un agente de IA sistemas sensoriales.
// Por ejemplo, la vista o el o�do.

public class AgentSenses : MonoBehaviour
{
    // Queremos que nuestro agente de IA tenga un rango de visi�n.
    public float VisionRange = 10.0f;
    // Si un objeto que deber�a de ver est� dentro de ese rango de visi�n, entonces deber�a poder verlo;
    // si est� fuera de ese rango, no lo podr� ver.

    /*
     * �Por qu� hacemos funciones en el c�digo?
     * M�s f�cil de depurar,
     * M�s f�cil de reutilizar
     * M�s ordenado
     * Menos propenso a error humano.
     */

    // 1) Poner las cosas de inter�s seteadas de antemano.
    // en este caso, eso podr�a ser poner una referencia del objeto que nos interesa en el editor.
    public Transform InfiltratorTransform = null;

    // Este video se us� de referencia para la parte del cono de visi�n y el �ngulo.
    // https://youtu.be/lV47ED8h61k?si=IV2KaIP9fU4Pwwxj



    // Start is called before the first frame update
    void Start()
    {
        Vector3 myVectorA = new Vector3(1, 1, 0);
        Vector3 myVectorB = new Vector3(3, 4, 0);

        Vector3 myDistVectorBToA = VectorDiff(myVectorB, myVectorA);
        Vector3 myDistVectorAToB = VectorDiff(myVectorA, myVectorB);

        // NOTA: Un operador (en este caso, el operador minus '-' o s�mbolo de menos) est� "sobrecargado" para poder realizar la 
        // resta por componente entre dos vectores A y B. Es decir, (A.x - B.x), (A.y - B.y), (A.z, B.z).
        // Esto nos da como resultado un vector C, cuyos 'x', 'y', y 'z' son el resultado de cada resta individual.
        // Es decir, es exactamente como la funci�n "VectorDiff" que nosotros realizamos abajo.
        // Para m�s informaci�n: https://learn.microsoft.com/es-es/dotnet/csharp/language-reference/operators/
        // https://learn.microsoft.com/es-es/dotnet/csharp/language-reference/operators/operator-overloading
        Vector3 myDistVectorAToBMinus = myVectorA - myVectorB;

        // Vector3.Distance()
        Debug.Log(" myDistVectorAToB is: " + myDistVectorAToB.ToString());
        Debug.Log(" myDistVectorAToBMinus is: " + myDistVectorAToBMinus.ToString());

        Debug.Log("La magnitud del vector myDistVectorAToB es: " + Magnitude(myDistVectorAToB));
        Debug.Log("La magnitud del vector myDistVectorAToB, con la funci�n de unity, es: " + Vector3.Magnitude(myDistVectorAToB));

        Vector3 TestPosition = new Vector3(1, 2, 3);

        // Vamos a usar la magnitud que calculamos y nuestra variable del rango de visi�n, para determinar si X punto en el 
        // espacio est� dentro del rango de visi�n del agente que posea este script.
        // Para esto, usaremos la posici�n de nuestro agente como el origen.
        Vector3 TestMinusCharacter = VectorDiff(TestPosition, transform.position);
        // Ahora queremos la magnitud de este vector
        float VecMagnitud = Magnitude(TestMinusCharacter);

        // Si est�n hablando de un "si pasa tal cosa..." o "tengo que checar tal cosa", muy probablemente eso se 
        // vaya a traducir en un "if" en el c�digo.
        // En este ejemplo, queremos ver si la magnitud entre nuestro personaje y nuestro de punto de prueba
        // es mayor o menor que el rango de visi�n, para determinar si lo ve (o ver�a) o no.
        if (VecMagnitud > VisionRange)
        {
            Debug.Log("No lo veo");
        }
        else
        {
            Debug.Log("Lo veo");
        }

        // Muchas veces vamos a querer saber la direcci�n en la que estamos viendo.
        // Por ejemplo, para hacer una comparaci�n entre la direcci�n en que estoy mirando, y la direcci�n en que est� mi objetivo.
    }

    // M�todo de pit�goras para c�lculo de la hipotenusa de un tri�ngulo rect�ngulo.
    // �sto lo har� con un vector3.
    // Nos regresa un solo valor, que es la magnitud de un vector.
    float Magnitude(Vector3 in_Vector)
    {
        // Aplicamos teorema de pit�goras a este vector de distancia.
        // La hipotenusa/magnitud la ra�z cuadrada de la suma de los cuadrados de los componentes.
        float sqrX = in_Vector.x * in_Vector.x;
        float sqrY = in_Vector.y * in_Vector.y;
        float sqrZ = in_Vector.z * in_Vector.z;
        return Mathf.Sqrt(sqrX + sqrY + sqrZ);
    }


    // Una funci�n que nos regresa un vector que es la diferencia entre el vector Destino menos el vector Origen
    Vector3 VectorDiff(Vector3 destination, Vector3 origin)
    {
        return new Vector3(destination.x - origin.x, destination.y - origin.y, destination.z - origin.z);
    }

    // Update is called once per frame
    void Update()
    {
        // Diferencia entre mi posici�n y la posici�n de mi objetivo.
        Vector3 DistVector = VectorDiff(InfiltratorTransform.position, transform.position);

        // Queremos la magnitud de esa distancia.
        float DistMagnitude = Magnitude(DistVector);

        // Si la distancia entre la posici�n de mi objetivo y mi posici�n es mayor que mi rango de visi�n, entonces...
        if (DistMagnitude > VisionRange)
        {
            // Entonces no podemos ver a ese objetivo.
            Debug.Log("No lo veo");
        }
        else
        {
            // Entonces s� lo podr�a ver.
            Debug.Log("Lo veo");
        }
    }

    public bool TargetIsInRange(Vector3 targetPosition)
    {
        // Diferencia entre mi posici�n y la posici�n de mi objetivo.
        Vector3 distVector = VectorDiff(targetPosition, transform.position);

        // Queremos la magnitud de esa distancia.
        float distMagnitude = Magnitude(distVector);

        // Si la distancia entre la posici�n de mi objetivo y mi posici�n es mayor que mi rango de visi�n, entonces...
        if (distMagnitude > VisionRange)
        {
            // Entonces no podemos ver a ese objetivo.
            return false;
        }

        // Entonces s� lo podr�a ver.
        return true;
    }

    // Agregar una funci�n que se va a parecer un poco a TargetIsInRange, pero que va a hacer la parte del Cono de visi�n.


    // Gizmo
    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        // Nos ayuda a dibujar feedback visual en la vista de Escena del Editor.
        Gizmos.DrawLine(transform.position, InfiltratorTransform.position);

        // Antes de dibujar nuestra esfera, podemos cambiar su color seg�n si nuestro objetivo est� dentro o fuera de nuestro
        // rango de visi�n.
        if (TargetIsInRange(InfiltratorTransform.position))
        {
            // Si s� est� en rango, hacemos que la esfera sea roja.
            Gizmos.color = Color.red;
        }
        else
        {
            Gizmos.color = Color.green;
        }
        

        // Dibujo de nuestra esfera de visi�n desde la posici�n del Agente. Por lo tanto, usamos un radio = Rango de visi�n.
        Gizmos.DrawWireSphere(transform.position, VisionRange);


    }
}
