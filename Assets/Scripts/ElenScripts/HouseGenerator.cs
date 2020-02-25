using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;

public class HouseGenerator : MonoBehaviour
{
    //The final meshes to be visualized(the house representation)
    List<Mesh> FinalMeshes = new List<Mesh>();

    //Materials for each Room set the materials in the inspector
    public Material[] myMaterials = new Material[20];

    //UI
    [SerializeField] GUISkin _skin = null;
    CancellationTokenSource _cancel = new CancellationTokenSource();
    public Rect RuleRect = new Rect(0, 0, 5, 2);
    public Rect NumRect = new Rect(0,0, 5, 2);
    public Rect GaRect = new Rect(0, 0, 5, 2);
    public GameObject Panel;

    //Initialize to avoid null
    string Rule = "A";
    string HouseProg = "kb";
    string first = "Best Labeling";

    //Call the house class
    House myHouse;

    // Start is called before the first frame update
    void Start()
    {
        myHouse = new House(Rule);
        myHouse.CreateRooms();
        myHouse.GetFinalRooms();
        
        foreach (Room item in myHouse.GetFinalRooms())
        {
            Mesh rect2 = item.Rec;
            FinalMeshes.Add(rect2);           
        }
        Panel.SetActive(false);
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
        
        //when you press Make House BUTTON it gets the number of rooms you entered
        //and tries to find a random rule that can give that number of rooms
        HouseProg = GUI.TextField(new Rect(s, s * i++, 200, 20), HouseProg);
        if (GUI.Button(new Rect(s, (s * i++)+5, 200, 20), "Make House"))
        {

            Panel.SetActive(false);
            MakeHouse();                       
        }
      
        //output the number of rooms
        NumRect = GUI.Window(1, NumRect, DoMyWindow, $"{myHouse.GetRoomNum()}");
        //output the current Rule
        RuleRect = GUI.Window(0, RuleRect, DoMyWindow, Rule);


        //When you press Generate Labels BUTTON you run the labels Optimization  
        if (GUI.Button(new Rect(s, (s * i++) + 5, 200, 20), "Generate Labels"))
        {           
            //Run the GA
            //the dictionary that will store the Genome versions and their fitness
            Dictionary<string, float> GenomeFitnessPairs = new Dictionary<string, float>();

            Population TestPopulation = new Population(HouseProg.Length, HouseProg, FinalMeshes, GenomeFitnessPairs);
            TestPopulation.WriteNextGeneration();

            for (int k = 0; k < 100; k++)
            {
                TestPopulation.NextGeneration();
                TestPopulation.WriteNextGeneration();                       
            }

            //output the best individual after 100 generations
            first = GenomeFitnessPairs.OrderBy(kvp => kvp.Value).First().ToString();

            //check if the last is actually worse than the first I am getting
            Debug.Log(GenomeFitnessPairs.OrderBy(kvp => kvp.Value).Last().ToString());
        }
            
        //output the best individual as a pair of labels and fitness
        GaRect = GUI.Window(3, GaRect, DoMyWindow, first);
    }
      
    public void MakeHouse()
    {      
        //check for accurate Room labels
        if (IsValid(HouseProg))
        {
            //Find a rule that can generate that number of rooms
            int counter = 0;
            while (HouseProg.Length != myHouse.GetRoomNum() && counter < 100)
            {
                counter++;
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
        {
            //error window pops up when you mistype rooms(rooms that don't exist in the GA)
            Panel.SetActive(true);          
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
   
    private static bool IsValid(string str)
    {
        //check for valid chars in user input
        return Regex.IsMatch(str, @"^[ktblwso]+$");
    }
}
