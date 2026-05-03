using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

[ExecuteAlways]
[RequireComponent(typeof(SplineContainer))]
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
public class SplineRoad : MonoBehaviour
{
    private SplineContainer splineContainer;
    private MeshFilter meshFilter;
    private MeshCollider meshCollider;
    private Mesh generatedMesh;

    [Header("Road Settings")]
    public float roadWidth = 8f; 
    public float distancePerSegment = 2f; 
    
    [Header("Wall Settings")]
    public bool generateWalls = true;
    public float wallHeight = 1.5f;

    [Header("UV Settings")]
    public float roadTextureTilingY = 10f; 
    public float roadTextureTilingX = 1f;
    public float wallTextureTilingY = 10f;

    // TỐI ƯU GC: Đưa các List lên làm biến toàn cục để tái sử dụng
    private List<Vector3> vertices = new List<Vector3>();
    private List<Vector2> uvs = new List<Vector2>();
    private List<int> roadTriangles = new List<int>();
    private List<int> wallTriangles = new List<int>();

    void OnEnable()
    {
        splineContainer = GetComponent<SplineContainer>();
        meshFilter = GetComponent<MeshFilter>();
        meshCollider = GetComponent<MeshCollider>();
        Spline.Changed += OnSplineChanged;
        GenerateRoadMesh();
    }

    void OnDisable()
    {
        Spline.Changed -= OnSplineChanged;
    }

    private void OnSplineChanged(Spline spline, int knotIndex, SplineModification modification)
    {
        // FIX LOGIC: Chỉ sinh lại lưới nếu Spline bị thay đổi CHÍNH LÀ Spline của Object này
        if (splineContainer == null || !splineContainer.Splines.Contains(spline)) return;

#if UNITY_EDITOR
        UnityEditor.EditorApplication.delayCall += () =>
        {
            if (this == null) return; 
            GenerateRoadMesh();
        };
#else
        GenerateRoadMesh();
#endif
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        if (splineContainer == null) splineContainer = GetComponent<SplineContainer>();
        if (meshFilter == null) meshFilter = GetComponent<MeshFilter>();
        if (meshCollider == null) meshCollider = GetComponent<MeshCollider>();

        if (splineContainer != null)
        {
            UnityEditor.EditorApplication.delayCall += () =>
            {
                if (this == null) return; 
                GenerateRoadMesh();
            };
        }
    }
#endif

    [ContextMenu("Generate Road & Walls")]
    private void GenerateRoadMesh()
    {
        if (splineContainer == null || splineContainer.Splines.Count == 0) return;
        Spline spline = splineContainer.Spline;
        if (spline == null || spline.Count == 0) return;

        if (generatedMesh == null)
        {
            generatedMesh = new Mesh { name = "Generated Road Mesh" };
            // Pro Tip: Giúp Editor không báo file Scene bị thay đổi (dirty) liên tục khi chưa lưu
            generatedMesh.hideFlags = HideFlags.DontSave; 
            meshFilter.sharedMesh = generatedMesh;
        }

        // TỐI ƯU GC: Dọn dẹp List cũ thay vì tạo mới
        vertices.Clear();
        uvs.Clear();
        roadTriangles.Clear();
        wallTriangles.Clear();

        float splineLength = spline.GetLength();
        int segments = Mathf.CeilToInt(splineLength / distancePerSegment);
        int vertsPerSegment = generateWalls ? 6 : 2;

        for (int i = 0; i <= segments; i++)
        {
            float currentDistance = i * distancePerSegment;
            if (i == segments) currentDistance = splineLength; 
            float t = currentDistance / splineLength;

            spline.Evaluate(t, out float3 position, out float3 tangent, out float3 up);

            // ==========================================
            // FIX LỖI TƯỜNG CHẮN NGANG (Tránh lật Vector ở khớp nối)
            // ==========================================
            if (spline.Closed && i == segments)
            {
                // Nếu là đường khép kín, ép điểm cuối lấy chính xác góc của điểm đầu
                spline.Evaluate(0f, out float3 startPos, out float3 startTangent, out float3 startUp);
                position = startPos;
                tangent = startTangent;
                up = startUp;
            }

            Vector3 localPos = position;
            Vector3 fwd = math.normalize(tangent);
            Vector3 upVec = math.normalize(up);
            Vector3 right = Vector3.Cross(upVec, fwd).normalized;
            
            Vector3 pointLeft = localPos - right * (roadWidth * 0.5f);
            Vector3 pointRight = localPos + right * (roadWidth * 0.5f);

            vertices.Add(pointLeft); 
            vertices.Add(pointRight); 
            
            float roadV = currentDistance / roadTextureTilingY; 
            uvs.Add(new Vector2(0, roadV));
            uvs.Add(new Vector2(roadTextureTilingX, roadV));

            if (generateWalls)
            {
                Vector3 pointLeftTop = pointLeft + upVec * wallHeight;
                Vector3 pointRightTop = pointRight + upVec * wallHeight;

                vertices.Add(pointLeft);
                vertices.Add(pointLeftTop);
                
                vertices.Add(pointRight);
                vertices.Add(pointRightTop);

                float wallV = currentDistance / wallTextureTilingY;
                
                uvs.Add(new Vector2(0, wallV));
                uvs.Add(new Vector2(1, wallV));
                uvs.Add(new Vector2(0, wallV));
                uvs.Add(new Vector2(1, wallV));
            }
        }

        for (int i = 0; i < segments; i++)
        {
            int root = i * vertsPerSegment;
            int next = (i + 1) * vertsPerSegment;

            roadTriangles.Add(root + 0); roadTriangles.Add(next + 0); roadTriangles.Add(root + 1);
            roadTriangles.Add(root + 1); roadTriangles.Add(next + 0); roadTriangles.Add(next + 1);

            if (generateWalls)
            {
                wallTriangles.Add(root + 2); wallTriangles.Add(root + 3); wallTriangles.Add(next + 2);
                wallTriangles.Add(root + 3); wallTriangles.Add(next + 3); wallTriangles.Add(next + 2);

                wallTriangles.Add(root + 4); wallTriangles.Add(next + 4); wallTriangles.Add(root + 5);
                wallTriangles.Add(root + 5); wallTriangles.Add(next + 4); wallTriangles.Add(next + 5);
            }
        }

        generatedMesh.Clear();
        generatedMesh.SetVertices(vertices);
        generatedMesh.SetUVs(0, uvs);
        
        if (generateWalls)
        {
            generatedMesh.subMeshCount = 2;
            generatedMesh.SetTriangles(roadTriangles, 0); 
            generatedMesh.SetTriangles(wallTriangles, 1); 
        }
        else
        {
            generatedMesh.subMeshCount = 1;
            generatedMesh.SetTriangles(roadTriangles, 0);
        }
        
        generatedMesh.RecalculateNormals(); 
        generatedMesh.RecalculateTangents(); 

        if (meshCollider != null)
        {
            meshCollider.sharedMesh = generatedMesh;
        }
    }
}