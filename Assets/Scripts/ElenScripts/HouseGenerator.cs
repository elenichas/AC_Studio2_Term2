using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine.UI;
using TMPro;
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

    //UI
    public GameObject Panel;
    public Text myText;
    public Text myOtherText;

    //Initialize Strings
    string Rule = "";
    string HouseProg = "";


    //Call the house class
    House myHouse;

    //Call the Population class
    Population TestPopulation;

    //time for prefabs
    public GameObject k;
    public GameObject b;
    public GameObject l;
    public GameObject s;
    public GameObject o;
    public GameObject w;

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



    }

    public void MakeRule(string st)
    {
        FinalMeshes.Clear();
        Debug.Log(st+"sto make rule");
        Rule = st;
        Debug.Log(Rule + "sto make house");
        myHouse = new House(Rule);
        myHouse.CreateRooms();
        myHouse.GetFinalRooms();

        //For each Room instance get the mesh
        foreach (Room item in myHouse.GetFinalRooms())
        {
            Mesh rect2 = item.Rec;
            FinalMeshes.Add(rect2);
        }
        myOtherText.text = $"{myHouse.GetRoomNum()}" + " Rooms in " + Rule;

    }
    

    //STEP 1
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
                    GetHouseAsMesh(4);
                else if (HouseProg.Length <= 3)
                    GetHouseAsMesh(2);
                else
                    GetHouseAsMesh(3);
            }
        }
        else
        {
            //error window pops up when you mistype rooms(rooms that don't exist in the GA)
            Panel.SetActive(true);
        }
        myOtherText.text = $"{myHouse.GetRoomNum()}" + " Rooms in " + Rule;

    }

    //STEP 2
    public void DrawHouse()
    {
        //kill 'em all(so you don't have houses created on the to of each other)
        for (int j = 0; j < RoomParent.childCount; j++)
        {
            Destroy(RoomParent.GetChild(j).gameObject);
        }

        //For each Room you made in Run() draw the mesh
        for (int i = 0; i < myHouse.GetFinalRooms().Count; i++)
        {

            GameObject gameob = new GameObject("Mesh" + $"{i}", typeof(MeshFilter), typeof(MeshRenderer), typeof(Text));
            gameob.GetComponent<MeshFilter>().mesh = FinalMeshes[i];
            gameob.GetComponent<MeshRenderer>().material = myMaterials[i];

            //put all the gameobjects to a parent object
            gameob.transform.SetParent(RoomParent);
        }
    }


    //STEP 3
    public void RunGa()
    {
        // //Run the GA
        TestPopulation = new Population(HouseProg.Length, HouseProg, FinalMeshes);
        TestPopulation.WriteNextGeneration();
        for (int i = 0; i < TestPopulation.GenomesList.Count; i++)
        {
            WriteString(TestPopulation.GenomesList[i] + " " + TestPopulation.OnlyF[i]);
        }

        for (int k = 0; k < 10; k++)
        {
            WriteString(k.ToString());
            TestPopulation.NextGeneration();
            TestPopulation.WriteNextGeneration();
            for (int i = 0; i < TestPopulation.GenomesList.Count; i++)
            {
                WriteString(TestPopulation.GenomesList[i]+" "+TestPopulation.OnlyF[i]);
            }
        }

        //here should be the very best individual(it's not) 
        
        myText.text = TestPopulation.GenomesList[0]+" "+ TestPopulation.OnlyF[0];
        

    }

    //STEP 4
    public void DrawLabels()
    {
        List<char> myfinallabels = TestPopulation.GenomesList[0].ToList();
        Debug.Log(TestPopulation.OnlyF[0]);

        for (int j = 0; j < RoomParent.childCount; j++)
        {
            Vector3 pos = RoomParent.GetChild(j).GetComponent<MeshRenderer>().bounds.center;
             //GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            //obj.transform.position = pos;
            Vector3 up = new Vector3(0, 0.2f, 0);
            switch (myfinallabels[j])
            {
               case 'k':
                    Instantiate(k, pos + up, Quaternion.identity);break;
                     
                case 'l':
                    Instantiate(l, pos+ up, Quaternion.identity); break;
                case 'o':
                   Instantiate(o, pos + up, Quaternion.identity); break;
                case 'w':
                    Instantiate(w, pos + up, Quaternion.identity); break;
                case 'b':
                    Instantiate(b, pos + up, Quaternion.identity); break;
                case 's':
                    Instantiate(s, pos + up, Quaternion.identity); break;


            }
            
        }
    }

    //Get the rooms defined by the user
    public void onUserInput(string st)
    {
        HouseProg = st;
    }


    void GetHouseAsMesh(int length)
    {
        //Clear the lists from previous given Rule, to visualize the next
        FinalMeshes.Clear();


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

        }
    }

    private static bool IsValid(string str)
    {
        //check for valid chars in user input
        return Regex.IsMatch(str, @"^[ktblwso]+$");
    }


    [MenuItem("Tools/Write file")]
    static void WriteString(string g)
    {
        string path = "Assets/WriteGens.txt";

        //Write some text to the test.txt file
        StreamWriter writer = new StreamWriter(path, true);
        writer.WriteLine(g);
        writer.Close();

        //Re-import the file to update the reference in the editor
       // AssetDatabase.ImportAsset(path);
       // TextAsset asset = (TextAsset)Resources.Load(path);


    }
}
