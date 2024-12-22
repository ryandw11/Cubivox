using CubivoxClient;
using CubivoxClient.Texturing;
using CubivoxCore;
using CubivoxCore.Voxels;
using CubivoxRender;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CurrentVoxel : MonoBehaviour
{

    public GameObject voxelObject;
    public TextMeshProUGUI voxelText;

    private static CurrentVoxel instance;

    private MeshFilter meshFilter;
    private ClientItemRegistry clientItemRegistry;

    private short currentVoxel = 1;

    private void Awake()
    {
        if( instance != null )
        {
            Debug.Assert(false, "Current Voxel UI script already has an instance!");
        }
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        meshFilter = voxelObject.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = voxelObject.AddComponent<MeshRenderer>();
        meshRenderer.sharedMaterial = ((ClientTextureAtlas)Cubivox.GetTextureAtlas()).GetMaterial();

        clientItemRegistry = ClientCubivox.GetClientInstance().GetClientItemRegistry();

        // The first item
        var item = clientItemRegistry.GetVoxelMap()[currentVoxel];

        UpdateVoxel(item);
    }

    // Update is called once per frame
    void Update()
    {

        if(ClientCubivox.GetClientInstance().CurrentState != GameState.PLAYING )
        {
            if( voxelObject.activeInHierarchy )
            {
                voxelObject.SetActive(false);
                voxelText.gameObject.SetActive(false);
            }
            return;
        }
        else
        {
            if (!voxelObject.activeInHierarchy)
            {
                voxelObject.SetActive(true);
                voxelText.gameObject.SetActive(true);
            }
        }

        if( Input.GetKeyDown(KeyCode.G) )
        {
            currentVoxel++;

            VoxelDef voxelDef;
            if( clientItemRegistry.GetVoxelMap().TryGetValue(currentVoxel, out voxelDef) )
            {
                UpdateVoxel(voxelDef);
            }
            else
            {
                currentVoxel = 1;
                UpdateVoxel(clientItemRegistry.GetVoxelMap()[currentVoxel]);
            }
        }
    }

    private void OnDestroy()
    {
        instance = null;
    }

    public VoxelDef GetCurrentVoxel()
    {
        return clientItemRegistry.GetVoxelMap()[currentVoxel];
    }

    private void UpdateVoxel(VoxelDef voxel)
    {
        var textureAtlasRows = Cubivox.GetTextureAtlas().GetNumberOfRows();
        var atlasTexture = voxel.GetAtlasTexture();

        List<Vector2> textureCords = new List<Vector2>();
        foreach( var baseTextureCords in VoxelData.TextureCoordinates )
        {
            textureCords.Add(new Vector2(baseTextureCords.x / textureAtlasRows + atlasTexture.XOffset, baseTextureCords.y / textureAtlasRows + atlasTexture.YOffset));
        }
        var mesh = new Mesh
        {
            vertices = VoxelData.Vertices.ToArray(),
            triangles = VoxelData.Indices.ToArray(),
            uv = textureCords.ToArray()
        };

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        mesh.RecalculateTangents();

        meshFilter.mesh = mesh;

        voxelText.text = voxel.GetName();
    }

    public static CurrentVoxel GetInstance()
    {
        return instance;
    }
}
