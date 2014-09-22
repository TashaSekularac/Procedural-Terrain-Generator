﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PathGenerator2 : MonoBehaviour{
	
	public Texture2D texture;
	
	private Terrain p_terrain;
	public Terrain terrain{set{p_terrain = value;}}
	
	public PathGenerator2(){
		
	}
	
	
	
	
	
	
	public void generate(List<Vector2> p){
		
		List<Vector2> pp = getNewPoints (p);

		List<Vector3> points = get3DPoints(pp);
		List<Vector2> UV = assignUV(pp);
		List<int> triangles = assignTriangles(pp);
		
		createObject(points.ToArray(), UV.ToArray(), triangles.ToArray());
		
		
	}
	
	private List<Vector2> getNewPoints(List<Vector2> p){

		List<Vector2> points = new List<Vector2>();

		points.Add (new Vector3 (p[0].x, getHeight (p[0].x, p[0].y), p[0].y));
		points.Add (new Vector3 (p[1].x, getHeight (p[1].x, p[1].y), p[1].y));
		
		
		for (int i=2; i< p.Count; i+=2){
			
			Vector2 midPoint = p[i-2] + p[i-1] + p[i] + p[i+1];
			midPoint /= 4;
			
			points.Add (new Vector3 (midPoint.x, getHeight (midPoint.x, midPoint.y), midPoint.y));
			points.Add (new Vector3 (p[i].x, getHeight (p[i].x, p[i].y), p[i].y));
			points.Add (new Vector3 (p[i+1].x, getHeight (p[i+1].x, p[i+1].y), p[i+1].y));
		}

		return points;

	}
	
	private List<Vector3> get3DPoints(List<Vector2> p){
		
		List<Vector3> points = new List<Vector3>();
		
		foreach (Vector2 pp in p) {
			points.Add (new Vector3 (pp.x, getHeight (pp.x, pp.y), pp.y));
			
		}
		
		return points;
			
	}
			
	private float getHeight(float x, float y){
		return Terrain.activeTerrain.SampleHeight(new Vector3(x,0,y))+0.4f;
	}


	private List<Vector2> assignUV(List<Vector2> points){
		
		List<Vector2> uv = new List<Vector2>();
		
		for (int i=0; i < points.Count; i++)
						if (i % 3 == 2)
								uv.Add (new Vector2 (0.5f, 0.5f));
						else
								uv.Add (new Vector2 (i % 6 / 3, i % 6 - 3));

		
		return uv;	
	}


	private List<int> assignTriangles(List<Vector2> points){
		
		List<int> triangles = new List<int>();
		
		for(int i=2; i< points.Count; i+=3){
			int[] arr = {
				i-2,i,i-1,
				i+1,i,i-2,
				i+2,i,i+1,
				i-1,i,i+2
		};
			triangles.AddRange(new List<int>(arr));
			
		}
		
		return triangles;
		
	}
	
	private void createObject(Vector3[] newVertices, Vector2[] newUV, int[] newTriangles){
		GameObject gameObject = new GameObject("Some Name Here");
		
		MeshFilter meshFilter = (MeshFilter) gameObject.AddComponent("MeshFilter");
		MeshRenderer meshRenderer = (MeshRenderer) gameObject.AddComponent("MeshRenderer");
		Mesh mesh = meshFilter.mesh;
		mesh.Clear();
		mesh.vertices = newVertices;
		mesh.uv = newUV;
		mesh.triangles = newTriangles;
		mesh.RecalculateNormals();
		mesh.RecalculateBounds();
		mesh.Optimize();
		
		meshRenderer.material.mainTexture = texture;
	}
			
}