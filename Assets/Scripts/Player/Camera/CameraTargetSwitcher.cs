using UnityEngine;

public class CameraTargetSwitcher : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform orbitPivot;

    [Header("Mouse Orbit")]
    [SerializeField] private float horizontalSensitivity = 180f;
    [SerializeField] private float verticalSensitivity = 120f;

    [SerializeField] private float minimumPitch = -30f;
    [SerializeField] private float maximumPitch = 70f;

    [Header("Cursor")]
    [SerializeField] private bool lockCursor = true;

    private Transform currentTarget;

    private float yaw;
    private float pitch;

    public Transform CurrentTarget => currentTarget;

    private void Awake()
    {
        if (orbitPivot == null)
        {
            Debug.LogError(
                "CameraTargetSwitcher necesita un Orbit Pivot.",
                this
            );

            enabled = false;
            return;
        }

        yaw = orbitPivot.eulerAngles.y;
        pitch = NormalizeAngle(orbitPivot.eulerAngles.x);

        if (lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    private void Update()
    {
        if (currentTarget == null)
            return;

        ReadMouseInput();
    }

    private void LateUpdate()
    {
        if (currentTarget == null)
            return;

        UpdatePivot();
    }

    private void ReadMouseInput()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        yaw += mouseX *
               horizontalSensitivity *
               Time.deltaTime;

        pitch -= mouseY *
                 verticalSensitivity *
                 Time.deltaTime;

        pitch = Mathf.Clamp(
            pitch,
            minimumPitch,
            maximumPitch
        );
    }

    private void UpdatePivot()
    {
        orbitPivot.position = currentTarget.position;

        orbitPivot.rotation = Quaternion.Euler(
            pitch,
            yaw,
            0f
        );
    }

    public void SetTarget(Transform newTarget)
    {
        if (newTarget == null)
        {
            Debug.LogWarning(
                "Se intentó asignar un CameraTarget nulo.",
                this
            );

            return;
        }

        currentTarget = newTarget;
    }

    private float NormalizeAngle(float angle)
    {
        if (angle > 180f)
            angle -= 360f;

        return angle;
    }
}