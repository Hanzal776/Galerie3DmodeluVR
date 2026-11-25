using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using GLTFast;

public class RuntimeModelLoader : MonoBehaviour
{
    [Header("Web & Placement")]
    public string catalogUrl = "https://poly.pizza";
    public Transform spawnPoint;

    [Header("Storage & Cleanup")]
    [Tooltip("Smazat stažené .glb modely při zavření aplikace a při startu nové relace")]
    public bool deleteDownloadedOnQuit = true;

    [Header("Optional Components")]
    public bool addGrabInteractable = true;
    public bool addVRModelAdapter = true;

    private GameObject currentSpawnedObject;
    private string CacheFolderPath => Path.Combine(Application.temporaryCachePath, "DownloadedModels");

    private void Awake()
    {
        if (deleteDownloadedOnQuit)
        {
            CleanupDownloadedModels();
        }

        if (!Directory.Exists(CacheFolderPath))
        {
            Directory.CreateDirectory(CacheFolderPath);
        }
    }

    private void OnEnable()
    {
        Application.deepLinkActivated += OnDeepLinkActivated;
        if (!string.IsNullOrEmpty(Application.absoluteURL))
        {
            OnDeepLinkActivated(Application.absoluteURL);
        }
    }

    private void OnDisable()
    {
        Application.deepLinkActivated -= OnDeepLinkActivated;
    }

    private void OnApplicationQuit()
    {
        if (deleteDownloadedOnQuit)
        {
            CleanupDownloadedModels();
        }
    }

    public void OpenModelCatalog()
    {
        if (!string.IsNullOrWhiteSpace(catalogUrl))
        {
            Application.OpenURL(catalogUrl);
        }
        else
        {
            Debug.LogWarning("RuntimeModelLoader: Není vyplněna catalogUrl.");
        }
    }

    public void LoadModelFromClipboard()
    {
        string clip = GUIUtility.systemCopyBuffer?.Trim();
        if (string.IsNullOrWhiteSpace(clip) || !Uri.IsWellFormedUriString(clip, UriKind.Absolute))
        {
            Debug.LogWarning("RuntimeModelLoader: Ve schránce není platná URL.");
            return;
        }

        if (clip.Contains("poly.pizza/m/", StringComparison.OrdinalIgnoreCase))
        {
            Debug.LogWarning("RuntimeModelLoader: Odkaz poly.pizza/m/... je webová stránka, ne přímý .glb soubor.");
            return;
        }

        _ = DownloadAndLoadModelAsync(clip);
    }

