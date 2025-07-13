using UnityEngine;
using System.Runtime.InteropServices;
using System.Collections.Generic;


public class ComputeStruct : MonoBehaviour
{
    public struct ParticleData
    {
        public Vector3 position;
        public Vector3 dir;
    }

    ParticleData[] particles;
    int particleCount = 10000;

    public Particle particlePrefab;

    int kernelIndex;
    public ComputeShader computeShader;
    ComputeBuffer buffer;

    List<Particle> particleList;

    void Start()
    {
        particles = new ParticleData[particleCount];
        particleList = new List<Particle>();
        for (int i = 0; i < particles.Length; ++i)
        {
            particles[i] = new ParticleData()
            {
                position = new Vector3(0, 0, 0),
                dir = Random.onUnitSphere
            };
            Particle obj = Instantiate(particlePrefab, particles[i].position, Random.rotation);
            particleList.Add(obj);
        }

        kernelIndex = computeShader.FindKernel("UpdateParticle");
        buffer = new ComputeBuffer(particleCount, Marshal.SizeOf(typeof(ParticleData)));

        buffer.SetData(particles);
        computeShader.SetBuffer(kernelIndex, "_buffer", buffer);
    }

    void Update()
    {
        computeShader.SetFloat("_DeltaTime", Time.deltaTime);
        computeShader.Dispatch(kernelIndex, particleCount / 100, 1, 1);
        buffer.GetData(particles);

        for (int i = 0; i < particles.Length; ++i)
        {
            particleList[i].Position = particles[i].position;
        }
    }

    private void OnDestroy()
    {
        buffer.Dispose();
    }
}
