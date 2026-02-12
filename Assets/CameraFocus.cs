using Cinemachine;
using UnityEngine;

public class CameraFocus : MonoBehaviour
{
    public CinemachineBrain Brain;
    public ICinemachineCamera camA;
    public ICinemachineCamera camB;

    void Start()
    {
        if (Brain == null)
        {
            Brain = FindObjectOfType<CinemachineBrain>();
        }

        if (camA == null)
        {
            camA = GetComponent<ICinemachineCamera>();
        }

        if (camB == null)
        {
            camB = GetComponent<ICinemachineCamera>();
        }

        if (Brain != null)
        {
            //Override Parameters
            int layer = 1;
            // int priority = 1; // Unused
            float weight = 1;
            float blendTime = .5f;
            
            Brain.SetCameraOverride(layer, camA, camB, weight, blendTime);
        }
        else
        {
            Debug.LogError("CinemachineBrain not found or assigned to CameraFocus script on " + gameObject.name);
        }
    }
}
