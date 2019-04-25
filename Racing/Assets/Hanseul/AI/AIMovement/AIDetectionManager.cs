using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIDetectionManager : MonoBehaviour
{
    public bool LeftForwardSideDetection(Vector3 sensorStartPos, ref RaycastHit hit, float sensorLength, Vector3 frontSensorPosition)
    {
        sensorStartPos -= GetComponent<Transform>().right * 2 *sensorLength; 
        return Physics.Raycast(sensorStartPos, -GetComponent<Transform>().right, out hit, sensorLength);
    }

    public bool LeftForwardAngledDection(Vector3 sensorStartPos, ref RaycastHit hit, float sensorLength, float frontSensorAngle)
    {
        sensorStartPos -= GetComponent<Transform>().right * 2 * sensorLength;
        return (Physics.Raycast(sensorStartPos, Quaternion.AngleAxis(-frontSensorAngle, GetComponent<Transform>().up) * GetComponent<Transform>().forward, out hit, sensorLength));
    }

    public bool RightForwardSideDetection(Vector3 sensorStartPos, ref RaycastHit hit, float sensorLength)
    {
        sensorStartPos += GetComponent<Transform>().right * sensorLength;
        return Physics.Raycast(sensorStartPos, GetComponent<Transform>().right, out hit, sensorLength);
    }

    public bool RightForwardAngledDectection(Vector3 sensorStartPos, ref RaycastHit hit, float sensorLength, float frontSensorAngle)
    {
        sensorStartPos += GetComponent<Transform>().right * sensorLength;
        return (Physics.Raycast(sensorStartPos, Quaternion.AngleAxis(frontSensorAngle, GetComponent<Transform>().up) * GetComponent<Transform>().forward, out hit, sensorLength));
    }
    public bool LeftSideDetection(ref RaycastHit hit, float sensorLength)
    {
        Vector3 sensorStartPos = GetComponent<Transform>().position;
        sensorStartPos -= GetComponent<Transform>().right * 2;
        return Physics.Raycast(sensorStartPos, -GetComponent<Transform>().right, out hit, sensorLength);
    }
    public bool RightSideDetection(ref RaycastHit hit, float sensorLength)
    {
        Vector3 sensorStartPos = GetComponent<Transform>().position;
        sensorStartPos = GetComponent<Transform>().right * 2;
        return Physics.Raycast(sensorStartPos, GetComponent<Transform>().right, out hit, sensorLength);
    }

    public bool LeftBackDetection(Vector3 position, ref RaycastHit hit, float sensorLength)
    {
        position -= GetComponent<Transform>().right * 2 * sensorLength;
        return Physics.Raycast(position, -GetComponent<Transform>().right, out hit, sensorLength);
    }

    public bool LeftBackAngledDetection(Vector3 position, ref RaycastHit hit, float sensorLength, float backSensorAngle)
    {
        position -= GetComponent<Transform>().right * 2 * sensorLength;
        return (Physics.Raycast(position, Quaternion.AngleAxis(-backSensorAngle, GetComponent<Transform>().up) * -GetComponent<Transform>().forward, out hit, sensorLength));
    }

    public bool RightBackDetection(Vector3 position, ref RaycastHit hit, float sensorLength)
    {
        position += GetComponent<Transform>().right * sensorLength;
        return Physics.Raycast(position, GetComponent<Transform>().right, out hit, sensorLength);
    }

    public bool RightBackAngledDetection(Vector3 position, ref RaycastHit hit, float sensorLength, float backSensorAngle)
    {
        position += GetComponent<Transform>().right * sensorLength;
        return (Physics.Raycast(position, Quaternion.AngleAxis(backSensorAngle, GetComponent<Transform>().up) * -GetComponent<Transform>().forward, out hit, sensorLength));
    }


}
