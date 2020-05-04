using UnityEngine;
using System.Collections;

public class ControlAgent : Control {

    public float speed = 5f;

    // Use this for initialization
    void Start () {
        Init();
	}
	
	// Update is called once per frame
	void Update () {

        if (core.type == Core.Type.PLAYER || core.type == Core.Type.GATHER)
        {
            Vector3 translation = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            transform.Translate(translation * speed * Time.deltaTime);

            GatherUpdate();
        }
        else if(core.type == Core.Type.GENETICS)
        {
            Vector3 translation = new Vector3((float)core.GetOutput()[0], 0, (float)core.GetOutput()[1]);
            transform.Translate(translation * speed * Time.deltaTime);
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

            Vector3 translation = new Vector3(rotation, 0, move);
            transform.Translate(translation * speed * Time.deltaTime);
        }

    }
}
