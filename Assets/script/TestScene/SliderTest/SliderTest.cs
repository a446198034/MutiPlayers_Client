using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class SliderTest : MonoBehaviour {

    public Slider slider;
    public Text text;
    public InputField Inpr;
    public GameObject go;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        text.text = slider.value.ToString();

        List<GameObject> goList = go.GetComponent<CubeManager>().CubeObjList;

        for (int i = 0; i < goList.Count; i++)
        {
            goList[i].GetComponent<CubeController>().LeapSpeed = slider.value;
            goList[i].GetComponent<CubeController>().LeapMinDistance = float.Parse(Inpr.text);
        }

        

	}
}
