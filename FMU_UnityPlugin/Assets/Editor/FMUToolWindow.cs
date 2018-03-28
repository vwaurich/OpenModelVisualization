using UnityEngine;
using UnityEditor;
using UnityEngine.Windows;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

public class FMUToolWindow : EditorWindow {

    string fullpath = "";
    string pathName = "";
    string fileName = "";
    string xmlFileName = "";
    XmlDocument doc;
    GameObject simulatorGO;

    // The editor window 
    [MenuItem("FMUvizualizer/Load Scene with FMU")]
    private static void loadFMU()
    {
        EditorWindow.GetWindow<FMUToolWindow>(true, "FMUvisualizer");
    }

    string getFileNameFromType(string type)
    {
        if (type.Length > 12)
        {
            //Debug.Log("STRIPED NAME " + type.Substring(0, 11));
            if (type.Substring(0, 11) == "modelica://")
            {
              //Debug.Log("Type NAME " + type +" size "+type.Length);
              return type.Substring(11, (type.Length-11));
            }
            else
                return type;
        }
        else
            return type;
    }


    void analyseShapeAttribute(GameObject shapeGO, XmlNode nodeIn, string shapeName, int attr)
    {
        // the vraiables attributes have to be put in the simulator
        FMU_Simulation fmuSim = simulatorGO.GetComponent<FMU_Simulation>();
        // the constant attributes have to go in the gameObject directly
        FMUshapeBehaviour fmuShape = shapeGO.GetComponent<FMUshapeBehaviour>();
        if (nodeIn.Name == "exp")
        {
            float val = Single.Parse(nodeIn.InnerText);
            //Debug.Log("constant exp "+attr.ToString()+" VAL: "+val.ToString());
            fmuShape.setVarAttribute(attr, val);
        }
        else if(nodeIn.Name == "cref")
        {
            fmuSim.goIDs.Add(shapeName);
            fmuSim.attrIDs.Add(attr);
            fmuSim.varNames.Add(nodeIn.InnerText);
            //FMU_Simulation.varAttr v = new FMU_Simulation.varAttr(shapeName,attr, nodeIn.InnerText, -1);
            //Debug.Log("THE VARATTR " + v1.debugString());
        }
    }

