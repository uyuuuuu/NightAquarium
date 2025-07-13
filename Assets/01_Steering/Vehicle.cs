using System;
using UnityEngine;

public class Vehicle : MonoBehaviour
{
    Vector3 velocity;

    public float maxForce = 0.1f; // 最大力
    public float maxSpeed = 4.0f; // 最高速度

    public GameObject target;

    void Start()
    {
        velocity = new Vector3(0, 0, 0);
    }

    void Update()
    {
        // velocity = target.transform.position - this.transform.position;
        // velocity = velocity.normalized;
        // 操舵力実装の際は、上記の移動のプログラム（直接ターゲットに向かう）はコメントアウトすること
        // 操舵力を実装する
        Vector3 desired = target.transform.position - this.transform.position;
        // スケーリング
        desired = desired.normalized * maxSpeed;

        Vector3 steer = desired - velocity;
        steer = Vector3.ClampMagnitude(steer, maxForce);

        velocity += steer;
        velocity = Vector3.ClampMagnitude(velocity, maxSpeed);
        // 速度の適用
        transform.position += velocity * Time.deltaTime * 5.0f;

        // 速度が0だとエラーになる
        if (velocity.magnitude > 0.01f)
        {
            // 速度方向に向きを変える
            transform.rotation = Quaternion.LookRotation(velocity);
        }
    }
}
