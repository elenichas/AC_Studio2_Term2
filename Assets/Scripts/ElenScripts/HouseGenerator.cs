using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine.UI;
using System.IO;
using UnityEditor;

public class HouseGenerator : MonoBehaviour
{
    //The parent object who stores all the Meshes of the house
    public Transform RoomParent;
   
    //The final meshes to be visualized(the house representation)
    List<Mesh> FinalMeshes = new List<Mesh>();
   
    //Materials for each Room, set the materials in the inspector
    public Material[] myMaterials = new Material[20];

    //the list for graph representation
    List<double> fitnessessum = new List<double>();

    //UI
    public GameObject CanvasB;
    public GameObject CanvasP;
    public Text myOtherText;
    public Text myText;
    public Slider mSlider;
    public Slider bSlider;
    public ProgressBar Pb;


    //Initialize 
    string Rule = "";
    string HouseProg = "";
    int num = 0;

    public GameObject forGraph;
    public House myHouse;
    public Population TestPopulation;
    public Combinatorics test;


    //Prefabs
    public GameObject k; public GameObject b; public GameObject l;
    public GameObject n; public GameObject o; public GameObject w;
    public GameObject h; public GameObject s;
    public GameObject room;
    
   
    void Start()
    {
        Debug.Log(num.ToString());
        myHouse = new House(Rule,num);
        myHouse.CreateRooms();
        myHouse.GetFinalRooms();

        foreach (Room item in myHouse.GetFinalRooms())
        {
            Mesh rect2 = item.Rec;
            FinalMeshes.Add(rect2);
        }     
    }
   
    void Update()
    {

    }
    //Restarts the whole application
    public void Kill()
    {           
        myText.text = "";
        mSlider.value = 0;
        bSlider.value = 0;
        myOtherText.text = "";
        Pb.BarValue = 0;
        fitnessessum.Clear();

        var clones = GameObject.FindGameObjectsWithTag("Finish");
        foreach (var clone in clones)
        Destroy(clone); 
        
    }

    //assign value to slider
    public void AssignNum()
    {
        num = (int) mSlider.value;
        Debug.Log("pink"+ num.ToString());
    }

    //assign value to ther slider
    public void AssignNumOther()
    {
        num = (int)bSlider.value;
        Debug.Log("blue"+num.ToString());     
    }

    //Create the house DIRECTLY from the user input Rules(A,B,C,D...) 
    public void MakeRule(string st)
    {
        FinalMeshes.Clear();
        Rule = st;
        myHouse = new House(Rule, num);
        myHouse.CreateRooms();
        myHouse.GetFinalRooms();

        //For each Room instance get the mesh
        foreach (Room item in myHouse.GetFinalRooms())
        {
            Mesh rect2 = item.Rec;
            FinalMeshes.Add(rect2);
        }
        //output message with te number of rooms
        myOtherText.text = $"{myHouse.GetRoomNum()}" + " ROOMS";

    }

