using UnityEngine;
using System;

public class UI_TowerQuickSlot : MonoBehaviour
{
    public event Action<int> OnSlotSelected;

    [Header("슬롯 리스트")]
    [SerializeField] private Slot_TowerQuickSlot[] slots;

    private void OnDestroy()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] != null)
            {
                slots[i].OnClicked -= HandleSlotClicked;
            }
        }
    }

    public void SetupUI(TowerData[] activeTowers)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] == null) continue;

            //TowerDataSO data = (activeTowers != null && i < activeTowers.Length) ? activeTowers[i] : null;
            slots[i].Initialize(i);
            slots[i].SetSlotData(activeTowers[i]);

            // 하위 슬롯 클릭 이벤트 구독
            slots[i].OnClicked += HandleSlotClicked;
        }
    }

    private void HandleSlotClicked(int index)
    {
        UpdateHighlight(index);

        OnSlotSelected?.Invoke(index);
    }

    public void UpdateHighlight(int selectedIndex)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] != null)
            {
                slots[i].SetHighlight(i == selectedIndex);
            }
        }
    }

    public void ClearHighlight()
    {
        foreach (var slot in slots)
        {
            if (slot != null) slot.SetHighlight(false);
        }
    }


}
