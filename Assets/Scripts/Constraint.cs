using System.Collections.Generic;
using UnityEngine;

public struct Constraint
{
    public enum ContraintType 
    {
        Contains,
        StatsWith,
        EndsWith,
        HasLength,
    }

    public ContraintType type;
    public string stringArg;
    public int intArg;

    public static Constraint GetRandomConstraint()
    {
        ContraintType enumValue = (ContraintType)Random.Range(0, System.Enum.GetValues(typeof(ContraintType)).Length);

        Constraint res = new();
        res.type = enumValue;

        if (enumValue == ContraintType.Contains)
        {
            res.stringArg = Util.GetRandomElement(new string[] {
                "AR", "AT", "AP", "AS", "AL", "AM", "AC", "AB", "AN", "EA", "ER", "ET", "ES", "ED", "EL", "EM", "EC", "EN", "RA", "RE", "RI", "RO", "RS", "TA", "TE", "TR", "TI", "TO", "TH", "UR", "US", "UL", "UN", "IA", "IE", "IT", "IO", "IS", "ID", "IL", "IC", "IN", "OR", "OT", "OU", "OP", "OS", "OG", "OL", "OM", "OC", "ON", "PA", "PE", "PR", "PI", "PO", "PH", "SE", "ST", "SU", "SI", "SS", "SH", "DE", "DI", "GE", "HA", "HE", "HI", "HO", "LA", "LE", "LY", "LI", "LO", "LL", "MA", "ME", "MI", "MO", "CA", "CE", "CT", "CI", "CO", "CH", "VE", "BL", "NA", "NE", "NT", "NI", "NO", "NS", "ND", "NG", "NC",
            });
        }
        else if (enumValue == ContraintType.StatsWith)
        {
            res.stringArg = Util.GetRandomElement(new string[] {
                "AN", "RE", "UN", "IN", "PA", "PR", "SU", "DE", "DI", "MA", "CA", "CO", "NO",
            });
        }
        else if (enumValue == ContraintType.EndsWith)
        {
            res.stringArg = Util.GetRandomElement(new string[] {
                "AL", "AN", "ER", "ES", "ED", "RS", "TE", "TY", "US", "IA", "IC", "ON", "ST", "SS", "LE", "LY", "NE", "NT", "NG", 
            });
        }
        else if (enumValue == ContraintType.HasLength)
        {
            res.intArg = Random.Range(6, 11);
        }
        else throw new System.Exception("Branch missing!");

        return res;
    }

    public bool IsWordAllowed(string word)
    {
        word = word.ToUpper();

        if (type == ContraintType.Contains)
        {
            return word.Contains(stringArg);
        }
        else if (type == ContraintType.StatsWith)
        {
            return word.StartsWith(stringArg);
        }
        else if (type == ContraintType.EndsWith)
        {
            return word.EndsWith(stringArg);
        }
        else if (type == ContraintType.HasLength)
        {
            return word.Length == intArg;
        }
        else throw new System.Exception("Branch missing!");
    }


    public string GetDescription()
    {
        if (type == ContraintType.Contains)
        {
            return $"Must contain {DecorateArgument(stringArg)}";
        }
        else if (type == ContraintType.StatsWith)
        {
            return $"Must start with {DecorateArgument(stringArg)}";
        }
        else if (type == ContraintType.EndsWith)
        {
            return $"Must end with {DecorateArgument(stringArg)}";
        }
        else if (type == ContraintType.HasLength)
        {
            return $"Must be exactly {DecorateArgument(intArg)} letters long";
        }
        else throw new System.Exception("Branch missing!");
    }

    private static string DecorateArgument<T>(T arg)
    {
        return $"<color=#555><size=130>{arg}</size></color>";
    }
}
