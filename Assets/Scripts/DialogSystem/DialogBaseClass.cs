using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DialogSystem
{
    public class DialogBaseClass : MonoBehaviour
    {
        // [SerializeField] public TextAsset jsonFile;
        [SerializeField] GameObject dialogGameObject;
        public MultipleLine conversationInJson = new MultipleLine();
        private void Awake()
        {
            // conversationInJson = JsonUtility.FromJson<MultipleLine>(jsonFile.text);
        }

        public bool finished = false;
        public bool isProcessing = false;
        protected IEnumerator WriteText(TextAsset jsonFile, Text textHolder, Color textColor, Font textFont, float delay, string Sname)
        {
            isProcessing = true;
            conversationInJson = JsonUtility.FromJson<MultipleLine>(jsonFile.text);
            textHolder.color = textColor;
            textHolder.font = textFont;
            Debug.Log("Here");
            Debug.Log(conversationInJson.conversation[0].text);
            foreach (Line line in conversationInJson.conversation)
            {
                Debug.Log(line.text);
                textHolder.text = string.Empty;
                for (int i = 0; i < line.text.Length; ++i)
                {
                    textHolder.text += line.text[i];
                    if (finished == true)
                    {
                        textHolder.text = line.text;
                        // finished = false;
                        yield return new WaitForSeconds((float)0.5);
                        break;
                    }
                    Debug.Log(Sname);
                    yield return new WaitForSeconds(delay);
                }
                yield return new WaitUntil(() => Input.GetMouseButton(0));
                finished = false;
            }
            isProcessing = false;
            dialogGameObject.SetActive(false);
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                finished = true;
            }
        }
    }
}
