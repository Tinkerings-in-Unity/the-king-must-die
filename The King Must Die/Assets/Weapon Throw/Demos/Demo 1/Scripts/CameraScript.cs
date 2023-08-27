using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public float SpeedH = 2f;
    public float SpeedV = 2f;

    private float yaw = 0f;
    private float pitch = 0f;
    private float minPitch = -10f;
    private float maxPitch = 10f;

    private float unique;
    private float unique2;


    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void LateUpdate()
    {
        CameraRotate();
    }

    void CameraRotate()
    {
        yaw += Input.GetAxis("Mouse X") * SpeedH;
        pitch -= Input.GetAxis("Mouse Y") * SpeedV;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
        transform.eulerAngles = new Vector3(pitch, yaw, 0f);
    }

}
