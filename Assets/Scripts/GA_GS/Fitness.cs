
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Fitnesses;
using GeneticSharp.Domain.Randomizations;
using System;

public class Fitness : IFitness
{

    public Fitness() { }
    public double Evaluate(IChromosome ind)
    {
        var total_fitness = 0.0;
        var myInd = (Individual)ind;
        var program = myInd.HouseProg.ToList();
        var FinalMeshes = myInd.FinalMeshes;
        for (int i = 0; i < program.Count; i++)
        {
            switch (program[i])
            {
                //kitchen
                case 'k':

                    double idealK = 30;
                    var areak = MeshArea(FinalMeshes[i]);
                    var rfk = Math.Abs(idealK - areak);
                    total_fitness += rfk;
                    break;

                //living room
                case 'l':

                    double idealL = 25;
                    double areal = MeshArea(FinalMeshes[i]);
                    double rfl = Math.Abs(idealL - areal);
                    total_fitness += rfl;
                    break;

                //bedroom
                case 'b':

                    double idealB = 20;
                    double areab = MeshArea(FinalMeshes[i]);
                    double rfb = Math.Abs(idealB - areab);
                    total_fitness += rfb;
                    break;

                //bathroom wc
                case 'w':

                    double idealW = 4;
                    double areaw = MeshArea(FinalMeshes[i]);
                    double rfw = Math.Abs(idealW - areaw);
                    total_fitness += rfw;
                    break;

                //office-workspace
                case 'o':

                    double idealO = 10;
                    double areao = MeshArea(FinalMeshes[i]);
                    double rfo = Math.Abs(idealO - areao);
                    total_fitness += rfo;
                    break;


                //storage
                case 's':

                    double idealS = 2.5f;
                    double areas = MeshArea(FinalMeshes[i]);
                    double rfs = Math.Abs(idealS - areas);
                    total_fitness += rfs;
                    break;
            }
        }
        // add penalty if we have repeated rooms
        var diff = program.Count - program.Distinct().Count();

        if (diff > 0)
        {
            total_fitness /= diff;
        }

        if (total_fitness < 0)
        {
            total_fitness = 0;
        }
        
        return total_fitness;
    }
    private double MeshArea(Mesh mesh)
    {
        //double Area = mesh.bounds.size.x * mesh.bounds.size.z;
        var point = mesh.vertices;
        double width = Math.Sqrt(Math.Pow(point[1].x - point[0].x, 2) + Math.Pow(point[1].y - point[0].y, 2));
        double height = Math.Sqrt(Math.Pow(point[2].x - point[1].x, 2) + Math.Pow(point[2].y - point[1].y, 2));
        return width * height;
    }
}
