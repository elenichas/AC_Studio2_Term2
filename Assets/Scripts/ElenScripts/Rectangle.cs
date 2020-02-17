using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Rectangle))]
[CanEditMultipleObjects]
public class Rectangle : Editor
{ 
        //This is the value of the Slider
        float m_Value;

        public override void OnInspectorGUI()
        {
            //This is the Label for the Slider
            GUI.Label(new Rect(0, 300, 100, 30), "Rectangle Width");
            //This is the Slider that changes the size of the Rectangle drawn
            m_Value = GUI.HorizontalSlider(new Rect(100, 300, 100, 30), m_Value, 1.0f, 250.0f);

            //The rectangle is drawn in the Editor (when MyScript is attached) with the width depending on the value of the Slider
            EditorGUI.DrawRect(new Rect(50, 350, m_Value, 70), Color.green);
        }


    

}
