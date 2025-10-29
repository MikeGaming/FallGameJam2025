using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GridController : MonoBehaviour
{
    UILineRenderer lineRenderer;
    List<int> inputs = null;
    [HideInInspector]
    public Vector2 position = Vector2.zero;
    public float cooldown = 1f;
    public Color defaultCol;
    public UnityEvent<List<int>> result = new UnityEvent<List<int>>();
    public UnityEvent reset = new UnityEvent();
    float curCooldown = 0f;
    RectTransform rectTransform;

    void Start()
    {
        lineRenderer = GetComponent<UILineRenderer>();
        rectTransform = GetComponent<RectTransform>();
    }

	void OnDisable()
    {
        lineRenderer?.Clear();
        inputs = null;
        reset.Invoke();
        curCooldown = 0f;
        GetComponent<Image>().color = defaultCol;
    }

	// Update is called once per frame
	void Update()
    {
        if (curCooldown > 0f)
		{
            curCooldown -= Time.deltaTime;
            if (curCooldown <= 0f)
			{
                curCooldown = 0f;
                lineRenderer.Clear();
                GetComponent<Image>().color = defaultCol;
			}
		}
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rectTransform, Mouse.current.position.value, Camera.current, out position);
        if (Input.GetMouseButtonDown(0) && curCooldown <= 0f)
        {
            inputs = new List<int>();
        }
        if (Input.GetMouseButtonUp(0) && inputs != null)
        {
            if (inputs.Count > 0) {
                result.Invoke(inputs);
                curCooldown = cooldown;
                reset.Invoke();
                lineRenderer.Stop();
            }
            inputs = null;
        }
    }

    public bool TryInput(GridNode node)
	{
        if (inputs == null) return false;

        if (!inputs.Contains(node.index))
        {
            inputs.Add(node.index);
            lineRenderer.Add(node.GetComponent<RectTransform>().anchoredPosition);
            return true;
        }
        return false;
	}
}
