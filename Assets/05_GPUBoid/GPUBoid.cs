using UnityEngine;

public class GPUBoid : MonoBehaviour
{
    public Vector3 Velocity; // 速度
    public Vector3 Acceleration; // boidの計算によって得られた操舵力
    float borderLength = 100.0f; // シミュレーション領域の1辺の長さ

    float maxSpeed = 5;

    void Start()
    {
        Acceleration = Vector3.zero;
        Velocity = transform.forward * 2.0f;
    }

    void Update()
    {
        Velocity += Acceleration;
        Velocity = Vector3.ClampMagnitude(Velocity, maxSpeed);

        transform.position += Velocity * Time.deltaTime * 2.0f;
        transform.rotation = Quaternion.LookRotation(Velocity);

        border(); // 境界処理
    }

    // 境界でループするように処理する
    private void border()
    {
        if (this.transform.position.x < -borderLength / 2.0f)
        {
            this.transform.position = new Vector3(borderLength / 2.0f, this.transform.position.y, this.transform.position.z);
        }
        if (this.transform.position.y < -borderLength / 2.0f)
        {
            this.transform.position = new Vector3(this.transform.position.x, borderLength / 2.0f, this.transform.position.z);
        }
        if (this.transform.position.z < -borderLength / 2.0f)
        {
            this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, borderLength / 2.0f);
        }

        if (this.transform.position.x > borderLength / 2.0f)
        {
            this.transform.position = new Vector3(-borderLength / 2.0f, this.transform.position.y, this.transform.position.z);
        }
        if (this.transform.position.y > borderLength / 2.0f)
        {
            this.transform.position = new Vector3(this.transform.position.x, -borderLength / 2.0f, this.transform.position.z);
        }
        if (this.transform.position.z > borderLength / 2.0f)
        {
            this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, -borderLength / 2.0f);
        }

    }
}
