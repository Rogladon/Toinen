using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour {
	private float[] xPositions;
	private float[] yPositions;
	private float[] velocities;
	private float[] accelerations;
	private LineRenderer lineRender;
	private GameObject[] meshObjects;
	private Mesh[] meshes;
	
	private float height;
	private float left;
	private float bottom;
	private float width;

	[Header("Constants")]
	public int splitCount = 5;
	public float spring = 0.02f;
	public float damping = 0.04f;
	public float spread = 0.05f;
	public float z;
	public float velocity;
	public int deltaCount;
	public float deltaTime;

	[Header("Size")]
	public float heightWater;
	public float widhtWater;

	[Header("Components")]
	public Material mat;
	public GameObject watermesh;
	

	
	private float _time;
	private int edgecount = 0;
	private int nodecount = 0;


	private void Start() {
		left = transform.position.x;
		width = widhtWater;
		bottom = transform.position.y;
		height = heightWater+bottom;
		SpawnWater();
	}

	public void ReSpawn() {
		RemoveWater();
		width = widhtWater;
		SpawnWater();
	}

	private void SpawnWater() {
		// TODO: комментарии
		edgecount = Mathf.RoundToInt(width) * splitCount;
		nodecount = edgecount + 1;
		if (lineRender == null)
			lineRender = gameObject.AddComponent<LineRenderer>();
		else lineRender.enabled = true;
		lineRender.material = mat;
		lineRender.positionCount = nodecount;
		lineRender.startWidth = 0.05f;
		lineRender.endWidth = 0.05f; 
		xPositions = new float[nodecount];
		yPositions = new float[nodecount];
		velocities = new float[nodecount];
		accelerations = new float[nodecount];

		meshObjects = new GameObject[edgecount];
		meshes = new Mesh[edgecount];

		for (int i = 0; i < nodecount; i++) {
			yPositions[i] = height;
			xPositions[i] = left + width * i / edgecount;
			accelerations[i] = 0;
			velocities[i] = 0;
			lineRender.SetPosition(i, new Vector3(xPositions[i], yPositions[i], z));
		}
		for (int i = 0; i < edgecount; i++) {
			meshes[i] = new Mesh();
			Vector3[] Vertices = new Vector3[4];
			Vertices[0] = new Vector3(xPositions[i], yPositions[i], z);
			Vertices[1] = new Vector3(xPositions[i + 1], yPositions[i + 1], z);
			Vertices[2] = new Vector3(xPositions[i], bottom, z);
			Vertices[3] = new Vector3(xPositions[i + 1], bottom, z);
			Vector2[] UVs = new Vector2[4];
			UVs[0] = new Vector2(0, 1);
			UVs[1] = new Vector2(1, 1); 
			UVs[2] = new Vector2(0, 0); 
			UVs[3] = new Vector2(1, 0); 
			int[] tris = new int[6] { 0, 1, 3, 3, 2, 0 };
			meshes[i].vertices = Vertices;
			meshes[i].uv = UVs;
			meshes[i].triangles = tris;
			meshObjects[i] = Instantiate(watermesh, Vector3.zero, Quaternion.identity) as GameObject;
			meshObjects[i].GetComponent<MeshFilter>().mesh = meshes[i];
			meshObjects[i].transform.parent = transform;
		}
	}

	public void RemoveWater() {
		for(int i = 0; i < edgecount; i++) {
			Destroy(meshes[i]);
			Destroy(meshObjects[i]);
		}
		edgecount = 0;
		nodecount = 0;
		if(lineRender != null) 
			lineRender.enabled = false;
	}

	void UpdateMeshes() {
		for (int i = 0; i < edgecount; i++) {

			Vector3[] Vertices = new Vector3[4];
			Vertices[0] = new Vector3(xPositions[i], yPositions[i], z);
			Vertices[1] = new Vector3(xPositions[i + 1], yPositions[i + 1], z);
			Vertices[2] = new Vector3(xPositions[i], bottom, z);
			Vertices[3] = new Vector3(xPositions[i + 1], bottom, z);

			meshes[i].vertices = Vertices;
			Vector3 pos = meshObjects[i].transform.localPosition;
			pos.z = 0;
			meshObjects[i].transform.localPosition = pos;
		}
	}

	void Update() {
		if (nodecount == 0 || edgecount == 0) {
			return;
		}
		_time += Time.deltaTime;
		if (_time >= deltaTime) {
			for(int i = 0; i < xPositions.Length; i+=deltaCount) {
				deltaCount = Random.Range(1, 3);
				velocity = -velocity;
				Splash(i);
			}
			_time = 0;
		}
		UpdateMeshes();
		for (int i = 0; i < nodecount; i++) {
			float force = spring * (yPositions[i] - height) + velocities[i] * damping;
			accelerations[i] = -force;
			yPositions[i] += velocities[i];
			velocities[i] += accelerations[i];
			lineRender.SetPosition(i, new Vector3(xPositions[i], yPositions[i], -meshObjects[0].transform.position.z));
		}
		float[] leftDeltas = new float[xPositions.Length];
		float[] rightDeltas = new float[xPositions.Length];

		for (int j = 0; j < 8; j++) {
			for (int i = 0; i < xPositions.Length; i++) {
				if (i > 0) {
					leftDeltas[i] = spread * (yPositions[i] - yPositions[i - 1]);
					velocities[i - 1] += leftDeltas[i];
				}
				if (i < xPositions.Length - 1) {
					rightDeltas[i] = spread * (yPositions[i] - yPositions[i + 1]);
					velocities[i + 1] += rightDeltas[i];
				}
			}
		}
		for (int i = 0; i < xPositions.Length; i++) {
			if (i > 0) {
				yPositions[i - 1] += leftDeltas[i];
			}
			if (i < xPositions.Length - 1) {
				yPositions[i + 1] += rightDeltas[i];
			}
		}
	}

	public void Splash(int index) {
		velocities[index] = velocity;
	}
}
