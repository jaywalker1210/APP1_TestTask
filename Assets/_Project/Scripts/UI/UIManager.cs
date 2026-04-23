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
    public InventoryManager inventoryManager; // нужен DI

    void Start()
    {
        // здесь тоже нужен DI? или привязка кнопок таким способом нормальная практика?

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
            coinsText.text = $"Монеты: {coins.ToString()}";
    }

    public void UpdateWeightUI(float weight)
    {
        if (weightText != null)
            weightText.text = $"Вес: {weight.ToString("0.###")}";
    }

    private void OnShootButtonClick()
    {
        if (inventoryManager != null)
            inventoryManager.Shoot();
    }

    private void OnAddAmmoButtonClick()
    {
        if (inventoryManager != null)
            inventoryManager.AddRandomAmmo();
    }

    private void OnAddItemButtonClick()
    {
        if (inventoryManager != null)
            inventoryManager.AddRandomItem();
    }

    private void OnRemoveItemButtonClick()
    {
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