using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

[CreateAssetMenu(fileName = "GameSettings", menuName = "Roguelike/GameSettings")]
public class GameSettings : ScriptableObject
{
    [Header("Инвертарь")]
    public int totalSlots = 50;
    public int initialUnlockedSlots = 15;
    public int slotsPerRow = 5;

    [Header("Стоимость разблокировки слота")]
    public int[] slotUnlockCost;

    public int GetSlotUnlockCost(int slotIndex)
    {
        if (slotUnlockCost != null && slotIndex < slotUnlockCost.Length)
        {
            return slotUnlockCost[slotIndex];
        }

        return 50;
    }
}
