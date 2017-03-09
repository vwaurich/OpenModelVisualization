using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Runtime.InteropServices;
using System.Collections.Generic;

public class FMU_Simulation : MonoBehaviour {

    public string fmuPath;
    public string fmuDir;
    IntPtr simulator;
    public float speedUP = 1;
    //public List<varAttr> varAttrLst;

    //unfortunately, a varAttr struct as list type does not work. instances got deleted after editor script finishes
    public List<string> goIDs;
    public List<int> attrIDs;
    public List<string> varNames;
    public List<int> fmuVarRefs;

    public List<string> RealInputNames = new List<string>();
    public List<float> RealInputs = new List<float>();

    List<string> allFMUShapes;

    /*
        public struct varAttr
        {
            public string goID;
            public int attrID;
            public string varName;
            public int fmuVarRef;

        public varAttr(string gameObj, int attribute, string variable, int fmuVR) // Constructor.
        {
                goID = gameObj;
                attrID = attribute;
                varName = variable;
                fmuVarRef = fmuVR;
        }

        public string debugString()
            {
                return "(" + goID + ":" + attrID.ToString() + ")->" + varName+"(vr:"+fmuVarRef.ToString()+")";
            }
};
*/

    [DllImport("FMUsimulatorLib.dll", CallingConvention = CallingConvention.Cdecl)]
    static extern int getIntPlusTen(int value);

    [DllImport("FMUsimulatorLib.dll", CallingConvention = CallingConvention.Cdecl)]
    static extern IntPtr createFMUsimulator(string fmuPath, string workDir);

    [DllImport("FMUsimulatorLib.dll", CallingConvention = CallingConvention.Cdecl)]
    static extern int setEulerStepSize(IntPtr simulator, float h);

    [DllImport("FMUsimulatorLib.dll", CallingConvention = CallingConvention.Cdecl)]
    static extern int getFMUVersion(IntPtr simulator);

    [DllImport("FMUsimulatorLib.dll", CallingConvention = CallingConvention.Cdecl)]
    static extern int simulateToTime(IntPtr simulator, float nextTime);

    [DllImport("FMUsimulatorLib.dll", CallingConvention = CallingConvention.Cdecl)]
    static extern int initSimulation(IntPtr simulator);

    [DllImport("FMUsimulatorLib.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    static extern int getVarRefForVarName(IntPtr simulator, [MarshalAs(UnmanagedType.LPStr)]string name);

    [DllImport("FMUsimulatorLib.dll", CallingConvention = CallingConvention.Cdecl)]
    static extern float getRealValueForVarRef(IntPtr simulator, int varRef);

    [DllImport("FMUsimulatorLib.dll", CallingConvention = CallingConvention.Cdecl)]
    static extern int getRealValuesForVarRefLst(IntPtr simulator, IntPtr varRef, int numVars, IntPtr val);

    [DllImport("FMUsimulatorLib.dll", CallingConvention = CallingConvention.Cdecl)]
    static extern int destroySimulator(IntPtr simulator);

    [DllImport("FMUsimulatorLib.dll", CallingConvention = CallingConvention.Cdecl)]
    static extern int getSomeVRbyString(string name);

    public void GetInitialVariables()
    {
        Debug.Log("HALLLO0!\n");

        simulator = createFMUsimulator(fmuPath, System.IO.Directory.GetCurrentDirectory().ToString());
        Debug.Log("HALLLO!\n");
        RealInputNames.Add("Input_1");
        RealInputs.Add(1.3f);
        destroySimulator(simulator);
    }

    void Awake()
    {
        Time.fixedDeltaTime = 0.10f;
        Time.timeScale = 1.0f;
    }

    void Start()
    {
        Debug.Log("DIR " + System.IO.Directory.GetCurrentDirectory().ToString());
        //Debug.Log("GOT " + getIntPlusTen(8).ToString());

        simulator = createFMUsimulator(fmuPath, System.IO.Directory.GetCurrentDirectory().ToString());
        setEulerStepSize(simulator, 0.001f);
        //int version = getFMUVersion(simulator);
        //Debug.Log("GOT VERSION " + version.ToString());
        initSimulation(simulator);
        Debug.Log("initialized");

        //get the fmu var references
        fmuVarRefs = new List<int>();
        int vr = 0;
        for (int i = 0; i < attrIDs.Count;i++)
        {
            Debug.Log("set var reference " + i.ToString());
            vr = getVarRefForVarName(simulator, varNames[i]);
            if (vr < 0)
            {
                Debug.Log("Warning: There is no valueRef for " + varNames[i]);
                vr = 0;
            }
            fmuVarRefs.Add(vr);
            Debug.Log("for var " + varNames[i]);
            Debug.Log("got var reference " + fmuVarRefs[i].ToString());
        }

        allFMUShapes = goIDs.Distinct().ToList<string>();
    }

    void Update()
    {
    }

    void FixedUpdate()
    {
        
        Debug.Log("Time " + (Time.time/speedUP));
        simulateToTime(simulator, (Time.time/speedUP));
        //simulateToTime(simulator, 0);
        //Debug.Log("simulated");
        float val;
        for (int i = 0; i < fmuVarRefs.Count; i++)
        {
            val = getRealValueForVarRef(simulator, fmuVarRefs[i]);
            //Debug.Log("get "+val.ToString()+" for var reference " + fmuVarRefs[i].ToString());
            FMUshapeBehaviour shapeGO = GameObject.Find(goIDs[i]).GetComponent<FMUshapeBehaviour>();
            shapeGO.setVarAttribute(attrIDs[i], val);
        }
        for (int i = 0; i < allFMUShapes.Count; i++)
        {
            FMUshapeBehaviour shapeGO = GameObject.Find(allFMUShapes[i]).GetComponent<FMUshapeBehaviour>();
            shapeGO.updateShapes();
        }
        
    }


    void onDestroy()
    {
        destroySimulator(simulator);
        Debug.Log("Destroyed Simulator");

    }

    public static IntPtr DoubleArrayToIntPtr(double[] d)
    {
        IntPtr p = Marshal.AllocCoTaskMem(sizeof(double) * d.Length);
        Marshal.Copy(d, 0, p, d.Length);
        return p;
    }
    public static IntPtr IntArrayToIntPtr(int[] i)
    {
        IntPtr p = Marshal.AllocCoTaskMem(sizeof(int) * i.Length);
        Marshal.Copy(i, 0, p, i.Length);
        return p;
    }

}
