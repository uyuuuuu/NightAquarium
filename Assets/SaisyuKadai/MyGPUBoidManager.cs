using UnityEngine;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public class MyGPUBoidManager : MonoBehaviour
{
    // Boidの種類設定
    [System.Serializable]
    public class BoidSettings
    {
        public MyGPUBoid boidPrefab;
        public Material boidMaterial;
        public int boidCount;
        public float alignmentOmomi = 1.0f;
        public float cohesionOmomi = 1.5f;
        public float separationOmomi = 1.0f;
        public float maxSpeed = 5.0f;
        public float maxForce = 0.05f;
    }

    // GPUに渡すBoid自体のデータ
    public struct BoidDataGPU
    {
        public Vector3 position;
        public Vector3 velocity;
        public int typeId;
    }

    // GPUに渡す種類ごとの設定データ
    private struct BoidSettingsGPU
    {
        public float alignmentOmomi;
        public float cohesionOmomi;
        public float separationOmomi;
        public float maxSpeed;
        public float maxForce;
    }

    // 種類の設定一覧
    public List<BoidSettings> boidSettingsList = new List<BoidSettings>();
    // 全てのBoid
    public List<MyGPUBoid> boids = new List<MyGPUBoid>();

    // GPU関連
    public ComputeShader computeShader;

    private int kernelIndex;
    private BoidDataGPU[] boidArray;
    private ComputeBuffer boidBuffer;
    private ComputeBuffer settingsBuffer;

    void Start()
    {
        int totalBoidCount = 0;
        foreach (var settings in boidSettingsList)
        {
            totalBoidCount += settings.boidCount;
        }
        boidArray = new BoidDataGPU[totalBoidCount];
        boids.Capacity = totalBoidCount;

        int boidIndex = 0;
        for (int typeId = 0; typeId < boidSettingsList.Count; typeId++)
        {
            var settings = boidSettingsList[typeId];
            for (int i = 0; i < settings.boidCount; i++)
            {
                Vector3 position = Random.insideUnitSphere * 40;
                MyGPUBoid boid = Instantiate(settings.boidPrefab, position, Random.rotation);
                boid.typeId = typeId;
                // マテリアル置換
                foreach (MeshRenderer mr in boid.GetComponentsInChildren<MeshRenderer>())
                {
                    mr.material = settings.boidMaterial;
                }
                boids.Add(boid);

                // GPU用
                boidArray[boidIndex] = new BoidDataGPU()
                {
                    position = position,
                    velocity = transform.forward * 2.0f,
                    typeId = typeId
                };
                boidIndex++;
            }
        }

        // GPUに渡す設定データ作成
        boidBuffer = new ComputeBuffer(totalBoidCount, Marshal.SizeOf(typeof(BoidDataGPU)));
        
        var settingsGPUArray = new BoidSettingsGPU[boidSettingsList.Count];
        for(int i = 0; i < boidSettingsList.Count; i++)
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
        if (boids.Count == 0) return;

        kernelIndex = computeShader.FindKernel("UpdateMyBoid");

        for (int i = 0; i < boids.Count; i++)
        {
            boidArray[i].position = boids[i].transform.position;
            boidArray[i].velocity = boids[i].Velocity;
        }
        boidBuffer.SetData(boidArray);

        computeShader.SetBuffer(kernelIndex, "_BoidBuffer", boidBuffer);
        computeShader.SetBuffer(kernelIndex, "_SettingsBuffer", settingsBuffer);
        computeShader.SetInt("_BoidCount", boids.Count);

        computeShader.Dispatch(kernelIndex, boids.Count / 100, 1, 1);

        boidBuffer.GetData(boidArray);
        for (int i = 0; i < boids.Count; i++)
        {
            boids[i].Acceleration = boidArray[i].velocity - boids[i].Velocity;
        }

        // boidBuffer.Release();
    }

    private void OnDestroy()
    {
        boidBuffer.Dispose();
        settingsBuffer.Dispose();
    }
}
