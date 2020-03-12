using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class startingcube : MonoBehaviour
{
    public Slider pink;
    public Slider blue;
    int num = 0;
    GameObject obj;
    // Start is called before the first frame update
    void Start()
    {


        Debug.Log(num + "on script");
       obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        obj.transform.localPosition= new Vector3(9, 0, 7.5f);
    }  

    // Update is called once per frame
    void Update()
    {
        num = (int)pink.value;
        obj.transform.localScale = new Vector3(num+3, 2, num);
       
    }
}
