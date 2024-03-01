using System;
using UnityEngine;

public enum PieceType
{
    Core,
    Center,
    Edge,
    Corner
}
[Serializable]
public class Piece : MonoBehaviour
{
    public int id;
    public PieceType type;
    public float rotation = 0;

    public Piece(int id)
    {
        this.id = id;
    }
}
