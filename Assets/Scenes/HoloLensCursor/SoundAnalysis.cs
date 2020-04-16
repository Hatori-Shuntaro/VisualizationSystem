using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundAnalysis : MonoBehaviour {

    private int samplingNumber = 256;

    private AudioSource audio;

    private LineRenderer lineRenderer;

    private Vector3 startPosition;

    private Vector3 endPosition;

    private float[] samplingDatas;

    private float gain = 200f;

    private int sectionOne, sectionTwo, sectionThree, sectionFour;

    private int[] index;

	// Use this for initialization
	void Start () {
        audio = GetComponent<AudioSource>();
        lineRenderer = GetComponent<LineRenderer>();

        startPosition = lineRenderer.GetPosition(0);
        endPosition = lineRenderer.GetPosition(lineRenderer.positionCount - 1);

        samplingDatas = new float[samplingNumber];
        index = new int[samplingNumber];
	}
	
	// Update is called once per frame
	void Update () {
        AudioListener.GetSpectrumData(samplingDatas, 0, FFTWindow.Hamming);

        int levelCount = samplingDatas.Length;
        int maxIndex = 0;
        float maxValue = 0f;
        Vector3[] positions = new Vector3[levelCount];
        
        for(int i = 0; i < levelCount; i++)
        {
            positions[i] = startPosition + (endPosition - startPosition) * (float)i / (float)(levelCount - 1);
            positions[i].y += samplingDatas[i] * gain;

            // 周波数成分の中から最も大きくなっている周波数を計算する
            float value = samplingDatas[i];

            if(value > maxValue)
            {
                maxValue = value;
                maxIndex = i;
            }
        }

        Debug.Log("---------------------------------------");
        Debug.Log("最も大きい周波数のindex番号:" + maxIndex);
        Debug.Log("---------------------------------------");

        float maxFrequency = maxIndex * AudioSettings.outputSampleRate / 2 / samplingDatas.Length;
        index[maxIndex]++;

        if(maxFrequency > 0 & maxFrequency < 500)
        {
            sectionOne++;
        }
        else if(maxFrequency >= 500 & maxFrequency < 1000)
        {
            sectionTwo++;
        }
        else if(maxFrequency >= 1000 & maxFrequency < 1500)
        {
            sectionThree++;
        }
        else if(maxFrequency >= 1500 & maxFrequency < 2000)
        {
            sectionFour++;
        }

        Debug.Log("0-500:" + sectionOne + "  500-1000:" + sectionTwo + "  1000-1500:" + sectionThree + "  1500-2000:" + sectionFour);

        int max = 0;
        for(int i = 0; i < index.Length; i++)
        {
            int value = index[i];
            if(value > max)
            {
                max = value;
                maxIndex = i;
            }
        }
        Debug.Log("maxIndex:" + maxIndex + "  value:" + max + "  freq:" + maxIndex * AudioSettings.outputSampleRate / 2 / samplingDatas.Length);

        lineRenderer.positionCount = levelCount;
        lineRenderer.SetPositions(positions);
	}
}
