using UnityEngine;

public class MyGPUBoid : MonoBehaviour
{
    public Vector3 Velocity; // 速度
    public Vector3 Acceleration; // boidの計算によって得られた操舵力
    public int typeId;
    public MyGPUBoidManager.BoidType BoidType { get; set; } // Boidの種類

    float borderLengthX = 80.0f; // シミュレーション領域の横の長さ
    float borderLengthY = 160.0f; // 高さ

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

        if (BoidType == MyGPUBoidManager.BoidType.Jellyfish)
        {
            // くらげなら上向き
            transform.rotation = Quaternion.identity * Quaternion.Euler(-90, 0, 0);
        }
        else
        {
            // 魚なら進行方向
            transform.rotation = Quaternion.LookRotation(Velocity) * Quaternion.Euler(0, -90, 0);
            // if (Velocity.sqrMagnitude > 0.001f)
            // {
            //     transform.rotation = Quaternion.LookRotation(Velocity) * Quaternion.Euler(0, -90, 0);
            // }
        }

        border(); // 境界処理
    }

    // 境界でループするように処理する
    private void border()
    {
        if (this.transform.position.x < -borderLengthX / 2.0f)
        {
            this.transform.position = new Vector3(borderLengthX / 2.0f, this.transform.position.y, this.transform.position.z);
        }
        if (this.transform.position.y < -borderLengthY / 2.0f)
        {
            this.transform.position = new Vector3(this.transform.position.x, borderLengthY / 2.0f, this.transform.position.z);
        }
        if (this.transform.position.z < -borderLengthX / 2.0f)
        {
            this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, borderLengthX / 2.0f);
        }

        if (this.transform.position.x > borderLengthX / 2.0f)
        {
            this.transform.position = new Vector3(-borderLengthX / 2.0f, this.transform.position.y, this.transform.position.z);
        }
        if (this.transform.position.y > borderLengthY / 2.0f)
        {
            this.transform.position = new Vector3(this.transform.position.x, -borderLengthY / 2.0f, this.transform.position.z);
        }
        if (this.transform.position.z > borderLengthX / 2.0f)
        {
            this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, -borderLengthX / 2.0f);
        }

    }
}
