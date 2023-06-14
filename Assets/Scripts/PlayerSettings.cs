using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSettings : MonoBehaviour
{
    public Color benchColor;
    public MeshRenderer benchMeshRenderer;
    public Banner banner;
    public Material ballMaterial;

    public static PlayerSettings Instance { get; private set; }

    private void Start()
    {
        Instance = this;
        //Mesh targetMesh = null;
        //foreach (SkinnedMeshRenderer meshRenderer in go.GetComponentsInChildren<SkinnedMeshRenderer>())
        //{
        //    if (meshRenderer)
        //    {
        //        Debug.Log("meshFilter : " + meshRenderer.name);
        //        Mesh mesh = meshRenderer.sharedMesh;
        //        if(mesh.name == "LP001")
        //        targetMesh = mesh;
        //    }
        //}
        //targetMeshFilter.mesh = targetMesh;
    }

    public void SetBenchColor(Color color)
    {
        benchMeshRenderer.material.color = color;
    }

    public void SetPlayerName(string player1Name, string player2Name)
    {
        Debug.Log("name : " + player1Name + " " + player2Name);
        ((FootballController)GameMatchController.Instance).scoreController.ChangePlayer1Name(player1Name);
        ((FootballController)GameMatchController.Instance).scoreController.ChangePlayer2Name(player2Name);
    }

    public void SetPlayerNameSprite(Texture2D texture)
    {
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f, 100, 1, SpriteMeshType.FullRect, new Vector4(60, 60, 60, 60));
        ((FootballController)GameMatchController.Instance).scoreController.ChangePlayer1NameBackground(sprite);
        ((FootballController)GameMatchController.Instance).scoreController.ChangePlayer2NameBackground(sprite);
    }

    public void SetScoreContainerSprite(Texture2D texture)
    {
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f, 100, 1, SpriteMeshType.FullRect, new Vector4(50, 50, 50, 50));
        ((FootballController)GameMatchController.Instance).scoreController.ChangeScoreBackground(sprite);
    }

    public void SetScoreOnSprite(Texture2D texture)
    {
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f);
        ((FootballController)GameMatchController.Instance).scoreController.scoreSprite = sprite;
    }

    public void SetScoreOffSprite(Texture2D texture)
    {
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f);
        ((FootballController)GameMatchController.Instance).scoreController.missSprite = sprite;
    }

    public void SetBallTexture(Texture2D texture)
    {
        ballMaterial.mainTexture = texture;
    }

    public void SetBannerTexture(Texture2D texture, int width, int height)
    {
        Debug.Log("set banner");
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, width, height), Vector2.one * 0.5f);
        banner.RefreshLayout(sprite);
    }

    public void SetField1Texture(Texture2D diffuseTexture)
    {
        TerrainLayer[] terrainLayers = Terrain.activeTerrain.terrainData.terrainLayers;
        terrainLayers[0].diffuseTexture = diffuseTexture;
    }

    public void SetField2Texture(Texture2D diffuseTexture)
    {
        TerrainLayer[] terrainLayers = Terrain.activeTerrain.terrainData.terrainLayers;
        terrainLayers[1].diffuseTexture = diffuseTexture;
    }

    public void SetFieldTexture(int index, Texture2D diffuseTexture)
    {
        TerrainLayer[] terrainLayers = Terrain.activeTerrain.terrainData.terrainLayers;
        terrainLayers[index].diffuseTexture = diffuseTexture;
    }
}
