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
    bool destroyAbilityActive = false;

    // New: destruction tracking for gaze-hold and recursion settings
    private GameObject currentDestroyTarget = null;
    [SerializeField] private float destroyHoldTime = 0f;
    [SerializeField] private float requiredHoldTime = 1f;
    [SerializeField] private int maxReflections = 4;
    [SerializeField] private float rayDistance = 50f;

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

    float t;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.B))
        {
            ActivateBlindingAbility(true);
            ActivateSeethroughAbility(false);
            ActivateDestroyAbility(false);
        }
        else if(Input.GetKeyDown(KeyCode.N))
        {
            ActivateBlindingAbility(false);
            ActivateSeethroughAbility(true);
            ActivateDestroyAbility(false);
        }
        else if(Input.GetKeyDown(KeyCode.M))
        {
            ActivateBlindingAbility(false);
            ActivateSeethroughAbility(false);
            ActivateDestroyAbility(true);
        }

        if (blindingAbilityActive)
        {
            if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0 || Input.GetKey(KeyCode.Space))
            {
                UpdateMoveSpeed(defaultMoveSpeed * 4);
                if (camMat != null) camMat.SetFloat("_Blind", 1);
            }
            else
            {
                UpdateMoveSpeed(defaultMoveSpeed);
                if (camMat != null) camMat.SetFloat("_Blind", 0);
            }
        }

        if(destroyAbilityActive)
        {
            Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));

            // Draw debug visualization of the ray and its reflections each frame.
            DrawDebugRayPath(ray, maxReflections);

            if (TryGetHitThroughMirrors(ray, maxReflections, out RaycastHit hitInfo))
            {
                GameObject hitObj = hitInfo.transform.gameObject;

                // If we are still looking at the same object, accumulate hold time.
                if (currentDestroyTarget == hitObj)
                {
                    destroyHoldTime += Time.deltaTime;
                }
                else
                {
                    // New target -> reset timer and set as current
                    currentDestroyTarget = hitObj;
                    destroyHoldTime = Time.deltaTime;
                }

                if (destroyHoldTime >= requiredHoldTime)
                {
                    if (hitObj.CompareTag("Enemy"))
                        Destroy(hitObj.transform.parent.parent.gameObject);
                    else
                        Destroy(hitObj);
                    currentDestroyTarget = null;
                    destroyHoldTime = 0f;
                }
            }
            else
            {
                // Nothing hit (or only mirrors but exceeded reflections) -> reset hold state
                currentDestroyTarget = null;
                destroyHoldTime = 0f;
            }
        }
    }

    private bool TryGetHitThroughMirrors(Ray ray, int remainingReflections, out RaycastHit hit)
    {
        if (Physics.Raycast(ray, out hit, rayDistance))
        {
            if (hit.collider != null && hit.collider.CompareTag("Mirror"))
            {
                if (remainingReflections <= 0)
                {
                    // Reached max reflections and still hit a mirror -> treat as no valid target.
                    return false;
                }

                // Reflect and continue from a tiny offset to avoid immediately hitting the same collider
                Vector3 reflectDirection = Vector3.Reflect(ray.direction, hit.normal);
                Ray reflectedRay = new Ray(hit.point + reflectDirection * 0.01f, reflectDirection);
                return TryGetHitThroughMirrors(reflectedRay, remainingReflections - 1, out hit);
            }

            // Hit a non-mirror object
            return true;
        }

        // No hit
        hit = default;
        return false;
    }

    // New: draw the ray path including mirror reflections for debugging
    private void DrawDebugRayPath(Ray ray, int remainingReflections)
    {
        Vector3 origin = ray.origin;
        Vector3 direction = ray.direction.normalized;
        float remainingDistance = rayDistance;
        int reflectionsLeft = remainingReflections;
        float traveled = 0f;

        // Iterate up to reflectionsLeft times + 1 segment
        while (true)
        {
            if (Physics.Raycast(origin, direction, out RaycastHit hit, remainingDistance))
            {
                // Draw the segment from origin to hit point.
                Color segmentColor = hit.collider != null && hit.collider.CompareTag("Mirror") ? Color.yellow : Color.red;
                Debug.DrawLine(origin, hit.point, segmentColor, 0f, false);
                // Draw a small normal indicator at hit point
                Debug.DrawRay(hit.point, hit.normal * 0.5f, Color.white, 0f, false);

                // Update distances for next iteration
                float hitDistance = Vector3.Distance(origin, hit.point);
                traveled += hitDistance;
                remainingDistance = Mathf.Max(0f, rayDistance - traveled);

                // If this hit is a mirror and we still have reflections allowed, reflect and continue
                if (hit.collider != null && hit.collider.CompareTag("Mirror") && reflectionsLeft > 0 && remainingDistance > 0f)
                {
                    Vector3 reflectDir = Vector3.Reflect(direction, hit.normal).normalized;
                    origin = hit.point + reflectDir * 0.01f; // offset to avoid re-collision with same surface
                    direction = reflectDir;
                    reflectionsLeft--;
                    continue;
                }

                // Either hit non-mirror or can't reflect further -> stop drawing
                break;
            }
            else
            {
                // Nothing hit: draw remaining ray out to max distance
                Debug.DrawLine(origin, origin + direction * remainingDistance, Color.gray, 0f, false);
                break;
            }
        }
    }

    public void ActivateBlindingAbility(bool state)
    {
        blindingAbilityActive = state;
        UpdateMoveSpeed(state ? defaultMoveSpeed * 4 : defaultMoveSpeed);
        if (camMat != null) camMat.SetFloat("_Blind", 0);
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

    public void ActivateDestroyAbility(bool state)
    {
        destroyAbilityActive = state;

        // Reset any in-progress destruction when toggling off
        if (!state)
        {
            currentDestroyTarget = null;
            destroyHoldTime = 0f;
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
        if (camMat != null) camMat.SetFloat("_Blind", 0);
    }
}
