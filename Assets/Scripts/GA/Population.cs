using System;
using System.Collections;
 
using System.Collections.Generic;
using UnityEngine;



/// <summary>
/// Summary description for Population.
/// </summary>
public class Population
	{

		const int kLength = 5;
		const int kCrossover = kLength/2;
		const int kInitialPopulation = 1000;
		const int kPopulationLimit = 1000;
		const int kMin = 1;
		const int kMax = 10;
		const float  kMutationFrequency = 0.10f;
		const float  kDeathFitness = 0.00f;
		const float  kReproductionFitness = 0.0f;

		ArrayList Genomes = new ArrayList();
		ArrayList GenomeReproducers  = new ArrayList();
		ArrayList GenomeResults = new ArrayList();
		ArrayList GenomeFamily = new ArrayList();

        
		int		  CurrentPopulation = kInitialPopulation;
		int		  Generation = 1;
		bool	  Best2 = true;

        public int HousePorgramLength;
         public string HouseProg;
         public List<Mesh> FinalMeshes;



        public Population(int HousePorgramLength,string HouseProg,List <Mesh> FinalMeshes)
		{
          this.HousePorgramLength = HousePorgramLength;
          this.HouseProg = HouseProg;
           this.FinalMeshes = FinalMeshes;


            for  (int i = 0; i < kInitialPopulation; i++)
			{
				ListGenome aGenome = new ListGenome(HousePorgramLength,HouseProg,FinalMeshes);
				aGenome.SetCrossoverPoint(kCrossover);
				aGenome.CalculateFitness();
				Genomes.Add(aGenome);
			}

		}

		private void Mutate(Genome aGene)
		{
			if (ListGenome.TheSeed.Next(100) < (int)(kMutationFrequency * 100.0))
			{
			  	aGene.Mutate(HouseProg );
			}
		}

		public void NextGeneration()
		{
			// increment the generation;
			Generation++; 


			// check who can die
			for  (int i = 0; i < Genomes.Count; i++)
			{
				if  (((Genome)Genomes[i]).CanDie(kDeathFitness))
				{
					Genomes.RemoveAt(i);
					i--;
				}
			}


			// determine who can reproduce
			GenomeReproducers.Clear();
			GenomeResults.Clear();
			for  (int i = 0; i < Genomes.Count; i++)
			{
				if (((Genome)Genomes[i]).CanReproduce(kReproductionFitness))
				{
					GenomeReproducers.Add(Genomes[i]);			
				}
			}
			
			// do the crossover of the genes and add them to the population
           // DoCrossover(GenomeReproducers);

			//Genomes = (ArrayList)GenomeResults.Clone();

			// mutate a few genes in the new population
			for  (int i = 0; i < Genomes.Count; i++)
			{
				Mutate((Genome)Genomes[i]);
			}

			// calculate fitness of all the genes
			for  (int i = 0; i < Genomes.Count; i++)
			{
				((Genome)Genomes[i]).CalculateFitness();
			}


               

			// kill all the genes above the population limit
			if (Genomes.Count > kPopulationLimit)
				Genomes.RemoveRange(kPopulationLimit, Genomes.Count - kPopulationLimit);
			
			CurrentPopulation = Genomes.Count;
			
		}

		public void CalculateFitnessForAll(ArrayList genes)
		{
			foreach(ListGenome lg in genes)
			{
			  lg.CalculateFitness();
			}
		}

	 

		public  List <string> WriteNextGeneration()
		{

          List<string> str = new List<string>();
          
			for  (int i = 0; i <  CurrentPopulation ; i++)
			{
                 Debug.Log(((Genome)Genomes[i]).ToString())  ;         
			  str.Add(((Genome)Genomes[i]).ToString());
            
            }
        
          return str;
            			 
		}
	}

