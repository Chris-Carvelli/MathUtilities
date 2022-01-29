using UnityEngine;

public class CCDIKJoint : MonoBehaviour {
  public Vector3 axis = Vector3.right;
  public float maxAngle = 180;
  Vector3 perpendicular; 
  public Transform overrideParent;

  [Header("Debug")]
  public Vector3 baseRot;
  public Vector3 hingeRot;
  public Vector3 limitRot;
  public Vector3 normal;
  public Vector3 parentRotDebug;
  public bool rotateToDirection;

  void Start()
  {
      perpendicular = axis.Perpendicular();
      if (overrideParent is null)
          overrideParent = transform.parent;
  }

  public void Evaluate(Transform ToolTip, Transform Target, bool rotateToDirection = false) {
    var t = transform;
    var rot = t.rotation;
    var pos = t.position;
    var parentRot = overrideParent.rotation;
    parentRotDebug = parentRot.eulerAngles;
    this.rotateToDirection = rotateToDirection;
    
    //Rotate the assembly so the tooltip better matches the target position/direction
    rot = (rotateToDirection ? Quaternion.FromToRotation(ToolTip.up, Target.forward) : Quaternion.FromToRotation(ToolTip.position - pos, Target.position - pos)) * rot;
    baseRot = rot.eulerAngles;
    
    //Enforce only rotating with the hinge
    rot = Quaternion.FromToRotation(rot * axis, parentRot * axis) * rot;
    hingeRot = rot.eulerAngles;

    //Enforce Joint Limits
    rot = Quaternion.FromToRotation(rot * perpendicular, (rot * perpendicular).ConstrainToNormal(parentRot * perpendicular, maxAngle)) * rot;
    limitRot = rot.eulerAngles;
    normal = parentRot * perpendicular;

    t.rotation = rot;
  }
}
