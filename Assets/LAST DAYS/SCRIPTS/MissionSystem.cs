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
        // xóa marker cũ
        if(currentMarker != null)
        {
            Destroy(currentMarker);
        }

        // spawn marker mới
        currentMarker = Instantiate(
            markerPrefab,
            targets[currentTargetIndex].position,
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