using System.Collections;
using UnityEngine;

public class CutSceneFuntions : MonoBehaviour
{
    public IEnumerator ActorMoveEvent(Actor actor, float moveTime)
    {
        if (actor == null)
        {
               yield break;
        } 
    }

    public IEnumerator CameraMoveEvent(Camera camera, Vector3 firstPos, Vector3 lastPos, float moveTime)
    {
        if (camera == null)
        {
            yield break;
        }
    }
}
