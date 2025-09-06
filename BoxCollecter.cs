using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BoxCollecter : MonoBehaviour
{
    private int BoxCount = 0;
    public TextMeshProUGUI BoxCountText;
    public AudioSource collectSound;
    void Start()
    {
        UpdateBoxUI();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Box"))
        {
            Debug.Log("Player touched a box!");
            BoxCount++;
            if (collectSound != null)
            {
                collectSound.Play();
            }
            Destroy(other.gameObject);
            UpdateBoxUI();
        }
    }

    void UpdateBoxUI()
    {
        if (BoxCountText != null)
            BoxCountText.text = "Score: " + BoxCount;
    }
}
