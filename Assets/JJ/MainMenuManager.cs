using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    public GridController gridController;
    public List<CodeObject> codes;
    public RectTransform lacreatura;
    public ParticleSystem confetti;

    void Start()
    {
        gridController.result.AddListener(CheckCode);
    }

    void CheckCode(List<int> inputs)
    {
        foreach (var code in codes)
        {
            if (code.CompareLists(code.code, inputs))
            {
                switch (code.name)
                {
                    case "Start":
                        StartGame();
                        break;
                    case "Stop":
                        LeaveGame();
                        break;
                    case "Funny":
                        Funny();
                        break;
                }

                gridController.GetComponent<Image>().color *= Color.green;
                return;
            }
        }
        gridController.GetComponent<Image>().color *= Color.red;
    }

    void StartGame()
    {
        Debug.Log("Start");
    }

    void LeaveGame()
    {
        Application.Quit();
    }

    void Funny()
    {
        //Do a funny thing
        if (!jumpscaring)
		{
            StartCoroutine(FunnyLol());
		}
    }

    bool jumpscaring = false;
    IEnumerator FunnyLol()
    {
        jumpscaring = true;
        confetti.Play();
        while (lacreatura.localScale != Vector3.one)
        {
            lacreatura.localScale = Vector3.MoveTowards(lacreatura.localScale, Vector3.one, Time.deltaTime * 5f);
            yield return null;
        }

        yield return new WaitForSeconds(0.25f);

        while (lacreatura.localScale != Vector3.zero)
        {
            lacreatura.localScale = Vector3.MoveTowards(lacreatura.localScale, Vector3.zero, Time.deltaTime * 7.5f);
            yield return null;
        }

        jumpscaring = false;
	}
}
