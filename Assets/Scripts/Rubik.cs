using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.HID;
using Random = System.Random;

public enum Faces
{
	White,
	Yellow,
	Blue,
	Green,
	Orange,
	Red
}

public class Rubik : MonoBehaviour
{
	private const int CubeSize = 3;
	public readonly Piece[,,] CubeMatrix = new Piece[CubeSize,CubeSize,CubeSize];
	private Piece[,,] _solvedMatrix = new Piece[CubeSize,CubeSize,CubeSize];
	public static Dictionary<Faces, Vector3> FaceNormals = new Dictionary<Faces, Vector3>
	{
		{Faces.White, new Vector3(0,1,0)},
		{Faces.Yellow, new Vector3(0,-1,0)},
		{Faces.Orange, new Vector3(1,0,0)},
		{Faces.Red, new Vector3(-1,0,0)},
		{Faces.Green, new Vector3(0,0,1)},
		{Faces.Blue, new Vector3(0,0,-1)}
		
	};

	private List<Action> _functions = new List<Action>();
	public GameObject piecePrefab;
	void Start()
	{
		var offset = CubeSize / 2;
		transform.position = new Vector3(offset, offset, offset);
		int id = 0;
		for (int i = 0; i < CubeMatrix.GetLength(0); i++)
		{
			for (int j = 0; j < CubeMatrix.GetLength(1); j++)
			{
				for (int k = 0; k < CubeMatrix.GetLength(2); k++)
				{
					CubeMatrix[i, j, k] =  Instantiate(piecePrefab.GetComponent<Piece>(), new Vector3(i, j, k), Quaternion.identity);
					//cubeMatrix[i, j, k].transform.parent = gameObject.transform;
					CubeMatrix[i, j, k].id = id++;
					
					//Is a corner
					if (i == 0 && j == 0 && k == 0 ||
					    i == 0 && j == 0 && k == CubeSize-1 ||
					    i == 0 && j == CubeSize-1 && k == 0 ||
					    i == 0 && j == CubeSize-1 && k == CubeSize-1 ||
					    i == CubeSize-1 && j == 0 && k == 0 ||
					    i == CubeSize-1 && j == 0 && k == CubeSize-1 ||
					    i == CubeSize-1 && j == CubeSize-1 && k == 0 ||
					    i == CubeSize-1 && j == CubeSize-1 && k == CubeSize-1)
					{
						CubeMatrix[i, j, k].type = PieceType.Corner;
					}
					else
					//Is a center
					//TODO no escalable (>0 && <cubeSize-1?)
					if (i == j && k == 0 ||
						i == j && k == CubeSize-1 ||
					    j == k && i == 0 ||
					    j == k && i == CubeSize-1 ||
					    i == k && j == 0 ||
					    i == k && j == CubeSize-1)
					{
						CubeMatrix[i, j, k].type = PieceType.Center;
					}
					else
					//Is an edge
					if (i == 0 ||
					    i == CubeSize-1 ||
					    j == 0 ||
					    j == CubeSize-1 ||
					    k == 0 ||
					    k == CubeSize-1)
					{
						CubeMatrix[i, j, k].type = PieceType.Edge;
					}
					
				}
			}
		}

		_solvedMatrix = (Piece[,,])CubeMatrix.Clone();
		
		_functions.Add(() => TurnWhiteFace(true));
		_functions.Add(() => TurnWhiteFace(false));
		_functions.Add(() => TurnYellowFace(true));
		_functions.Add(() => TurnYellowFace(false));
		_functions.Add(() => TurnOrangeFace(true));
		_functions.Add(() => TurnOrangeFace(false));
		_functions.Add(() => TurnRedFace(true));
		_functions.Add(() => TurnRedFace(false));
		_functions.Add(() => TurnGreenFace(true));
		_functions.Add(() => TurnGreenFace(false));
		_functions.Add(() => TurnBlueFace(true));
		_functions.Add(() => TurnBlueFace(false));
	}

