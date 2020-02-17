using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading;
using System.Linq;
using System.Text.RegularExpressions;

public class HouseGenerator : MonoBehaviour
{
    //tHE final meshes to be visualized
    List<Mesh> FinalMeshes = new List<Mesh>();

    // Set the materials in the inspector
    public Material[] myMaterials = new Material[20];

    //UI
    [SerializeField] GUISkin _skin = null;
    CancellationTokenSource _cancel = new CancellationTokenSource();
    public Rect RuleRect = new Rect(0, 0, 5, 2);
    public Rect NumRect = new Rect(0,0, 5, 2);
     

    //Start with Rule to avoid null
    string Rule = "A";
    string HouseProg = "kb";
    
    House myHouse;

    // Start is called before the first frame update
    void Start()
    {
        myHouse = new House(Rule);
        myHouse.CreateRooms();
        myHouse.GetFinalRooms();
        Debug.Log(myHouse.GetFinalRooms().Count);
        foreach (Room item in myHouse.GetFinalRooms())
        {
            Mesh rect2 = item.Rec;
            FinalMeshes.Add(rect2);
            Debug.Log(FinalMeshes.Count);
        }
    }
    // Update is called once per frame
    void Update()
    {
        //For each Room you made in Run() draw the mesh
        for (int i = 0; i < myHouse.GetFinalRooms().Count; i++)
        {
          Graphics.DrawMesh(FinalMeshes[i], new Vector3(0, 0, 0), Quaternion.identity, myMaterials[i], 0);
        }
               
    }

    void OnGUI()
    {
        int i = 1;
        int s = 25;
        GUI.skin = _skin;
        

        HouseProg = GUI.TextField(new Rect(s, s * i++, 200, 20), HouseProg);
        if (GUI.Button(new Rect(s, (s * i++)+5, 200, 20), "Make House baby"))
        {
            //check for accurate Room labels
            if (isValid(HouseProg))
            {
                //Find a rule that can generate that number of rooms
                while (HouseProg.Length != myHouse.GetRoomNum())
                {
                    //try a shorter rule length for less rooms programms(it crushes otherwise)
                    if (HouseProg.Length > 5)
                        Run(4);
                    else if (HouseProg.Length <= 3)
                        Run(2);
                    else
                        Run(3);
                }
            }
            else
                Debug.Log("Some of the Rooms are not valid");
                         
        }
      
        NumRect = GUI.Window(1, NumRect, DoMyWindow, $"{myHouse.GetRoomNum()}");
        RuleRect = GUI.Window(0, RuleRect, DoMyWindow, Rule);
       
        //Optimization  
        if (GUI.Button(new Rect(s, (s * i++) + 5, 200, 20), "Generate Labels"))
        {
            //Run the GA
             Population TestPopulation = new Population(HouseProg.Length,HouseProg,FinalMeshes);
             TestPopulation.WriteNextGeneration();

             for (int j = 0; j <1000; j++)
             {
               TestPopulation.NextGeneration();
               
               TestPopulation.WriteNextGeneration();
                   
             }         
        }
       
    }
    void DoMyWindow(int windowID)
    {
        // Make the window be draggable.
        GUI.DragWindow(new Rect(0, 0, 10000, 10000));       
    }

    private void OnApplicationQuit()
    {
        _cancel.Cancel();
    }

    void Run( int length)
    {
        //Clear the lists from previous given Rule, to visualize the next
        FinalMeshes.Clear();
        _cancel.Cancel();
        _cancel = new CancellationTokenSource();

        // Create the House Class Instance  
        RandomRuleGen R = new RandomRuleGen(length);
        Rule = R.MakeRule();
        myHouse = new House(Rule);
        myHouse.CreateRooms();
        myHouse.GetFinalRooms();
            
        //For each Room instance get the mesh
        foreach (Room item in myHouse.GetFinalRooms())
        {
           Mesh rect2 = item.Rec;
           FinalMeshes.Add(rect2);
           Debug.Log(FinalMeshes.Count);
        } 
    }
    //check for valid chars in user input
    private static bool isValid(string str)
    {
        return Regex.IsMatch(str, "^[ktblwso]+$");
    }

}


