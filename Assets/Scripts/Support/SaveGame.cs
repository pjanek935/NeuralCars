using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Logic
{
    [Serializable]
    public class SaveGame
    {
        public List<int> StarsGained = new List<int> ();

        public int GetTotalStarsGained ()
        {
            int result = 0;

            if (StarsGained != null)
            {
                for (int i = 0; i < StarsGained.Count; i ++)
                {
                    result += StarsGained [i];
                }
            }

            return result;
        }

        public int GetStarsGained (int stageId)
        {
            int result = 0;

            if (StarsGained != null)
            {
                if (stageId >= 0 && stageId < StarsGained.Count)
                {
                    result = StarsGained [stageId];
                }
            }

            return result;
        }

        public void SetStarsGained (int stageId, int starsGained)
        {
            if (StarsGained == null)
            {
                StarsGained = new List<int> ();
            }

            if (stageId >= 0)
            {
                while (stageId + 1 > StarsGained.Count)
                {
                    StarsGained.Add (0);
                }

                StarsGained [stageId] = starsGained;
            }
        }
    }
}

