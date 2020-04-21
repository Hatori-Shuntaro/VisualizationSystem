using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HoloToolkit.Unity.InputModule;
using HoloToolkit.Unity.SpatialMapping;

public class DefectHittingSound : MonoBehaviour {
    
    public Text debugText;

    // 叩いた場所に生成するプレハブ
    public GameObject hittingMarkPrefab;

    public GameObject cursor;

    // 最終的な音量にかける倍率
    private float gain = 200f;

    // 前フレームの音量と比較したときの閾値
    private float threshold = 0.5f;

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
            Debug.Log(samplingTime);

            audioSource.clip = Microphone.Start(deviceName, true, samplingTime, minFrequency);
            while (!(Microphone.GetPosition(deviceName) > 0)) { }
            audioSource.Play();
        }
    }
	
	// Update is called once per frame
	void Update () {
        audioSource.GetSpectrumData(samplingValues, 1, FFTWindow.Hamming);

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
            debugText.text = counter + "hit\n";
            debugText.text += "Position: " + cursor.transform.position + "\n";
            debugText.text += "Rotation: " + cursor.transform.rotation * Quaternion.Euler(new Vector3(90, 0, 0));
            GameObject markPrefab = Instantiate(hittingMarkPrefab, cursor.transform.position, cursor.transform.rotation * Quaternion.Euler(new Vector3(90, 0, 0)));

            // カーソルの方向にRayを飛ばす
            //RaycastHit hitInfo;

            //bool hit = Physics.Raycast(GazeManager.Instance.GazeOrigin, GazeManager.Instance.GazeNormal, out hitInfo, 20f, SpatialMappingManager.Instance.LayerMask);

            //if (hit)
            //{
            //    // 表面から少し浮かせる
            //    float hoverDistance = 0.15f;
            //    Vector3 placementPosition = hitInfo.point + (hoverDistance * hitInfo.normal);
            //    // 壁の方向を向かせる
            //    Quaternion rotation = Quaternion.LookRotation(hitInfo.normal, Vector3.up) * Quaternion.Euler(new Vector3(90, 0, 0));
            //    // 印を生成
            //    GameObject markPrefab = Instantiate(hittingMarkPrefab, placementPosition, rotation);
            //}
        }

        lastVolume = volume;
    }
}