	public Vector3 FindPieceById(int id)
	{
		for (int i = 0; i < CubeMatrix.GetLength(0); i++)
		{
			for (int j = 0; j < CubeMatrix.GetLength(1); j++)
			{
				for (int k = 0; k < CubeMatrix.GetLength(2); k++)
				{
					if (CubeMatrix[i, j, k].id == id)
					{
						return new Vector3(i, j, k);
					}
				}
			}
		}

		return Vector3.negativeInfinity;
	}
	

	public static Faces IdentifyFace(Vector3 normal) {
        switch (normal) {
            case Vector3 v when v == FaceNormals[Faces.White]:
				return Faces.White;
            case Vector3 v when v == FaceNormals[Faces.Yellow]:
                return Faces.Yellow;
            case Vector3 v when v == FaceNormals[Faces.Blue]:
                return Faces.Blue;
            case Vector3 v when v == FaceNormals[Faces.Green]:
                return Faces.Green;
            case Vector3 v when v == FaceNormals[Faces.Orange]:
                return Faces.Orange;
            case Vector3 v when v == FaceNormals[Faces.Red]:
                return Faces.Red;
			default:
				return Faces.White;
        }
    }

	public void Turn (Faces face, bool clockwise) {
        switch (face) {
            case Faces.White:
				TurnWhiteFace(clockwise);
                break;
            case Faces.Yellow:
				TurnYellowFace(clockwise);
                break;
            case Faces.Blue:
				TurnBlueFace(clockwise);
                break;
            case Faces.Green:
				TurnGreenFace(clockwise);
                break;
            case Faces.Orange:
				TurnOrangeFace(clockwise);
                break;
            case Faces.Red:
				TurnRedFace(clockwise);
                break;
        }
    }
    public void TurnWhiteFace(bool clockwise)
	{
		const int matrixLength = CubeSize - 1;
		for (int i = 0; i < CubeSize/2; i++)
		{
			//Corners
			if (clockwise)
			{
				RotatePiecesCw(
                	ref CubeMatrix[i, matrixLength, i],
                	ref CubeMatrix[i, matrixLength, matrixLength-i],
                	ref CubeMatrix[matrixLength-i, matrixLength, matrixLength-i],
                	ref CubeMatrix[matrixLength-i, matrixLength, i],
                    FaceNormals[Faces.White]
                );
			}
			else
			{
				RotatePiecesCcw(
					ref CubeMatrix[i, matrixLength, i],
					ref CubeMatrix[i, matrixLength, matrixLength-i],
					ref CubeMatrix[matrixLength-i, matrixLength, matrixLength-i],
					ref CubeMatrix[matrixLength-i, matrixLength, i],
                    FaceNormals[Faces.White]
                );
			}
			
			
			//Edges
			for(int j=i+1; j<=matrixLength-i-1; j++)
			{
				if (clockwise)
				{
					RotatePiecesCw(
						ref CubeMatrix[i, matrixLength, j],
						ref CubeMatrix[j, matrixLength, matrixLength-i],
						ref CubeMatrix[matrixLength-i, matrixLength, matrixLength-j],
						ref CubeMatrix[matrixLength-j, matrixLength, i],
						FaceNormals[Faces.White]
                    );
				}
				else
				{
					RotatePiecesCcw(
						ref CubeMatrix[i, matrixLength, j],
						ref CubeMatrix[j, matrixLength, matrixLength-i],
						ref CubeMatrix[matrixLength-i, matrixLength, matrixLength-j],
						ref CubeMatrix[matrixLength-j, matrixLength, i],
						FaceNormals[Faces.White]
                    );
				}
				
			}
			//TODO Center?
		}
	}
	
