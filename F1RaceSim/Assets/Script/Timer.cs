using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI bestTimeText;
    private float elapsedTime = 0f;
    private float bestTime = Mathf.Infinity; // Initialize with infinity to ensure the first lap time is saved
    private bool isTiming = false;

    void Update()
    {
        if (isTiming)
        {
            // Update elapsed time
            elapsedTime += Time.deltaTime;

            // Calculate minutes, seconds, and milliseconds
            int minutes = (int)(elapsedTime / 60);
            int seconds = (int)(elapsedTime % 60);
            int milliseconds = (int)((elapsedTime - (minutes * 60) - seconds) * 1000);

            // Format the timer text
            string timerString = "Time:" + string.Format("{0}'{1:00}.{2:000}", minutes, seconds, milliseconds);

            // Update the timer text
            timerText.text = timerString;
        }

        // Check if best time is set
        if (PlayerPrefs.HasKey("BestTime"))
        {
            // Get the best time from PlayerPrefs
            bestTime = PlayerPrefs.GetFloat("BestTime");
            bestTimeText.text = "Best: " + FormatTime(bestTime);
        }
        else
        {
            // If best time is not set, display default text
            bestTimeText.text = "Best: 0'00.000";
        }
    }


    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!isTiming)
            {
                // Start the timer
                isTiming = true;
            }
            else
            {
                // Stop the timer
                isTiming = false;

                // Save the best time if it's better than the current best time
                if (elapsedTime < bestTime)
                {
                    bestTime = elapsedTime;
                    PlayerPrefs.SetFloat("BestTime", bestTime);
                }
            }
        }
    }

    // Helper function to format time as a string
    string FormatTime(float time)
    {
        // Calculate minutes, seconds, and milliseconds
        int minutes = (int)(time / 60);
        int seconds = (int)(time % 60);
        int milliseconds = (int)((time - (minutes * 60) - seconds) * 1000);

        // Format the timer text
        return string.Format("{0}'{1:00}.{2:000}", minutes, seconds, milliseconds);
    }
}
