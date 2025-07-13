using UnityEngine;

public class ComputeAdd : MonoBehaviour
{
    public ComputeShader computeShader;
    ComputeBuffer computeBuffer;

    void Start()
    {
        int kernelIndex = computeShader.FindKernel("KernelAdd");

        computeBuffer = new ComputeBuffer(4, sizeof(int));
        int[] input = new int[4];

        for (int i = 0; i < input.Length; ++i)
        {
            input[i] = i;
        }
        computeBuffer.SetData(input);

        computeShader.SetBuffer(kernelIndex, "_Buffer", computeBuffer);
        computeShader.SetInt("_AddValue", 1);
        computeShader.Dispatch(kernelIndex, 1, 1, 1);

        int[] result = new int[4];

        computeBuffer.GetData(result);

        for (int i = 0; i < result.Length; ++i)
        {
            Debug.Log(result[i]);
        }

        computeBuffer.Release();
    }
}
