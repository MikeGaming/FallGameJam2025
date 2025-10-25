using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PlayerController : MonoBehaviour
{
    FirstPersonController fpc;
    [SerializeField] private float defaultMoveSpeed = 5f;
    [SerializeField] Camera playerCamera;

    private Material camMat;
    private FullScreenPassRendererFeature fullScreenPass;

    bool blindingAbilityActive = false;

    private void Start()
    {
        // Fix: Use rendererDataList to get the ScriptableRendererData
        UniversalRenderPipelineAsset urpAsset = GraphicsSettings.currentRenderPipeline as UniversalRenderPipelineAsset;
        ScriptableRendererData rendererData = null;
        if (urpAsset != null && urpAsset.rendererDataList.Length > 0)
        {
            rendererData = urpAsset.rendererDataList[0];
        }
        if (rendererData != null)
        {
            fullScreenPass = rendererData.rendererFeatures.Find(feature => feature is FullScreenPassRendererFeature) as FullScreenPassRendererFeature;
        }
        if (fullScreenPass != null)
        {
            camMat = fullScreenPass.passMaterial;
        }
        else
        {
            Debug.LogError("FullScreenPassRendererFeature not found in the renderer features.");
        }
        fpc = GetComponent<FirstPersonController>();
        if (fpc != null)
        {
            fpc.walkSpeed = defaultMoveSpeed;
        }
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.B))
        {
            ActivateBlindingAbility(true);
            ActivateSeethroughAbility(false);
        }
        else if(Input.GetKeyDown(KeyCode.N))
        {
            ActivateBlindingAbility(false);
            ActivateSeethroughAbility(true);
        }

        if (blindingAbilityActive)
        {
            if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0 || Input.GetKey(KeyCode.Space))
            {
                UpdateMoveSpeed(defaultMoveSpeed * 4);
                camMat.SetFloat("_Blind", 1);
            }
            else
            {
                UpdateMoveSpeed(defaultMoveSpeed);
                camMat.SetFloat("_Blind", 0);
            }
        }
    }

    public void ActivateBlindingAbility(bool state)
    {
        blindingAbilityActive = state;
        UpdateMoveSpeed(state ? defaultMoveSpeed * 4 : defaultMoveSpeed);
        camMat.SetFloat("_Blind", 0);
    }

    public void ActivateSeethroughAbility(bool state)
    {
        
        if (state)
        {
            playerCamera.cullingMask &= ~(1 << LayerMask.NameToLayer("Environment"));
        }
        else
        {
            playerCamera.cullingMask |= (1 << LayerMask.NameToLayer("Environment"));
        }
    }

    public void UpdateMoveSpeed(float newSpeed)
    {
        if (fpc != null)
        {
            fpc.walkSpeed = newSpeed;
        }
    }

    public void ResetMoveSpeed()
    {
        if (fpc != null)
        {
            fpc.walkSpeed = defaultMoveSpeed;
        }
    }

    private void OnApplicationQuit()
    {
        camMat.SetFloat("_Blind", 0);
    }
}
