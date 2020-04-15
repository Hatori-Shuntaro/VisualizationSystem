using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundAnalysis : MonoBehaviour {

    public float threshold = 0.4f;

    public Text debugText;

    private float oldMaxValue = 0.0f;
    
    private int counter = 0;

    private string microphoneNumber;

    private bool isInitialize;

    private AudioSource audioSource;

	// Use this for initialization
	void Start () {
        microphoneNumber = Microphone.devices[0];
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = Microphone.Start(microphoneNumber, false, 999, AudioSettings.outputSampleRate);

        while(!(Microphone.GetPosition(microphoneNumber) > 0)) { }

        audioSource.Play();
        isInitialize = true;
    }
	
	// Update is called once per frame
	void Update () {
        debugText.text = counter + " hit";
    }

    // dataの要素数は2048
    private void OnAudioFilterRead(float[] data, int channels)
    {
        if (!isInitialize)
        {
            return;
        }

        float maxValue = 0.0f;
        int dataLength = data.Length / channels;

        for (int i = 0; i < dataLength; i++)
        {
            float value = data[i];
            if (value > maxValue)
            {
                maxValue = value;
            }
        }

        if (maxValue - oldMaxValue > threshold)
        {
            counter++;
            Debug.Log("Max Value: " + maxValue + " HitNumber: " + counter);
        }

        oldMaxValue = maxValue;
    }
}
