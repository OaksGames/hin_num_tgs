using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        iTween.MoveTo(this.gameObject, iTween.Hash("path", iTweenPath.GetPath("New Path 1"), "time", 6,"easetype",iTween.EaseType.linear));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
