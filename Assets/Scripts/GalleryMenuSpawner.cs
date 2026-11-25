using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GalleryMenuSpawner : MonoBehaviour
{
    [Header("UI Menu & Layout")]
    [Tooltip("Plovoucí Canvas s menu")]
    public GameObject menuCanvas;

    [Tooltip("Panel s komponentou Grid Layout Group, kam se budou tvořit tlačítka")]
    public Transform buttonContainer;

    [Tooltip("Prefab UI Tlačítka s TextMeshPro komponentou")]
    public GameObject buttonPrefab;

    [Header("Spawn Nastavení")]
    [Tooltip("Místo, kam se má model po vybrání spawnout (pokud necháš prázdné, spawnuje se na pozici tohoto objektu)")]
    public Transform spawnPoint;

    [Header("Web Model Loader")]
    [SerializeField] private RuntimeModelLoader modelLoader;
    [Tooltip("Popisek tlačítka pro otevření webu s modely")]
    [SerializeField] private string webCatalogButtonLabel = "Web Modely";
    [Tooltip("Popisek tlačítka pro načtení zkopírované URL ze schránky")]
    [SerializeField] private string clipboardButtonLabel = "Načíst URL";

    private GameObject currentSpawnedModel;
    private GameObject[] modelPrefabs;

    void Start()
    {
        if (menuCanvas != null)
        {
            menuCanvas.SetActive(false);
        }

        modelPrefabs = Resources.LoadAll<GameObject>("Models");

        if (modelPrefabs.Length == 0)
        {
            Debug.LogWarning("Ve složce Assets/Resources/Models nebyly nalezeny žádné modely!");
        }

        GenerateMenuButtons();
    }

    void GenerateMenuButtons()
    {
        if (buttonPrefab == null || buttonContainer == null)
        {
            Debug.LogWarning("Chybí přiřazený Button Prefab nebo Button Container!");
            return;
        }

        foreach (Transform child in buttonContainer)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < modelPrefabs.Length; i++)
        {
            int index = i;
            GameObject newButtonObj = Instantiate(buttonPrefab, buttonContainer);

            TextMeshProUGUI btnText = newButtonObj.GetComponentInChildren<TextMeshProUGUI>();
            if (btnText != null && modelPrefabs[i] != null)
            {
                btnText.text = modelPrefabs[i].name;
            }

            Button btn = newButtonObj.GetComponent<Button>();
            if (btn != null)
            {
                btn.onClick.AddListener(() => SelectAndSpawnModel(index));
            }
        }

        if (modelLoader != null)
        {
            GameObject webBtnObj = Instantiate(buttonPrefab, buttonContainer);
            webBtnObj.name = "Button_WebCatalog";

            TextMeshProUGUI webBtnText = webBtnObj.GetComponentInChildren<TextMeshProUGUI>();
            if (webBtnText != null)
            {
                webBtnText.text = webCatalogButtonLabel;
            }

            Button webBtn = webBtnObj.GetComponent<Button>();
            if (webBtn != null)
            {
                webBtn.onClick.AddListener(OnWebCatalogClicked);
            }

            GameObject clipBtnObj = Instantiate(buttonPrefab, buttonContainer);
            clipBtnObj.name = "Button_LoadClipboard";

            TextMeshProUGUI clipBtnText = clipBtnObj.GetComponentInChildren<TextMeshProUGUI>();
            if (clipBtnText != null)
            {
                clipBtnText.text = clipboardButtonLabel;
            }

            Button clipBtn = clipBtnObj.GetComponent<Button>();
            if (clipBtn != null)
            {
                clipBtn.onClick.AddListener(OnLoadClipboardClicked);
            }
        }
    }

    private void OnWebCatalogClicked()
    {
        if (menuCanvas != null)
        {
            menuCanvas.SetActive(false);
        }

        if (modelLoader != null)
        {
            modelLoader.OpenModelCatalog();
        }
    }

    private void OnLoadClipboardClicked()
    {
        if (menuCanvas != null)
        {
            menuCanvas.SetActive(false);
        }

        if (modelLoader != null)
        {
            modelLoader.LoadModelFromClipboard();
        }
    }

    public void ToggleMenu()
    {
        if (menuCanvas != null)
        {
            menuCanvas.SetActive(!menuCanvas.activeSelf);
        }
    }

    public void SelectAndSpawnModel(int index)
    {
        if (modelPrefabs == null || index < 0 || index >= modelPrefabs.Length)
        {
            Debug.LogWarning("Neplatný index modelu!");
            return;
        }

        if (currentSpawnedModel != null)
        {
            Destroy(currentSpawnedModel);
        }

        Vector3 targetPosition = spawnPoint != null ? spawnPoint.position : transform.position;
        Quaternion targetRotation = spawnPoint != null ? spawnPoint.rotation : transform.rotation;

        currentSpawnedModel = Instantiate(modelPrefabs[index], targetPosition, targetRotation);

        if (currentSpawnedModel.GetComponent<VRModelAdapter>() == null)
        {
            currentSpawnedModel.AddComponent<VRModelAdapter>();
        }

        if (menuCanvas != null)
        {
            menuCanvas.SetActive(false);
        }
    }
}