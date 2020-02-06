using System.Collections.Generic;
using UnityEngine;

namespace Misc
{
    public class LevelUtils
    {
        public static bool IsOverlapping(Vector2 colliderPosition, BoxCollider2D colliderToCheck, List<GameObject> listToCheck)
        {
            Collider2D[] overlapping = Physics2D.OverlapBoxAll(colliderPosition, colliderToCheck.size, 0);

            for (int k = 0; k < overlapping.Length; k++)
            {
                try
                {
                    if (listToCheck.Contains(overlapping[k].gameObject) || listToCheck.Contains(overlapping[k].transform.parent.gameObject))
                    {
                        return true;
                    }
                }
                catch (System.NullReferenceException)
                {
                    continue;
                }
            }

            return false;
        }

        public static bool IsReaching(Vector2 colliderPosition, BoxCollider2D colliderToCheck, List<GameObject> listToCheck, int ignoreFirstItems)
        {
            Collider2D[] overlapping = Physics2D.OverlapBoxAll(colliderPosition, colliderToCheck.size, 0);

            if (listToCheck.Count < ignoreFirstItems)
                return true;

            for (int k = 0; k < overlapping.Length; k++)
            {
                if (listToCheck.Contains(overlapping[k].gameObject))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