    private void OnDeepLinkActivated(string url)
    {
        const string prefix = "vrgalerie://load?url=";
        if (!string.IsNullOrWhiteSpace(url) && url.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
        {
            string encodedTarget = url.Substring(prefix.Length);
            string decodedTarget = Uri.UnescapeDataString(encodedTarget);
            _ = DownloadAndLoadModelAsync(decodedTarget);
        }
    }

    public async void LoadModelFromUrl(string url)
    {
        await DownloadAndLoadModelAsync(url);
    }

    public string[] GetDownloadedModelPaths()
    {
        if (!Directory.Exists(CacheFolderPath))
        {
            return Array.Empty<string>();
        }

        return Directory.GetFiles(CacheFolderPath, "*.glb", SearchOption.TopDirectoryOnly);
    }

    public async Task<string> DownloadModelToStorageAsync(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            Debug.LogWarning("RuntimeModelLoader: Prázdná URL.");
            return null;
        }

        if (!Directory.Exists(CacheFolderPath))
        {
            Directory.CreateDirectory(CacheFolderPath);
        }

        string safeFileName = SanitizeFileName(Path.GetFileName(new Uri(url).LocalPath));
        if (string.IsNullOrWhiteSpace(safeFileName) || !safeFileName.EndsWith(".glb", StringComparison.OrdinalIgnoreCase))
        {
            safeFileName = $"model_{DateTime.UtcNow:yyyyMMdd_HHmmss}.glb";
        }

        string localFilePath = Path.Combine(CacheFolderPath, safeFileName);

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            request.downloadHandler = new DownloadHandlerFile(localFilePath);
            UnityWebRequestAsyncOperation operation = request.SendWebRequest();

            while (!operation.isDone)
            {
                await Task.Yield();
            }

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"RuntimeModelLoader: Chyba při stahování .glb ({request.error}): {url}");
                if (File.Exists(localFilePath))
                {
                    File.Delete(localFilePath);
                }
                return null;
            }
        }

        Debug.Log($"RuntimeModelLoader: Model uložen na zařízení: {localFilePath}");
        return localFilePath;
    }

    public async Task DownloadAndLoadModelAsync(string url)
    {
        string localPath = await DownloadModelToStorageAsync(url);
        if (!string.IsNullOrEmpty(localPath))
        {
            await LoadLocalGlbFileAsync(localPath);
        }
    }

    public async Task LoadLocalGlbFileAsync(string localFilePath)
    {
        if (string.IsNullOrWhiteSpace(localFilePath) || !File.Exists(localFilePath))
        {
            Debug.LogWarning("RuntimeModelLoader: Lokální .glb soubor neexistuje.");
            return;
        }

        if (spawnPoint == null)
        {
            Debug.LogError("RuntimeModelLoader: Chybí přiřazený Spawn Point!");
            return;
        }

        if (currentSpawnedObject != null)
        {
            Destroy(currentSpawnedObject);
        }

        string fileUri = new Uri(localFilePath).AbsoluteUri;
        var gltf = new GltfImport();
        bool loaded = await gltf.Load(fileUri);
        if (!loaded)
        {
            Debug.LogError($"RuntimeModelLoader: Nepodařilo se načíst lokální .glb: {localFilePath}");
            return;
        }

        GameObject root = new GameObject(Path.GetFileNameWithoutExtension(localFilePath));
        root.transform.SetPositionAndRotation(spawnPoint.position, spawnPoint.rotation);

        bool instantiated = await gltf.InstantiateMainSceneAsync(root.transform);
        if (!instantiated)
        {
            Debug.LogError("RuntimeModelLoader: Nepodařilo se instancovat scénu glTF.");
            Destroy(root);
            return;
        }

        SetupInteractableAndBounds(root);
        currentSpawnedObject = root;
        Debug.Log($"RuntimeModelLoader: Spawnut model ze souboru: {localFilePath}");
    }

    public void CleanupDownloadedModels()
    {
        if (!Directory.Exists(CacheFolderPath))
        {
            return;
        }

        try
        {
            Directory.Delete(CacheFolderPath, true);
            Debug.Log("RuntimeModelLoader: Stažené modely byly smazány.");
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"RuntimeModelLoader: Nepodařilo se smazat složku modelů: {ex.Message}");
        }
    }

    private static string SanitizeFileName(string name)
    {
        foreach (char invalidChar in Path.GetInvalidFileNameChars())
        {
            name = name.Replace(invalidChar, '_');
        }
        return name;
    }

    private void SetupInteractableAndBounds(GameObject root)
    {
        Renderer[] renderers = root.GetComponentsInChildren<Renderer>();
        if (renderers.Length > 0)
        {
            Bounds bounds = renderers[0].bounds;
            for (int i = 1; i < renderers.Length; i++)
            {
                bounds.Encapsulate(renderers[i].bounds);
            }

            BoxCollider box = root.AddComponent<BoxCollider>();
            box.center = root.transform.InverseTransformPoint(bounds.center);

            Vector3 localSize = root.transform.InverseTransformVector(bounds.size);
            box.size = new Vector3(
                Mathf.Abs(localSize.x),
                Mathf.Abs(localSize.y),
                Mathf.Abs(localSize.z)
            );
        }

        if (addGrabInteractable)
        {
            if (root.GetComponent<Rigidbody>() == null)
            {
                var rb = root.AddComponent<Rigidbody>();
                rb.useGravity = false;
                rb.isKinematic = true;
            }

            if (root.GetComponent<XRGrabInteractable>() == null)
            {
                root.AddComponent<XRGrabInteractable>();
            }
        }

        if (addVRModelAdapter && root.GetComponent<VRModelAdapter>() == null)
        {
            root.AddComponent<VRModelAdapter>();
        }
    }
}