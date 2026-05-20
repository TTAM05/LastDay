using UnityEngine;

public class MissionSystem : MonoBehaviour
{
    public Transform[] targets;

    public GameObject markerPrefab;

    private GameObject currentMarker;

    private int currentTargetIndex;

    void Start()
    {
        SpawnMarker();
    }

    void SpawnMarker()
    {
        if(currentMarker != null)
        {
            Destroy(currentMarker);
        }

        Collider col =
            targets[currentTargetIndex]
            .GetComponent<Collider>();

        Vector3 spawnPos =
            col.bounds.center;

        currentMarker = Instantiate(
            markerPrefab,
            spawnPos,
            Quaternion.identity
        );
    }

    public void CompleteCurrentMission()
    {
        currentTargetIndex++;

        // hết mission
        if(currentTargetIndex >= targets.Length)
        {
            Debug.Log("All missions complete");

            if(currentMarker != null)
            {
                Destroy(currentMarker);
            }

            return;
        }

        SpawnMarker();
    }

    public void DisableMaker()
    {
        if(currentMarker != null)
        {
            Destroy(currentMarker);
        }
    }
}