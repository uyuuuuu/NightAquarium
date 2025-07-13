using UnityEngine;

public class ComputeDispatchThreadID : MonoBehaviour
{
    public ComputeShader computeShader;
    ComputeBuffer computeBuffer;

    void Start()
    {
        int kernelIndex = computeShader.FindKernel("KernelDispatchThreadID");

        computeBuffer = new ComputeBuffer(8, sizeof(int));
        int[] input = new int[8];

        for (int i = 0; i < input.Length; ++i)
        {
            input[i] = 0;
        }
        computeBuffer.SetData(input);

        computeShader.SetBuffer(kernelIndex, "_Buffer", computeBuffer);
        computeShader.Dispatch(kernelIndex, 2, 1, 1);

        int[] result = new int[8];

        computeBuffer.GetData(result);

        for (int i = 0; i < result.Length; ++i)
        {
            Debug.Log(result[i]);
        }

        computeBuffer.Release();
    }
}
