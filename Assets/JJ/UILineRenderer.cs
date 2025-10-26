using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;
using UnityEngine.InputSystem;

public class UILineRenderer : MonoBehaviour
{
    public GameObject lineObj;
    List<GameObject> lineObjects = new List<GameObject>();
    GridController controller;
    
    void Start()
    {
        controller = GetComponentInParent<GridController>();
        enabled = false;
    }

    Vector2 prevVal = Vector2.zero;
    void Update()
	{
        if (controller.position != prevVal) {
            RenderLine(lineObjects[lineObjects.Count - 1].GetComponent<RectTransform>(), controller.position);
			prevVal = controller.position;
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
        RenderLine(rectTransform, controller.position);
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
