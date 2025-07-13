using UnityEngine;

public class Particle : MonoBehaviour
{
    public Vector3 Position;

    void Start()
    {
        Position = Vector3.zero;
    }

    void Update()
    {
        transform.position = Position;
    }
}
