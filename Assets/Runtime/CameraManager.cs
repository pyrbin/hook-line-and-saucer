using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

public class CameraManager : MonoBehaviour
{
    [SerializeField]
    private CinemachineVirtualCamera FishingCam;

    [SerializeField]
    private CinemachineVirtualCamera OverviewCam;

    [SerializeField]
    private CinemachineVirtualCamera TrackingCam;

    public FMODUnity.EventReference TransitionSound;

    private Animator Animator;

    private void Awake()
    {
        TryGetComponent(out Animator);
    }

    public void GoToFishing()
    {
        FMODUnity.RuntimeManager.PlayOneShot(TransitionSound, transform.position);
        Animator.Play("Fishing");
    }

    public void GoToOverview()
    {
        FMODUnity.RuntimeManager.PlayOneShot(TransitionSound, transform.position);
        Animator.Play("Overview");
    }

    public void GoToTracking(Transform tracked)
    {
        Animator.Play("Track");
        TrackingCam.Follow = tracked;
    }
}
