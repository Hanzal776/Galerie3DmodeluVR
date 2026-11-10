using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class AutoFitCollider : MonoBehaviour
{
    void Start()
    {
        FitCollider();
    }

    public void FitCollider()
    {
        BoxCollider boxCollider = GetComponent<BoxCollider>();
        MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer>();

        if (renderers.Length == 0) return;

        Bounds bounds = renderers[0].bounds;
        foreach (MeshRenderer r in renderers)
        {
            bounds.Encapsulate(r.bounds);
        }

        boxCollider.center = transform.InverseTransformPoint(bounds.center);
        
        Vector3 worldSize = bounds.size;
        Vector3 lossyScale = transform.lossyScale;
        boxCollider.size = new Vector3(
            worldSize.x / Mathf.Max(lossyScale.x, 0.0001f),
            worldSize.y / Mathf.Max(lossyScale.y, 0.0001f),
            worldSize.z / Mathf.Max(lossyScale.z, 0.0001f)
        );
    }
}