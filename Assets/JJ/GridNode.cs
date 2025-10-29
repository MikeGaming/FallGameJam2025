using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GridNode : MonoBehaviour
{
	public int index = 0;
	public float radius = 17f;
	GridController controller;
	Image image;
	Color defaultCol;
	RectTransform rect;
	void Start()
	{
		image = GetComponent<Image>();
		defaultCol = image.color;
		rect = GetComponent<RectTransform>();

		controller = GetComponentInParent<GridController>();
		controller.reset.AddListener(ResetColour);
	}

	void Update()
	{
		if (Vector2.Distance(controller.position, rect.anchoredPosition) < radius)
		{
			if (controller.TryInput(this))
			{
				image.color = defaultCol * 1.25f;
				enabled = false;
			}
		}
	}

	void ResetColour()
	{
		enabled = true;
		image.color = defaultCol;
	}
}
