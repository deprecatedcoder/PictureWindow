/*
 *
 *  FlipNormals.cs	(c) Ryan Sullivan 2018
 *  url: http://smirkingcat.software/
 *
 *  Grabs the Mesh and flips all the tris
 *  
 */

using System.Linq;
using UnityEngine;

public class FlipNormals : MonoBehaviour {

	
    void Start ()
    {
		
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        mesh.triangles = mesh.triangles.Reverse().ToArray();

    }


}
