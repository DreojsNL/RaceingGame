using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    public TextMeshProUGUI timerText;
    private float elapsedTime = 0f;

    void Update()
    {
        // Update elapsed time
        elapsedTime += Time.deltaTime;

        // Calculate minutes, seconds, and milliseconds
        int minutes = (int)(elapsedTime / 60);
        int seconds = (int)(elapsedTime % 60);
        int milliseconds = (int)((elapsedTime - (minutes * 60) - seconds) * 1000);

        // Format the timer text
        string timerString = string.Format("{0}'{1:00}.{2:000}", minutes, seconds, milliseconds);

        // Update the timer text
        timerText.text = timerString;
    }
}
