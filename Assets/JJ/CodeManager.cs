using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CodeManager : MonoBehaviour
{
    public PlayerController playerController;
    public GridController gridController;
    public RectTransform stare;
    public RectTransform stareOffset;
    public float speed = 1000f;
    public List<CodeObject> codes;
    public int maxDestroys = 1;
    public int maxSeeThroughs = 1;
    public int maxBlinds = 1;
    public RectTransform usesParent;
    public AudioSource source;
    RectTransform gridRect;
    Canvas canvas;
    Vector2 offset = Vector2.zero;
    Vector2 startPos;

    void Start()
    {
        gridController.result.AddListener(CheckCode);

        canvas = GetComponent<Canvas>();
        gridRect = gridController.GetComponent<RectTransform>();

        startPos = gridRect.anchoredPosition;
        offset.y = -(canvas.pixelRect.height + gridRect.sizeDelta.y);
        offset.x = startPos.x;
        gridRect.anchoredPosition = offset;
        gridController.gameObject.SetActive(false);

        UpdateUI("Blind", maxBlinds);
        UpdateUI("SeeThrough", maxSeeThroughs);
        UpdateUI("Destroy", maxDestroys);
    }

    void Update()
    {
        if (Input.GetMouseButton(1))
        {
            if (!gridController.gameObject.activeSelf)
            {
                Cursor.lockState = CursorLockMode.Confined;
                gridController.gameObject.SetActive(true);
            }
            gridRect.anchoredPosition = Vector2.MoveTowards(gridRect.anchoredPosition, startPos, Time.deltaTime * speed);
            stare.anchoredPosition = Vector2.MoveTowards(stare.anchoredPosition, stareOffset.anchoredPosition, Time.deltaTime * speed);
        }
        else if (gridController.gameObject.activeSelf)
        {
            gridRect.anchoredPosition = Vector2.MoveTowards(gridRect.anchoredPosition, offset, Time.deltaTime * speed);
            stare.anchoredPosition = Vector2.MoveTowards(stare.anchoredPosition, Vector2.zero, Time.deltaTime * speed);
            if (gridRect.anchoredPosition == offset)
		    {
			    Cursor.lockState = CursorLockMode.Locked;
                gridController.gameObject.SetActive(false);
		    }
        }
    }

    void CheckCode(List<int> inputs)
    {
        foreach (var code in codes)
        {
            if (code.CompareLists(code.code, inputs))
            {
                if ((code.name == "Blind" && maxBlinds-- == 0) ||
                    (code.name == "SeeThrough" && maxSeeThroughs-- == 0) ||
                    (code.name == "Destroy" && maxDestroys-- == 0))
                {
                    //Do something to notify that it's wrong
                    break;
                }
                UpdateUI(code.name,
                    code.name == "Blind" ? maxBlinds :
                    code.name == "SeeThrough" ? maxSeeThroughs :
                    maxDestroys);
                playerController.ActivateBlindingAbility(code.name == "Blind");
                playerController.ActivateSeethroughAbility(code.name == "SeeThrough");
                playerController.ActivateDestroyAbility(code.name == "Destroy");
                gridController.GetComponent<Image>().color *= Color.green;
                return;
            }
        }
        source.Play();
        gridController.GetComponent<Image>().color *= Color.red;
    }

    void UpdateUI(string goName, int value)
    {
        TMPro.TMP_Text text = usesParent.Find(goName)?.GetComponent<TMPro.TMP_Text>();
        if (text)
		{
            text.text = "Uses: " + value;
		}
    }
}


    [Serializable]
    public class CodeObject
    {
        public String name = "lmao";
        public List<int> code = new List<int>();
        
        public bool CompareLists(List<int> one, List<int> two)
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