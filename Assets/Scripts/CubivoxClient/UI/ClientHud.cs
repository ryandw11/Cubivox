using CubivoxClient.UI;
using CubivoxCore.UI;
using UnityEngine;
using UnityEngine.UIElements;

public class ClientHud : MonoBehaviour, Hud
{
    private static ClientHud instance;
    private VisualElement rootVisualElement;

    void Awake()
    {
        instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rootVisualElement = GetComponent<UIDocument>().rootVisualElement;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnDestroy()
    {
        instance = null;
    }

    public void AddElement(Element element)
    {
        rootVisualElement.Add((VisualElement) element);
    }

    public void RemoveElement(Element element)
    {
        rootVisualElement.Remove((VisualElement) element);
    }

    public CubivoxCore.UI.Label CreateLabel()
    {
        return new CubiLabel();
    }

    public static ClientHud GetInstance()
    {
        return instance;
    }
}
