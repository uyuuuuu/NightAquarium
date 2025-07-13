using UnityEngine;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public class GPUBoidManager : MonoBehaviour
{
    // 追加部分
    public struct BoidData
    {
        public Vector3 position;
        public Vector3 velocity;
    }
    /////

    public GPUBoid boidPrefab; // GPUBoidのプレハブ
    int boidCount = 10000; // boidの数
    public List<GPUBoid> boids = new List<GPUBoid>(); //GPUBoidのオブジェクト保持用

    public ComputeShader computeShader;

    // 追加部分
    int kernelIndex;
    BoidData[] boidArray; // gpuに渡す用
    ComputeBuffer boidBuffer;
    /////

    void Start()
    {
        boidArray = new BoidData[boidCount];
        // プレハブからゲームオブジェクトの生成
        for (int i = 0; i < boidCount; i++)
        {
            Vector3 position = Random.insideUnitSphere * 40;
            GPUBoid boid = Instantiate(boidPrefab, position, Random.rotation);
            boids.Add(boid);

            boidArray[i] = new BoidData(){position = position, velocity = transform.forward * 2.0f};
        }
    }

    void Update()
    {
        kernelIndex = computeShader.FindKernel("UpdateBoid");

        // 位置と速度取得
        for (int i = 0; i < boidCount; i++)
        {
            boidArray[i].position = boids[i].transform.position;
            boidArray[i].velocity = boids[i].Velocity;
        }
        // バッファにセット
        boidBuffer = new ComputeBuffer(boidCount, Marshal.SizeOf(typeof(BoidData)));
        boidBuffer.SetData(boidArray);

        computeShader.SetBuffer(kernelIndex, "_BoidBuffer", boidBuffer);
        computeShader.SetInt("_BoidCount", boidCount);

        computeShader.Dispatch(kernelIndex, boidCount / 100, 1, 1);

        // 計算結果を反映
        boidBuffer.GetData(boidArray);
        for (int i = 0; i < boidCount; i++)
        {
            boids[i].Acceleration = boidArray[i].velocity - boids[i].Velocity;
        }

        boidBuffer.Release();
    }

    private void OnDestroy()
    {
        boidBuffer.Dispose();
    }
}
