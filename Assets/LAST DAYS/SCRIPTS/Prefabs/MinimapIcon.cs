// using UnityEngine;

// public class MinimapIcon : MonoBehaviour
// {
//     [Header("Object thật ngoài world")]
//     public Transform target;

//     public bool alwaysPinned = false;

//     private Transform player;
//     private Camera minimapCam;

//     public float visibleRange = 40f;
//     public float edgeDistance = 18f;

//     private float iconY;

//     void Start()
//     {
//         player = GameObject.FindWithTag("Player")?.transform;

//         GameObject camGO = GameObject.Find("MiniMapCamera");

//         if (camGO != null)
//             minimapCam = camGO.GetComponent<Camera>();

//         iconY = transform.position.y;
//     }

//     void LateUpdate()
//     {
//         if (target == null || player == null || minimapCam == null)
//             return;

//         float distance = Vector3.Distance(player.position, target.position);

//         // target gần
//         if (distance <= visibleRange && !alwaysPinned)
//         {
//             transform.position = new Vector3(
//                 target.position.x,
//                 iconY,
//                 target.position.z
//             );
//         }
//         // target xa -> ghim mép
//         else
//         {
//             Vector3 dir = (target.position - player.position).normalized;

//             Vector3 camCenter = minimapCam.transform.position;

//             Vector3 pinnedPos = new Vector3(
//                 camCenter.x + dir.x * edgeDistance,
//                 iconY,
//                 camCenter.z + dir.z * edgeDistance
//             );

//             transform.position = pinnedPos;
//         }
//     }
// }