    void initShapes()
    {
        XmlNode node, T, r, r_shape, lengthDir, widthDir, color;
        XmlNode attrNode;
        XmlNode length, width, height, extra, specCoeff;
        string ident, type;
        //check all shapes
        XmlNodeList nodes = doc.DocumentElement.SelectNodes("/visualization/shape");
        GameObject shapeObject;

        for (int i = 0; i < nodes.Count; i++)
        {
            node = nodes[i];
            ident = node.SelectSingleNode("ident").InnerText;
            type = getFileNameFromType(node.SelectSingleNode("type").InnerText);
            Debug.Log("Create a game object of type " + type);
            if (type.Substring(type.Length - 3) == "dae")
            {
                shapeObject = createFileShapeAsset(type, ident);
                //set or link the shape attributes
                r = node.SelectSingleNode("r");
                T = node.SelectSingleNode("T");
                r_shape = node.SelectSingleNode("r_shape");
                r = node.SelectSingleNode("r");
                lengthDir = node.SelectSingleNode("lengthDir");
                widthDir = node.SelectSingleNode("widthDir");
                length = node.SelectSingleNode("length");
                width = node.SelectSingleNode("width");
                height = node.SelectSingleNode("height");
                extra = node.SelectSingleNode("extra");
                color = node.SelectSingleNode("color");
                specCoeff = node.SelectSingleNode("specCoeff");
                //set the constant or variable shape attributes
                attrNode = T.ChildNodes.Item(0);
                analyseShapeAttribute(shapeObject, attrNode, ident, 1);
                attrNode = T.ChildNodes.Item(1);
                analyseShapeAttribute(shapeObject, attrNode, ident, 2);
                attrNode = T.ChildNodes.Item(2);
                analyseShapeAttribute(shapeObject, attrNode, ident, 3);
                attrNode = T.ChildNodes.Item(3);
                analyseShapeAttribute(shapeObject, attrNode, ident, 4);
                attrNode = T.ChildNodes.Item(4);
                analyseShapeAttribute(shapeObject, attrNode, ident, 5);
                attrNode = T.ChildNodes.Item(5);
                analyseShapeAttribute(shapeObject, attrNode, ident, 6);
                attrNode = T.ChildNodes.Item(6);
                analyseShapeAttribute(shapeObject, attrNode, ident, 7);
                attrNode = T.ChildNodes.Item(7);
                analyseShapeAttribute(shapeObject, attrNode, ident, 8);
                attrNode = T.ChildNodes.Item(8);
                analyseShapeAttribute(shapeObject, attrNode, ident, 9);

                attrNode = r.ChildNodes.Item(0);
                analyseShapeAttribute(shapeObject, attrNode, ident, 10);
                attrNode = r.ChildNodes.Item(1);
                analyseShapeAttribute(shapeObject, attrNode, ident, 11);
                attrNode = r.ChildNodes.Item(2);
                analyseShapeAttribute(shapeObject, attrNode, ident, 12);

                attrNode = r_shape.ChildNodes.Item(0);
                analyseShapeAttribute(shapeObject, attrNode, ident, 13);
                attrNode = r_shape.ChildNodes.Item(1);
                analyseShapeAttribute(shapeObject, attrNode, ident, 14);
                attrNode = r_shape.ChildNodes.Item(2);
                analyseShapeAttribute(shapeObject, attrNode, ident, 15);

                attrNode = lengthDir.ChildNodes.Item(0);
                analyseShapeAttribute(shapeObject, attrNode, ident, 16);
                attrNode = lengthDir.ChildNodes.Item(1);
                analyseShapeAttribute(shapeObject, attrNode, ident, 17);
                attrNode = lengthDir.ChildNodes.Item(2);
                analyseShapeAttribute(shapeObject, attrNode, ident, 18);

                attrNode = widthDir.ChildNodes.Item(0);
                analyseShapeAttribute(shapeObject, attrNode, ident, 19);
                attrNode = widthDir.ChildNodes.Item(1);
                analyseShapeAttribute(shapeObject, attrNode, ident, 20);
                attrNode = widthDir.ChildNodes.Item(2);
                analyseShapeAttribute(shapeObject, attrNode, ident, 21);

                attrNode = length.ChildNodes.Item(0);
                analyseShapeAttribute(shapeObject, attrNode, ident, 22);
                attrNode = width.ChildNodes.Item(0);
                analyseShapeAttribute(shapeObject, attrNode, ident, 23);
                attrNode = height.ChildNodes.Item(0);
                analyseShapeAttribute(shapeObject, attrNode, ident, 24);
                attrNode = extra.ChildNodes.Item(0);
                analyseShapeAttribute(shapeObject, attrNode, ident, 25);

                attrNode = color.ChildNodes.Item(0);
                analyseShapeAttribute(shapeObject, attrNode, ident, 26);
                attrNode = color.ChildNodes.Item(1);
                analyseShapeAttribute(shapeObject, attrNode, ident, 27);
                attrNode = color.ChildNodes.Item(2);
                analyseShapeAttribute(shapeObject, attrNode, ident, 28);

                attrNode = specCoeff.ChildNodes.Item(0);
                analyseShapeAttribute(shapeObject, attrNode, ident, 29);
            }
        }

    }

    void readXML()
    {
        xmlFileName = "";
        //get the visxml file name
        string[] strLst = fileName.Split(new string[] { "." }, StringSplitOptions.None);
        for (int i = 0; i < (strLst.Length - 1); i++)
        {
            xmlFileName = xmlFileName + strLst[i] + ".";
        }
        xmlFileName = pathName + xmlFileName.TrimEnd(new char[] {'.'}) + "_visual.xml";
        Debug.Log("THE xmlFileName " + xmlFileName);

        //parse the xml
        doc = new XmlDocument();
        doc.Load(xmlFileName);
    }

