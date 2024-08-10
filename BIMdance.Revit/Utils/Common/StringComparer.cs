namespace BIMdance.Revit.Utils.Common;

public class StringComparer : IComparer<object>
{
    private readonly bool _zeroesFirst;

    public StringComparer(bool zeroesFirst = true) => _zeroesFirst = zeroesFirst;

    public int Compare(object obj1, object obj2)
    {
        switch (obj1)
        {
            case null when obj2 == null:
                return 0;

            case null:
                return -1;
        }

        if (obj2 == null) return 1;

        var s1 = obj1.ToString();
        var s2 = obj2.ToString();

        switch (s1.Length)
        {
            case <= 0 when s2.Length <= 0:
                return 0;
            case <= 0:
                return -1;
        }

        if (s2.Length <= 0) return 1;

        //special case
        var sp1 = char.IsLetterOrDigit(s1[0]);
        var sp2 = char.IsLetterOrDigit(s2[0]);

        switch (sp1)
        {
            case true when !sp2:
                return 1;

            case false when sp2:
                return -1;
        }

        int i1 = 0, i2 = 0; //current index

        while (true)
        {
            var c1 = s1[i1];
            var c2 = s2[i2];
            sp1 = char.IsDigit(c1);
            sp2 = char.IsDigit(c2);
            int r; // temp result

            switch (sp1)
            {
                case false when !sp2:
                {
                    var letter1 = char.IsLetter(c1);
                    var letter2 = char.IsLetter(c2);

                    switch (letter1)
                    {
                        case true when letter2:
                            r = string.Compare(char.ToUpper(c1).ToString(), char.ToUpper(c2).ToString(),
                                StringComparison.Ordinal);
                            if (r != 0) return Math.Sign(r);
                            break;

                        case false when !letter2:
                            r = c1.CompareTo(c2);
                            if (r != 0) return Math.Sign(r);
                            break;

                        case false when true:
                            return -1;

                        case true when true:
                            return 1;
                    }

                    break;
                }
                case true when sp2:
                    r = CompareNum(s1, ref i1, s2, ref i2, _zeroesFirst);
                    if (r != 0) return Math.Sign(r);
                    break;

                case true:
                    return -1;

                default:
                    return 1;
            }

            i1++;
            i2++;

            switch (i1 >= s1.Length)
            {
                case true when (i2 >= s2.Length):
                    return 0;

                case true:
                    return -1;

                default:
                    if (i2 >= s2.Length) return 1;
                    break;
            }
        }
    }

    private static int CompareNum(string s1, ref int i1, string s2, ref int i2, bool zeroesFirst)
    {
        var nzStart1 = i1;
        var nzStart2 = i2; // nz = non zero
        var end1 = i1;
        var end2 = i2;

        ScanNumEnd(s1, i1, ref end1, ref nzStart1);
        ScanNumEnd(s2, i2, ref end2, ref nzStart2);
        var start1 = i1;
        i1 = end1 - 1;
        var start2 = i2;
        i2 = end2 - 1;

        if (zeroesFirst)
        {
            var zl1 = nzStart1 - start1;
            var zl2 = nzStart2 - start2;
            if (zl1 > zl2) return -1;
            if (zl1 < zl2) return 1;
        }

        var nzLength1 = end1 - nzStart1;
        var nzLength2 = end2 - nzStart2;

        if (nzLength1 < nzLength2) return -1;
        else if (nzLength1 > nzLength2) return 1;

        for (int j1 = nzStart1, j2 = nzStart2; j1 <= i1; j1++, j2++)
        {
            var r = s1[j1].CompareTo(s2[j2]);
            if (r != 0) return r;
        }

        // the nz parts are equal
        var length1 = end1 - start1;
        var length2 = end2 - start2;
        if (length1 == length2) return 0;
        if (length1 > length2) return -1;
        return 1;
    }

    //lookahead
    private static void ScanNumEnd(string s, int start, ref int end, ref int nzStart)
    {
        nzStart = start;
        end = start;
        var countZeros = true;
        while (char.IsDigit(s, end))
        {
            if (countZeros && s[end].Equals('0'))
            {
                nzStart++;
            }
            else countZeros = false;

            end++;
            if (end >= s.Length) break;
        }
    }
}