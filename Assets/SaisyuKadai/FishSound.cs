using UnityEngine;

public class FishSound : MonoBehaviour
{
    private AudioSource audioSource;

    private static bool canPlaySoundGlobally = true;

    private static int overlappingFishCount = 0;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.playOnAwake = false;
        
        // SphereCollider追加してトリガー設定
        SphereCollider sphereCollider = gameObject.AddComponent<SphereCollider>();
        sphereCollider.isTrigger = true;
        sphereCollider.radius = 1.0f; // SEが鳴る範囲
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            overlappingFishCount++;
            // プレイヤーがトリガーに入り、かつSE再生が可能な場合
            if (canPlaySoundGlobally)
            {
                audioSource.Play();
                canPlaySoundGlobally = false; // 一度再生したら無効化
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            overlappingFishCount--;
            // プレイヤーが全ての魚のトリガーから出た場合
            if (overlappingFishCount <= 0)
            {
                overlappingFishCount = 0; // 念のため0にリセット
                canPlaySoundGlobally = true; // 再び再生可能にする
            }
        }
    }
}
