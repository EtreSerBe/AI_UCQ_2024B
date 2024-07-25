using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


// https://stackoverflow.com/questions/70568157/cant-use-c-sharp-net-6-priorityqueue-in-unity
// Tuvimos que hacer nuestra propia PriorityQueue porque Unity no tiene tal cual la de C#
public class PriorityQueue
{
    private List<Node> nodes = new List<Node>();

    public List<Node> Nodes
    {
        get { return nodes; }
    }

    public int Count
    {
        get { return nodes.Count; }
    }

    // Add/Enqueue lo mete hasta el final
    public void Enqueue(Node in_node)
    {
        nodes.Add(in_node);
    }


    public void InsertBestFS(Node in_node)
    {
        // Idealmente, habría que medir el tiempo promedio/amortizado de la ejecución con los ifs juntos o separados.
        // Inserta a in_node en la posición de la lista donde haya algún elemento con prioridad mayor
        for (int i = 0; i < nodes.Count; i++)
        {
            // Si encontramos un nodo 'i' que tiene una menor prioridad que nuestro nodo a insertar in_node, metemos a 
            // in_node antes que a dicho nodo 'i'.
            if (nodes[i].Distance > in_node.Distance)
            {
                nodes.Insert(i, in_node);
                return;
            }
        }
        // Si nunca encontró a alguien con mayor costo que él, entonces in_node es el de mayor costo
        // y debe ir hasta atrás de la lista de prioridad.
        Enqueue(in_node);
    }

    public void InsertDjikstra(Node in_node)
    {
        // Idealmente, habría que medir el tiempo promedio/amortizado de la ejecución con los ifs juntos o separados.
        // Inserta a in_node en la posición de la lista donde haya algún elemento con prioridad mayor
        for (int i = 0; i < nodes.Count; i++)
        {
            // Si encontramos un nodo 'i' que tiene una menor prioridad que nuestro nodo a insertar in_node, metemos a 
            // in_node antes que a dicho nodo 'i'.
            if (nodes[i].TotalCost > in_node.TotalCost)
            {
                nodes.Insert(i, in_node);
                return;
            }
        }
        // Si nunca encontró a alguien con mayor costo que él, entonces in_node es el de mayor costo
        // y debe ir hasta atrás de la lista de prioridad.
        Enqueue(in_node);
    }

    // Menor prioridad significa más importante.
    //public void InsertAStar(Node in_node)
    //{
    //    // Idealmente, habría que medir el tiempo promedio/amortizado de la ejecución con los ifs juntos o separados.
    //    // Inserta a in_node en la posición de la lista donde haya algún elemento con prioridad mayor
    //    for (int i = 0; i < nodes.Count; i++)
    //    {
    //        if (nodes[i].f_Cost > in_node.f_Cost)
    //        {
    //            nodes.Insert(i, in_node);
    //            return;
    //        }
    //        else if (nodes[i].f_Cost == in_node.f_Cost && nodes[i].h_Cost > in_node.h_Cost)
    //        {
    //            // Este es el caso en que tienen el mismo f_cost pero el nodo a insertar tiene menor h_cost.
    //            // https://youtu.be/i0x5fj4PqP4?t=194
    //            nodes.Insert(i, in_node);
    //            return;
    //        }
    //    }
    //    // Si nunca encontró a alguien con mayor costo que él, entonces in_node es el de mayor costo
    //    // y debe ir hasta atrás de la lista de prioridad.
    //    nodes.Add(in_node);
    //}



    public void Remove(Node in_node)
    {
        nodes.Remove(in_node);
    }

    public Node Dequeue()
    {
        Node out_node = nodes[0];
        nodes.RemoveAt(0);
        return out_node;
    }

    //public void RemoveAt(int in_index)
    //{ 
    //    nodes.RemoveAt(in_index);
    //}

    public bool Contains(Node in_node)
    {
        return nodes.Contains(in_node);
    }

}
