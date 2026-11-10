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

    private GameObject currentSpawnedModel;
    private GameObject[] modelPrefabs;

    void Start()
    {
        // Při startu hry schováme menu
        if (menuCanvas != null)
        {
            menuCanvas.SetActive(false);
        }

        // Automaticky načte všechny prefaby ze složky Assets/Resources/Models
        modelPrefabs = Resources.LoadAll<GameObject>("Models");

        if (modelPrefabs.Length == 0)
        {
            Debug.LogWarning("Ve složce Assets/Resources/Models nebyly nalezeny žádné modely!");
        }

        // Vygeneruje mřížku tlačítek podle nalezených modelů
        GenerateMenuButtons();
    }

    void GenerateMenuButtons()
    {
        if (buttonPrefab == null || buttonContainer == null)
        {
            Debug.LogWarning("Chybí přiřazený Button Prefab nebo Button Container!");
            return;
        }

        // Vyčistíme případná stará tlačítka
        foreach (Transform child in buttonContainer)
        {
            Destroy(child.gameObject);
        }

        // Pro každý nalezený model vytvoříme tlačítko v tabulce
        for (int i = 0; i < modelPrefabs.Length; i++)
        {
            int index = i; // Nutné pro správné předání indexu do skriptu tlačítka
            GameObject newButtonObj = Instantiate(buttonPrefab, buttonContainer);
            
            // Nastavení textu tlačítka podle názvu souboru/prefabu
            TextMeshProUGUI btnText = newButtonObj.GetComponentInChildren<TextMeshProUGUI>();
            if (btnText != null && modelPrefabs[i] != null)
            {
                btnText.text = modelPrefabs[i].name;
            }

            // Programové přidání funkce po kliknutí na tlačítko
            Button btn = newButtonObj.GetComponent<Button>();
            if (btn != null)
            {
                btn.onClick.AddListener(() => SelectAndSpawnModel(index));
            }
        }
    }

    // Metoda pro otevření/zavření menu (přiřazuje se do Select Entered na domě)
    public void ToggleMenu()
    {
        if (menuCanvas != null)
        {
            menuCanvas.SetActive(!menuCanvas.activeSelf);
        }
    }

    // Metoda pro spawnutí zvoleného modelu
    public void SelectAndSpawnModel(int index)
    {
        if (modelPrefabs == null || index < 0 || index >= modelPrefabs.Length)
        {
            Debug.LogWarning("Neplatný index modelu!");
            return;
        }

        // Pokud už na místě nějaký model stojí, nejprve ho smažeme
        if (currentSpawnedModel != null)
        {
            Destroy(currentSpawnedModel);
        }

        Vector3 targetPosition = spawnPoint != null ? spawnPoint.position : transform.position;
        Quaternion targetRotation = spawnPoint != null ? spawnPoint.rotation : transform.rotation;

        // Vytvoříme nový model z prefabu ve složce Resources/Models
        currentSpawnedModel = Instantiate(modelPrefabs[index], targetPosition, targetRotation);

        // AUTOMATICKÁ POJISTKA: Pokud model nemá skript VRModelAdapter, kód ho sám okamžitě doplní
        if (currentSpawnedModel.GetComponent<VRModelAdapter>() == null)
        {
            currentSpawnedModel.AddComponent<VRModelAdapter>();
        }

        // Po výběru modelu menu automaticky schováme
        if (menuCanvas != null)
        {
            menuCanvas.SetActive(false);
        }
    }
}