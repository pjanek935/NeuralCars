using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassCreator : MonoBehaviour
{
    [SerializeField] MeshCollider roadMeshCollider;
    [SerializeField] GameObject grassPrefab;
    List<GameObject> grass = new List<GameObject> ();
    [SerializeField] float scale = 1f;
    [SerializeField] int size = 100;
    [SerializeField] float range = 250f;
    [Range (0,1)] [SerializeField] float density = 0.1f;

    private void OnEnable ()
    {
        randomize ();
        disableGrassOnRoad ();
    }

    void disableGrassOnRoad ()
    {
        for (int i = 0; i < grass.Count; i ++)
        {
            RaycastHit hit;
            int layerMask = LayerMask.GetMask (GlobalConst.ROAD_LAYER);

            if (Physics.SphereCast (grass [i].transform.position + new Vector3 (0f, 5f, 0f), 2f, Vector3.down, out hit, 100f, layerMask))
            {
                grass [i].SetActive (false);
            }
            else
            {
                grass [i].SetActive (true);
            }
        }
    }

    void randomize ()
    {
        System.Random random = new System.Random ();

        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                float perlin = Mathf.PerlinNoise (x * scale, y * scale);

                if (perlin > 1f - density)
                {
                    double r = random.NextDouble ();
                    Vector3 pos = new Vector3 (x - size / 2f + (float) random.NextDouble (),
                        0, y - size / 2f + (float) random.NextDouble ());
                    float d = Vector3.Distance (pos, Vector3.zero);
                    bool add = false;

                    if (d < range)
                    {
                        add = true;
                    }
                    else
                    {
                        float diff = (d - range) / 100f;
                        diff = Mathf.Clamp01 (diff);

                        r = random.NextDouble ();

                        if (r > diff)
                        {
                            add = true;
                        }
                    }

                    if (add)
                    {
                        GameObject newGameObject = Instantiate (grassPrefab);
                        newGameObject.transform.SetParent (this.transform, false);
                        newGameObject.SetActive (true);
                        newGameObject.transform.position = pos;
                        grass.Add (newGameObject);
                    }
                    
                }
            }
        }
    }
}
