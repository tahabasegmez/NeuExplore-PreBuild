using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BrainManager : MonoBehaviour
{
    public TextMeshProUGUI partNameText;
    public GameObject[] brainParts; 
    private XMLReader xmlReader;
    private Dictionary<int, Dictionary<string, int>> transitions; 
    private int currentIndex;
    private int finalState;


    void Start()
    {
        xmlReader = GetComponent<XMLReader>();
        transitions = new Dictionary<int, Dictionary<string, int>>();
        currentIndex = 0; 

        
        foreach (BrainPart part in xmlReader.brainParts)
        {
            if (part.index < brainParts.Length)
            {
                brainParts[part.index].name = part.name; 
                brainParts[part.index].SetActive(true); 
            }
        }

        
        foreach (Transition transition in xmlReader.transitions)
        {
            if (!transitions.ContainsKey(transition.from))
            {
                transitions[transition.from] = new Dictionary<string, int>();
            }
            transitions[transition.from][transition.input] = transition.to;
        }

        finalState = xmlReader.finalState;
        SeparateBrainPart(currentIndex);
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.KeypadPlus)) 
        {
            HandleInput("+");
        }
        else if (Input.GetKeyDown(KeyCode.KeypadMinus)) 
        {
            HandleInput("-");
        }
    }




    void HandleInput(string input)
    {
        int nextIndex = GetNextPart(currentIndex, input);
        if (nextIndex != -1)
        {
            SeparateBrainPart(nextIndex); 
            currentIndex = nextIndex; 

            if (currentIndex == finalState)
            {
                Debug.Log("Final state varildi: " + currentIndex);
            }
            else
            {
                Debug.Log("Final state degil. Index:" + currentIndex);            }
        }
        else
        {
            Debug.Log("Indexte gecis bulunamadi." + currentIndex + " inputuyla: " + input);
        }
    }

    
    public void HideAllParts()
    {
        foreach (GameObject part in brainParts)
        {
            part.SetActive(false);
        }
    }

    
    public void SeparateBrainPart(int index)
    {
        if (index >= 0 && index < brainParts.Length)
        {
            HideAllParts(); 
            partNameText.text = brainParts[index].name; 
            brainParts[index].SetActive(true); 
        }
        else
        {
            Debug.LogError("Gecersiz beyin parcasi indexi: " + index);
        }
    }

    public int GetNextPart(int currentIndex, string input)
    {
        if (transitions.ContainsKey(currentIndex) && transitions[currentIndex].ContainsKey(input))
        {
            return transitions[currentIndex][input];
        }
        else
        {
            Debug.LogWarning("Bu index icin gecis bulunamadi: " + currentIndex + " inputuyla: " + input);
            return -1;
        }
    }
}
