using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.Networking;
using Random = UnityEngine.Random;

public static class Extensions {

    // Возвращает переведенную фразу на текущий язык, принимая ключ этой фразы.
    // Если вся строка это не ключ, то части строк между '[' и ']' будут ключом, и только они переведутся, каждая отедльно
    public static string Localize(this string original)
    {
        if (!original.Contains('['))
            return LanguageManager.Instance.GetValue(original);

        StringBuilder builder = new StringBuilder(original);
        int openIndex = -1;
        for (int i = 0; i < builder.Length; i++)
        {
            if (openIndex == -1 && builder[i] == '[')
            {
                openIndex = i;
                continue;
            }
            
            if (openIndex != -1 && builder[i] == ']')
            {
                string key = original.Substring(openIndex, i - openIndex + 1);
                builder.Remove(openIndex, key.Length);
                builder.Insert(openIndex, LanguageManager.Instance.GetValue(key.Substring(1, key.Length - 2)));
                openIndex = -1;
            }
        }

        return builder.ToString();
    }

    // Для задания еще и нужного шрифта компоненту текста. Т.к. у каждого языка свой шрифт
    public static void SetLocalizedText(this TextMeshProUGUI tmp, string text)
    {
        tmp.text = text.Localize();

        TMP_FontAsset font;
        if (tmp.font != (font = LanguageManager.Instance.CurrentLanguage.Value.Font))
            tmp.font = font;
    }
}
