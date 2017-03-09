using UnityEngine;
using System.Collections;

public class FMUshapeBehaviour : MonoBehaviour {

    //the original values of the FMU
    public string ident = "";
    public string type = "";
    public Matrix4x4 T = new Matrix4x4();
    public Vector3 r = new Vector3(0.0f, 0.0f, 0.0f);
    public Vector3 r_shape = new Vector3(0.0f, 0.0f, 0.0f);
    public Vector3 lengthDir = new Vector3(0.0f, 0.0f, 0.0f);
    public Vector3 widthDir = new Vector3(0.0f, 0.0f, 0.0f);
    public float length = 1;
    public float width = 1;
    public float height = 1;
    public float extra = 1;
    public Vector3 color = new Vector3(0.0f, 0.0f, 0.0f);
    public float specCoeff = 1;

    //the values to make untiy gameobjects look like in Modelica viewer
    Vector3 r_unity = new Vector3(0,0,0);
    Vector3 euler_rh = new Vector3(0,0,0);
    Vector3 euler_unity = new Vector3(0,0,0);
    Quaternion quat_unity = new Quaternion();
    Matrix4x4 T_unity = new Matrix4x4();
    Matrix4x4 mToggle_YZ = new Matrix4x4();//transforms right hand system to left hand system by switching y and z direction
    Color color_unity = new Color(100,100,100);

    //identifier for the gameoBject that holds the mesh
    public string meshID; 

    void Start()
    {
        //init the matrix to switch the coords from lhs to rhs
        mToggle_YZ.SetRow(0, new Vector4(-1, 0, 0, 0));
        mToggle_YZ.SetRow(1, new Vector4(0, 0, 1, 0));
        mToggle_YZ.SetRow(2, new Vector4(0, -1, 0, 0));
        mToggle_YZ.SetRow(3, new Vector4(0, 0, 0, 1));

        T[3, 3] = 1;
    }

   

    public void setVarAttribute(int attr, float val)
    {
        switch(attr)
        {
            case 1:
                T[0,0] = val;break;
            case 2:
                T[0, 1] = val; break;
            case 3:
                T[0, 2] = val; break;
            case 4:
                T[1, 0] = val; break;
            case 5:
                T[1, 1] = val; break;
            case 6:
                T[1, 2] = val; break;
            case 7:
                T[2, 0] = val; break;
            case 8:
                T[2, 1] = val; break;
            case 9:
                T[2, 2] = val; break;
            case 10:
                r[0] = val; break;
            case 11:
                r[1] = val; break;
            case 12:
                r[2] = val; break;
            case 13:
                r_shape[0] = val; break;
            case 14:
                r_shape[1] = val; break;
            case 15:
                r_shape[2] = val; break;
            case 16:
                lengthDir[0] = val; break;
            case 17:
                lengthDir[1] = val; break;
            case 18:
                lengthDir[2] = val; break;
            case 19:
                widthDir[0] = val; break;
            case 20:
                widthDir[1] = val; break;
            case 21:
                widthDir[2] = val; break;
            case 22:
                length = val; break;
            case 23:
                width = val; break;
            case 24:
                height = val; break;
            case 25:
                extra = val; break;
            case 26:
                color[0] = val; break;
            case 27:
                color[1] = val; break;
            case 28:
                color[2] = val; break;
            case 29:
                specCoeff = val; break;
            default:
                Debug.Log("setVArAttributes lacks a case");break;
        }
    }


