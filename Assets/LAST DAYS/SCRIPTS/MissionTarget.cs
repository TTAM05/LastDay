using UnityEngine;

public class MissionTarget : MonoBehaviour
{
    public MissionSystem missionSystem;

    private bool completed;

    private void OnTriggerEnter(Collider other)
    {
        if(completed) return;

        if(other.CompareTag("Player"))
        {


            Debug.Log("Come Mission");

            missionSystem.DisableMaker();
        }
    }
}