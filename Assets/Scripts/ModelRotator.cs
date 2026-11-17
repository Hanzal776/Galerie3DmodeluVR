using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRGrabInteractable))]
public class ModelRotator : MonoBehaviour
{
    [Header("Right Controller Actions (A / B)")]
    public InputActionReference rightRotateRightAction; // obvykle B (Secondary)
    public InputActionReference rightRotateLeftAction;  // obvykle A (Primary)

    [Header("Left Controller Actions (X / Y)")]
    public InputActionReference leftRotateRightAction;  // obvykle Y (Secondary)
    public InputActionReference leftRotateLeftAction;   // obvykle X (Primary)

    [Header("Settings")]
    public float rotationSpeed = 90f;

    [Header("Debug")]
    [SerializeField] private bool isHeld = false; // Vidíš v Inspectoru, jestli je model v ruce

    private XRGrabInteractable grabInteractable;

    void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
    }

    void OnEnable()
    {
        grabInteractable.selectEntered.AddListener(OnGrabbed);
        grabInteractable.selectExited.AddListener(OnReleased);

        EnableAction(rightRotateRightAction);
        EnableAction(rightRotateLeftAction);
        EnableAction(leftRotateRightAction);
        EnableAction(leftRotateLeftAction);
    }

    void OnDisable()
    {
        grabInteractable.selectEntered.RemoveListener(OnGrabbed);
        grabInteractable.selectExited.RemoveListener(OnReleased);

        DisableAction(rightRotateRightAction);
        DisableAction(rightRotateLeftAction);
        DisableAction(leftRotateRightAction);
        DisableAction(leftRotateLeftAction);
    }

    private void EnableAction(InputActionReference actionRef)
    {
        if (actionRef != null && actionRef.action != null) actionRef.action.Enable();
    }

    private void DisableAction(InputActionReference actionRef)
    {
        if (actionRef != null && actionRef.action != null) actionRef.action.Disable();
    }

    private void OnGrabbed(SelectEnterEventArgs args)
    {
        isHeld = true;
    }

    private void OnReleased(SelectExitEventArgs args)
    {
        isHeld = false;
    }

    void Update()
{
    if (!isHeld) return;

    float rotationStep = rotationSpeed * Time.deltaTime;

    bool rotateRight = IsPressed(rightRotateRightAction) || IsPressed(leftRotateRightAction);
    bool rotateLeft = IsPressed(rightRotateLeftAction) || IsPressed(leftRotateLeftAction);

    // --- DIAGNOSTIKA TLAČÍTEK ---
    if (rotateRight)
    {
        Debug.Log("Detekován stisk tlačítka pro OTÁČENÍ DOPRAVA (B / Y)");
        transform.Rotate(Vector3.up, -rotationStep, Space.World);
    }

    if (rotateLeft)
    {
        Debug.Log("Detekován stisk tlačítka pro OTÁČENÍ DOLEVA (A / X)");
        transform.Rotate(Vector3.up, rotationStep, Space.World);
    }
}

    private bool IsPressed(InputActionReference actionRef)
    {
        return actionRef != null && actionRef.action != null && actionRef.action.IsPressed();
    }
}