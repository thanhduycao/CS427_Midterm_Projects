using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DialogSystem
{
    public class DialogLine : DialogBaseClass
    {
        [Header ("Text options")]
        private Text textHolder;
        [SerializeField] public string inputField;
        [SerializeField] private Color textColor;
        [SerializeField] private Font textFont;

        [Header("Time parameters")]
        [SerializeField] private float delay;

        [Header("Sound settings")]
        [SerializeField] private string Sname;

        public TextAsset jsonFile;

        private void OnEnable()
        {
            textHolder = GetComponent<Text>();
            StartCoroutine(WriteText(jsonFile, textHolder, textColor, textFont, delay, Sname));
        }
    }
}
