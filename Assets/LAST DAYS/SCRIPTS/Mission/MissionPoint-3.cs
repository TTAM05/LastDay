using System.Collections;
using UnityEngine;

public class MissionPoint3 : MonoBehaviour
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

            Debug.Log("Explore Triggered (MissionPoint3)");

            if (missionSystem != null)
            {
                missionSystem.DisableMaker();
                Debug.Log("Marker Disabled (MissionPoint3)");
            }

            // Hiện UI nhiệm vụ
            GameManager.Instance.UIMission(3);
        }
    }

}
