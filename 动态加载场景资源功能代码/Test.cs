using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Test : MonoBehaviour {

    int Count = 0;
    Text txt;
	void Start () {
        txt = GameObject.Find("InputText").GetComponent<Text>();
	}
	
	
	void Update () {
        txt.text = "count:"+Count;
        Count++;
	}
}
