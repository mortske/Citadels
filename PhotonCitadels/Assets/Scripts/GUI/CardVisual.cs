using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CardVisual : MonoBehaviour 
{
    public Text text_Name;
    public Text text_Cost;
    public Image image_Color;
    public Image bg;

    public Color baseColor { get; set; }
    public Card card { get; set; }

    int siblingIndex;
    float hoverScale = 1.1f;

    void Start()
    {
        baseColor = bg.color;
    }

    public void OnHoverStart()
    {
        siblingIndex = transform.GetSiblingIndex();
        transform.SetAsLastSibling();
        transform.localScale *= hoverScale;
    }

    public void OnClick()
    {
        GameManager.instance.gameGUI.SetSelectedCard(this);
    }

    public void OnHoverEnd()
    {
        transform.SetSiblingIndex(siblingIndex);
        transform.localScale /= hoverScale;
    }
}
