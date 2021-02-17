using Microsoft.AspNetCore.Routing.Constraints;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace ABDMatrix_WebAPI.Models
{
    public class ABDMatrix_Calculator
    {
        //Ply form functions
        public double[,] Calculate_ABDMatrix(IEnumerable<Ply> plys, Material material, out double laminateThickness)
        {
            List<double[,]> QPly = new List<double[,]>();
            List<double[,]> QLam = new List<double[,]>();
            double[] Z_Bot = new double[plys.Count()], Z_Top = new double[plys.Count()];
            laminateThickness = 0;
            Ply ply;
            if (plys.Count() != 0) laminateThickness = plys.Sum(C => C.Thickness);

            for (int iCou = 0; iCou < plys.Count(); iCou++)
            {
                ply = (Ply)plys.ElementAt(iCou);
                //Material material = CommonClasses.PlyMaterials.Where(C => C.MatName == ply.MatName).First();
                QPly.Add(Calculate_QPly(material.E12, material.E21, material.G, material.nu12));
                QLam.Add(Calculate_QLam(QPly[iCou], ply.Angle * Math.PI / 180));
                if (iCou == 0)
                { Z_Top[iCou] = ply.Thickness - laminateThickness / 2; }
                else
                { Z_Top[iCou] = Z_Top[iCou - 1] + ply.Thickness; }
                Z_Bot[iCou] = Z_Top[iCou] - ply.Thickness;
            }
            return Calculate_ABDMatx(QLam, Z_Bot, Z_Top);
        }

        private double[,] Calculate_QPly(double E12, double E21, double G, double nu12)
        {
            double nu21 = nu12 * E21 / E12;
            double[,] QMatx = new double[3, 3];
            QMatx[0, 0] = E12 / (1 - nu12 * nu21);
            QMatx[0, 1] = E12 * nu21 / (1 - nu12 * nu21);
            QMatx[1, 0] = E21 * nu12 / (1 - nu12 * nu21);
            QMatx[1, 1] = E21 / (1 - nu12 * nu21);
            QMatx[2, 2] = G;

            return QMatx;
        }

        private double[,] Calculate_QLam(double[,] QPly, double Angle)
        {
            double[,] QLam = new double[3, 3];
            QLam[0, 0] = QPly[0, 0] * Math.Pow(Math.Cos(Angle), 4) + 2 * (QPly[0, 1] + 2 * QPly[2, 2]) * Math.Pow(Math.Sin(Angle), 2) * Math.Pow(Math.Cos(Angle), 2) + QPly[1, 1] * Math.Pow(Math.Sin(Angle), 4);
            QLam[0, 1] = (QPly[0, 0] + QPly[1, 1] - 4 * QPly[2, 2]) * Math.Pow(Math.Sin(Angle), 2) * Math.Pow(Math.Cos(Angle), 2) + QPly[0, 1] * (Math.Pow(Math.Sin(Angle), 4) + Math.Pow(Math.Cos(Angle), 4));
            QLam[1, 0] = QLam[0, 1];
            QLam[1, 1] = QPly[0, 0] * Math.Pow(Math.Sin(Angle), 4) + 2 * (QPly[0, 1] + 2 * QPly[2, 2]) * Math.Pow(Math.Sin(Angle), 2) * Math.Pow(Math.Cos(Angle), 2) + QPly[1, 1] * Math.Pow(Math.Cos(Angle), 4);
            QLam[0, 2] = (QPly[0, 0] - QPly[0, 1] - 2 * QPly[2, 2]) * Math.Sin(Angle) * Math.Pow(Math.Cos(Angle), 3) + (QPly[0, 1] - QPly[1, 1] + 2 * QPly[2, 2]) * Math.Pow(Math.Sin(Angle), 3) * Math.Cos(Angle);
            QLam[2, 0] = QLam[0, 2];
            QLam[1, 2] = (QPly[0, 0] - QPly[0, 1] - 2 * QPly[2, 2]) * Math.Pow(Math.Sin(Angle), 3) * Math.Cos(Angle) + (QPly[0, 1] - QPly[1, 1] + 2 * QPly[2, 2]) * Math.Sin(Angle) * Math.Pow(Math.Cos(Angle), 3);
            QLam[2, 1] = QLam[1, 2];
            QLam[2, 2] = (QPly[0, 0] + QPly[1, 1] - 2 * QPly[0, 1] - 2 * QPly[2, 2]) * Math.Pow(Math.Sin(Angle), 2) * Math.Pow(Math.Cos(Angle), 2) + QPly[2, 2] * (Math.Pow(Math.Sin(Angle), 4) + Math.Pow(Math.Cos(Angle), 4));
            return QLam;
        }

        private double[,] Calculate_ABDMatx(List<double[,]> QLam, double[] Z_Bot, double[] Z_Top)
        {
            double[,] aMatx = new double[3, 3];
            double[,] bMatx = new double[3, 3];
            double[,] dMatx = new double[3, 3];
            double[,] kMatx = new double[6, 6];

            int iCou1, i, j;
            for (iCou1 = 0; iCou1 < Z_Bot.Length; iCou1++)
            {
                for (i = 0; i <= 2; i++)
                {
                    for (j = 0; j <= 2; j++)
                    {
                        aMatx[i, j] = aMatx[i, j] + QLam[iCou1][i, j] * (Z_Top[iCou1] - Z_Bot[iCou1]); kMatx[i, j] = aMatx[i, j];
                        bMatx[i, j] = bMatx[i, j] + QLam[iCou1][i, j] * (Math.Pow(Z_Top[iCou1], 2) - Math.Pow(Z_Bot[iCou1], 2)) / 2;
                        kMatx[i, j + 3] = bMatx[i, j]; kMatx[i + 3, j] = bMatx[i, j];
                        dMatx[i, j] = dMatx[i, j] + QLam[iCou1][i, j] * (Math.Pow(Z_Top[iCou1], 3) - Math.Pow(Z_Bot[iCou1], 3)) / 3; kMatx[i + 3, j + 3] = dMatx[i, j];
                    }
                }
            }

            return kMatx;
        }
    }
}
