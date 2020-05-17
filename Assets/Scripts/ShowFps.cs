
using UnityEngine;
using UnityEngine.UI;

public class ShowFps : MonoBehaviour
{
    public float updateInterval = 0.5F;
    public Text guiText;

    private float accum = 0; // FPS accumulated over the interval
    private int frames = 0; // Frames drawn over the interval
    private float timeleft; // Left time for current interval

    void Start()
    {
        if (!guiText)
        {
            //Debug.Log("UtilityFramesPerSecond needs a GUIText component!");
            enabled = false;
            return;
        }
        timeleft = updateInterval;
    }

    void Update()
    {
        timeleft -= Time.deltaTime;
        accum += Time.timeScale / Time.deltaTime;
        ++frames;

        // Interval ended - update GUI text and start new interval
        if (timeleft <= 0.0)
        {
            // display two fractional digits (f2 format)
            float fps = accum / frames;
            string format = System.String.Format("fps: {0:F2} ", fps);
            guiText.text = format;

            if (fps < 30)
                guiText.material.color = Color.yellow;
            else
                if (fps < 10)
                    guiText.material.color = Color.red;
            else
                guiText.material.color = Color.green;
            //	DebugConsole.Log(format,level);
            timeleft = updateInterval;
            accum = 0.0F;
            frames = 0;
        }
    }
}
