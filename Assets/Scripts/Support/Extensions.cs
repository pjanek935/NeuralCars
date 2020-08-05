using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extensions
{
    public static string ToStringInLanguage (this Language language)
    {
        string result = string.Empty;

        switch (language)
        {
            case Language.ENGLISH:

                result = "ENGLISH";

                break;

            case Language.POLISH:

                result = "POLSKI";

                break;
        }

        return result;
    }
}
