using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Habrador_Computational_Geometry;



public class PolygonClippingController : MonoBehaviour
{
    public Transform polyAParent;
    public Transform polyBParent;



    void OnDrawGizmos()
    {
        //Generate the polygons
        List<Vector3> polygonA = GetVerticesFromParent(polyAParent);
        List<Vector3> polygonB = GetVerticesFromParent(polyBParent);

        //3d to 2d
        List<MyVector2> polygonA_2D = new List<MyVector2>();
        List<MyVector2> polygonB_2D = new List<MyVector2>();

        foreach (Vector3 v in polygonA)
        {
            polygonA_2D.Add(v.ToMyVector2());
        }

        foreach (Vector3 v in polygonB)
        {
            polygonB_2D.Add(v.ToMyVector2());
        }


        //Display the original polygons
        DisplayPolygon(polygonA, Color.white);
        DisplayPolygon(polygonB, Color.blue);



        List<MyVector2> poly = polygonB_2D;
        List<MyVector2> clipPoly = polygonA_2D;

        //Clipping algortihms
        //Algortihm 1.Sutherland-Hodgman will return the intersection of the polygons
        //Requires that the clipping polygon (the polygon we want to remove from the other polygon) is convex
        //TestSutherlandHodgman(poly, clipPoly);



        //Alorithm 2. Greiner-Hormann. Can do all boolean operations on all types of polygons
        //but fails when a vertex is on the other polygon's edge
        TestGreinerHormann(poly, clipPoly);
    }



    private void TestSutherlandHodgman(List<MyVector2> poly, List<MyVector2> clipPoly)
    {
        List<MyVector2> polygonAfterClipping = SutherlandHodgman.ClipPolygon(poly, clipPoly);

        //2d to 3d
        List<Vector3> polygonAfterClipping3D = new List<Vector3>();

        foreach (MyVector2 v in polygonAfterClipping)
        {
            polygonAfterClipping3D.Add(v.ToVector3());
        }

        DisplayPolygon(polygonAfterClipping3D, Color.red);
    }



    private void TestGreinerHormann(List<MyVector2> poly, List<MyVector2> clipPoly)
    {
        //In this case we can get back multiple parts of the polygon because one of the 
        //polygons doesnt have to be convex
        //If you pick boolean operation: intersection you should get the same result as with the Sutherland-Hodgman
        List<List<MyVector2>> finalPolygon = GreinerHormann.ClipPolygons(poly, clipPoly, BooleanOperation.Intersection);

        for (int i = 0; i < finalPolygon.Count; i++)
        {
            List<MyVector2> thisPolygon = finalPolygon[i];

            //2d to 3d
            List<Vector3> polygonAfterClipping3D = new List<Vector3>();

            foreach (MyVector2 v in thisPolygon)
            {
                polygonAfterClipping3D.Add(v.ToVector3());

                DisplayPolygon(polygonAfterClipping3D, Color.red);
            }
        }
    }



    //Display one polygon's vertices and lines between the vertices
    private void DisplayPolygon(List<Vector3> vertices, Color color)
    {
        Gizmos.color = color;

        //Draw the polygons vertices
        float vertexSize = 0.05f;

        for (int i = 0; i < vertices.Count; i++)
        {
            Gizmos.DrawSphere(vertices[i], vertexSize);
        }

        //Draw the polygons outlines
        for (int i = 0; i < vertices.Count; i++)
        {
            int iPlusOne = MathUtility.ClampListIndex(i + 1, vertices.Count);

            Gizmos.DrawLine(vertices[i], vertices[iPlusOne]);
        }
    }



    //Get child vertex positions from parent trans
    private List<Vector3> GetVerticesFromParent(Transform parent)
    {
        int childCount = parent.childCount;

        List<Vector3> children = new List<Vector3>();

        for (int i = 0; i < childCount; i++)
        {
            children.Add(parent.GetChild(i).position);
        }

        return children;
    }
}