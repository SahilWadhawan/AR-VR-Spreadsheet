using UnityEngine;

public class FirstPersonController : MonoBehaviour
{
    public float speed = 5f;
    public float lookSpeed = 2f;
    private float verticalRotation = 0f;

    void Update()
    {
        // Movement
        float moveDirectionY = Input.GetAxis("Vertical") * speed * Time.deltaTime;
        float moveDirectionX = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
        transform.Translate(moveDirectionX, 0, moveDirectionY);

        // Mouse look
        float mouseY = Input.GetAxis("Mouse Y") * lookSpeed;
        float mouseX = Input.GetAxis("Mouse X") * lookSpeed;
        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -90f, 90f);
        transform.localRotation = Quaternion.Euler(verticalRotation, 0, 0);
        transform.parent.Rotate(Vector3.up * mouseX);
    }
}
