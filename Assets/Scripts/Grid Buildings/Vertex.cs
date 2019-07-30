using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vertex
{
    public int chunkIndex;
    public int index;
    public Vector3 position;

    public Vertex(int chunkIndex, int index, Vector3 position) {
        this.chunkIndex = chunkIndex;
        this.index = index;
        this.position = position;
    }
}