	public void TurnYellowFace(bool clockwise)
	{
		const int matrixLength = CubeSize - 1;
		for (int i = 0; i < CubeSize/2; i++)
		{
			//Corners
			if (clockwise)
			{
				RotatePiecesCw(
                	ref CubeMatrix[i, 0, i],
                	ref CubeMatrix[matrixLength-i, 0, i],
                	ref CubeMatrix[matrixLength-i, 0, matrixLength-i],
                	ref CubeMatrix[i, 0, matrixLength-i],
                    FaceNormals[Faces.Yellow]
                );
			}
			else
			{
				RotatePiecesCcw(
					ref CubeMatrix[i, 0, i],
					ref CubeMatrix[matrixLength-i, 0, i],
					ref CubeMatrix[matrixLength-i, 0, matrixLength-i],
					ref CubeMatrix[i, 0, matrixLength-i],
                    FaceNormals[Faces.Yellow]
                );
			}
			
			//Edges
			for(int j=i+1; j<=matrixLength-i-1; j++)
			{
				if (clockwise)
				{
					RotatePiecesCw(
						ref CubeMatrix[j, 0, i],
						ref CubeMatrix[matrixLength-i, 0, j],
						ref CubeMatrix[matrixLength-j, 0, matrixLength-i],
						ref CubeMatrix[i, 0, matrixLength-j],
						FaceNormals[Faces.Yellow]
                    );
				}
				else
				{
					RotatePiecesCcw(
						ref CubeMatrix[j, 0, i],
						ref CubeMatrix[matrixLength-i, 0, j],
						ref CubeMatrix[matrixLength-j, 0, matrixLength-i],
						ref CubeMatrix[i, 0, matrixLength-j],
						FaceNormals[Faces.Yellow]
                    );
				}
				
			}
			//TODO Center?
		}
	}
	
	public void TurnOrangeFace(bool clockwise)
	{
		const int matrixLength = CubeSize - 1;
		for (int i = 0; i < CubeSize/2; i++)
		{
			//Corners
			if (clockwise)
			{
				RotatePiecesCw(
					ref CubeMatrix[matrixLength, matrixLength-i, i],
					ref CubeMatrix[matrixLength, matrixLength-i, matrixLength-i],
					ref CubeMatrix[matrixLength, i, matrixLength-i],
					ref CubeMatrix[matrixLength, i, i],
                    FaceNormals[Faces.Orange]
                );
			}
			else
			{
				RotatePiecesCcw(
					ref CubeMatrix[matrixLength, matrixLength-i, i],
					ref CubeMatrix[matrixLength, matrixLength-i, matrixLength-i],
					ref CubeMatrix[matrixLength, i, matrixLength-i],
					ref CubeMatrix[matrixLength, i, i],
                    FaceNormals[Faces.Orange]
                );
			}
			
			//Edges
			for(int j=i+1; j<=matrixLength-i-1; j++)
			{
				if (clockwise)
				{
					RotatePiecesCw(
						ref CubeMatrix[matrixLength, matrixLength-i, j],
						ref CubeMatrix[matrixLength, matrixLength-j, matrixLength-i],
						ref CubeMatrix[matrixLength, i, matrixLength-j],
						ref CubeMatrix[matrixLength, j, i],
						FaceNormals[Faces.Orange]
                    );
				}
				else
				{
					RotatePiecesCcw(
						ref CubeMatrix[matrixLength, matrixLength-i, j],
						ref CubeMatrix[matrixLength, matrixLength-j, matrixLength-i],
						ref CubeMatrix[matrixLength, i, matrixLength-j],
						ref CubeMatrix[matrixLength, j, i],
						FaceNormals[Faces.Orange]
					);
				}
			}
			//TODO Center?
		}
	}
	
