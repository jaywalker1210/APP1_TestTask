using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameSettings", menuName = "Roguelike/GameSettings")]
public class GameSettings : ScriptableObject
{
    [Header("Инвертарь")]
    public int totalSlots = 50;
    public int initialUnlockedSlots = 15;
    public int slotsPerRow = 5;

    [Header("Стоимость разблокировки слота")]
    public int slotUnlockCost = 50;
}
