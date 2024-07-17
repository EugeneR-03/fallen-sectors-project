using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExperienceController : MonoBehaviour
{
    private bool _isGathered;
    public float ExperienceCount { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        _isGathered = false;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.CompareTag("Player") && !_isGathered) {
            _isGathered = true;
            other.gameObject.GetComponent<PlayerController>().GatherExperience(ExperienceCount);
            Destroy(gameObject);
        }
    }
}
