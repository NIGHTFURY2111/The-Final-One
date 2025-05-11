using UnityEngine;

[CreateAssetMenu(fileName = "Player Camera", menuName = "Scriptable Object/Component/Player Camera")]
public class EC_Camera : AC_Component
{
    float rotationX = 0f;
    [SerializeField]float lookXLimit = 45f;
    public override ComponentType componentType => ComponentType.Camera;

    public Camera camera { get; private set; }

    public override void ComponentAwake()
    {
        camera = Camera.main;
    }

    public override void ComponentStart() { }

    public override void ComponentUpdate() { }

    public void UpdateCameraTransform(Vector2 rotation)
    {

        Debug.Log(rotation);
        rotationX += -rotation.y;

        rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);

        camera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
    }
}
