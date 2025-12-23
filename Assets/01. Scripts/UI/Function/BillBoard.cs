using UnityEngine;

public class BillBoard : MonoBehaviour
{
    Camera mainCam;
    void Awake()
    {
        mainCam = Camera.main;
    }
    void LateUpdate()
    {
        transform.LookAt(transform.position
            + mainCam.transform.rotation * Vector3.forward,
            mainCam.transform.rotation * Vector3.up);
    }
}
