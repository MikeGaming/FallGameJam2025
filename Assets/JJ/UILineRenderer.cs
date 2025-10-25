using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;
using UnityEngine.InputSystem;

public class UILineRenderer : MonoBehaviour
{
    public GameObject lineObj;
    List<GameObject> lineObjects = new List<GameObject>();

    void Start()
    {
        enabled = false;
    }

    Vector2 prevVal = Vector2.zero;
    void Update()
	{
        Vector2 value;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
                GetComponent<RectTransform>(), Mouse.current.position.value, Camera.current, out value);
        if (value != prevVal) {
            RenderLine(lineObjects[lineObjects.Count - 1].GetComponent<RectTransform>(), value);
			prevVal = value;
		}
    }

	public void Add(Vector2 position)
    {
        if (enabled)
            RenderLine(lineObjects[lineObjects.Count - 1].GetComponent<RectTransform>(), position);
        else
            enabled = true;
        RectTransform rectTransform = Instantiate(lineObj, transform).GetComponent<RectTransform>();
        lineObjects.Add(rectTransform.gameObject);
        rectTransform.anchoredPosition = position;
        Vector2 value;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            GetComponent<RectTransform>(), Mouse.current.position.value, Camera.current, out value);
        RenderLine(rectTransform, value);
    }

    public void Clear()
    {
        lineObjects.ForEach(obj => Destroy(obj));
        lineObjects.Clear();
        enabled = false;
    }
    
    void RenderLine(RectTransform rectTransform, Vector2 targetPosition)
	{
        var childRect = rectTransform.GetChild(0).GetComponent<RectTransform>();
        childRect.sizeDelta = new Vector2(Vector2.Distance(rectTransform.anchoredPosition, targetPosition) + childRect.sizeDelta.y, childRect.sizeDelta.y);
        childRect.localPosition = Vector3.right * ((childRect.sizeDelta.x - childRect.sizeDelta.y) * 0.5f);
        rectTransform.localRotation = Quaternion.Euler(0f, 0f, Vector2.SignedAngle(Vector2.right, targetPosition - rectTransform.anchoredPosition));
	}
}
