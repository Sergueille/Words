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
        ContainsNot,
    }

    public ContraintType type;
    public string stringArg;
    public int intArg;

    public static Constraint GetRandomConstraint()
    {
        float rand = Random.Range(0.0f, 1.0f);

        Constraint res = new();

        if (rand < 0.3f) // 0.3
        {
            res.type = ContraintType.Contains;
            res.stringArg = Util.GetRandomElement(new string[] {
                "AR", "AT", "AP", "AS", "AL", "AM", "AC", "AB", "AN", "EA", "ER", "ET", "ES", "ED", "EL", "EM", "EC", "EN", "RA", "RE", "RI", "RO", "RS", 
                "TA", "TE", "TR", "TI", "TO", "TH", "UR", "US", "UL", "UN", "IA", "IE", "IT", "IO", "IS", "ID", "IL", "IC", "IN", "OR", "OT", "OU", "OP", 
                "OS", "OG", "OL", "OM", "OC", "ON", "PA", "PE", "PR", "PI", "PO", "PH", "SE", "ST", "SU", "SI", "SS", "SH", "DE", "DI", "GE", "HA", "HE", 
                "HI", "HO", "LA", "LE", "LY", "LI", "LO", "LL", "MA", "ME", "MI", "MO", "CA", "CE", "CT", "CI", "CO", "CH", "VE", "BL", "NA", "NE", "NT", 
                "NI", "NO", "NS", "ND", "NG", "NC",
                "J", "Q", "X"
            });
        }
        else if (rand < 0.45f) // 0.15
        {
            res.type = ContraintType.StatsWith;
            res.stringArg = Util.GetRandomElement(new string[] {
                "AN", "RE", "UN", "IN", "PA", "PR", "SU", "DE", "DI", "MA", "CA", "CO", "NO",
            });
        }
        else if (rand < 0.63f) // 0.18
        {
            res.type = ContraintType.EndsWith;
            res.stringArg = Util.GetRandomElement(new string[] {
                "AL", "AN", "ER", "ES", "ED", "RS", "TE", "TY", "US", "IA", "IC", "ON", "ST", "SS", "LE", "LY", "NE", "NT", "NG", 
            });
        }
        else if (rand < 0.78f) // 0.15
        {
            res.type = ContraintType.HasLength;
            res.intArg = Random.Range(9, 11);
        }
        else  // 0.22
        {
            res.type = ContraintType.ContainsNot;
            res.stringArg = Util.GetRandomElement(new string[] {
                "E", "T", "A", "S"
            });
        }

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
        else if (type == ContraintType.ContainsNot)
        {
            return !word.Contains(stringArg);
        }
        else throw new System.Exception("Branch missing!");
    }


    public string GetDescription()
    {
        if (type == ContraintType.Contains)
        {
            return $"Must contain {Util.DecorateArgument(stringArg)}";
        }
        else if (type == ContraintType.StatsWith)
        {
            return $"Must start with {Util.DecorateArgument(stringArg)}";
        }
        else if (type == ContraintType.EndsWith)
        {
            return $"Must end with {Util.DecorateArgument(stringArg)}";
        }
        else if (type == ContraintType.HasLength)
        {
            return $"Must be exactly {Util.DecorateArgument(intArg)} letters long";
        }
        else if (type == ContraintType.ContainsNot)
        {
            return $"Must NOT contain {Util.DecorateArgument(stringArg)}";
        }
        else throw new System.Exception("Branch missing!");
    }
}
