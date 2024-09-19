namespace BIMdance.Revit.Logic.Calculators;

public static class RtmKpTable
{
    private static readonly double[] TableKu = { 0.10, 0.15, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7, 0.8 };

    private static readonly int[] TableNe = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 17, 18, 19, 20, 21, 22, 23, 24, 25, 30, 35, 40, 45, 50, 60, 70, 80, 90, 100 };

    private static readonly double[,] TableKp =
    {
        {8.00, 5.33, 4.00, 2.67, 2.00, 1.60, 1.33, 1.14, 1.00},
        {6.22, 4.33, 3.39, 2.45, 1.98, 1.60, 1.33, 1.14, 1.00},
        {4.05, 2.89, 2.31, 1.74, 1.45, 1.34, 1.22, 1.14, 1.00},
        {3.24, 2.35, 1.91, 1.47, 1.25, 1.21, 1.12, 1.06, 1.00},
        {2.84, 2.09, 1.72, 1.35, 1.16, 1.16, 1.08, 1.03, 1.00},
        {2.64, 1.96, 1.62, 1.28, 1.14, 1.13, 1.06, 1.01, 1.00},
        {2.49, 1.86, 1.54, 1.23, 1.12, 1.10, 1.04, 1.00, 1.00},
        {2.37, 1.78, 1.48, 1.19, 1.10, 1.08, 1.02, 1.00, 1.00},
        {2.27, 1.71, 1.43, 1.16, 1.09, 1.07, 1.01, 1.00, 1.00},
        {2.18, 1.65, 1.39, 1.13, 1.07, 1.05, 1.00, 1.00, 1.00},
        {2.11, 1.61, 1.35, 1.10, 1.06, 1.04, 1.00, 1.00, 1.00},
        {2.02, 1.56, 1.32, 1.08, 1.05, 1.03, 1.00, 1.00, 1.00},
        {1.99, 1.52, 1.29, 1.06, 1.04, 1.01, 1.00, 1.00, 1.00},
        {1.94, 1.49, 1.27, 1.05, 1.02, 1.00, 1.00, 1.00, 1.00},
        {1.81, 1.41, 1.21, 1.00, 1.00, 1.00, 1.00, 1.00, 1.00},
        {1.78, 1.39, 1.19, 1.00, 1.00, 1.00, 1.00, 1.00, 1.00},
        {1.75, 1.36, 1.17, 1.00, 1.00, 1.00, 1.00, 1.00, 1.00},
        {1.72, 1.35, 1.16, 1.00, 1.00, 1.00, 1.00, 1.00, 1.00},
        {1.69, 1.33, 1.15, 1.00, 1.00, 1.00, 1.00, 1.00, 1.00},
        {1.67, 1.31, 1.13, 1.00, 1.00, 1.00, 1.00, 1.00, 1.00},
        {1.64, 1.30, 1.12, 1.00, 1.00, 1.00, 1.00, 1.00, 1.00},
        {1.62, 1.28, 1.11, 1.00, 1.00, 1.00, 1.00, 1.00, 1.00},
        {1.60, 1.27, 1.10, 1.00, 1.00, 1.00, 1.00, 1.00, 1.00},
        {1.51, 1.21, 1.05, 1.00, 1.00, 1.00, 1.00, 1.00, 1.00},
        {1.44, 1.16, 1.00, 1.00, 1.00, 1.00, 1.00, 1.00, 1.00},
        {1.40, 1.13, 1.00, 1.00, 1.00, 1.00, 1.00, 1.00, 1.00},
        {1.35, 1.10, 1.00, 1.00, 1.00, 1.00, 1.00, 1.00, 1.00},
        {1.30, 1.07, 1.00, 1.00, 1.00, 1.00, 1.00, 1.00, 1.00},
        {1.25, 1.03, 1.00, 1.00, 1.00, 1.00, 1.00, 1.00, 1.00},
        {1.20, 1.00, 1.00, 1.00, 1.00, 1.00, 1.00, 1.00, 1.00},
        {1.16, 1.00, 1.00, 1.00, 1.00, 1.00, 1.00, 1.00, 1.00},
        {1.13, 1.00, 1.00, 1.00, 1.00, 1.00, 1.00, 1.00, 1.00},
        {1.10, 1.00, 1.00, 1.00, 1.00, 1.00, 1.00, 1.00, 1.00}
    };

    public static double GetKp(int ne, double ku)
    {
        var minNe = TableNe[0];
        var maxNe = TableNe[TableNe.Length - 1];

        var minKu = TableKu[0];
        var maxKu = TableKu[TableKu.Length - 1];

        var neValue = ne;
        if (ne < minNe)
            neValue = minNe;
        else if (ne > maxNe)
            neValue = maxNe;

        var kuValue = ku;
        if (ku < minKu)
            kuValue = minKu;
        else if (ku > maxKu)
            kuValue = maxKu;

        var (neStartIndex, neEndIndex) = GetNeIndexes(neValue);
        var (kuStartIndex, kuEndIndex) = GetKuIndexes(kuValue);

        if (neStartIndex == neEndIndex && kuStartIndex == kuEndIndex)
        {
            return TableKp[neStartIndex, kuStartIndex];
        }

        if (neStartIndex == neEndIndex)
        {
            var startKu = TableKp[neStartIndex, kuStartIndex];
            var endKu = TableKp[neStartIndex, kuEndIndex];

            return GetInterpolatedY(TableKu[kuStartIndex], startKu, TableKu[kuEndIndex], endKu, kuValue);
        }

        if (kuStartIndex == kuEndIndex)
        {
            var startKu = TableKp[neStartIndex, kuStartIndex];
            var endKu = TableKp[neEndIndex, kuStartIndex];

            return GetInterpolatedY(TableNe[neStartIndex], startKu, TableNe[neEndIndex], endKu, neValue);
        }

        var leftTopKu = TableKp[neStartIndex, kuStartIndex];
        var leftBottomKu = TableKp[neEndIndex, kuStartIndex];
        var rightTopKu = TableKp[neStartIndex, kuEndIndex];
        var rightBottomKu = TableKp[neEndIndex, kuEndIndex];

        var leftKu = GetInterpolatedY(TableNe[neStartIndex], leftTopKu, TableNe[neEndIndex], leftBottomKu, neValue);
        var rightKu = GetInterpolatedY(TableNe[neStartIndex], rightTopKu, TableNe[neEndIndex], rightBottomKu, neValue);

        return GetInterpolatedY(TableKu[kuStartIndex], leftKu, TableKu[kuEndIndex], rightKu, kuValue);
    }

    private static double GetInterpolatedY(double startX, double startY, double endX, double endY, double x)
    {
        var k = (startY - endY) / (startX - endX);
        var b = startY - k * startX;

        return k * x + b;
    }

    private static (int, int) GetNeIndexes(int ne)
    {
        int startIndex = -1;
        int endIndex = -1;

        for (var i = 0; i < TableNe.Length; i++)
        {
            if (TableNe[i] == ne)
            {
                return (i, i);
            }

            if (TableNe[i] < ne)
            {
                startIndex = i;
            }
            else if (TableNe[i] > ne)
            {
                endIndex = i;
                break;
            }
        }

        return (startIndex, endIndex);
    }

    private static (int, int) GetKuIndexes(double ku)
    {
        int startIndex = -1;
        int endIndex = -1;

        for (var i = 0; i < TableKu.Length; i++)
        {
            if (Math.Abs(TableKu[i] - ku) < 1e-3)
            {
                return (i, i);
            }

            if (TableKu[i] < ku)
            {
                startIndex = i;
            }
            else if (TableKu[i] > ku)
            {
                endIndex = i;
                break;
            }
        }

        return (startIndex, endIndex);
    }
}