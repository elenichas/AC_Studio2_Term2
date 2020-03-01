using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Randomizations;
using System.Linq;

public class Individual : ChromosomeBase
{

    public Individual(string HouseProg, List<Mesh> FinalMeshes) : base(HouseProg.Length)
    {
        var shuffledProg ="";
        this.FinalMeshes = FinalMeshes;
        var letters = HouseProg.ToList();

        var programIdx = RandomizationProvider.Current.GetUniqueInts(Length, 0, Length);

        for (int i = 0; i < Length; i++)
        {
            ReplaceGene(i, new Gene(letters[i]));
            shuffledProg += letters[i];
        }
        this.HouseProg = shuffledProg;
    }

    public string HouseProg { get; private set; }
    public List<Mesh> FinalMeshes { get; private set; }

    public override IChromosome CreateNew()
    {
        return new Individual(HouseProg, FinalMeshes);
    }

    public override Gene GenerateGene(int geneIndex)
    {
        return new Gene(HouseProg[RandomizationProvider.Current.GetInt(0, Length)]);
    }
}
