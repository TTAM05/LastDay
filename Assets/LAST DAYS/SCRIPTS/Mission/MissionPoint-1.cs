using UnityEngine;

public class MissionPoint : MonoBehaviour
{
    public MissionSystem missionSystem;
    private bool activated;
    public AudioSource audioSource;
    public AudioClip Clip;

    void Start()
    {
        audioSource = GetComponentInParent<AudioSource>();
        audioSource.clip = Clip;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(activated) return;

        if(other.CompareTag("Player"))
        {
            activated = true;
            audioSource.PlayOneShot(audioSource.clip);
            GameManager.Instance.StartExplore();

            Debug.Log("Explore Triggered");

            // tắt marker
            missionSystem.DisableMaker();
        }
    }
}