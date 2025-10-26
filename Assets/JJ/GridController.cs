using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class GridController : MonoBehaviour
{
    UILineRenderer lineRenderer;
    List<int> inputs = null;
    public Vector2 position = Vector2.zero;
    public UnityEvent<List<int>> result = new UnityEvent<List<int>>();
    void Start()
    {
        lineRenderer = GetComponent<UILineRenderer>();
    }

	void OnEnable()
    {
        lineRenderer?.Clear();
        inputs = null;
    }

	// Update is called once per frame
	void Update()
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
                GetComponent<RectTransform>(), Mouse.current.position.value, Camera.current, out position);
        if (Input.GetMouseButtonDown(0))
        {
            inputs = new List<int>();
        }
        if (Input.GetMouseButtonUp(0))
        {
            result.Invoke(inputs);
            lineRenderer.Clear();
            inputs = null;
        }
    }

    public void TryInput(GridNode node)
	{
        if (inputs == null) return;

        if (!inputs.Contains(node.index))
		{
            inputs.Add(node.index);
            lineRenderer.Add(node.GetComponent<RectTransform>().anchoredPosition);
		}
	}
}
