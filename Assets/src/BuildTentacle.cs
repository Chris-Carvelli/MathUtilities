using UnityEngine;

namespace src
{
    public class BuildTentacle : MonoBehaviour
    {
        public int nSegments = 10;
        public float maxAngle = 5;
        public Vector3 segmentNormal;
        public float segmentDistance = .2f;
        public float segmentDistanceFalloff = 1;
        public Vector3 segmentSize;
        public float sizeFalloff = 1;

        private Vector3[] axis = new Vector3[] { Vector3.forward, Vector3.right, Vector3.up };
        private string[] axisLbl = new string[] { "Z", "X", "Y" };

        [ContextMenu("Build")]
        public void Build()
        {
            var t = transform;
            for (int i = 0; i < nSegments; i++)
            {
                for (int j = 0; j < axis.Length; j++)
                {
                    var segment = new GameObject();
                    segment.transform.SetParent(t, false);
                    segment.transform.localPosition = j == 0
                        ? segmentNormal * segmentDistance * Mathf.Pow(segmentDistanceFalloff, i)
                        : Vector3.zero;
                    segment.name = $"segment ({i})({axisLbl[j]})";
                    
                    var joint = segment.AddComponent<CCDIKJoint>();
                    joint.axis = axis[j];
                    joint.maxAngle = maxAngle;
                    joint.freeHinge = true;
                    joint.shouldOverrideNormal = false;

                    if (j == 0)
                    {
                        var mesh = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        mesh.name = $"segment ({i}) visuals";
                        mesh.transform.SetParent(segment.transform);
                        mesh.transform.localPosition = Vector3.zero;
                        mesh.transform.localScale = segmentSize * Mathf.Pow(sizeFalloff, i);
                    }
                    
                    t = segment.transform;
                }
            }
        }
    }
}