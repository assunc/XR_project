using UnityEngine;
using Seb.Fluid.Simulation;

namespace Seb.Fluid.Rendering
{
	[ImageEffectAllowedInSceneView]
	public class RayMarchingTest : MonoBehaviour
	{
		[Header("Settings")]
		public float densityOffset = 150;
		public int numRefractions = 4;
		public Vector3 extinctionCoefficients;
		public float densityMultiplier = 0.001f;
		[Min(0.01f)] public float stepSize = 0.02f;
		public float lightStepSize = 0.4f;
		[Min(1)] public float indexOfRefraction = 1.33f;
		public Vector3 testParams;
		public EnvironmentSettings environmentSettings;

		[Header("References")]
		public FluidSim sim; 

		public Transform[] cubeTransforms;  
		// public Transform cubeTransform;  
		// public Transform cubeTransform2;  
		// public ComputeBuffer cubeBufferWorldToLocal{get;  private set; }
		// public ComputeBuffer cubeBufferLocalToWorld{get;  private set; }

		public Shader shader;
		Material raymarchMat;

		void Start()
		{
			raymarchMat = new Material(shader);
			sim = GameObject.FindGameObjectWithTag("SimulationBox").GetComponent<FluidSim>();
			Debug.Log("Rendering: " + sim.DensityMap);
			Camera.main.depthTextureMode = DepthTextureMode.Depth;
		}

		[ImageEffectOpaque]
		void OnRenderImage(RenderTexture src, RenderTexture target)
		{

			if (sim.DensityMap != null)
			{
				SetShaderParams();
				Graphics.Blit(src, target, raymarchMat);
			}
			else
			{
				Graphics.Blit(src, target);
			}
		}

		void SetShaderParams()
		{
			//fix
			SetEnvironmentParams(raymarchMat, environmentSettings);
			raymarchMat.SetTexture("DensityMap", sim.DensityMap);
			raymarchMat.SetVector("boundsSize", sim.Scale);
			raymarchMat.SetFloat("volumeValueOffset", densityOffset);
			raymarchMat.SetVector("testParams", testParams);
			raymarchMat.SetFloat("indexOfRefraction", indexOfRefraction);
			raymarchMat.SetFloat("densityMultiplier", densityMultiplier / 1000);
			raymarchMat.SetFloat("viewMarchStepSize", stepSize);
			raymarchMat.SetFloat("lightStepSize", lightStepSize);
			raymarchMat.SetInt("numRefractions", numRefractions);
			raymarchMat.SetVector("extinctionCoeff", extinctionCoefficients);

			// New
			//for (int i = 0; i < cubeTransforms.Length; i++)
			//{
			//	Matrix4x4 localToWorld = Matrix4x4.TRS(cubeTransforms[i].position, cubeTransforms[i].rotation, cubeTransforms[i].localScale / 2);
			//	Matrix4x4 worldToLocal = localToWorld.inverse;
			//	raymarchMats.SetMatrixArray("cubeLocalToWorld", cubeTransforms.Select(t => Matrix4x4.TRS(t.position, t.rotation, t.localScale / 2)).ToArray());
			//	raymarchMats.SetMatrixArray("cubeWorldToLocal", cubeTransforms.Select(t => Matrix4x4.TRS(t.position, t.rotation, t.localScale / 2).inverse).ToArray());
			//}
			
			
			if(cubeTransforms != null && cubeTransforms.Length >0){
				Matrix4x4[] cubeLocalToWorldArray = new Matrix4x4[cubeTransforms.Length];
				Matrix4x4[] cubeWorldToLocalArray = new Matrix4x4[cubeTransforms.Length];
				for (int i = 0; i < cubeTransforms.Length; i++)
				{
					var cube = cubeTransforms[i];
					Matrix4x4 localToWorld = Matrix4x4.TRS(cube.position, cube.rotation, cube.localScale / 2);
					cubeLocalToWorldArray[i] = localToWorld;
					cubeWorldToLocalArray[i] = localToWorld.inverse;
				}
				raymarchMat.SetMatrixArray("cubesLocalToWorld", cubeLocalToWorldArray);
				raymarchMat.SetMatrixArray("cubesWorldToLocal", cubeWorldToLocalArray);
				raymarchMat.SetInt("cubeCount", cubeTransforms.Length); 

				//cubeBufferLocalToWorld = CreateStucturedBuffer<float4x4>(cubeTransforms.Length); 
				//cubeBufferWorldToLocal = CreateStucturedBuffer<float4x4>(cubeTransforms.Length); 
				
				//cubeBufferLocalToWorld.SetData(cubeTransforms.Select(cube=>Matrix4x4.TRS(cube.position, cube.rotation, cube.localScale / 2) )); 
				//cubeBufferWorldToLocal.SetData(cubeTransforms.Select(cube=>Matrix4x4.TRS(cube.position, cube.rotation, cube.localScale / 2).inverse));
				
				//raymarchMat.setBuffer(2, "cubesLocalToWorld", cubeBufferLocalToWorld);
				//raymarchMat.setBuffer(2, "cubesWorldToLocal", cubeBufferWorldToLocal);

				//raymarchMat.SetMatrixArray("cubesLocalToWorld", cubeBufferLocalToWorld); 

			}

			// raymarchMat.SetMatrix("cubeLocalToWorld", Matrix4x4.TRS(cubeTransform.position, cubeTransform.rotation, cubeTransform.localScale / 2));
			// raymarchMat.SetMatrix("cubeWorldToLocal", Matrix4x4.TRS(cubeTransform.position, cubeTransform.rotation, cubeTransform.localScale / 2).inverse);
			
			// raymarchMat.SetMatrix("cube2LocalToWorld", Matrix4x4.TRS(cubeTransform2.position, cubeTransform2.rotation, cubeTransform2.localScale / 2));
			// raymarchMat.SetMatrix("cube2WorldToLocal", Matrix4x4.TRS(cubeTransform2.position, cubeTransform2.rotation, cubeTransform2.localScale / 2).inverse);
			
			Vector3 floorSize = new Vector3(30, 0.05f, 30);
			float floorHeight = -sim.Scale.y / 2 + sim.transform.position.y - floorSize.y/2;
			raymarchMat.SetVector("floorPos", new Vector3(0, floorHeight, 0));
			raymarchMat.SetVector("floorSize", floorSize);
		}

		public static void SetEnvironmentParams(Material mat, EnvironmentSettings environmentSettings)
		{
			mat.SetColor("tileCol1", environmentSettings.tileCol1);
			mat.SetColor("tileCol2", environmentSettings.tileCol2);
			mat.SetColor("tileCol3", environmentSettings.tileCol3);
			mat.SetColor("tileCol4", environmentSettings.tileCol4);
			mat.SetVector("tileColVariation", environmentSettings.tileColVariation);
			mat.SetFloat("tileScale", environmentSettings.tileScale);
			mat.SetFloat("tileDarkOffset", environmentSettings.tileDarkOffset);
			mat.SetVector("dirToSun", -environmentSettings.light.transform.forward);
		}

		[System.Serializable]
		public struct EnvironmentSettings
		{
			public Color tileCol1;
			public Color tileCol2;
			public Color tileCol3;
			public Color tileCol4;
			public Vector3 tileColVariation;
			public float tileScale;
			public float tileDarkOffset;
			public Light light;
		}
		// void OnDrawGizmos()
		// {
		// 	if (sim != null)
		// 	{
		// 		Gizmos.color = Color.red; // Or any color for comparison
		// 		Gizmos.DrawWireCube(sim.transform.position, sim.transform.localScale);
		// 	}
		// }
	}
}