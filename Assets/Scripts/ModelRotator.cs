using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRGrabInteractable))]
public class ModelRotator : MonoBehaviour
{
    [Header("Actions")]
    public InputActionReference rotateRightAction; 
    public InputActionReference rotateLeftAction;  

    [Header("Settings")]
    public float rotationSpeed = 90f;

    [SerializeField] private bool isHeld = false;
    private XRGrabInteractable grabInteractable;

    void Awake() => grabInteractable = GetComponent<XRGrabInteractable>();

    void OnEnable()
    {
        grabInteractable.selectEntered.AddListener(s => isHeld = true);
        grabInteractable.selectExited.AddListener(s => isHeld = false);
        
        if (rotateRightAction?.action != null) rotateRightAction.action.Enable();
        if (rotateLeftAction?.action != null) rotateLeftAction.action.Enable();
    }

    void OnDisable()
    {
        grabInteractable.selectEntered.RemoveListener(s => isHeld = false);
        grabInteractable.selectExited.RemoveListener(s => isHeld = true);

        if (rotateRightAction?.action != null) rotateRightAction.action.Disable();
        if (rotateLeftAction?.action != null) rotateLeftAction.action.Disable();
    }

    void Update()
    {
        if (!isHeld) return;

        float step = rotationSpeed * Time.deltaTime;

        if (rotateRightAction != null && rotateRightAction.action != null && rotateRightAction.action.IsPressed())
        {
            transform.Rotate(Vector3.up, -step, Space.World);
        }

        if (rotateLeftAction != null && rotateLeftAction.action != null && rotateLeftAction.action.IsPressed())
        {
            transform.Rotate(Vector3.up, step, Space.World);
        }
    }
}