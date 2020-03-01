using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Population
{
    const int kCrossover = 3;
    const int kInitialPopulation = 50;
    const int kPopulationLimit = 50;

    const float kMutationFrequency = 0.10f;
    const float kDeathFitness = 0.0f;
    const float kReproductionFitness = 30.0f;

    //In ArrayList we can store different datatype variables.
    ArrayList Genomes = new ArrayList();
    ArrayList GenomeReproducers = new ArrayList();
    ArrayList GenomeResults = new ArrayList();
    ArrayList GenomeFamily = new ArrayList();

    int CurrentPopulation = kInitialPopulation;
    int Generation = 1;
    bool Best2 = true;

    public int HousePorgramLength;
    public string HouseProg;
    public List<Mesh> FinalMeshes;
    public List <string> GenomesList;


    public Population(int HousePorgramLength, string HouseProg, List<Mesh> FinalMeshes, List<string> GenomesList)
    {
        this.HousePorgramLength = HousePorgramLength;
        this.HouseProg = HouseProg;
        this.FinalMeshes = FinalMeshes;
        this.GenomesList = GenomesList;


        for (int i = 0; i < kInitialPopulation; i++)
        {
            ListGenome aGenome = new ListGenome(HousePorgramLength, HouseProg, FinalMeshes, GenomesList);
            aGenome.SetCrossoverPoint(kCrossover);
            aGenome.CalculateFitness();
            Genomes.Add(aGenome);
          
             
        }
        Debug.Log(Genomes.Count+"the number in constructor");
    }

    private Genome Mutate(Genome aGene)
    {
        if (ListGenome.TheSeed.Next(100) < (int)(kMutationFrequency * 100.0))
        {
            ((ListGenome)aGene).Mutate( );
        }
        return aGene;
    }

    public void NextGeneration()
    {
        // increment the generation;
       
        Debug.Log("this is generation " + Generation);
        // check who can die
        Debug.Log(Genomes.Count+"the number in Next Generation");
        //// for (int i = 0; i < Genomes.Count; i++)
        // {
        //  if (((Genome)Genomes[i]).CanDie(kDeathFitness))
        //  {
        //   Genomes.RemoveAt(i);
        //   i--;
        // }
        // }

        // determine who can reproduce
        GenomeReproducers = new ArrayList();
        GenomeResults=new ArrayList();
        Debug.Log("genome count after clearing other arraylists" + Genomes.Count);
        //    for (int i = 0; i < Genomes.Count; i++)
        //    {
        //        if (((ListGenome)Genomes[i]).CanReproduce(kReproductionFitness))
        //        {
        //            GenomeReproducers.Add(Genomes[i]);
        //    }

        //}
        GenomeReproducers = new ArrayList(Genomes);
        // do the crossover of the genes and add them to the population
        DoCrossover(GenomeReproducers);
        Debug.Log("this is count after crossover" +Genomes.Count);
        Debug.Log("this is GenomeReproducers count after crossover" + GenomeResults.Count);

        Genomes = GenomeResults;
        Debug.Log("this is count after cloning " + Genomes.Count);

        // mutate a few genes in the new population
        for (int i = 0; i < Genomes.Count; i++)
        {
           Genomes[i]= Mutate((ListGenome)Genomes[i]);
        }
        Debug.Log("this is count after mutate" + Genomes.Count);

        // calculate fitness of all the genes
        for (int i = 0; i < Genomes.Count; i++)
        {
             
            ((ListGenome)Genomes[i]).CalculateFitness();
        }
        Debug.Log("this is count after calc fitness" + Genomes.Count);
        // kill all the genes above the population limit
        //if (Genomes.Count > kPopulationLimit)

            //Genomes.RemoveRange(kPopulationLimit, Genomes.Count - kPopulationLimit);
        CurrentPopulation = Genomes.Count;
        Generation++;
    }

    public void CalculateFitnessForAll(ArrayList genes)
    {
        foreach (ListGenome lg in genes)
        {
            lg.CalculateFitness();
        }
    }

    public void DoCrossover(ArrayList genes)
    {
        Debug.Log("genes coming in crossover " + genes.Count);

        ArrayList GeneMoms = new ArrayList();
        ArrayList GeneDads = new ArrayList();

        for (int i = 0; i < genes.Count; i++)
        {
            GeneMoms.Add(genes[ListGenome.TheSeed.Next(genes.Count)]);
            GeneDads.Add(genes[ListGenome.TheSeed.Next(genes.Count)]);
        }
        Debug.Log("genes moms count " + GeneMoms.Count);
        Debug.Log("genes dads count " + GeneDads.Count);

        // now cross them over and add them according to fitness
        for (int i = 0; i < GeneDads.Count; i++)
        {
            // pick best 2 from parent and children
            ListGenome babyGene1 = (ListGenome)((ListGenome)GeneDads[i]).Crossover((ListGenome)GeneMoms[i]);
            ListGenome babyGene2 = (ListGenome)((ListGenome)GeneMoms[i]).Crossover((ListGenome)GeneDads[i]);

        GenomeFamily.Clear();

            GenomeFamily.Add(GeneDads[i]);
            GenomeFamily.Add(GeneMoms[i]);
            GenomeFamily.Add(babyGene1);
            GenomeFamily.Add(babyGene2);
            CalculateFitnessForAll(GenomeFamily);
            GenomeFamily.Sort();

            //if (Best2 == true)
            //{
                // if Best2 is true, add top fitness genes
                GenomeResults.Add(GenomeFamily[0]);
                //GenomeResults.Add(GenomeFamily[1]);

            //}
            //else
            //{
            //    GenomeResults.Add(babyGene1);
            //    //GenomeResults.Add(babyGene2);
            //}
        }


        Debug.Log("debug from inside crossover" + GenomeResults.Count);
    }


    public void WriteNextGeneration()
    {
         // Debug.Log(Generation);
         // Debug.Log(CurrentPopulation);
          for (int i = 0; i < CurrentPopulation; i++)
          {
                ((Genome)Genomes[i]).ToDictionary();
          }      

    }
}
