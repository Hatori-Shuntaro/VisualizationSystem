using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

/// <summary>
/// テキストファイルからデータを読み込むスクリプト
/// </summary>
public class LoadTextData : MonoBehaviour {

    public GameObject markPrefab;

    public GameObject origin;

    private string filePath = "D:/Test.csv";

    // Use this for initialization
    void Start ()
    {
        try
        {
            using (StreamReader sr = new StreamReader(new FileStream(filePath, FileMode.Open)))
            {
                string line;

                while ((line = sr.ReadLine()) != null)
                {
                    char[] charSeparators = new char[] { ',' };
                    string[] arr = line.Split(charSeparators, StringSplitOptions.None);
                    Debug.Log("x:" + arr[0] + " y:" + arr[1]);

                    float valueX = float.Parse(arr[0]);
                    float valueY = float.Parse(arr[1]);

                    // マークの作成
                    GameObject mark = Instantiate(markPrefab, Vector3.zero, Quaternion.identity * Quaternion.Euler(-90.0f, 0f, 0f), origin.transform);
                    mark.transform.localPosition = new Vector3(valueX / 1000.0f, valueY / 1000.0f, -0.01f);
                }
            }
        }
        catch(Exception e)
        {
            Debug.Log("The file could not be read: ");
            Debug.Log(e.Message);
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
