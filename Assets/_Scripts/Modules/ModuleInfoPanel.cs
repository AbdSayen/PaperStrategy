using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModuleInfoPanel : MonoBehaviour
{
    public static ModuleInfoPanel Instance;

    [SerializeField] private Image image;
    [SerializeField] private Text moduleName;
    [SerializeField] private Text description;
    [SerializeField] private GameObject panelsParent;
    [SerializeField] private GameObject panelPrefab;

    private void Awake() => Instance = this;

    private void Start() => gameObject.SetActive(false);

    public void SetInfo(ModuleInfo moduleInfo, Materials materials)
    {
        RemovePanels();
        image.sprite = moduleInfo.sprite;
        moduleName.text = moduleInfo.moduleName;
        if (description != null) description.text = moduleInfo.description;

        foreach (KeyValuePair<string, int> mat in moduleInfo.PriceList)
        {
            if (mat.Value > 0)
            {
                GameObject panel = Instantiate(panelPrefab, panelsParent.transform);
                panel.GetComponent<MaterialInfoPanel>().SetInfo(MaterialsSpritesManager.Instance.Sprites[mat.Key], mat.Value.ToString());
            }
        }
    }

    public void RemovePanels()
    {
        foreach (Transform child in panelsParent.transform)
            Destroy(child.gameObject);
    }
}
