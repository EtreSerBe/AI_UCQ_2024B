using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

// Nuestro grafo está compuesto por dos sets de cosas: Vértices (Nodos) y aristas (Edges).



public class Node
{
    public Node(string id, int inX, int inY)
    {
        this.Id = id;
        X = inX;
        Y = inY;
    }

    public Node()
    {
    }

    public string Id;
    public float Distance = 1000000.0f; // puesto a un valor alto para cuando implementemos Djikstra.
    public float TerrainCost = 1;
    public float TotalCost = 1000000.0f;
    public int X = 0;
    public int Y = 0;

    // Función que calcula ese costo total pasándole el valor del nodo que quiere volverse su nodo padre.
    public bool UpdateTotalCost(Node ParentNode)
    {
        if (ParentNode.TotalCost + this.TerrainCost < TotalCost)
        {
            // Aquí es que se encontró un mejor camino a este nodo.
            // Entonces tenemos que reajustar su posición en la lista abierta.
            TotalCost = ParentNode.TotalCost + this.TerrainCost;
            // Y también decirle que este nodo es ahora su padre.
            Parent = ParentNode;

            // regresamos True porque sí se actualizó el costo total del nodo y
            // es necesario ajustar su posición en la lista abierta.
            return true;
        }

        return false;
    }

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

    // Vamos a guardar, en esta implementación específicamente, cuáles dos nodos está conectando.
    public Node A;
    public Node B;
    // public float Weight;
}


public class Graph : MonoBehaviour
{
    public List<Edge> edges = new List<Edge>();
    public List<Node> nodes = new List<Node>();

    // Cuál es el problema ahorita?
    // El algoritmo DFS puede regresar al nodo desde el que acaba de llegar, y entonces se cicla.


    // Start is called before the first frame update
    void Start()
    {
        //Node A = new Node("A");
        //Node B = new Node("B");
        //Node C = new Node("C");
        //Node D = new Node("D");
        //Node E = new Node("E");
        //Node F = new Node("F");
        //Node G = new Node("G");
        //Node H = new Node("H");

        //nodes.Add(A);
        //nodes.Add(B);
        //nodes.Add(C);
        //nodes.Add(D);
        //nodes.Add(E);
        //nodes.Add(F);
        //nodes.Add(G);
        //nodes.Add(H);

        //Edge AB = new Edge(A, B);
        //Edge BC = new Edge(B, C);
        //Edge BD = new Edge(B, D);
        //Edge AE = new Edge(A, E);
        //Edge EF = new Edge(E, F);
        //Edge EG = new Edge(E, G);
        //Edge EH = new Edge(E, H);

        //edges.Add(AB);
        //edges.Add(BC);
        //edges.Add(BD);
        //edges.Add(AE);
        //edges.Add(EF);
        //edges.Add(EG);
        //edges.Add(EH);

        // Node Grid[][] = new Node[3][];
        Node X1Y1 = new Node("X1Y1", 1, 1);
        Node X1Y2 = new Node("X1Y2", 1, 2);
        Node X1Y3 = new Node("X1Y3", 1, 3);
        Node X2Y1 = new Node("X2Y1", 2, 1);
        Node X2Y2 = new Node("X2Y2", 2, 2);
        Node X2Y3 = new Node("X2Y3", 2, 3);
        Node X3Y1 = new Node("X3Y1", 3, 1);
        Node X3Y2 = new Node("X3Y2", 3, 2);
        Node X3Y3 = new Node("X3Y3", 3, 3);

        nodes.Add(X1Y1);
        nodes.Add(X1Y2);
        nodes.Add(X1Y3);
        nodes.Add(X2Y1);
        nodes.Add(X2Y2);
        nodes.Add(X2Y3);
        nodes.Add(X3Y1);
        nodes.Add(X3Y2);
        nodes.Add(X3Y3);

        // Upper left corner.
        Edge X1Y1_X1Y2 = new Edge(X1Y1, X1Y2);
        Edge X1Y1_X2Y1 = new Edge(X1Y1, X2Y1);

        // Middle upper
        Edge X2Y1_X3Y1 = new Edge(X2Y1, X3Y1);
        Edge X2Y1_X2Y2 = new Edge(X2Y1, X2Y2);

        // Upper right corner
        Edge X3Y1_X3Y2 = new Edge(X3Y1, X3Y2);

        // Mid row left
        Edge X1Y2_X2Y2 = new Edge(X1Y2, X2Y2);
        Edge X1Y2_X1Y3 = new Edge(X1Y2, X1Y3);

        // Middle middle
        Edge X2Y2_X3Y2 = new Edge(X2Y2, X3Y2);
        Edge X2Y2_X2Y3 = new Edge(X2Y2, X2Y3);

        // Middle right
        Edge X3Y2_X3Y3 = new Edge(X3Y2, X3Y3);

        // Lower left
        Edge X1Y3_X2Y3 = new Edge(X1Y3, X2Y3);

        // Lower middle
        Edge X2Y3_X3Y3 = new Edge(X2Y3, X3Y3);

        edges.Add(X1Y1_X1Y2);
        edges.Add(X1Y1_X2Y1);
        edges.Add(X2Y1_X3Y1);
        edges.Add(X2Y1_X2Y2);
        edges.Add(X3Y1_X3Y2);
        edges.Add(X1Y2_X2Y2);
        edges.Add(X1Y2_X1Y3);
        edges.Add(X2Y2_X3Y2);
        edges.Add(X2Y2_X2Y3);
        edges.Add(X3Y2_X3Y3);
        edges.Add(X1Y3_X2Y3);
        edges.Add(X2Y3_X3Y3);


        Debug.Log("Iniciando DFS");
        // H.Parent = H;  // Ponemos el valor especial para que no se cicle e identificar que él es la raíz de nuestro pathfinding
        // DepthFirstSearch(H, D);
        BestFirstSearch(X1Y1, X3Y1);
        Debug.Log("Terminó DFS");
    }