    // Creates an asset for a given file
    GameObject createFileShapeAsset(string fullFileName, string identifier)
    {
        string[] pathLst = fullFileName.Split(new string[] { "/" }, StringSplitOptions.None);
        string fileName = pathLst[pathLst.Length-1];
        pathLst = fileName.Split(new string[] { "." }, StringSplitOptions.None);
        string fileNameNoEnding = pathLst[0];
        Debug.Log("TYPE FILENAME "+fileName + " fileNameNoEnding "+ fileNameNoEnding);

        string destPath = "Assets/Resources/"+fileName;
        if (!System.IO.Directory.Exists("Assets/Resources"))// dir doesn't exists
        {
            System.IO.Directory.CreateDirectory("Assets/Resources");
        }

        if (!System.IO.File.Exists(destPath))//file does not exist
        {
            FileUtil.CopyFileOrDirectory(fullFileName, destPath);
        }
        else
        {
            //FileUtil.ReplaceFile(fullFileName, destPath);
        }
        AssetDatabase.ImportAsset(destPath);

        UnityEngine.Object prefab = Resources.Load(fileNameNoEnding);
        if (GameObject.Find(identifier) != null) //it does exist
        {
            Debug.Log("There is already a GameObject named "+fileNameNoEnding);
            DestroyImmediate(GameObject.Find(identifier));
        }
        GameObject go = (GameObject)Instantiate(prefab, new Vector3(0, 0, 0), Quaternion.identity);
        go.name = identifier;
        FMUshapeBehaviour script =  go.AddComponent<FMUshapeBehaviour>();
        script.meshID = makeFirstCharUpperCase(fileNameNoEnding);
        //go.transform.rotation = Quaternion.AngleAxis(180,new Vector3(0,1,0));
        return go;
    }

    string makeFirstCharUpperCase(string s)
    {
        char c = s[0];
        string s1 = s.Remove(0, 1);
        s1 = s1.Insert(0, c.ToString().ToUpper());
        return s1;
    }

    // GUI events to implement FMUToolWindow
    void OnGUI()
    {
        if(GUILayout.Button("Select FMU"))
        {
            string delimiterString = "/";
            fullpath = EditorUtility.OpenFilePanel("Select a FMU to be visualized.", "", "fmu");
            string[] pathLst = fullpath.Split(new string[] { delimiterString }, StringSplitOptions.None);
            for (int i=0; i < (pathLst.Length-1); i++)
            {
                pathName = pathName + pathLst[i] + delimiterString;
            }
            fileName = pathLst[pathLst.Length-1];
            Debug.Log("Selected fileName " + fileName);
            Debug.Log("Selected pathName " + pathName);
        }
        if (fullpath != null)
        {
            GUILayout.Label("Selected File: " + fullpath);
        }
        else
        {
            GUILayout.Label("No File selected.");
        }

        if (GUILayout.Button("Generate GameObjects."))
        {
            //create the simulation master
            Debug.Log("CREATE FMU_SIMULATOR");
            simulatorGO = GameObject.Find("FMU_Simulator");
            if (simulatorGO != null) //it exists
            {
                DestroyImmediate(simulatorGO);
            }
            simulatorGO = new GameObject("FMU_Simulator");
            FMU_Simulation fmusim = simulatorGO.AddComponent<FMU_Simulation>();
            fmusim.fmuPath = fullpath;
            fmusim.fmuDir = pathName;
            fmusim.goIDs = new List<string>();
            fmusim.attrIDs = new List<int>();
            fmusim.varNames = new List<string>();

            //read XML
            readXML();

            //init all shapes
            initShapes();
        }
		/*
        if (GUILayout.Button("Generate Interfaces"))
        {
            GameObject simulatorGO = GameObject.Find("FMU_Simulator");
            FMU_Simulation fmusim = simulatorGO.GetComponent<FMU_Simulation>();
            fmusim.GetInitialVariables();
        }
		*/
    }
}

