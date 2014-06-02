using UnityEngine;
using System.Collections;

public class GuyFPS : MonoBehaviour {
    public float updateInterval = 0.5F;

    private float accum = 0; // FPS accumulated over the interval
    private int frames = 0; // Frames drawn over the interval
    private float timeleft; // Left time for current interval
    private float longestFrame;

    private string format;

    void Start(){
        timeleft = updateInterval;
        longestFrame = 0.0f;
    }

    void Update(){
        timeleft -= Time.deltaTime;
        accum += Time.timeScale / Time.deltaTime;
        ++frames;
        if (Time.deltaTime > longestFrame) longestFrame = Time.deltaTime;

        // Interval ended - update GUI text and start new interval
        if (timeleft <= 0.0){
            // display two fractional digits (f2 format)
            float fps = accum / frames;
            format = System.String.Format("{0:F2} fps", fps);

            //  DebugConsole.Log(format,level);
            timeleft = updateInterval;
            accum = 0.0F;
            frames = 0;
        }
    }

    void OnGUI(){
        GUI.Label(new Rect(Screen.width - 50, Screen.height / 2 - 30, 50, 20), format);
    }
}
