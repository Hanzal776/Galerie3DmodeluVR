using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class VRModelAdapter : MonoBehaviour
{
    [Header("Nastavení velikosti pro VR")]
    [Tooltip("Maximální velikost objektu v metrech (např. 0.25 = 25 cm, aby se skvěle držel v ruce)")]
    public float targetSize = 0.25f;

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

        // 2. Zjistíme největší rozměr modelu
        float maxDimension = Mathf.Max(bounds.size.x, Mathf.Max(bounds.size.y, bounds.size.z));

        if (maxDimension > 0)
        {
            // Spočítáme koeficient, o kolik model zvětšit/zmenšit, aby odpovídal targetSize
            float scaleMultiplier = targetSize / maxDimension;
            transform.localScale *= scaleMultiplier;
        }

        // 3. Po přeschálování přepočítáme nové bounds pro collider
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
