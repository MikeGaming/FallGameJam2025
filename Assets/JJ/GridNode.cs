using UnityEngine;
using UnityEngine.EventSystems;

public class GridNode : MonoBehaviour, IPointerEnterHandler
{
    public int index = 0;
    GridController controller;
	void Start()
	{
		controller = GetComponentInParent<GridController>();
	}

	public void OnPointerEnter(PointerEventData eventData)
    {
        controller.TryInput(this);
    }
}
