using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public static class StringsToOptionDatas
{
    public static List<Dropdown.OptionData> ToOptionDatas(this List<string> val)
    {
        var optionDatas = new List<Dropdown.OptionData>();
        foreach (var str in val) optionDatas.Add(new Dropdown.OptionData(str));

        return optionDatas;
    }
}
