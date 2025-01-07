using UnityEngine;
using TMPro;

public class TextCopier : MonoBehaviour
{
    public TextMeshProUGUI sourceText; // The TextMeshPro component whose text we want to copy
    private TextMeshProUGUI targetText; // The TextMeshPro component where we'll copy the text

    void Start()
    {
        targetText = GetComponent<TextMeshProUGUI>();

        if (targetText == null)
        {
            Debug.LogError("Target TextMeshPro component not found!");
            return;
        }

        if (sourceText != null)
        {
            sourceText.OnPreRenderText += CopyText;
        }
        else
        {
            Debug.LogWarning("Source TextMeshPro component not assigned in the inspector!");
        }
    }



    void CopyText(TMP_TextInfo info)
    {
        if (targetText != null)
        {
            targetText.text = sourceText.text;
        }
    }

    void OnDestroy()
    {
        // Remove the listener when the component is destroyed
        if (sourceText != null)
        {
            sourceText.OnPreRenderText -= CopyText;
        }
    }
}