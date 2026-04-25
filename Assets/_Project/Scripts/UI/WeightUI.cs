using TMPro;
using UnityEngine;

namespace Assets._Project.Scripts.UI.Components
{
    public class WeightUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI weightText;

        public void UpdateWeight(float weight)
        {
            if (weightText != null)
                weightText.text = $"Вес: {weight:0.###}";
        }
    }
}