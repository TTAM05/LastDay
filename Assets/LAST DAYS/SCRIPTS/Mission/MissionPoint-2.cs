using System.Collections;
using UnityEngine;

public class MissionPoint2 : MonoBehaviour
{
   
    private bool activated;

    private void OnTriggerEnter(Collider other)
    {
        if (activated) return;

        if (other.CompareTag("Player"))
        {
            activated = true;

            Debug.Log("Explore Triggered (MissionPoint2)");

           
            MissionSystem.Instance.DisableMaker();
            Debug.Log("Marker Disabled (MissionPoint2)");
            

            // Hiện UI nhiệm vụ
            GameManager.Instance.UIMission(1);
        }
    }

}
