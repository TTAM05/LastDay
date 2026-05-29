// // MinimapTarget.cs - gắn vào Player, Zombie, MissionPoint...
// using UnityEngine;

// public class MinimapTarget : MonoBehaviour
// {
//     public Sprite iconn;
//     public float iconSize = 20f;
//     public Color iconColor ;
//     public bool alwaysPinned = false; // MissionPoint luôn ghim
    
//     [HideInInspector] public RectTransform uiIcon; // UI icon tương ứng

//     void OnEnable()
//     {
//         MinimapSystem.Instance?.RegisterTarget(this);
//     }

//     void OnDisable()
//     {
//         MinimapSystem.Instance?.OnTargetDestroyed(this);
//     }
// }