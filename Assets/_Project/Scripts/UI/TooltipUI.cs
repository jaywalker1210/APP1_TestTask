using TMPro;
using UnityEngine;
using Assets._Project.Scripts.Data;

namespace Assets._Project.Scripts.UI.Components
{
    public class TooltipUI : MonoBehaviour
    {
        [SerializeField] private GameObject tooltipPanel;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI infoText;

        void Start()
        {
            tooltipPanel?.SetActive(false);
        }

        public void Show(ItemData item, int amount)
        {
            if (tooltipPanel == null) return;
            nameText.text = item.itemName;
            infoText.text = item.GetDescription(amount);
            tooltipPanel.SetActive(true);
        }

        public void Hide()
        {
            if (tooltipPanel != null)
                tooltipPanel.SetActive(false);
        }
    }
}