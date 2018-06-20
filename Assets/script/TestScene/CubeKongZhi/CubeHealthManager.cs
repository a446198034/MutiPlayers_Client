using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CubeHealthManager : MonoBehaviour {

    public int MyHealth = 100;

    public Slider healthSlider;
	// Use this for initialization
	void Start () {
		
	}

    public void getHurt(int p)
    {
        MyHealth -= p;
        healthSlider.value = MyHealth;

        if (MyHealth <= 0)
        {
            //game over
        }
    }

}
