using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GridNode : MonoBehaviour, IPointerEnterHandler
{
    public int index = 0;
	GridController controller;
	Image image;
	Color defaultCol;
	void Start()
	{
		image = GetComponent<Image>();
		defaultCol = image.color;
		controller = GetComponentInParent<GridController>();
		controller.reset.AddListener(ResetColour);
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		if (controller.TryInput(this))
			image.color = defaultCol * 1.25f;
	}
	
	void ResetColour()
	{
		image.color = defaultCol;
	}
}
