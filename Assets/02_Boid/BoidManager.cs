using UnityEngine;
using System.Collections.Generic;

public class BoidManager : MonoBehaviour
{
    public Boid boidPrefab;
    public int boidCount = 150;
    public List<Boid> boids = new List<Boid>();

    void Start()
    {
        // boidの生成
        for (int i = 0; i < boidCount; i++)
        {
            // 初期位置は半径25の球内にランダム
            Vector3 position = Random.insideUnitSphere * 25;
            Boid boid = Instantiate(boidPrefab, position, Random.rotation);
            boids.Add(boid);
        }
    }

    void Update()
    {
        
    }
}
