using UnityEngine;

public class ComputeGroupThread : MonoBehaviour
{
    public ComputeShader computeShader;
    ComputeBuffer computeBuffer;

    void Start()
    {
        int kernelIndex = computeShader.FindKernel("KernelGroupThread");

        computeBuffer = new ComputeBuffer(12, sizeof(int));
        int[] input = new int[12];

        for (int i = 0; i < input.Length; ++i)
        {
            input[i] = 0;
        }
        computeBuffer.SetData(input);

        computeShader.SetBuffer(kernelIndex, "_Buffer", computeBuffer);
        // 生成するグループ数
        computeShader.Dispatch(kernelIndex, 2, 1, 1); // 0~7
        // computeShader.Dispatch(kernelIndex, 3, 1, 1); // 0~11

        int[] result = new int[12];

        computeBuffer.GetData(result);

        for (int i = 0; i < result.Length; ++i)
        {
            Debug.Log(result[i]);
        }

        computeBuffer.Release();
    }
}
