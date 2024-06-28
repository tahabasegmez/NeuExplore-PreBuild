using System;
using System.Collections.Generic;
using System.Net;
using System.Xml;
using System.Xml.Schema;
using UnityEngine;

public class XMLReader : MonoBehaviour
{
    public TextAsset xmlFile; 
    public TextAsset dtdFile; 

    public List<BrainPart> brainParts;
    public List<Transition> transitions;
    public int finalState;

    private bool hasValidationErrors = false; 

    void Awake()
    {
        brainParts = new List<BrainPart>();
        transitions = new List<Transition>();
        LoadXML();
    }

    void LoadXML()
    {
        
        XmlReaderSettings settings = new XmlReaderSettings();
        settings.DtdProcessing = DtdProcessing.Parse;
        settings.ValidationType = ValidationType.DTD;
        settings.XmlResolver = new CustomXmlResolver(dtdFile); 
        settings.ValidationEventHandler += new ValidationEventHandler(ValidationCallback);

        try
        {
           
            using (XmlReader reader = XmlReader.Create(new System.IO.StringReader(xmlFile.text), settings))
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(reader);

                XmlNodeList partsList = xmlDoc.GetElementsByTagName("Part");
                foreach (XmlNode partInfo in partsList)
                {
                    XmlElement partElement = (XmlElement)partInfo;
                    string name = partElement.GetAttribute("name");
                    int index = int.Parse(partElement.GetAttribute("index"));
                    brainParts.Add(new BrainPart(name, index));
                }

                XmlNodeList transitionList = xmlDoc.GetElementsByTagName("Transition");
                foreach (XmlNode transitionInfo in transitionList)
                {
                    XmlElement transitionElement = (XmlElement)transitionInfo;
                    int from = int.Parse(transitionElement.GetAttribute("from"));
                    string input = transitionElement.GetAttribute("input");
                    int to = int.Parse(transitionElement.GetAttribute("to"));
                    transitions.Add(new Transition(from, input, to));
                }

                XmlNode finalStateNode = xmlDoc.GetElementsByTagName("FinalState")[0];
                finalState = int.Parse(finalStateNode.Attributes["index"].Value);
            }

            if (!hasValidationErrors)
            {
                Debug.Log("DTD uygun");
            }
        }
        catch (XmlException e)
        {
            Debug.LogError($"XML Exception: {e.Message}");
        }
        catch (XmlSchemaException e)
        {
            Debug.LogError($"XML Schema Exception: {e.Message}");
        }
    }

    
    private void ValidationCallback(object sender, ValidationEventArgs e)
    {
        hasValidationErrors = true; 

        if (e.Severity == XmlSeverityType.Warning)
        {
            Debug.LogWarning($"XML Uyarisi: {e.Message}");
        }
        else if (e.Severity == XmlSeverityType.Error)
        {
            Debug.LogError($"XML Hatasi: {e.Message}");
        }
    }
}

public class CustomXmlResolver : XmlResolver
{
    private readonly TextAsset dtdFile;

    public CustomXmlResolver(TextAsset dtdFile)
    {
        this.dtdFile = dtdFile;
    }

    public override object GetEntity(Uri absoluteUri, string role, Type ofObjectToReturn)
    {
        if (ofObjectToReturn == typeof(System.IO.Stream))
        {
            return new System.IO.MemoryStream(System.Text.Encoding.UTF8.GetBytes(dtdFile.text));
        }
        throw new System.NotImplementedException($"Cannot handle type {ofObjectToReturn}");
    }

    public override ICredentials Credentials
    {
        set { throw new System.NotImplementedException(); }
    }
}

[System.Serializable]
public class BrainPart
{
    public string name;
    public int index;

    public BrainPart(string name, int index)
    {
        this.name = name;
        this.index = index;
    }
}

[System.Serializable]
public class Transition
{
    public int from;
    public string input;
    public int to;

    public Transition(int from, string input, int to)
    {
        this.from = from;
        this.input = input;
        this.to = to;
    }
}