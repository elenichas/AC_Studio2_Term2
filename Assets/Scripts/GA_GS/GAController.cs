using System;
using System.Collections;
using System.Collections.Generic;
using GeneticSharp.Domain;
using UnityEngine;
using System.Text.RegularExpressions;
using UnityEngine.UI;
using TMPro;
using GeneticSharp.Domain;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Populations;
using GeneticSharp.Domain.Selections;
using GeneticSharp.Domain.Terminations;
using GeneticSharp.Infrastructure.Framework.Threading;
using System.Threading;
using System.Linq;

public class GAController : MonoBehaviour
{
    Fitness m_fitness;
    House myHouse;
    public List<Mesh> FinalMeshes { get; private set; }
    string Rule = "";
    public string HouseProg { get; private set; }

    //UI
    public GameObject Panel;
    public Text myText;
    public Text myOtherText;

    public Transform RoomParent;
    public Material[] myMaterials = new Material[20];

    bool isInitialized;

    public void Start()
    {
        Application.runInBackground = true;
        m_gaThread = new Thread(() =>
        {
            try
            {
                Thread.Sleep(1000);
                GA.Start();
            }
            catch (Exception ex)
            {
                Debug.LogError($"GA thread error: {ex.Message}");
            }
        });

        myHouse = new House(Rule);
        FinalMeshes = new List<Mesh>();

        myHouse.CreateRooms();
        myHouse.GetFinalRooms();

        foreach (Room item in myHouse.GetFinalRooms())
        {
            Mesh rect2 = item.Rec;
            FinalMeshes.Add(rect2);
        }
        Panel.SetActive(false);
    }

    public void Initialize()
    {
        MakeHouse();
        DrawHouse();
        StartTemplate();
        isInitialized = true;
    }
    private void Update()
    {
        if (isInitialized)
        {
            UpdateTemplate();
        }
    }
    protected GeneticAlgorithm CreateGA()
    {
        m_fitness = new Fitness();
        Debug.Log(HouseProg.Length.ToString());
        var Adam = new Individual(HouseProg, FinalMeshes);
        var crossover = new OrderedCrossover();
        var mutation = new ReverseSequenceMutation();
        var selection = new RouletteWheelSelection();
        var population = new GeneticSharp.Domain.Populations.Population(50, 100, Adam);
        var ga = new GeneticAlgorithm(population, m_fitness, selection, crossover, mutation);
        ga.Termination = new TimeEvolvingTermination(System.TimeSpan.FromDays(1));
        ga.TaskExecutor = new ParallelTaskExecutor
        {
            MinThreads = 100,
            MaxThreads = 200
        };

        return ga;
    }
    protected  void StartSample()
    {
        
    }
    protected  void UpdateSample()
    {
        if (GA.Population.CurrentGeneration == null)
            return;

        var c = GA.BestChromosome as Individual;

        if (c != null)
        {
            SetFitnessText($"Distance: {c.Fitness}");
        }
    }
    public void onUserInput(string st)
    {
        HouseProg = st;
        Debug.Log(st);
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
    private static bool IsValid(string str)
    {
        //check for valid chars in user input
        return Regex.IsMatch(str, @"^[ktblwso]+$");
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

            GameObject gameob = new GameObject("Mesh" + $"{i}", typeof(MeshFilter), typeof(MeshRenderer), typeof(Text));
            gameob.GetComponent<MeshFilter>().mesh = FinalMeshes[i];
            gameob.GetComponent<MeshRenderer>().material = myMaterials[i];

            //put all the gameobjects to a parent object
            gameob.transform.SetParent(RoomParent);
        }
    }

    ////////////////////////////Genetic Sharp template stuff
    private Thread m_gaThread;
    private Text m_generationText;
    private Text m_fitnessText;
    private Text m_previousGenerationText;
    private Text m_previousFitnessText;
    private double m_previousBestFitness;
    private double m_previousAverageFitness;

    protected GeneticAlgorithm GA { get; private set; }
    protected bool ChromosomesCleanupEnabled { get; set; }
    protected bool ShowPreviousInfoEnabled { get; set; } = true;
    public Rect Area { get; private set; }

    private void StartTemplate()
    {
        ///Application.runInBackground = true;
        var sampleArea = GameObject.Find("SampleArea");
        Area = sampleArea == null
            ? Camera.main.rect
            : sampleArea.GetComponent<RectTransform>().rect;

        var generationTextGO = GameObject.Find("CurrentInfo/Background/GenerationText");

        if (generationTextGO != null)
        {
            var fitnessTextGO = GameObject.Find("CurrentInfo/Background/FitnessText");
            m_generationText = generationTextGO.GetComponent<Text>();
            m_fitnessText = fitnessTextGO.GetComponent<Text>();

            m_previousGenerationText = GameObject.Find("PreviousInfo/Background/GenerationText").GetComponent<Text>();
            m_previousFitnessText = GameObject.Find("PreviousInfo/Background/FitnessText").GetComponent<Text>();
            m_previousGenerationText.text = string.Empty;
            m_previousFitnessText.text = string.Empty;
        }

        if (m_generationText != null)
        {
            m_generationText.text = string.Empty;
            m_fitnessText.text = string.Empty;
        }

        GA = CreateGA();
        GA.GenerationRan += delegate
        {
            m_previousBestFitness = GA.BestChromosome.Fitness.Value;
            m_previousAverageFitness = GA.Population.CurrentGeneration.Chromosomes.Average(c => c.Fitness.Value);
            Debug.Log($"Generation: {GA.GenerationsNumber} - Best: ${m_previousBestFitness} - Average: ${m_previousAverageFitness}");


            if (ChromosomesCleanupEnabled)
            {
                foreach (var c in GA.Population.CurrentGeneration.Chromosomes)
                {
                    c.Fitness = null;
                }
            }
        };
        StartSample();


        m_gaThread.Start();

    }
    void UpdateTemplate()
    {
        if (m_generationText != null && GA.Population.CurrentGeneration != null)
        {
            var averageFitness = GA.Population.CurrentGeneration.Chromosomes.Average(c => c.Fitness.HasValue ? c.Fitness.Value : 0);
            var bestFitness = GA.Population.CurrentGeneration.Chromosomes.Max(c => c.Fitness.HasValue ? c.Fitness.Value : 0);

            UpdateTexts(
                m_generationText,
                m_fitnessText,
                GA.GenerationsNumber,
                bestFitness,
                averageFitness);

            if (ShowPreviousInfoEnabled && GA.GenerationsNumber > 1)
            {
                UpdateTexts(
                    m_previousGenerationText,
                    m_previousFitnessText,
                    GA.GenerationsNumber - 1,
                    m_previousBestFitness,
                    m_previousAverageFitness);
            }
        }

        UpdateSample();
    }
    private void OnDestroy()
    {
        GA.Stop();
        m_gaThread.Abort();

        m_generationText.text = String.Empty;
        m_fitnessText.text = String.Empty;

        m_previousGenerationText.text = String.Empty;
        m_previousFitnessText.text = String.Empty;
    }
    protected void SetFitnessText(string text)
    {
        if (m_fitnessText != null)
        {
            m_fitnessText.text = text;
        }
    }

    private void UpdateTexts(Text generationText, Text fitnessText, int generationsNumber, double bestFitness, double averageFitness)
    {
        generationText.text = $"Generation: {generationsNumber}";
        fitnessText.text = $"Best: {bestFitness:N2}\nAverage: {averageFitness:N2}";
    }
}
