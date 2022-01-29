using UnityEngine;

[ExecuteAlways]
public class CCDIKJoint : MonoBehaviour {
  public Vector3 axis = Vector3.right;
  public float maxAngle = 180;
  public bool shouldOverrideNormal;
  public Vector3 overrideNormal;
  public bool freeHinge;
  
  private Vector3 perpendicular; 

  [Header("Debug")]
  public Vector3 baseRot;
  public Vector3 hingeRot;
  public Vector3 limitRot;
  public Vector3 normal;
  public Vector3 parentRotDebug;
  public bool rotateToDirection;

  private void Start()
  {
      perpendicular = axis.Perpendicular();
  }

  public void Evaluate(Transform ToolTip, Transform Target, bool rotateToDirection = false) {
    var t = transform;
    var rot = t.rotation;
    var pos = t.position;
    var parentRot = t.parent.rotation;
    parentRotDebug = parentRot.eulerAngles;
    normal = shouldOverrideNormal ? overrideNormal : parentRot * perpendicular;
    this.rotateToDirection = rotateToDirection;
    
    //Rotate the assembly so the tooltip better matches the target position/direction
    rot = (
        rotateToDirection
            ? Quaternion.FromToRotation(ToolTip.up, Target.forward)
            : Quaternion.FromToRotation(ToolTip.position - pos, Target.position - pos)
          ) * rot;
    baseRot = rot.eulerAngles;
    
    //Enforce only rotating with the hinge
    if (!freeHinge)
    {
        rot = Quaternion.FromToRotation(rot * axis, parentRot * axis) * rot;
        hingeRot = rot.eulerAngles;
    }

    //Enforce Joint Limits
    rot = Quaternion.FromToRotation(
        rot * perpendicular,
        (rot * perpendicular).ConstrainToNormal(normal, maxAngle)
      ) * rot;
    limitRot = rot.eulerAngles;

    t.rotation = rot;
  }
  
  #if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        var pos = transform.position;
        Gizmos.color = Color.red;
        Gizmos.DrawRay(pos, axis);
        
        Gizmos.color = Color.green;
        Gizmos.DrawRay(pos, normal);
        
        Gizmos.color = Color.white;
    }
#endif
}
