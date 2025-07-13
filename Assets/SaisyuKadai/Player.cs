using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Movement")]
    public float normalSpeed = 5.0f;
    public float maxSpeed = 15.0f;
    public float acceleration = 5.0f;
    private float currentSpeed;

    [Header("Camera")]
    public float sensitivity = 2.0f;
    private float rotationX = 0.0f;

    void Start()
    {
        // カーソルをロックして非表示にする
        Cursor.lockState = CursorLockMode.Locked;
        currentSpeed = normalSpeed;
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity;

        transform.Rotate(0, mouseX, 0);

        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -90f, 90f);

        transform.localRotation = Quaternion.Euler(rotationX, transform.localEulerAngles.y, 0);

        if (Input.GetKey(KeyCode.LeftShift))
        {
            // Shiftキーが押されている間、徐々に加速
            currentSpeed = Mathf.MoveTowards(currentSpeed, maxSpeed, acceleration * Time.deltaTime);
        }
        else
        {
            // Shiftキーが離されたら、通常速度に戻る
            currentSpeed = Mathf.MoveTowards(currentSpeed, normalSpeed, acceleration * Time.deltaTime);
        }

        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");
        
        float moveY = 0;
        if (Input.GetKey(KeyCode.Space)) // 上昇
        {
            moveY = 1;
        }
        else if (Input.GetKey(KeyCode.LeftControl)) // 下降
        {
            moveY = -1;
        }

        Vector3 move = transform.right * moveX + transform.forward * moveZ + transform.up * moveY;

        transform.position += move.normalized * currentSpeed * Time.deltaTime;

        // 中央からの距離
        float distance = Vector3.Distance(transform.position, Vector3.zero);
        if (distance > 100.0f)
        {
            Vector3 fromOrigin = transform.position - Vector3.zero;
            fromOrigin = fromOrigin.normalized * 50.0f;
            transform.position = fromOrigin;
        }
    }
}
