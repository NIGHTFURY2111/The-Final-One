using DG.Tweening;
using UnityEngine;

[CreateAssetMenu(fileName = "Camera Tilt", menuName = "Scriptable Object/Camera Effects/Camera Tilt")]
public class CameraTilt : ScriptableObject
{
    //tiliting the camera object using DoTween based on the player's forward as compared to the provided normal
    [Range(1f, 10f), SerializeField, Tooltip("Tilt Smoothing")]
    private float tiltSmoothing = 5f;
    [Range(1f, 30f), SerializeField, Tooltip("Maximum Tilt Angle")]
    private float maxTiltAngle = 15f;

    public GameObject camera { get; private set; }

    public void initialize(GameObject cam)
    {
        camera = cam;
    }

    public void calculateTilt(Vector3 normal, Vector3 forward, Vector3 right)
    {
        if (camera == null) return;

        float side =  Vector3.Dot(right, normal)>0? 1: -1;
        // side < 0 → wall on left, side > 0 → wall on right

        float alignment = 1f - Mathf.Abs(Vector3.Dot(forward, normal));


        tiltCamera(side * alignment);
    }

    public void resetTilt()
    {
        if (camera == null) return;
        camera.transform.DOLocalRotateQuaternion(Quaternion.identity, 1f / tiltSmoothing).SetEase(Ease.OutSine);
    }

    public void tiltCamera(float amount)
    {// tilts the local rotation of the camera gameobject using DOTWEEN, amount is between -1 and 1, multiplyed by the maxtilt value

        if (camera == null) return;
        float tiltAngle = Mathf.Clamp(amount, -1f, 1f) * maxTiltAngle;
        Quaternion targetRotation = Quaternion.Euler(0f, 0f, tiltAngle);
        camera.transform.DOLocalRotateQuaternion(targetRotation, 1f / tiltSmoothing).SetEase(Ease.OutSine);
    }

}