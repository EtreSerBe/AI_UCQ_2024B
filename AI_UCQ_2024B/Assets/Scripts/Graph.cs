using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

// Nuestro grafo est� compuesto por dos sets de cosas: V�rtices (Nodos) y aristas (Edges).

public class Node
{
    public Node(string id)
    {
        this.Id = id;
    }

    public string Id; 

    // Lista de aristas que posee.
    // public List<Edge> Neighbors;

    // un nodo "Parent"
    public Node Parent;
}


public class Edge
{
    public Edge(Node a, Node b)
    {
        this.A = a;
        this.B = b;
    }

    // Vamos a guardar, en esta implementaci�n espec�ficamente, cu�les dos nodos est� conectando.
    public Node A;
    public Node B;
    // public float Weight;
}


public class Graph : MonoBehaviour
{
    public List<Edge> edges = new List<Edge>();
    public List<Node> nodes = new List<Node>();

    // Cu�l es el problema ahorita?
    // El algoritmo DFS puede regresar al nodo desde el que acaba de llegar, y entonces se cicla.


    // Start is called before the first frame update
    void Start()
    {
        Node A = new Node("A");
        Node B = new Node("B");
        Node C = new Node("C");
        Node D = new Node("D");
        Node E = new Node("E");
        Node F = new Node("F");
        Node G = new Node("G");
        Node H = new Node("H");

        nodes.Add(A);
        nodes.Add(B);
        nodes.Add(C);
        nodes.Add(D);
        nodes.Add(E);
        nodes.Add(F);
        nodes.Add(G);
        nodes.Add(H);

        Edge AB = new Edge(A, B);
        Edge BC = new Edge(B, C);
        Edge BD = new Edge(B, D);
        Edge AE = new Edge(A, E);
        Edge EF = new Edge(E, F);
        Edge EG = new Edge(E, G);
        Edge EH = new Edge(E, H);

        edges.Add(AB);
        edges.Add(BC);
        edges.Add(BD);
        edges.Add(AE);
        edges.Add(EF);
        edges.Add(EG);
        edges.Add(EH);

        Debug.Log("Iniciando DFS");
        // H.Parent = H;  // Ponemos el valor especial para que no se cicle e identificar que �l es la ra�z de nuestro pathfinding
        DepthFirstSearch(H, D);
        Debug.Log("Termin� DFS");
    }

    // Encuentra los nodos que compartan una arista con el nodo "node".
    public List<Node> FindNeighbors(Node node)
    {
        List<Node> neighbors = new List<Node>();

        // Buscar en todas las aristas, cu�les aristas mencionen al nodo de entrada "node".
        foreach (Edge edge in edges)
        {
            if (edge.A == node)
            {
                // esta arista conecta con un vecino de "node".
                neighbors.Add(edge.B);
            }
            if (edge.B == node)
            {
                // esta arista conecta con un vecino de "node".
                neighbors.Add(edge.A);
            }
        }

        // Encuentra
        return neighbors;
    }

    // La clave del Breadth-first search es que usa una Queue, en vez de una Stack para su lista abierta.

    public bool DepthFirstSearch(Node startNode, Node goalNode)
    {
        // Ponemos el valor especial al startNode para identificarlo como la ra�z del �rbol de nuestro pathfinding.
        startNode.Parent = null;

        Stack<Node> openList = new Stack<Node>();
        openList.Push(startNode);
        List<Node> closedList = new List<Node>();

        Node currentNode = null;

        // Cu�ndo terminaba la recursi�n de la DFS recursiva?
        // La forma 1 era cuando ya encontramos el nodo objetivo.
        // La forma 2 es cuando ya no hay nodos por explorar.
        while (openList.Count > 0)
        {
            currentNode = openList.Pop();
            // Cuando sacamos a alguien de la lista abierta, lo metemos inmediatamente en la lista cerrada.
            closedList.Add(currentNode);

            // Checamos si ya llegamos al destino.
            if (currentNode == goalNode)
            {
                while (currentNode != null)
                {
                    Debug.Log("El nodo: " + currentNode.Id + " fue parte del camino a la meta.");
                    currentNode = currentNode.Parent;  // Backtracking
                }

                // Si s� llegamos, nos salimos de la funci�n y regresamos true porque s� hubo un camino de startNode a goalNode.
                return true;
            }

            // Si no hemos llegado todav�a, visitamos a los vecinos.
            List<Node> neighbors = FindNeighbors(currentNode);
            foreach (Node neighbor in neighbors)
            {
                // Si tu vecino ya est� en la lista abierta o ya est� en la lista cerrada, entonces no lo agregues.
                if (openList.Contains(neighbor) || closedList.Contains(neighbor))
                    continue;

                // Como currentNode lo est� metiendo a la lista abierta, entonces le decimos a ese nodo que currentNode es su padre.
                // En mi b�squeda yo llegu� a Neighbor desde currentNode.
                neighbor.Parent = currentNode;
                // En vez de hacer la recursi�n de la funci�n, lo metemos en nuestra openList, para visitarlo cuando corresponda.
                openList.Push(neighbor);
            }
        }

        // Por defecto decimos que no hubo camino.
        return false;
    }

    public bool DepthFirstSearchRecursive(Node currentNode, Node goalNode)
    {
        Debug.Log("Nodo actualmente visitado: " + currentNode.Id);
        // Primero checamos si ya llegamos al nodo Objetivo (goalNode)
        if (currentNode == goalNode)
        {
            // Si s�, entonces ya llegamos y s� hay un camino desde el inicio hasta la meta.
            return true;
        }

        // Si no hemos llegado a la meta, tenemos que aplicar DFS a todos los nodos vecinos.
        List<Node> neighbors = FindNeighbors(currentNode);
        bool DFS_Result = false;
        foreach (Node neighbor in neighbors)
        {
            // Si tu vecino No tiene padre, le asignas el nodo actual como su padre y s� aplicas el algoritmo otra vez a partir de �l.
            if (neighbor.Parent == null) // entonces no tiene padre
            {
                neighbor.Parent = currentNode;
                DFS_Result = DepthFirstSearchRecursive(neighbor, goalNode);
            }


            if (DFS_Result)
            {
                Debug.Log("El nodo: " + currentNode.Id + " fue parte del camino a la meta.");
                // Entonces mi vecino/hijo s� encontr� un camino hacia la meta, y por lo tanto yo tambi�n.
                return true;
            }
        }

        // Aqu� es: ninguno de mis vecinos logr� encontrar el camino, y por lo tanto yo tampoco, entonces regreso false.
        return false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
