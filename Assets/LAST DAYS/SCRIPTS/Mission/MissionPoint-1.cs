using UnityEngine;

public class MissionPoint : MonoBehaviour
{
    public MissionSystem missionSystem;
    private bool activated;

    private void OnTriggerEnter(Collider other)
    {
        if(activated) return;

        if(other.CompareTag("Player"))
        {
            activated = true;

            GameManager.Instance.StartExplore();

            Debug.Log("Explore Triggered");

            // tắt marker
            missionSystem.DisableMaker();
        }
    }
}