using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CarControlScipt : Control {

    public WheelCollider wheelFL;
    public WheelCollider wheelFR;
    public WheelCollider wheelRL;
    public WheelCollider wheelRR;
    public float maxTorque = 50;
    public float maxSteerAngle = 40;

    private float torque = 0;
    private float steerAngle = 0;



    // Use this for initialization
    /// <summary>
    /// 
    /// </summary>
    void Start () {
        GetComponent<Rigidbody>().centerOfMass.Set(0, -0.9f, 0);
        Init();
	}
	
	// Update is called once per frame
	void Update () {
        if (core == null) return;
        if (core.type == Core.Type.PLAYER || core.type == Core.Type.GATHER)
        {
            wheelRR.motorTorque = maxTorque * Input.GetAxis("Vertical");
            wheelRL.motorTorque = maxTorque * Input.GetAxis("Vertical");

            //Debug.Log(wheelRR.motorTorque);

            wheelFL.steerAngle = maxSteerAngle * Input.GetAxis("Horizontal");
            wheelFR.steerAngle = maxSteerAngle * Input.GetAxis("Horizontal");


            GatherUpdate();

        }
        else if(core.type == Core.Type.GENETICS)
        {

            Steer((float)core.GetOutput()[0]);
            Move((float)core.GetOutput()[1]);

            wheelRR.motorTorque = maxTorque * torque;
            wheelRL.motorTorque = maxTorque * torque;

            wheelFL.steerAngle = maxSteerAngle * steerAngle;
            wheelFR.steerAngle = maxSteerAngle * steerAngle;
        }
        else if(core.type == Core.Type.Q)
        {

            double[] steering = GetSteering();
            float move = (float)steering[0];
            float rotation = (float)steering[1];

            Steer(rotation);
            Move(move);

            wheelRR.motorTorque = maxTorque * torque;
            wheelRL.motorTorque = maxTorque * torque;

            wheelFL.steerAngle = maxSteerAngle * steerAngle;
            wheelFR.steerAngle = maxSteerAngle * steerAngle;
        }
    }

    public void SaveLearingVectors()
    {
        System.IO.StreamWriter file = new System.IO.StreamWriter("e:\\test.txt");
        for (int lineIndex = 0; lineIndex < learningVectors.Count; lineIndex++)
        {
            double[] line = learningVectors[lineIndex];
            string lineStr = "";
            for (int i=0; i<line.Length; i++)
            {
                lineStr += line[i] + " ";
            }
            file.WriteLine(lineStr);
        }
        file.Close();
    }

    public void Steer(float steerAngle)
    {
        this.steerAngle = steerAngle;
    }

    public void Move(float torque)
    {
        this.torque = torque;
    }
}
