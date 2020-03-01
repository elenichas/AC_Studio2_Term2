using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;



/// <summary>
/// Summary description for ListGenome.
/// </summary>
public class ListGenome : Genome
{
    public static System.Random TheSeed = new System.Random((int)DateTime.Now.Ticks);
    
    public List<char> TheArray = new List<char>();

    //the string as given from the user(no commas expected)
    public string HouseProg;

    //the list of characters taken from the HouseProg
    public List<char> letters;

    public List<Mesh> FinalMeshes;

    //the dictionarywith pairs of individuals and their fitnesses
    public  List <string>  GenomeFitnessPairs;

    public override int CompareTo(object a)
    {
        ListGenome Gene1 = this;
        ListGenome Gene2 = (ListGenome)a;
        if (Gene1.CurrentFitness > Gene2.CurrentFitness)
            return -1;
        else if (Gene1.CurrentFitness < Gene2.CurrentFitness)
            return 1;
        else
            return 0;
    }


    public override void SetCrossoverPoint(int crossoverPoint)
    {
        CrossoverPoint = crossoverPoint;
    }

    public ListGenome()
    {

    }


    public ListGenome(int HouseProgramLength, string HouseProg, List<Mesh> FinalMeshes, List<string> GenomeFitnessPairs)
    {
         
        Lengthother = HouseProgramLength;
        this.HouseProg = HouseProg;
        this.FinalMeshes = FinalMeshes;
        this.GenomeFitnessPairs = GenomeFitnessPairs;
        letters = HouseProg.ToList();

        for (int i = 0; i < Lengthother; i++)
        {
            char gene = GenerateGeneValue();

            TheArray.Add(gene);

        }
    }

    public override void Initialize()
    {

    }

    public override bool CanDie(float fitness)
    {
        if (CurrentFitness >= (fitness))
        {
            return true;
        }

        return false;
    }


    public override bool CanReproduce(float fitness)
    {
        if (CurrentFitness <=(fitness))
        {
            return true;
        }

        return false;
    }


    public override char GenerateGeneValue( )
    {
        char RandRoom = letters[UnityEngine.Random.Range(0, letters.Count)];
        letters.Remove(RandRoom);

        return RandRoom;

    }

    public override void Mutate()
    {
        MutationIndex = UnityEngine.Random.Range(1, HouseProg.Length);
        TheArray.Reverse(MutationIndex - 1, MutationIndex);


    }

    private float MeshArea(Mesh mesh)
    {
        float Area = mesh.bounds.size.x * mesh.bounds.size.z;

        return Area;

    }


    private float CalculateHouseFitnessSum()
    {
        float total_fitness = 0;
        for (int i = 0; i < TheArray.Count; i++)
        {
            switch (TheArray[i])
            {
                //kitchen
                case 'k':

                    float idealK = 30;
                    float areak = MeshArea(FinalMeshes[i]);
                    float rfk = Math.Abs(idealK - areak);
                    total_fitness += rfk;
                    break;

                //living room
                case 'l':

                    float idealL = 25;
                    float areal = MeshArea(FinalMeshes[i]);
                    float rfl = Math.Abs(idealL - areal);
                    total_fitness += rfl;
                    break;

                //bedroom
                case 'b':

                    float idealB = 20;
                    float areab = MeshArea(FinalMeshes[i]);
                    float rfb = Math.Abs(idealB - areab);
                    total_fitness += rfb;
                    break;

                //bathroom wc
                case 'w':

                    float idealW = 4;
                    float areaw = MeshArea(FinalMeshes[i]);
                    float rfw = Math.Abs(idealW - areaw);
                    total_fitness += rfw;
                    break;

                //office-workspace
                case 'o':

                    float idealO = 10;
                    float areao = MeshArea(FinalMeshes[i]);
                    float rfo = Math.Abs(idealO - areao);
                    total_fitness += rfo;
                    break;


                //storage
                case 's':

                    float idealS = 2.5f;
                    float areas = MeshArea(FinalMeshes[i]);
                    float rfs = Math.Abs(idealS - areas);
                    total_fitness += rfs;
                    break;
            }
        }
        return total_fitness;
    }

    public override float CalculateFitness()
    {
        CurrentFitness = CalculateHouseFitnessSum();
        return CurrentFitness;
    }

    public override void ToDictionary()
    {
        string strResult = "";
        for (int i = 0; i < Lengthother; i++)
        {
            strResult = strResult + (TheArray[i]).ToString();
        }

        //if (!GenomeFitnessPairs.ContainsKey(strResult))
            GenomeFitnessPairs.Add(strResult);
    }

    public override void CopyGeneInfo(Genome dest)
    {
        ListGenome theGene = (ListGenome)dest;
    }

    public override Genome Crossover(Genome g)
    {
        ListGenome aGene1 = new ListGenome();
        ListGenome aGene2 = new ListGenome();
        g.CopyGeneInfo(aGene1);
        g.CopyGeneInfo(aGene2);

        ListGenome CrossingGene = (ListGenome)g;
        for (int i = 0; i < CrossoverPoint; i++)
        {
            aGene1.TheArray.Add(CrossingGene.TheArray[i]);
            aGene2.TheArray.Add(TheArray[i]);
        }
        for (int j = CrossoverPoint; j < Lengthother; j++)
        {
            aGene1.TheArray.Add(TheArray[j]);
            aGene2.TheArray.Add(CrossingGene.TheArray[j]);
        }

        // 50/50 chance of returning gene1 or gene2
        ListGenome aGene = null;
        if (TheSeed.Next(2) == 1)
        {
            aGene = aGene1;
        }
        else
        {
            aGene = aGene2;
        }

        return aGene;
    }

}