	public void TurnRedFace(bool clockwise)
	{
		const int matrixLength = CubeSize - 1;
		for (int i = 0; i < CubeSize/2; i++)
		{
			//Corners
			if (clockwise)
			{
				RotatePiecesCw(
					ref CubeMatrix[0, matrixLength-i, matrixLength-i],
					ref CubeMatrix[0, matrixLength-i, i],
					ref CubeMatrix[0, i, i],
					ref CubeMatrix[0, i, matrixLength-i],
					FaceNormals[Faces.Red]
				);
			}
			else
			{
				RotatePiecesCcw(
					ref CubeMatrix[0, matrixLength-i, matrixLength-i],
					ref CubeMatrix[0, matrixLength-i, i],
					ref CubeMatrix[0, i, i],
					ref CubeMatrix[0, i, matrixLength-i],
                    FaceNormals[Faces.Red]
                );
			}
			
			
			//Edges
			for(int j=i+1; j<=matrixLength-i-1; j++)
			{
				if (clockwise)
				{
					RotatePiecesCw(
						ref CubeMatrix[0, matrixLength-i, j],
						ref CubeMatrix[0, j, i],
						ref CubeMatrix[0, i, matrixLength-j],
						ref CubeMatrix[0, matrixLength-j, matrixLength-i],
						FaceNormals[Faces.Red]
                    );
				}
				else
				{
					RotatePiecesCcw(
						ref CubeMatrix[0, matrixLength-i, j],
						ref CubeMatrix[0, j, i],
						ref CubeMatrix[0, i, matrixLength-j],
						ref CubeMatrix[0, matrixLength-j, matrixLength-i],
                        FaceNormals[Faces.Red]
                    );
				}
			}
			//TODO Center?
		}
	}
	
	public void TurnGreenFace(bool clockwise)
	{
		const int matrixLength = CubeSize - 1;
		for (int i = 0; i < CubeSize/2; i++)
		{
			//Corners
			if (clockwise)
			{
				RotatePiecesCw(
					ref CubeMatrix[matrixLength-i, matrixLength-i, matrixLength],
					ref CubeMatrix[i, matrixLength-i, matrixLength],
					ref CubeMatrix[i, i, matrixLength],
					ref CubeMatrix[matrixLength-i, i, matrixLength],
					FaceNormals[Faces.Green]
				);
			}
			else
			{
				RotatePiecesCcw(
					ref CubeMatrix[matrixLength-i, matrixLength-i, matrixLength],
					ref CubeMatrix[i, matrixLength-i, matrixLength],
					ref CubeMatrix[i, i, matrixLength],
					ref CubeMatrix[matrixLength-i, i, matrixLength],
                    FaceNormals[Faces.Green]
                );
			}
			
			
			//Edges
			for(int j=i+1; j<=matrixLength-i-1; j++)
			{
				if (clockwise)
				{
					RotatePiecesCw(
						ref CubeMatrix[matrixLength-j, matrixLength-i, matrixLength],
						ref CubeMatrix[i, matrixLength-j, matrixLength],
						ref CubeMatrix[j, i, matrixLength],
						ref CubeMatrix[matrixLength-i, j, matrixLength],
						FaceNormals[Faces.Green]
					);
				}
				else
				{
					RotatePiecesCcw(
						ref CubeMatrix[matrixLength-j, matrixLength-i, matrixLength],
						ref CubeMatrix[i, matrixLength-j, matrixLength],
						ref CubeMatrix[j, i, matrixLength],
						ref CubeMatrix[matrixLength-i, j, matrixLength],
						FaceNormals[Faces.Green]
					);
				}
				
			}
			//TODO Center?
		}
	}
	
	public void TurnBlueFace(bool clockwise)
	{
		const int matrixLength = CubeSize - 1;
		for (int i = 0; i < CubeSize/2; i++)
		{
			//Corners
			if (clockwise)
			{
				RotatePiecesCw(
					ref CubeMatrix[i, matrixLength-i, 0],
					ref CubeMatrix[matrixLength-i, matrixLength-i, 0],
					ref CubeMatrix[matrixLength-i, i, 0],
					ref CubeMatrix[i, i, 0],
                    FaceNormals[Faces.Blue]
                );
			}
			else
			{
				RotatePiecesCcw(
					ref CubeMatrix[i, matrixLength-i, 0],
					ref CubeMatrix[matrixLength-i, matrixLength-i, 0],
					ref CubeMatrix[matrixLength-i, i, 0],
					ref CubeMatrix[i, i, 0],
                    FaceNormals[Faces.Blue]
                );
			}
			
			
			//Edges
			for(int j=i+1; j<=matrixLength-i-1; j++)
			{
				if (clockwise)
				{
					RotatePiecesCw(
						ref CubeMatrix[j, matrixLength-i, 0],
						ref CubeMatrix[matrixLength-i, matrixLength-j, 0],
						ref CubeMatrix[matrixLength-j, i, 0],
						ref CubeMatrix[i, j, 0],
						FaceNormals[Faces.Blue]
                    );
				}
				else
				{
					RotatePiecesCcw(
						ref CubeMatrix[j, matrixLength-i, 0],
						ref CubeMatrix[matrixLength-i, matrixLength-j, 0],
						ref CubeMatrix[matrixLength-j, i, 0],
						ref CubeMatrix[i, j, 0],
						FaceNormals[Faces.Blue]
					);
				}
			}
			//TODO Center?
		}
	}
	
