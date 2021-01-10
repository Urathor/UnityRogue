using UnityEngine;
using UnityEngine.UI;
public class EquipmentDisplayItem : MonoBehaviour
{
    public Sprite EmptyIcon;
    public EquipmentType Type;
    public Image Icon;
    public Text NameText;

    public EquipmentItem item;
    [SerializeField] private Player Player;

    public void Init()
    {
        Player = GameManager.Player;

        SetEmpty();
    }

    public void SetEmpty()
    {
        item = null;
        NameText.text = "None";
        Icon.sprite = EmptyIcon;
    }

    public void OnSelectItemButton()
    {
        if (item != null)
            GameManager.InventorySystem.SetEquipmentDetails(item, GetComponent<Button>());
    }

    public void SetItem(EquipmentItem item)
    {
        this.item = item;
        if (item != null)
        {
            SetupItemValues();
        }
    }

    void SetupItemValues()
    {

        NameText.text = item.IsIdentified ? item.IdentifiedName : item.ItemName;
        Icon.sprite = item.ItemIcon;
    }

}
