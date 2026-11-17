using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRGrabInteractable))]
public class ModelRotator : MonoBehaviour
{
    [Header("Input Actions")]
    [Tooltip("Akce pro otočení doprava (např. tlačítko B)")]
    public InputActionReference rotateRightAction;

    [Tooltip("Akce pro otočení doleva (např. tlačítko A)")]
    public InputActionReference rotateLeftAction;

    [Header("Settings")]
    public float rotationSpeed = 90f; // Rychlost otáčení ve stupních za sekundu

    private XRGrabInteractable grabInteractable;
    private bool isHeld = false;

    void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
    }

    void OnEnable()
    {
        grabInteractable.selectEntered.AddListener(OnGrabbed);
        grabInteractable.selectExited.AddListener(OnReleased);
    }

    void OnDisable()
    {
        grabInteractable.selectEntered.RemoveListener(OnGrabbed);
        grabInteractable.selectExited.RemoveListener(OnReleased);
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

        // Kontrola tlačítek A/B
        if (rotateRightAction != null && rotateRightAction.action.IsPressed())
        {
            transform.Rotate(Vector3.up, -rotationStep, Space.World);
        }

        if (rotateLeftAction != null && rotateLeftAction.action.IsPressed())
        {
            transform.Rotate(Vector3.up, rotationStep, Space.World);
        }
    }
}