	private static void RotatePiecesCw(ref Piece piece1, ref Piece piece2, ref Piece piece3, ref Piece piece4, Vector3 axis)
	{

		Vector3 center = (piece1.transform.position + piece3.transform.position) /2;
        piece4.transform.RotateAround(center, axis, 90);
        piece1.transform.RotateAround(center, axis, 90);
        piece2.transform.RotateAround(center, axis, 90);
        piece3.transform.RotateAround(center, axis, 90);
        var tempPiece = piece4;
        piece4 = piece3;
		piece3 = piece2;
		piece2 = piece1;
		piece1 = tempPiece;
		
	}
	
	private static void RotatePiecesCcw(ref Piece piece1, ref Piece piece2, ref Piece piece3, ref Piece piece4, Vector3 axis)
	{

        Vector3 center = (piece1.transform.position + piece3.transform.position) / 2;
        piece1.transform.RotateAround(center, axis, -90);
		piece4.transform.RotateAround(center, axis, -90);
		piece3.transform.RotateAround(center, axis, -90);
		piece2.transform.RotateAround(center, axis, -90);

        var tempPiece = piece1;
        piece1 = piece2;
		piece2 = piece3;
		piece3 = piece4;
		piece4 = tempPiece;
	}

	public bool CheckSolved()
	{
		for (int i = 0; i < CubeMatrix.GetLength(0); i++)
		{
			for (int j = 0; j < CubeMatrix.GetLength(1); j++)
			{
				for (int k = 0; k < CubeMatrix.GetLength(2); k++)
				{
					//TODO ignore centers on >3x3
					if (CubeMatrix[i,j,k].id != _solvedMatrix[i,j,k].id)
					{
						return false;
					}
					if (CubeMatrix[i,j,k].type != PieceType.Center && 
					    CubeMatrix[i,j,k].transform.rotation.x != 0 &&
					    CubeMatrix[i,j,k].transform.rotation.y != 0 &&
					    CubeMatrix[i,j,k].transform.rotation.z != 0)
					{
						return false;
					}
				}
			}
		}

		return true;
	}

	public void Randomize()
	{
		Random rnd = new Random();
		for (int i = 0; i < 22; i++)
		{
			_functions[rnd.Next(0, _functions.Count)]();
		}
	}
	public static void PrintCubeMatrix(Piece[,,] matrix)
	{
		int[,,] array = new int[CubeSize,CubeSize,CubeSize];
		for (int i = 0; i < matrix.GetLength(0); i++)
		{
			for (int j = 0; j < matrix.GetLength(1); j++)
			{
				for (int k = 0; k < matrix.GetLength(2); k++)
				{
					array[i, j, k] = matrix[i, j, k].id;
				}
			}
		}
		
		string result = "";
		for(int d0 = 0; d0 < array.GetLength(0);d0++)
		{
			if (d0>0) result += ",";
			result += "[";
			for(int d1 = 0; d1 < array.GetLength(1);d1++)
			{
				if (d1>0) result += ",";
				result += "[";
				for(int d2 = 0; d2 < array.GetLength(2);d2++)
				{
					if (d2>0) result += ",";
					result += array[d0,d1,d2].ToString();
				}
				result += "]\t\t";
			}
			result += "]\n";
		}
		result += "]\n\n";
		print(result);
	}
}