    // Encuentra los nodos que compartan una arista con el nodo "node".
    public List<Node> FindNeighbors(Node node)
    {
        List<Node> neighbors = new List<Node>();

        // Buscar en todas las aristas, cuáles aristas mencionen al nodo de entrada "node".
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

    // Primero haremos la función para sacar la heurística de distancia.
    // Como es distancia euclidiana, usamos el teorema de Pitágoras.
    float GetDistance(Node start, Node goal)
    {
        float diffX = Mathf.Pow((start.X - goal.X), 2);
        float diffY = Mathf.Pow((start.Y - goal.Y), 2);
        return Mathf.Sqrt(diffY + diffX);
    }

    public bool DjikstraSearch(Node startNode, Node goalNode)
    {
        // Ponemos el valor especial al startNode para identificarlo como la raíz del árbol de nuestro pathfinding.
        startNode.Parent = null;

        PriorityQueue openList = new PriorityQueue();
        openList.Enqueue(startNode);
        List<Node> closedList = new List<Node>();

        Node currentNode = null;

        // Cuándo terminaba la recursión de la DFS recursiva?
        // La forma 1 era cuando ya encontramos el nodo objetivo.
        // La forma 2 es cuando ya no hay nodos por explorar.
        while (openList.Count > 0)
        {
            currentNode = openList.Dequeue();
            // Cuando sacamos a alguien de la lista abierta, lo metemos inmediatamente en la lista cerrada.
            closedList.Add(currentNode);

            // Checamos si ya llegamos al destino.
            if (HasReachedGoal(currentNode, goalNode))
                return true;

            // Si no hemos llegado todavía, visitamos a los vecinos.
            List<Node> neighbors = FindNeighbors(currentNode);
            foreach (Node neighbor in neighbors)
            {
                // Si tu vecino ya está en la lista cerrada, entonces no lo agregues.
                if (closedList.Contains(neighbor))
                    continue;

                //ya está en la lista abierta o
                if (openList.Contains(neighbor))
                {
                    // tenemos que checar si su TotalCost Actual es el mejor.
                    if (neighbor.UpdateTotalCost(currentNode))
                    {
                        // Sí es verdadero, entonces tenemos que sacar al nodo neighbor de la lista Abierta
                        // y luego volver a insertarlo con su nueva prioridad
                        openList.Remove(neighbor);
                        openList.InsertDjikstra(neighbor);
                    }
                }
                else
                {
                    // Si los vamos a meter a la lista, primero les asignamos su valor de la heurística de TotalCost.
                    neighbor.UpdateTotalCost(goalNode);

                    Debug.Log("La distancia del nodo: " + neighbor.Id + " al objetivo es de: " + neighbor.TotalCost);

                    // En vez de hacer la recursión de la función, lo metemos en nuestra openList, para visitarlo cuando corresponda.
                    // OJO: aquí es con el InsertBestFS paraque lo acomode según su valor de Distance (su prioridad).
                    openList.InsertDjikstra(neighbor);
                }
            }
        }

        // Por defecto decimos que no hubo camino.
        return false;
    }

    public bool BestFirstSearch(Node startNode, Node goalNode)
    {
        // Ponemos el valor especial al startNode para identificarlo como la raíz del árbol de nuestro pathfinding.
        startNode.Parent = null;

        PriorityQueue openList = new PriorityQueue();
        openList.Enqueue(startNode);
        List<Node> closedList = new List<Node>();

        Node currentNode = null;

        // Cuándo terminaba la recursión de la DFS recursiva?
        // La forma 1 era cuando ya encontramos el nodo objetivo.
        // La forma 2 es cuando ya no hay nodos por explorar.
        while (openList.Count > 0)
        {
            currentNode = openList.Dequeue();
            // Cuando sacamos a alguien de la lista abierta, lo metemos inmediatamente en la lista cerrada.
            closedList.Add(currentNode);

            // Checamos si ya llegamos al destino.
            if (HasReachedGoal(currentNode, goalNode))
                return true;

            // Si no hemos llegado todavía, visitamos a los vecinos.
            List<Node> neighbors = FindNeighbors(currentNode);
            foreach (Node neighbor in neighbors)
            {
                // Si tu vecino ya está en la lista abierta o ya está en la lista cerrada, entonces no lo agregues.
                if (openList.Contains(neighbor) || closedList.Contains(neighbor))
                    continue;

                // Si los vamos a meter a la lista, primero les asignamos su valor de la heurística de Distance.
                neighbor.Distance = GetDistance(neighbor, goalNode);

                Debug.Log("La distancia del nodo: " + neighbor.Id + " al objetivo es de: " + neighbor.Distance);

                // Como currentNode lo está metiendo a la lista abierta, entonces le decimos a ese nodo que currentNode es su padre.
                // En mi búsqueda yo llegué a Neighbor desde currentNode.
                neighbor.Parent = currentNode;
                // En vez de hacer la recursión de la función, lo metemos en nuestra openList, para visitarlo cuando corresponda.
                // OJO: aquí es con el InsertBestFS paraque lo acomode según su valor de Distance (su prioridad).
                openList.InsertBestFS(neighbor);
            }
        }

        // Por defecto decimos que no hubo camino.
        return false;
    }


    public bool DepthFirstSearch(Node startNode, Node goalNode)
    {
        // Ponemos el valor especial al startNode para identificarlo como la raíz del árbol de nuestro pathfinding.
        startNode.Parent = null;

        Stack<Node> openList = new Stack<Node>();
        openList.Push(startNode);
        List<Node> closedList = new List<Node>();

        Node currentNode = null;

        // Cuándo terminaba la recursión de la DFS recursiva?
        // La forma 1 era cuando ya encontramos el nodo objetivo.
        // La forma 2 es cuando ya no hay nodos por explorar.
        while (openList.Count > 0)
        {
            currentNode = openList.Pop();
            // Cuando sacamos a alguien de la lista abierta, lo metemos inmediatamente en la lista cerrada.
            closedList.Add(currentNode);

            // Checamos si ya llegamos al destino.
            if (HasReachedGoal(currentNode, goalNode))
                return true;

            // Si no hemos llegado todavía, visitamos a los vecinos.
            List<Node> neighbors = FindNeighbors(currentNode);
            foreach (Node neighbor in neighbors)
            {
                // Si tu vecino ya está en la lista abierta o ya está en la lista cerrada, entonces no lo agregues.
                if (openList.Contains(neighbor) || closedList.Contains(neighbor))
                    continue;

                // Como currentNode lo está metiendo a la lista abierta, entonces le decimos a ese nodo que currentNode es su padre.
                // En mi búsqueda yo llegué a Neighbor desde currentNode.
                neighbor.Parent = currentNode;
                // En vez de hacer la recursión de la función, lo metemos en nuestra openList, para visitarlo cuando corresponda.
                openList.Push(neighbor);
            }
        }

        // Por defecto decimos que no hubo camino.
        return false;
    }

    public bool HasReachedGoal(Node currentNode, Node goalNode)
    {
        // Checamos si ya llegamos al destino.
        if (currentNode == goalNode)
        {
            while (currentNode != null)
            {
                Debug.Log("El nodo: " + currentNode.Id + " fue parte del camino a la meta.");
                currentNode = currentNode.Parent;  // Backtracking
            }

            // Si sí llegamos, nos salimos de la función y regresamos true porque sí hubo un camino de startNode a goalNode.
            return true;
        }

        return false;
    }

    public bool DepthFirstSearchRecursive(Node currentNode, Node goalNode)
    {
        Debug.Log("Nodo actualmente visitado: " + currentNode.Id);
        // Primero checamos si ya llegamos al nodo Objetivo (goalNode)
        if (currentNode == goalNode)
        {
            // Si sí, entonces ya llegamos y sí hay un camino desde el inicio hasta la meta.
            return true;
        }

        // Si no hemos llegado a la meta, tenemos que aplicar DFS a todos los nodos vecinos.
        List<Node> neighbors = FindNeighbors(currentNode);
        bool DFS_Result = false;
        foreach (Node neighbor in neighbors)
        {
            // Si tu vecino No tiene padre, le asignas el nodo actual como su padre y sí aplicas el algoritmo otra vez a partir de él.
            if (neighbor.Parent == null) // entonces no tiene padre
            {
                neighbor.Parent = currentNode;
                DFS_Result = DepthFirstSearchRecursive(neighbor, goalNode);
            }


            if (DFS_Result)
            {
                Debug.Log("El nodo: " + currentNode.Id + " fue parte del camino a la meta.");
                // Entonces mi vecino/hijo sí encontró un camino hacia la meta, y por lo tanto yo también.
                return true;
            }
        }

        // Aquí es: ninguno de mis vecinos logró encontrar el camino, y por lo tanto yo tampoco, entonces regreso false.
        return false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
