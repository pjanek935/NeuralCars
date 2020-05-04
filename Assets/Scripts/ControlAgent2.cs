using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.IO;

public class ControlAgent2 : Control {

    public float speed = 4;
    public float rotationSpeed = 4;

    //private Core core;

    // Use this for initialization
    void Start () {
        Init();
    }
	
	// Update is called once per frame
	void Update () {

        if (core.type == Core.Type.PLAYER || core.type == Core.Type.GATHER)
        {
            float rotation = Input.GetAxis("Horizontal");
            float move = Input.GetAxis("Vertical");
            transform.Rotate(0, rotationSpeed * rotation * Time.deltaTime, 0);
            Vector3 direction = transform.forward;
            direction.y = 0;
            direction.Normalize();
            transform.position += direction * speed * move * Time.deltaTime;

            GatherUpdate();
        }
        else if (core.type == Core.Type.GENETICS)
        {
            float rotation = (float)core.GetOutput()[0];
            float move = (float)core.GetOutput()[1];
            transform.Rotate(0, rotationSpeed * rotation * Time.deltaTime, 0);
            Vector3 direction = transform.forward;
            direction.y = 0;
            direction.Normalize();
            transform.position += direction * speed * move * Time.deltaTime;
        }
        else if (core.type == Core.Type.Q)
        {
            double[] output = core.GetOutput();
            int winner = 0;
            double maxVar = 0;
            for (int i = 0; i < 8; i++)
            {
                if (output[i] > maxVar)
                {
                    maxVar = output[i];
                    winner = i;
                }
            }

            float rotation = 0;
            float move = 0;
            switch (winner)
            {
                case 0:
                    move = 1;
                    rotation = 0;
                    break;

                case 1:
                    move = -1;
                    rotation = 0;
                    break;

                case 2:
                    move = 0;
                    rotation = 1;
                    break;

                case 3:
                    move = 0;
                    rotation = -1;
                    break;

                case 4:
                    move = 1;
                    rotation = 1;
                    break;

                case 5:
                    move = 1;
                    rotation = -1;
                    break;

                case 6:
                    move = -1;
                    rotation = 1;
                    break;

                case 7:
                    move = -1;
                    rotation = -1;
                    break;
            }

            transform.Rotate(0, rotationSpeed * rotation * Time.deltaTime, 0);
            Vector3 direction = transform.forward;
            direction.y = 0;
            direction.Normalize();
            transform.position += direction * speed * move * Time.deltaTime;
        }
        

    }

    
}
