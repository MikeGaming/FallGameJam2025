using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridController : MonoBehaviour
{
    UILineRenderer lineRenderer;
    List<int> inputs = null;

	void Start()
	{
		lineRenderer = GetComponent<UILineRenderer>();
	}

	// Update is called once per frame
	void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            inputs = new List<int>();
        }
        if (Input.GetMouseButtonUp(0))
        {
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
