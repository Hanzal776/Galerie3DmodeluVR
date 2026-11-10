using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables; // Pro Unity 6 / XRI 3.x

[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(XRGrabInteractable))]
public class VRModelAdapter : MonoBehaviour
{
    [Header("Nastavení velikosti pro VR")]
    [Tooltip("Maximální velikost v metrech (0.25 = ideální do ruky)")]
    public float targetSize = 0.25f;

    void Awake()
    {
        // Kód sám zaručí, že objekt bude mít fyziku a XR chytání, i když je zapomeneš přidat
        if (GetComponent<Rigidbody>() == null)
            gameObject.AddComponent<Rigidbody>();

        if (GetComponent<BoxCollider>() == null)
            gameObject.AddComponent<BoxCollider>();

        if (GetComponent<XRGrabInteractable>() == null)
            gameObject.AddComponent<XRGrabInteractable>();
    }

    void Start()
    {
        AdaptAndScaleModel();
    }

    public void AdaptAndScaleModel()
    {
        MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer>();
        if (renderers.Length == 0) return;

        // 1. Spočítáme reálné ohraničení 3D modelu
        Bounds bounds = renderers[0].bounds;
        foreach (MeshRenderer r in renderers)
        {
            bounds.Encapsulate(r.bounds);
        }

        // 2. Zjistíme největší rozměr a oškálujeme objekt
        float maxDimension = Mathf.Max(bounds.size.x, Mathf.Max(bounds.size.y, bounds.size.z));

        if (maxDimension > 0)
        {
            float scaleMultiplier = targetSize / maxDimension;
            transform.localScale *= scaleMultiplier;
        }

        // 3. Přepočítáme kolizi na přesný rozměr po oškálování
        Bounds updatedBounds = renderers[0].bounds;
        foreach (MeshRenderer r in renderers)
        {
            updatedBounds.Encapsulate(r.bounds);
        }

        BoxCollider boxCollider = GetComponent<BoxCollider>();
        boxCollider.center = transform.InverseTransformPoint(updatedBounds.center);
        
        Vector3 worldSize = updatedBounds.size;
        Vector3 lossyScale = transform.lossyScale;
        boxCollider.size = new Vector3(
            worldSize.x / Mathf.Max(lossyScale.x, 0.0001f),
            worldSize.y / Mathf.Max(lossyScale.y, 0.0001f),
            worldSize.z / Mathf.Max(lossyScale.z, 0.0001f)
        );
    }
}