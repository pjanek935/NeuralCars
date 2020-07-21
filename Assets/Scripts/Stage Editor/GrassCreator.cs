using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassCreator : MonoBehaviour
{
    [SerializeField] GameObject grassPrefab;
    List<GameObject> grass = new List<GameObject> ();
    [SerializeField] float scale = 1f;
    [SerializeField] int size = 100;

    private void OnEnable ()
    {
        randomize ();
    }

    void randomize ()
    {
        System.Random random = new System.Random ();

        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                float perlin = Mathf.PerlinNoise (x * scale, y * scale);

                if (perlin > 0.5)
                {
                    double r = random.NextDouble ();

                    GameObject newGameObject = Instantiate (grassPrefab);
                    newGameObject.transform.SetParent (this.transform, false);
                    newGameObject.SetActive (true);
                    newGameObject.transform.position = new Vector3 (x - size / 2f + (float) random.NextDouble (), 0, y - size / 2f + (float) random.NextDouble ());
                }
            }
        }
    }
}
