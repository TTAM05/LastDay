using System.Collections;
using UnityEngine;

public class MissionPoint2 : MonoBehaviour
{
    public MissionSystem missionSystem;
    public GameObject missionUI;
    private bool activated;

    private void OnTriggerEnter(Collider other)
    {
        if (activated) return;

        if (other.CompareTag("Player"))
        {
            activated = true;

            Debug.Log("Explore Triggered (MissionPoint2)");

            if (missionSystem != null)
            {
                missionSystem.DisableMaker();
                Debug.Log("Marker Disabled (MissionPoint2)");
            }

            // Hiện UI nhiệm vụ
            GameManager.Instance.UIMission(2);
        }
    }

}
