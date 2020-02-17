using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.Mathf;

class Drawing : MonoBehaviour
{
    [SerializeField]
    Material _opaque = null;
    [SerializeField]
    Material _transparent = null;

    static Drawing _instance;
    Material _black;
    static Gradient _gradient = new Gradient();

    //Mesh _box;
    static Mesh _unitBox;
    static Mesh _unitFace;
    static Mesh _cylinder;
    static MaterialPropertyBlock _properties;

    public static System.Random Random = new System.Random(42);

    void Awake()
    {
        _instance = this;

        _unitBox = UnitBox();
        _unitFace = Quad();
        _cylinder = Primitive(PrimitiveType.Cylinder);

        var texture = GradientTexture();
        _opaque.mainTexture = texture;
        _transparent.mainTexture = texture;

        _black = new Material(Shader.Find("Unlit/Color"));
        _black.color = Color.black;
        _properties = new MaterialPropertyBlock();
        _properties.SetTexture("_MainTex", new Texture2D(1, 1));
    }

    static Texture2D GradientTexture()
    {
        var gck = new GradientColorKey[2];
        var gak = new GradientAlphaKey[0];
        gck[0].color = new Color(0.2f, 0.8f, 0.2f);
        gck[0].time = 0.0f;
        gck[1].color = new Color(0.8f, 0.2f, 0.2f);
        gck[1].time = 1.0f;
        _gradient.SetKeys(gck, gak);

        var texture = new Texture2D(256, 2)
        {
            wrapMode = TextureWrapMode.Clamp,
        };

        for (int i = 0; i < 256; i++)
        {
            float t = i / 255f;
            var color = _gradient.Evaluate(t);
            texture.SetPixel(i, 0, color);
        }

        texture.SetPixel(0, 1, Color.yellow);
        texture.SetPixel(1, 1, Color.gray);
        texture.Apply();

        return texture;
    }

    static Mesh UnitBox()
    {
        var cube = Primitive(PrimitiveType.Cube);

        var box = new Mesh
        {
            vertices = cube.vertices,
            normals = cube.normals,
            uv = cube.uv.Select(_ => new Vector2(1.5f / 255f, 0.75f)).ToArray(),
            triangles = cube.triangles,
            tangents = cube.tangents
        };

        return box;
    }

    static Mesh Primitive(PrimitiveType primitive)
    {
        var go = GameObject.CreatePrimitive(primitive);
        var mesh = go.GetComponent<MeshFilter>().sharedMesh;
        Destroy(go);
        return mesh;
    }

    // drawing

    public static void DrawCube(Vector3 center, float size, float t = 0)
    {
        var color = _gradient.Evaluate(t);
        _properties.SetColor("_Color", color);

        var matrix = Matrix4x4.TRS(
                center,
                Quaternion.identity,
                Vector3.one * (size * 0.96f)
                );

        Graphics.DrawMesh(_unitBox, matrix, _instance._opaque, 0, null, 0, _properties);
    }

    public static void DrawFace(Vector3 center, Axis direction, float size)
    {
        Quaternion rotation = Quaternion.identity;

        switch (direction)
        {
            case Axis.X:
                rotation = Quaternion.Euler(0, 90, 0);
                break;
            case Axis.Y:
                rotation = Quaternion.Euler(90, 0, 0);
                break;
            case Axis.Z:
                rotation = Quaternion.Euler(0, 0, 0);
                break;
            default:
                break;
        }

        var matrix = Matrix4x4.TRS(
                center,
                rotation,
                Vector3.one * size
                );

        _instance._opaque.mainTexture = null;
        Graphics.DrawMesh(_unitFace, matrix, _instance._opaque, 0, null, 0);
        Graphics.DrawMesh(_unitFace, matrix, _instance._black, 0, null, 1);
    }

    public static void DrawBar(Vector3 start, Vector3 end, float radius, float t)
    {
        var color = _gradient.Evaluate(t);
        _properties.SetColor("_Color", color);

        var vector = end - start;

        var matrix = Matrix4x4.TRS(
                        start + vector * 0.5f,
                        Quaternion.LookRotation(vector) * Quaternion.Euler(90, 0, 0),
                        new Vector3(radius, vector.magnitude * 0.5f, radius)
                        );

        Graphics.DrawMesh(_cylinder, matrix, _instance._opaque, 0, null, 0, _properties);
    }

    public static void DrawMesh(bool isTransparent, params Mesh[] mesh)
    {
        var material = isTransparent ? _instance._transparent : _instance._opaque;

        foreach (var m in mesh)
        {
            Graphics.DrawMesh(m, Matrix4x4.identity, material, 0);

            if (m.subMeshCount > 1)
                Graphics.DrawMesh(m, Matrix4x4.identity, _instance._black, 0, null, 1);
        }
    }