    // Update is called once per frame
    public void updateShapes() {
        GameObject parentGO = gameObject;

        //switch y and z direction since we have lhs coords in unity
        r_unity.Set(-1 * r[0], r[2], -1 * r[1]);
        //switch to rhs-Coords, rotate, switch back to lhs coords

        /*
        r_unity.Set(r[1], r[0], r[2]);
        T_unity.SetRow(0, new Vector4(T[1,0], T[1,2], T[1,1], 0));
        T_unity.SetRow(1, new Vector4(T[0, 0], T[0, 2], T[0, 1], 0));
        T_unity.SetRow(2, new Vector4(T[2, 0], T[2, 2], T[2, 1], 0));
        */
        T_unity = mToggle_YZ.inverse * T * mToggle_YZ;

        //transform all the stuff
        parentGO.transform.position = r_unity;
        Vector3 euler_rh = QuaternionFromMatrix(T).eulerAngles;
        euler_unity.Set(-euler_rh[0], euler_rh[2], euler_rh[1]);
        parentGO.transform.rotation = Quaternion.Euler(euler_unity[0], euler_unity[1], -euler_unity[2]);
        GameObject meshGO = parentGO.transform.Find(meshID).gameObject;
        color_unity.r = color[0] /255f;
        color_unity.g = color[1] / 255f;
        color_unity.b = color[2] / 255f;
        meshGO.GetComponent<MeshRenderer>().material.color = color_unity;

        //parentGO.transform.rotation = QuaternionFromMatrix(T_unity);
        //Debug.Log("Got R " + parentGO.name + ":  " + r +" det"+ T.determinant);
        //Debug.Log("set position "+parentGO.name+":  " + parentGO.transform.position);
        //Debug.Log("set rotation " + parentGO.name + ":  " + parentGO.transform.rotation);
        //Debug.Log("set quat for " + parentGO.name + ":  " + QuaternionFromMatrix(T).eulerAngles.ToString() + ":  " + QuaternionFromMatrix2(T).eulerAngles.ToString());

    }

    public static Quaternion QuaternionFromMatrix(Matrix4x4 m)
    {
        // Adapted from: http://www.euclideanspace.com/maths/geometry/rotations/conversions/matrixToQuaternion/index.htm
        Quaternion q = new Quaternion();
        q.w = Mathf.Sqrt(Mathf.Max(0, 1 + m[0, 0] + m[1, 1] + m[2, 2])) / 2;
        q.x = Mathf.Sqrt(Mathf.Max(0, 1 + m[0, 0] - m[1, 1] - m[2, 2])) / 2;
        q.y = Mathf.Sqrt(Mathf.Max(0, 1 - m[0, 0] + m[1, 1] - m[2, 2])) / 2;
        q.z = Mathf.Sqrt(Mathf.Max(0, 1 - m[0, 0] - m[1, 1] + m[2, 2])) / 2;
        q.x *= Mathf.Sign(q.x * (m[2, 1] - m[1, 2]));
        q.y *= Mathf.Sign(q.y * (m[0, 2] - m[2, 0]));
        q.z *= Mathf.Sign(q.z * (m[1, 0] - m[0, 1]));
        return q;
    }

    public static Quaternion QuaternionFromMatrix2(Matrix4x4 m)// is equal to QuaternionFromMatrix
    {
        Quaternion q = new Quaternion();
        float tr = m[0, 0] + m[1, 1] + m[2, 2];

        if (tr > 0)
        {
            float S = Mathf.Sqrt(tr + 1.0f) * 2; // S=4*qw 
            q.w = 0.25f * S;
            q.x = (m[2,1] - m[1, 2]) / S;
            q.y = (m[0,2] - m[2, 0]) / S;
            q.z = (m[1,0] - m[0, 1]) / S;
        }
        else if ((m[0, 0] > m[1, 1]) & (m[0, 0] > m[2, 2]))
        {
            float S = Mathf.Sqrt(1.0f + m[0, 0] - m[1, 1] - m[2, 2]) * 2; // S=4*qx 
            q.w = (m[2, 1] - m[1, 2]) / S;
            q.x = 0.25f * S;
            q.y = (m[0, 1] + m[1, 0]) / S;
            q.z = (m[0, 2] + m[2, 0]) / S;
        }
        else if (m[1, 1] > m[2, 2])
        {
            float S = Mathf.Sqrt(1.0f + m[1, 1] - m[0, 0] - m[2, 2]) * 2; // S=4*qy
            q.w = (m[0, 2] - m[2, 0]) / S;
            q.x = (m[0, 1] + m[1, 0]) / S;
            q.y = 0.25f * S;
            q.z = (m[1, 2] + m[2, 1]) / S;
        }
        else
        {
            float S = Mathf.Sqrt(1.0f + m[2, 2] - m[0, 0] - m[1, 1]) * 2; // S=4*qz
            q.w = (m[1, 0] - m[0,1]) / S;
            q.x = (m[0, 2] + m[2,0]) / S;
            q.y = (m[1,2] + m[2, 1]) / S;
            q.z = 0.25f * S;
        }
        return q;
    }
}
