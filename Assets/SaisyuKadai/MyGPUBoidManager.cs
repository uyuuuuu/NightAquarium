using UnityEngine;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public class MyGPUBoidManager : MonoBehaviour
{
    public enum BoidType { Fish, Jellyfish }

    [System.Serializable]
    public class BoidSettings
    {
        public BoidType type;
        public ComputeShader computeShader;
        public MyGPUBoid boidPrefab;
        public Material boidMaterial;
        public int boidCount;
        public float alignmentOmomi = 1.0f;
        public float cohesionOmomi = 1.5f;
        public float separationOmomi = 1.0f;
        public float maxSpeed = 5.0f;
        public float maxForce = 0.05f;
    }

    public struct BoidDataGPU
    {
        public Vector3 position;
        public Vector3 velocity;
        public int typeId;
    }

    private struct BoidSettingsGPU
    {
        public float alignmentOmomi;
        public float cohesionOmomi;
        public float separationOmomi;
        public float maxSpeed;
        public float maxForce;
    }

    public List<BoidSettings> boidSettingsList = new List<BoidSettings>();

    private List<MyGPUBoid> allBoids = new List<MyGPUBoid>();
    // GPU関連
    private ComputeBuffer allBoidsBuffer;
    private BoidDataGPU[] allBoidsArray;

    // GPUに送るための変数セット
    private class BoidGroup
    {
        public List<MyGPUBoid> boids = new List<MyGPUBoid>();
        public BoidDataGPU[] boidArray;
        public ComputeBuffer boidBuffer;
        public BoidSettings settings;
        public int kernelIndex;
    }
    private List<BoidGroup> boidGroups = new List<BoidGroup>();
    private ComputeBuffer settingsBuffer;

    void Start()
    {
        int totalBoidCount = 0;
        foreach (var setting in boidSettingsList)
        {
            totalBoidCount += setting.boidCount;
        }
        allBoids.Capacity = totalBoidCount;
        allBoidsArray = new BoidDataGPU[totalBoidCount];

        for (int typeId = 0; typeId < boidSettingsList.Count; typeId++)
        {
            var settings = boidSettingsList[typeId];

            var group = new BoidGroup
            {
                settings = settings,
                boidArray = new BoidDataGPU[settings.boidCount],
            };
            group.boids.Capacity = settings.boidCount;

            for (int i = 0; i < settings.boidCount; i++)
            {
                Vector3 position = Random.insideUnitSphere * 40;
                MyGPUBoid boid = Instantiate(settings.boidPrefab, position, Random.rotation);
                boid.typeId = typeId;
                boid.BoidType = settings.type; // Boidの種類設定

                group.boids.Add(boid);
                allBoids.Add(boid);

                group.boidArray[i] = new BoidDataGPU { position = position, velocity = transform.forward * 2.0f, typeId = typeId };
            }

            group.boidBuffer = new ComputeBuffer(settings.boidCount, Marshal.SizeOf(typeof(BoidDataGPU)));
            string kernelName = settings.type == BoidType.Jellyfish ? "UpdateKurageBoid" : "UpdateMyBoid";
            group.kernelIndex = settings.computeShader.FindKernel(kernelName);
            boidGroups.Add(group);
        }

        allBoidsBuffer = new ComputeBuffer(totalBoidCount, Marshal.SizeOf(typeof(BoidDataGPU)));

        var settingsGPUArray = new BoidSettingsGPU[boidSettingsList.Count];
        for (int i = 0; i < boidSettingsList.Count; i++)
        {
            settingsGPUArray[i] = new BoidSettingsGPU
            {
                alignmentOmomi = boidSettingsList[i].alignmentOmomi,
                cohesionOmomi = boidSettingsList[i].cohesionOmomi,
                separationOmomi = boidSettingsList[i].separationOmomi,
                maxSpeed = boidSettingsList[i].maxSpeed,
                maxForce = boidSettingsList[i].maxForce,
            };
        }
        settingsBuffer = new ComputeBuffer(boidSettingsList.Count, Marshal.SizeOf(typeof(BoidSettingsGPU)));
        settingsBuffer.SetData(settingsGPUArray);
    }

    void Update()
    {
        for (int i = 0; i < allBoids.Count; i++)
        {
            allBoidsArray[i].position = allBoids[i].transform.position;
            allBoidsArray[i].velocity = allBoids[i].Velocity;
            allBoidsArray[i].typeId = allBoids[i].typeId;
        }
        if(allBoidsBuffer != null)
        {
            allBoidsBuffer.SetData(allBoidsArray);
        }

        foreach (var group in boidGroups)
        {
            for (int i = 0; i < group.boids.Count; i++)
            {
                group.boidArray[i].position = group.boids[i].transform.position;
                group.boidArray[i].velocity = group.boids[i].Velocity;
                group.boidArray[i].typeId = group.boids[i].typeId;
            }
            group.boidBuffer.SetData(group.boidArray);

            var shader = group.settings.computeShader;

            shader.SetBuffer(group.kernelIndex, "_BoidsBufferRead", allBoidsBuffer);
            shader.SetBuffer(group.kernelIndex, "_BoidsBufferWrite", group.boidBuffer);
            shader.SetBuffer(group.kernelIndex, "_SettingsBuffer", settingsBuffer);
            shader.SetInt("_BoidCountAll", allBoids.Count);
            shader.SetInt("_BoidCountGroup", group.boids.Count);
            shader.SetFloat("Time", Time.time);

            shader.Dispatch(group.kernelIndex, (group.boids.Count + 63) / 64, 1, 1);

            group.boidBuffer.GetData(group.boidArray);
            for (int i = 0; i < group.boids.Count; i++)
            {
                group.boids[i].Acceleration = group.boidArray[i].velocity - group.boids[i].Velocity;
            }
        }
    }

    private void OnDestroy()
    {
        allBoidsBuffer?.Dispose();
        settingsBuffer?.Dispose();
        foreach (var group in boidGroups)
        {
            group.boidBuffer?.Dispose();
        }
    }
}
