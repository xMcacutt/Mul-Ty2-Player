namespace MT2PClient;

public struct PlayerPositionData
{
    public float X;
    public float Y;
    public float Z;
    public float Pitch;
    public float Yaw;
    public float Roll;

    public float[] GetPosFloats()
    {
        return new[] { X, Y, Z };
    }
    
    public float[] GetRotFloats()
    {
        return new[] { Pitch, Yaw, Roll };
    }

    public void SetPos(float[] coords)
    {
        X = coords[0];
        Y = coords[1];
        Z = coords[2];
    }
    
    public void SetRot(float[] coords)
    {
        Pitch = coords[0];
        Yaw = coords[1];
        Roll = coords[2];
    }

    public byte[] GetPosBytes()
    {
        return BitConverter.GetBytes(X).Concat(BitConverter.GetBytes(Y)).Concat(BitConverter.GetBytes(Z)).ToArray();
    }
    
}