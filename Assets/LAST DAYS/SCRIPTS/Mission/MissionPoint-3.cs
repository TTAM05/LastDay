using System.Collections;
using UnityEngine;

public class MissionPoint3 : MonoBehaviour
{
   
    private bool activated;

    private void OnTriggerEnter(Collider other)
    {
        if (activated) return;

        if (other.CompareTag("Player"))
        {
            activated = true;

            Debug.Log("Explore Triggered (MissionPoint3)");

            if (MissionSystem.Instance != null)
            {
                MissionSystem.Instance.DisableMaker();
                Debug.Log("Marker Disabled (MissionPoint3)");
            }

            if (GameManager.Instance != null)
                GameManager.Instance.UIMission(3);

            HunterDialogueZone dialogue = FindObjectOfType<HunterDialogueZone>();

            if (dialogue != null)
                dialogue.UnlockSecondDialogue();    

            gameObject.SetActive(false);
        }
    }

}
