using System;
using System.Collections.Generic;
using UnityEngine;

public class CodeManager : MonoBehaviour
{
    [Serializable]
    public class CodeObject
    {
        public String name = "lmao";
        public List<int> code = new List<int>();
    }

    public GridController gridController;
    public float speed = 1000f;
    public List<CodeObject> codes;
    RectTransform gridRect;
    Canvas canvas;
    Vector2 offset;

    void Start()
    {
        gridController.result.AddListener(CheckCode);

        canvas = GetComponent<Canvas>();
        gridRect = gridController.GetComponent<RectTransform>();

        offset = Vector2.down * (canvas.pixelRect.height + gridRect.sizeDelta.y);
        gridRect.anchoredPosition = offset;
        gridController.gameObject.SetActive(false);
    }

    void Update()
    {
        if (Input.GetMouseButton(1))
        {
            if (!gridController.gameObject.activeSelf)
                gridController.gameObject.SetActive(true);
            gridRect.anchoredPosition = Vector2.MoveTowards(gridRect.anchoredPosition, Vector2.zero, Time.deltaTime * speed);
        }
        else if (gridController.gameObject.activeSelf)
        {
            gridRect.anchoredPosition = Vector2.MoveTowards(gridRect.anchoredPosition, offset, Time.deltaTime * speed);
            if (gridRect.anchoredPosition == offset)
                gridController.gameObject.SetActive(false);
        }
    }

    void CheckCode(List<int> inputs)
    {
        foreach (var code in codes)
        {
            if (CompareLists(code.code, inputs))
            {
                Debug.Log(code.name);
            }
        }
    }
    
    bool CompareLists(List<int> one, List<int> two)
    {
        if (one.Count != two.Count) return false;

        bool valid = true;
        for (int i = 0; i < one.Count; ++i) {
            if (one[i] != two[i]) {
                valid = false;
                break;
            }
        }

        if (valid) return true;

        valid = true;
        for (int i = 0; i < one.Count; ++i) {
            if (one[i] != two[one.Count - i - 1]) {
                valid = false;
                break;
            }
        }

        return valid;
	}
}
