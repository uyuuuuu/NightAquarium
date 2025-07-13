using UnityEngine;
using System.Collections.Generic;

public class Boid : MonoBehaviour
{
    public Vector3 Velocity;
    Vector3 acceleration;

    float maxForce; // 最大力
    float maxSpeed; // 最高速度

    private BoidManager manager;

    float borderLength = 100.0f; // シミュレーション領域の1辺の長さ

    void Start()
    {
        manager = FindFirstObjectByType<BoidManager>();

        acceleration = Vector3.zero;
        Velocity = transform.forward * 2.0f;

        maxSpeed = 5;
        maxForce = 0.05f;
    }

    void Update()
    {
        List<Boid> boids = manager.boids;

        Vector3 sep = separate(boids); // 分離
        Vector3 ali = align(boids); // 整列
        Vector3 coh = cohesion(boids); //結合

        // 操舵力の重み付け
        sep *= 1.0f;
        ali *= 1.5f;
        coh *= 1.0f;

        // 1つ前の操舵力も使うと、より生き物っぽくなるので採用
        acceleration = acceleration + sep + ali + coh;

        Velocity += acceleration;
        Velocity = Vector3.ClampMagnitude(Velocity, maxSpeed);

        transform.position += Velocity * Time.deltaTime * 2.0f;
        transform.rotation = Quaternion.LookRotation(Velocity);

        border(); // 境界処理
    }

    // 分離の計算
    private Vector3 separate(List<Boid> boids)
    {
        float neighborDist = 5.0f; // 近隣の範囲（距離）
        Vector3 sum = Vector3.zero;// 重み付き平均計算用
        int count = 0; // 平均計算用のカウンタ

        // 近隣のboidの位置から自身の位置へと向かうベクトルの重み付き平均を求める
        foreach (Boid item in boids)
        {
            float d = Vector3.Distance(transform.position, item.transform.position);
            if (d <= 0 || neighborDist < d) continue;
            // 重みは距離を使って、距離が遠ければ影響を小さくするようにする
            // すなわち、近隣のboidの位置から自身の位置へと向かうベクトルを正規化したあと、距離で割る
            Vector3 diff = (this.transform.position - item.transform.position).normalized / d;

            sum += diff;
            count++;
        }

        // 近隣にboidがいなければゼロベクトルを戻り値とする
        if (count <= 0) return Vector3.zero;

        // 求めた平均を必要な速度として、操舵力を求めて戻り値とする
        Vector3 desired = sum / count;
        // スケーリング
        desired = desired.normalized * maxSpeed;

        Vector3 steer = desired - Velocity;
        steer = Vector3.ClampMagnitude(steer, maxForce);

        return steer;
    }

    // 整列の計算
    private Vector3 align(List<Boid> boids)
    {
        float neighborDist = 10; // 近隣の範囲（距離）
        Vector3 sum = Vector3.zero; // 平均速度計算用
        int count = 0; // 平均計算用のカウンタ

        // 近隣のboidの向き（=速度）の平均を求める
        foreach (Boid item in boids)
        {
            float d = Vector3.Distance(transform.position, item.transform.position);
            if (d <= 0 || neighborDist < d) continue;
            sum += item.Velocity;
            count++;
        }
        // 近隣にboidがいなければゼロベクトルを戻り値とする
        if (count <= 0) return Vector3.zero;
        // 求めた平均を必要な速度として、操舵力を求めて戻り値とする
        Vector3 desired = sum / count;
        // スケーリング
        desired = desired.normalized * maxSpeed;

        Vector3 steer = desired - Velocity;
        steer = Vector3.ClampMagnitude(steer, maxForce);

        return steer;
    }

    // 結合の計算
    private Vector3 cohesion(List<Boid> boids)
    {
        float neighborDist = 10; // 近隣の範囲（距離）
        Vector3 sum = Vector3.zero; // 中心（平均）計算用
        int count = 0; // 中心（平均）計算用のカウンタ

        // 近隣のboidの位置の中心（平均）を求める
        foreach (Boid item in boids)
        {
            float d = Vector3.Distance(transform.position, item.transform.position);
            if (d <= 0 || neighborDist < d) continue;
            sum += item.transform.position;
            count++;
        }
        // 近隣にboidがいなければゼロベクトルを戻り値とする
        if (count <= 0) return Vector3.zero;
        // 求めた平均を目標（ターゲット）として、操舵力を求めて戻り値とする
        Vector3 desired =  sum / count - this.transform.position;

        // スケーリング
        desired = desired.normalized * maxSpeed;

        Vector3 steer = desired - Velocity;
        steer = Vector3.ClampMagnitude(steer, maxForce);

        return steer;
    }

    // 境界でループするように処理する
    private void border()
    {
        if(this.transform.position.x < -borderLength / 2.0f)
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
