using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DialogSystem
{
    public class DialogBaseClass : MonoBehaviour
    {
        protected IEnumerator WriteText(string input, Text textHolder, Color textColor, Font textFont, float delay, string Sname)
        {
            textHolder.color = textColor;
            textHolder.font = textFont;

            for (int i = 0; i < input.Length; ++i)
            {
                textHolder.text += input[i];
                Debug.Log(Sname);
                MusicManager.findMusic(Sname);
                yield return new WaitForSeconds(delay);
            }
        }
    }
}
