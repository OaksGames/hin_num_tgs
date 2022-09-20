using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AutoRotate : MonoBehaviour 
{
	public static AutoRotate myScript;
	public Vector3 RotateValues;

    public static bool Is_Rot;

	void Awake()
	{
		myScript=this;
        Is_Rot = true;
    }

	void Update () 
	{
        if(Is_Rot)
		transform.Rotate(RotateValues*Time.deltaTime);
	}


}
