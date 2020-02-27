using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine.UI;
using TMPro;

public class HouseGenerator : MonoBehaviour
{
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
    string first = " ";

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
        //kill 'em all
        for (int j = 0; j < RoomParent.childCount; j++)
        {
            Destroy(RoomParent.GetChild(j).gameObject);
        }

        //For each Room you made in Run() draw the mesh
        for (int i = 0; i < myHouse.GetFinalRooms().Count; i++)
        {
            
            GameObject gameob = new GameObject("Mesh"+ $"{i}", typeof(MeshFilter), typeof(MeshRenderer),typeof(Text));
            gameob.GetComponent<MeshFilter>().mesh = FinalMeshes[i];
            gameob.GetComponent<MeshRenderer>().material = myMaterials[i];
            
            //put all the gameobjects to a parent object
            gameob.transform.SetParent(RoomParent);      
        }
    }

   
    //STEP 3
    public void RunGa()
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
        myText.text = first;

    }

    //STEP 4
    public void DrawLabels()
    {
        for (int i = 0; i < FinalMeshes.Count; i++)
        {


        }
    }

    //Get the rooms defined by the user
    public void onUserInput(string st)
    {
        HouseProg = st;
    }
      

    void GetHouseAsMesh( int length)
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
}
