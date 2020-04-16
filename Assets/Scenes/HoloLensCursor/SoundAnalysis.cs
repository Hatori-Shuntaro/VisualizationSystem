using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HoloToolkit.Unity.InputModule;
using HoloToolkit.Unity.SpatialMapping;

public class SoundAnalysis : MonoBehaviour {
    
    public Text debugText;

    public GameObject hittingMarkPrefab;

    // 最終的な音量にかける倍率
    private float gain = 200f;

    // 前フレームの音量と比較したときの閾値
    private float threshold = 0.5f;

    private float volume;

    // 前フレームの音量
    private float lastVolume;

    private int samplingNumber = 256;

    private float[] samplingValues;

    private AudioSource audioSource;

    private int counter = 0;

	// Use this for initialization
	void Start () {
        audioSource = GetComponent<AudioSource>();

        samplingValues = new float[samplingNumber];

        // オーディオソースとマイクがあるか確認
        if((audioSource != null) & (Microphone.devices.Length > 0))
        {
            string deviceName = Microphone.devices[0];
            int minFrequency, maxFrequency;

            // サンプリング周波数の最大・最小値を取得
            Microphone.GetDeviceCaps(deviceName, out minFrequency, out maxFrequency);

            // 適切なサンプリング時間を計算
            int samplingTime = minFrequency / samplingNumber;

            audioSource.clip = Microphone.Start(deviceName, true, samplingTime, minFrequency);
            while (!(Microphone.GetPosition(deviceName) > 0)) { }
            audioSource.Play();
        }
    }
	
	// Update is called once per frame
	void Update () {
        audioSource.GetSpectrumData(samplingValues, 0, FFTWindow.Hamming);

        float sum = 0f;
        for(int i = 0; i < samplingValues.Length; i++)
        {
            // 各周波数ごとのデータを足す
            sum += samplingValues[i];
        }

        float volume = Mathf.Clamp01(sum * gain / (float)samplingValues.Length);

        // 前フレームとの音量の差を計算
        if (volume - lastVolume > threshold)
        {
            counter++;
            Debug.Log(volume);
            debugText.text = counter + "hit";
            //RaycastHit hitInfo;

            //// カーソルの方向にRayを飛ばす
            //bool isHit = Physics.Raycast(GazeManager.Instance.GazeOrigin, GazeManager.Instance.GazeNormal, out hitInfo, 20f, SpatialMappingManager.Instance.LayerMask);
            //if (isHit)
            //{
            //    // 印を生成
            //    Instantiate(hittingMarkPrefab, hitInfo.transform.position, Quaternion.identity);
            //}
        }

        lastVolume = volume;
    }
}
