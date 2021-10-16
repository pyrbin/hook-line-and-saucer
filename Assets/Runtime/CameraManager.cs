using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

public class CameraManager : MonoBehaviour
{
    [SerializeField]
    private InputAction Action;

    [SerializeField]
    private CinemachineVirtualCamera FishingCam;

    [SerializeField]
    private CinemachineVirtualCamera OverviewCam;

    [SerializeField]
    private CinemachineVirtualCamera TrackingCam;

    private bool fishCamera = true;

    private Animator Animator;

    private void Awake()
    {
        TryGetComponent(out Animator);
    }

    private void OnEnable()
    {
        Action.Enable();
    }

    private void OnDisable()
    {
        Action.Disable();
    }

    void Start()
    {
        Action.performed += _ =>
        {
            SwitchState();
        };
    }

    void SwitchState()
    {
        if (fishCamera)
        {
            GoToOverview();
            fishCamera = false;
        }
        else {
            fishCamera = true;
            GoToFishing();
        }
    }

    public void GoToFishing()
    {
        Animator.Play("Fishing");
    }

    public void GoToOverview()
    {
        Animator.Play("Overview");
    }

    public void GoToTracking(Transform tracked)
    {
        Animator.Play("Track");
        TrackingCam.Follow = tracked;
    }
}
