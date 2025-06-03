using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))] 
public class SelectModuleButton : MonoBehaviour
{
    [SerializeField] private ModuleInfo moduleInfo;
    private Button _button;

    private void Awake() => _button = GetComponent<Button>();
    private void Start() 
    {
        if (moduleInfo is null) return;

        _button.onClick.AddListener(OnClick);
        GetComponent<Image>().sprite = moduleInfo.sprite;
    }
    private void OnDestroy() => _button.onClick.RemoveListener(OnClick);

    private void OnClick()
    {
        BuildSystem.Instance.ChooseModule(moduleInfo);
    }
}