using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.Rendering.CoreUtils;

public class TooltipService : MonoBehaviour
{
    public static TooltipService Instance { get; private set; }

    [SerializeField]
    private TooltipView view;

    [SerializeField]
    private ProviderHeader prefab_header;
    [SerializeField]
    private ProviderKeyValue prefab_keyValue;
    [SerializeField]
    private ProviderSections prefab_section;
    [SerializeField]
    private ProviderBottom prefab_desc;
    [SerializeField]
    private ItemDEquipment prefab_equip;

    private ProviderHeader header;
    private ProviderKeyValue keyValue;
    private ProviderSections sections;
    private ProviderBottom bottom;
    private ItemDEquipment equip;
    //private readonly Queue<ItemDEquipment> equip = new();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        header = Instantiate(prefab_header, transform);
        keyValue = Instantiate(prefab_keyValue, transform);
        sections = Instantiate(prefab_section, transform);
        bottom = Instantiate(prefab_desc, transform);
        equip = Instantiate(prefab_equip, transform);
    }

    public void TooltipOpen(ITooltipProvider provider)
    {
        if (provider == null) return;

        if(provider is ITooltipHeaderProvider header)
            this.header.Setting(header, view);

        if (provider is ITooltipKeyValueProvider keyValue)
            this.keyValue.Setting(keyValue, view);

        if (provider is ITooltipSectionsProvider sections)
            this.sections.Setting(sections, view);

        if (provider is ITooltipBottomProvider bottom)
            this.bottom.Setting(bottom, view);

        RectTransform rect = view.GetComponent<RectTransform>();
        Vector2 size = new Vector2(this.header.Rect.rect.width, this.header.Rect.rect.height + this.keyValue.Rect.rect.height + this.sections.Rect.rect.height + this.bottom.Rect.rect.height);
        rect.sizeDelta = size;
        view.transform.position = provider.Transform.position + (Vector3)provider.Offset;
        UIManager.Instance.ClampPosition(view.GetComponent<RectTransform>());
    }

    public void TooltipClose()
    {
        header.gameObject.SetActive(false);
        keyValue.gameObject.SetActive(false);
        sections.gameObject.SetActive(false);
        bottom.gameObject.SetActive(false);
        equip.gameObject.SetActive(false);
        view.Clear();
    }

    public void TooltipMove(ITooltipProvider provider)
    {
        view.transform.position = Mouse.current.position.value + provider.Offset;
    }
}
