using Cinemachine;
using UnityEngine;
[RequireComponent(typeof(CinemachineFreeLook))]
public class MouseCameraControl : MonoBehaviour
{

    private CinemachineFreeLook freeLookCam;
    // Use this for initialization
    void Start()
    {
        freeLookCam = GetComponent<CinemachineFreeLook>();

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetJoystickNames().Length < 2)
        {
            freeLookCam.m_XAxis.Value += Input.GetAxis("Mouse X") * 10.0f;
            freeLookCam.m_YAxis.Value += Input.GetAxis("Mouse Y") * 0.05f;
        }
        else
        {
            freeLookCam.m_XAxis.Value = Input.GetAxis("RightJoystickX_1");
            freeLookCam.m_YAxis.Value = Input.GetAxis("RightJoystickY_1");
        }
    }
}