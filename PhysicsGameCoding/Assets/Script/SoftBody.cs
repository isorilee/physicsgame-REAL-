using System;
using UnityEngine;



//force the game object to always have a component 
//if it doesnt unity attaches one 
[RequireComponent (typeof(SkinnedMeshRenderer))]
public class SoftBody : MonoBehaviour
{
    [Range(0, 2f)]
    public float softness = 1; //how far verticies can move (higher = more floppy)
    //how much motion slows down (like friction)
    [Range(0.01f, 1f)]
    public float damping = 0.1f; 
    //how resistent it is to bending 
    public float stiffness = 1f;
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CreateSoftBodyPhysics();
    }

    // Update is called once per frame
    void CreateSoftBodyPhysics()
    {
        SkinnedMeshRenderer smr = GetComponent<SkinnedMeshRenderer>();
        if (smr == null) return; 

        //adds unity cloth physics component to object at runtime 
        Cloth cloth = gameObject.GetComponent<Cloth>();
        cloth.damping = damping; 
        cloth.bendingStiffness = stiffness;

        //every Vertex in the mesh gets a physics rule 
        //we generate the rules with our function 
        cloth.coefficients = GenerateClothCoefficients(smr.sharedMesh.vertices.Length);
    }

    //we are making an array so we have multiple coefficients for all the verticies 
    //ex: mesh might hvae 500 verticies so cloth needs 500 coefficients (one per vertex) 
    //so thats why we are returning an array
    private ClothSkinningCoefficient[] GenerateClothCoefficients(int vertexCount)
    {
        //[]creates an array, one entry per vertex 
        //make a list with verexcount slots 
        ClothSkinningCoefficient[] coefficients = new ClothSkinningCoefficient[vertexCount];

        //loop through every vertex 
        //set rules for eacg vertex 1 by 1 

        for (int i = 0; i < vertexCount; i++)
        {
            //how far that vertex can move 
            coefficients[i].maxDistance = softness;
            //collision buffer 0 = tight 
            coefficients[i].collisionSphereDistance = 0f;

            //so basically every vertex can move up to softness distance


        }

        return coefficients; //send it back to the cloth component 
    }
}
