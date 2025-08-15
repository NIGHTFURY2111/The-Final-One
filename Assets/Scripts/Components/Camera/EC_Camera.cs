using UnityEngine;

[CreateAssetMenu(fileName = "Player Camera", menuName = "Scriptable Object/Component/Player Camera")]
public class EC_Camera : AC_Component
{
    float rotationX = 0f;
    [SerializeField]float lookXLimit = 45f;
    public override Enum_ComponentType componentType => Enum_ComponentType.Camera;

    public Camera camera { get; private set; }

    public override void ComponentAwake()
    {
        camera = Camera.main;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public override void ComponentStart() { }

    public override void ComponentUpdate() { }

    public void UpdateCameraTransform(Vector2 rotation)
    {

        rotationX += -rotation.y;

        rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);

        camera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
    }
}
