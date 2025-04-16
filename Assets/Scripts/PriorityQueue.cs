using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PriorityQueue<T> where T : Pathfinding.Node
{
    private readonly List<T> nodes = new();

    public int Count => nodes.Count;

    public void Enqueue(T node)
    {
        nodes.Add(node);
        nodes.Sort((a, b) => a.FCost.CompareTo(b.FCost));
    }

    public T Dequeue()
    {
        T first = nodes[0];
        nodes.RemoveAt(0);
        return first;
    }

    public bool Contains(T node) => nodes.Contains(node);
}
