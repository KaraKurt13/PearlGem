using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereElement : MonoBehaviour
{
    public List<SphereElement> Neighbours;

    public Vector3 Center;

    public Rigidbody Rigidbody;

    public MeshRenderer Renderer;

    public void Deactivate()
    {
        Renderer.material.color = Color.red;

        foreach (var neighbour in Neighbours)
            neighbour.Renderer.material.color = Color.green;
    }

    private void OnTriggerEnter(Collider other)
    {
        Deactivate();
    }
}