    public static Mesh MakeTwistedBox(Vector3[] corners, float t, Mesh mesh = null)
    {
        Vector3 center = Vector3.zero;
        foreach (var corner in corners)
            center += corner;

        center /= corners.Length;

        corners = corners
            .Select(c => c + (center - c).normalized * 0.05f)
            .ToArray();

        var f = new[]
        {
            0,1,3,2,
            4,6,7,5,
            6,4,0,2,
            4,5,1,0,
            5,7,3,1,
            7,6,2,3
        };

        var v = f.Select(i => corners[i]).ToArray();

        if (mesh == null)
        {
            mesh = new Mesh()
            {
                vertices = v,
                uv = Enumerable.Repeat(new Vector2(t, 0), v.Length).ToArray()
            };

            var faces = Enumerable.Range(0, 24).ToArray();
            mesh.SetIndices(faces, MeshTopology.Quads, 0);
        }
        else
        {
            mesh.vertices = v;
        }

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        return mesh;
    }

    public static Mesh MakeFace(Vector3 center, Axis direction, float size, float u, float v = 0)
    {
        Quaternion rotation = Quaternion.identity;

        switch (direction)
        {
            case Axis.X:
                rotation = Quaternion.Euler(0, 90, 0);
                break;
            case Axis.Y:
                rotation = Quaternion.Euler(90, 0, 0);
                break;
            case Axis.Z:
                rotation = Quaternion.Euler(0, 0, 0);
                break;
        }

        var tris = new[]
        {
            0,1,2, 0,2,3,
            6,5,4, 7,6,4
        };

        float s = size * 0.5f;

        var vertices = new[]
        {
            new Vector3(-s,-s,0),
            new Vector3(s,-s,0),
            new Vector3(s,s,0),
            new Vector3(-s,s,0),
            new Vector3(-s,-s,0),
            new Vector3(s,-s,0),
            new Vector3(s,s,0),
            new Vector3(-s,s,0)
        };

        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] = rotation * vertices[i];
            vertices[i] += center;
        }

        var n = -Vector3.forward;
        n = rotation * n;

        var mesh = new Mesh()
        {
            vertices = vertices,
            normals = new[] { n, n, n, n, -n, -n, -n, -n },
            uv = Enumerable.Repeat(new Vector2(u, v), vertices.Length).ToArray(),
            indexFormat = UnityEngine.Rendering.IndexFormat.UInt32,
        };

        mesh.SetIndices(tris, MeshTopology.Triangles, 0);
        mesh.RecalculateBounds();
        // mesh.RecalculateTangents();
        return mesh;
    }


    // primitives

    static Mesh Quad()
    {
        const float s = 0.5f;

        var v = new[]
        {
            new Vector3(-s,-s),
            new Vector3(s,-s),
            new Vector3(s,s),
            new Vector3(-s,s)
        };

        var quad = new Mesh()
        {
            vertices = v,
            normals = Enumerable.Repeat(-Vector3.up, 4).ToArray(),
            subMeshCount = 2
        };

        quad.SetIndices(new[] { 0, 1, 2, 3, 0, 3, 2, 1 }, MeshTopology.Quads, 0);
        quad.SetIndices(new[] { 0, 1, 2, 3, 0 }, MeshTopology.LineStrip, 1);
        return quad;
    }

    static Mesh Circle()
    {
        const int sides = 32;
        const float step = (PI * 2) / sides;
        var vertices = new Vector3[sides + 1];

        for (int i = 0; i < sides; i++)
        {
            float angle = step * i;
            float x = Cos(angle);
            float y = Sin(angle);
            vertices[i + 1] = new Vector3(x, y, 0) * 0.5f;
        }

        var triangles = new int[sides * 3];

        for (int i = 0; i < sides; i++)
        {
            int j = i == sides - 1 ? 0 : i + 1;
            triangles[i * 3 + 1] = i + 1;
            triangles[i * 3 + 2] = j + 1;
        }

        var circle = new Mesh()
        {
            vertices = vertices,
            triangles = triangles,
            normals = Enumerable.Repeat(-Vector3.up, sides + 1).ToArray(),
            subMeshCount = 2
        };

        var perimeter = Enumerable.Range(1, sides + 1).ToArray();
        perimeter[sides] = 1;

        circle.SetIndices(perimeter, MeshTopology.LineStrip, 1);
        return circle;
    }
}