using UnityEngine;

public class PlayerController : MonoBehaviour
{
    FirstPersonController fpc;
    [SerializeField] private float defaultMoveSpeed = 5f;
    [SerializeField] Camera playerCamera;

    private void Start()
    {
        fpc = GetComponent<FirstPersonController>();
        if (fpc != null)
        {
            fpc.walkSpeed = defaultMoveSpeed;
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

    public void BlindAbility()
    {
        UpdateMoveSpeed(defaultMoveSpeed * 4f);
        if (playerCamera != null)
        {
            
        }
    }
}
