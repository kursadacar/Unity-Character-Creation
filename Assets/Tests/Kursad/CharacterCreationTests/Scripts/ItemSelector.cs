using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Linq;

public class ItemSelector : MonoBehaviour
{
    [SerializeField] Transform itemHolder;
    [SerializeField] GameObject imagePrefab;
    public GameObject selection;

    private void Start()
    {
        if (itemHolder == null)
            itemHolder = GetComponentInChildren<HorizontalLayoutGroup>().transform;
    }

    public GameObject AddItem(Texture2D itemTexture, params UnityAction[] onClickActions )
    {
        var holderRect = itemHolder.GetComponent<RectTransform>().rect;

        var newElement = Instantiate(imagePrefab, itemHolder);
        newElement.GetComponent<RectTransform>().sizeDelta = new Vector2(holderRect.height, holderRect.height);
        newElement.GetComponent<Image>().color = Color.black;

        var button = newElement.GetComponentInChildren<Button>();
        button.onClick.AddListener(delegate { Select(newElement); });
        foreach(var action in onClickActions)
        {
            button.onClick.AddListener(action);
        }
        button.GetComponent<Image>().overrideSprite = Sprite.Create(itemTexture, new Rect(Vector2.zero, new Vector2(itemTexture.width,itemTexture.height)), Vector2.zero);
        GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,GetComponentsInChildren<RectTransform>().Where(x=> x.transform.parent == transform).Sum(x=> x.rect.size.y));

        return newElement;
    }

    public void Select(GameObject sel)
    {
        if (selection != null)
        {
            selection.GetComponent<Image>().color = Color.black;
        }

        selection = sel;

        selection.GetComponent<Image>().color = Color.white;
        Debug.Log("Click..");
    }
}