    //STEP 1
    public void MakeHouse()
    {
        myText.text = " ";
        FinalMeshes.Clear();
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
                    GetHouseAsMesh(4);
                else if (HouseProg.Length <= 3)
                    GetHouseAsMesh(2);
                else
                    GetHouseAsMesh(3);
            }
            DrawHouse();
            myOtherText.text = $"{Rule +" WITH "+myHouse.GetRoomNum()}" + " ROOMS";
        }
        else
        {
            //error window pops up when you mistype rooms(rooms that don't exist in the GA)
            myText.text = "WRONG INPUT,TRY AGAIN";
        }
      
        
    }

    //Creates the 3d represention of the house with prefabs instantiation
    public void DrawHouse()
    {
       
        //kill 'em all(so you don't have houses created on the to of each other)
        for (int j = 0; j < RoomParent.childCount; j++)
        {
            Destroy(RoomParent.GetChild(j).gameObject);         
        }
        
        //For each Room you made in Run() draw the mesh(floor) and the prefab(walls)
        for (int i = 0; i < myHouse.GetFinalRooms().Count; i++)
        {
            GameObject floors = new GameObject("Mesh" + $"{i}", typeof(MeshFilter), typeof(MeshRenderer), typeof(Text));
            floors.GetComponent<MeshFilter>().mesh = FinalMeshes[i];
            floors.GetComponent<MeshRenderer>().material = myMaterials[i];
            floors.tag = "Finish";
            //put all the gameobjects to a parent object
            floors.transform.SetParent(RoomParent);
             
            //this is how the house rooms are created in 3D
            Vector3 corner = RoomParent.GetChild(i).GetComponent<MeshRenderer>().bounds.max;
            float xsize = RoomParent.GetChild(i).GetComponent<MeshFilter>().mesh.bounds.size.x;
            float zsize = RoomParent.GetChild(i).GetComponent<MeshFilter>().mesh.bounds.size.z;
            room.transform.localScale = new Vector3(xsize- xsize/16, 2, zsize- zsize/14);
            GameObject RoomWalls = (GameObject)Instantiate(room, corner, Quaternion.identity);
            RoomWalls.GetComponent<MeshRenderer>().material = myMaterials[i];
            RoomWalls.tag = "Finish";
        }
    }

    //Decides wether the house should be labeled with Combinatorics or a GA
    public void DecideLabelMethod()
    {
        Debug.Log(HouseProg.Length+"the length");
        if (HouseProg.Length > 6)
        {
            RunGa();
        }
        else
        {
            RunCombinatorics();
        }
    }

    //Calls the GA and outputs the best individuals of each generation on the Graph
    public void RunGa()
    {
        
        TestPopulation = new Population(HouseProg.Length, HouseProg, FinalMeshes);
        TestPopulation.WriteNextGeneration();
        //write the inital population in a text file
        for (int i = 0; i < TestPopulation.GenomesList.Count; i++)
        {
            WriteString(TestPopulation.GenomesList[i] + " " + TestPopulation.OnlyF[i]);
        }
        //write every other generation in the text file
        for (int k = 0; k < 100; k++)
        {         
           WriteString(k.ToString());
            TestPopulation.NextGeneration();
            TestPopulation.WriteNextGeneration();
           //forGraph.GetComponent<Window_Graph>().Next(TestPopulation.OnlyF);

            for (int i = 0; i < TestPopulation.GenomesList.Count; i++)
            {                             
              WriteString(TestPopulation.GenomesList[i]+" "+TestPopulation.OnlyF[i]);              
            }
            //the list with the best individual of each generation
            fitnessessum.Add(TestPopulation.OnlyF.Sum());
        }
        //Draw the Graph for the GA
        for (int i = 0; i < fitnessessum.Count; i++)
        {
            //to fit in the graph
            fitnessessum[i] /= 100;
        }
        //to draw the graph
        forGraph.GetComponent<Window_Graph>().Next(fitnessessum);
    }

    //Calls the Combinatorics scripts to find all the permutations for the given house program
    public void RunCombinatorics()
    {

        test = new Combinatorics(FinalMeshes, HouseProg);
        test.Permutations();
        string C = test.BestCombination;
        double F = test.bestFitness;

       Debug.Log(C + F);
        test.Fitnesses.Sort();
        Debug.Log(test.Fitnesses.Count+"I AM THE COUNT");
        for (int i = 0; i <  test.Fitnesses.Count; i++)
        {
            fitnessessum.Add(test.Fitnesses[i]);
        }

        //Draw the Graph for the Comb
        for (int i = 0; i < fitnessessum.Count; i++)
         {
        //to fit in the graph
          fitnessessum[i] /= 2;
         }
        fitnessessum.Reverse();
        //to draw the graph
        forGraph.GetComponent<Window_Graph>().Next(fitnessessum);


    }

    //Updates the fitness bar value
    public void Fillthebar()
    {     
        if (HouseProg.Length > 6)
        {
            Debug.Log(TestPopulation.OnlyF[0]);
            Pb.BarValue = (float)(100 - TestPopulation.OnlyF[0]);
        }
        else
        {
            Debug.Log(test.bestFitness);
            Pb.BarValue = (float)(100 - test.bestFitness);
        }
    }
   
    //Creates 3D  representations of the labels,above the rooms by instantiating prefabs
    public void DrawLabels()
    {
         List<char> myfinallabels = new List<char>();
       
        if (HouseProg.Length > 6)
        {
            myfinallabels = TestPopulation.GenomesList[0].ToList();
        }
        else
        {
            myfinallabels = test.BestCombination.ToList();
        }

        for (int j = 0; j < RoomParent.childCount; j++)
        {
            Vector3 pos = RoomParent.GetChild(j).GetComponent<MeshRenderer>().bounds.center;
            Vector3 up = new Vector3(0, 2.0f, 0);
      
            switch (myfinallabels[j])
            {
               case 'k':
                  GameObject kk= Instantiate(k, pos + up, Quaternion.identity);
                    kk.tag = "Finish";
                    break;                    
                case 'l':
                    GameObject ll = Instantiate(l, pos+ up, Quaternion.identity);
                    ll.tag = "Finish";
                    break;
                case 'o':
                    GameObject oo = Instantiate(o, pos + up, Quaternion.identity);
                    oo.tag = "Finish";
                    break;
                case 'w':
                    GameObject ww = Instantiate(w, pos + up, Quaternion.identity);
                    ww.tag = "Finish";
                    break;
                case 'b':
                    GameObject bb = Instantiate(b, pos + up, Quaternion.identity);
                    bb.tag = "Finish";
                    break;
                case 'n':
                    GameObject nn= Instantiate(n, pos + 1.2f*up, Quaternion.identity);
                    nn.tag = "Finish";
                    break;
                case 'h':
                    GameObject hh = Instantiate(h, pos + up, Quaternion.identity);
                    hh.tag = "Finish";
                    break;
                case 's':
                    GameObject ss = Instantiate(s, pos + up, Quaternion.identity);
                    ss.tag = "Finish";
                    break;
            }          
        }
    }

    //Get the rooms defined by the user
    public void onUserInput(string st)
    {
        HouseProg = st;
    }

    //Meshes the represent the house(floors of rooms for the final 3d representation)
    void GetHouseAsMesh(int length)
    {
        //Clear the lists from previous given Rule, to visualize the next
        FinalMeshes.Clear();

        // Create the House Class Instance  
        RandomRuleGen R = new RandomRuleGen(length);
        Rule = R.MakeRule();
        myHouse = new House(Rule,num);
        myHouse.CreateRooms();
        myHouse.GetFinalRooms();

        //For each Room instance get the mesh
        foreach (Room item in myHouse.GetFinalRooms())
        {
            Mesh rect2 = item.Rec;
            FinalMeshes.Add(rect2);
        }
    }

   // checked if the house program given is valid
    private static bool IsValid(string str)
    {
        //check for valid characters in user input
        return Regex.IsMatch(str, @"^[kblwnosh]+$");
    }

    //write the GA results to an exterior text file for documentation
    [MenuItem("Tools/Write file")]
    static void WriteString(string value)
    {
        string path = "Assets/WriteGens.txt";    
        StreamWriter writer = new StreamWriter(path, true);
        writer.WriteLine(value);
        writer.Close();
    }
}
