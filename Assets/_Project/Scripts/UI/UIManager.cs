using Assets._Project.Scripts.Managers;
using Assets._Project.Scripts.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Top Buttons")]
    public Button shootButton;
    public Button addAmmoButton;
    public Button addItemButton;
    public Button removeItemButton;

    [Header("Bottom Panels")]
    public TextMeshProUGUI coinsText;
    public Button addCoinsButton;
    public TextMeshProUGUI weightText;

    [Header("Inventory")]
    public InventoryManager inventoryManager;

    void Start()
    {
        // Привязываем кнопки
        if (shootButton != null)
            shootButton.onClick.AddListener(OnShootButtonClick);

        if (addAmmoButton != null)
            addAmmoButton.onClick.AddListener(OnAddAmmoButtonClick);

        if (addItemButton != null)
            addItemButton.onClick.AddListener(OnAddItemButtonClick);

        if (removeItemButton != null)
            removeItemButton.onClick.AddListener(OnRemoveItemButtonClick);

        if (addCoinsButton != null)
            addCoinsButton.onClick.AddListener(OnAddCoinsButtonClick);

        // Находим InventoryManager, если не привязан
        if (inventoryManager == null)
            inventoryManager = FindObjectOfType<InventoryManager>();
    }

    public void UpdateCoinsUI(int coins)
    {
        if (coinsText != null)
            coinsText.text = coins.ToString();
    }

    public void UpdateWeightUI(float weight)
    {
        if (weightText != null)
            weightText.text = weight.ToString("F3");
    }

    // --- Обработчики кнопок (пока заглушки, потом допишешь) ---

    private void OnShootButtonClick()
    {
        Debug.Log("Выстрел");
        if (inventoryManager != null)
            inventoryManager.Shoot();
    }

    private void OnAddAmmoButtonClick()
    {
        Debug.Log("Добавить патроны");
        if (inventoryManager != null)
            inventoryManager.AddRandomAmmo();
    }

    private void OnAddItemButtonClick()
    {
        Debug.Log("Добавить предмет");
        if (inventoryManager != null)
            inventoryManager.AddRandomItem();
    }

    private void OnRemoveItemButtonClick()
    {
        Debug.Log("Удалить предмет");
        if (inventoryManager != null)
            inventoryManager.RemoveRandomItem();
    }

    private void OnAddCoinsButtonClick()
    {
        GameManager gm = FindObjectOfType<GameManager>();
        if (gm != null)
            gm.AddRandomCoins();
    }
}