using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FIMSpace.FSpine;
using UnityEngine.Rendering;

public class PlayerFootsteps : MonoBehaviour
{
    [Header("FMOD")]
    [SerializeField] public FMODUnity.EventReference footstepEvent;
    private FMOD.Studio.EventInstance Player_Footsteps;


    [SerializeField]
    private CURRENT_MATERIAL currentMaterial;

    private enum CURRENT_MATERIAL { Wood, Concrete, Grass, Gravel, Snow };

    private Vector3 rayCastOffSet = new Vector3(0f, 0.1f, 0f);



    //Raycast variables
    public float raycastDistance = 0.03f;
    public Transform raycastOrigin;
    public Vector3 raycastOffset;
    Vector3 originPosition;
    Vector3 raycastDirection;
    Vector3 rotatedOffset;
    Vector3 raycastRotation;
    Vector3 rotationOffset;
    Quaternion rotation;
    private bool hasHitObject = false;
    public LayerMask layerToIgnore;
    public float hitBufferTime = 0.1f;
    private float lastHitTime = 0f;


    private void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        RaycastFootHitDetection();
    }



    //These two arrays are matching according to their index number
    private string[] textureKeywords = new string[]
    {
        "Unknown",   // Index 0 for textures not matching any keyword
        "Wood",
        "Floor",
        "Floor tileable",
        "Grass",
        "Gravel",
        "Ground",
        "Snow"
    };

    private CURRENT_MATERIAL[] textureMaterialMappings = new CURRENT_MATERIAL[]
    {
        CURRENT_MATERIAL.Concrete, // Index 0 for textures not matching any keyword
        CURRENT_MATERIAL.Wood,
        CURRENT_MATERIAL.Concrete,
        CURRENT_MATERIAL.Concrete,
        CURRENT_MATERIAL.Grass,
        CURRENT_MATERIAL.Gravel,
        CURRENT_MATERIAL.Gravel,
        CURRENT_MATERIAL.Snow
    };


    RaycastHit hit;
    bool isRaycastHit;

    private void RaycastFootHitDetection()
    {
        originPosition = raycastOrigin.position + raycastOrigin.TransformDirection(raycastOffset);
        raycastDirection = raycastOrigin.TransformDirection(Vector3.forward);


        isRaycastHit = Physics.Raycast(originPosition, raycastDirection, out hit, raycastDistance, ~layerToIgnore);

        

        if (isRaycastHit && !hasHitObject && Time.time - lastHitTime >= hitBufferTime)
        {
            DetermineMaterial();
            SelectAndPlayFootstep();
            hasHitObject = true;
            lastHitTime = Time.time;
        }
        else if (!isRaycastHit)
        {
            hasHitObject = false;
        }

        Debug.DrawRay(originPosition, raycastDirection * raycastDistance, Color.red);

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(originPosition, raycastDirection * raycastDistance);
    }



    private void DetermineMaterial()
    {

        //bool isRaycastHit = Physics.Raycast(originPosition, raycastDirection, out hit, raycastDistance, ~layerToIgnore);
        //Debug.DrawRay(originPosition, raycastDirection, Color.red);

        if (hit.collider is TerrainCollider terrainCollider)
        {
            DetermineTerrainMaterial(terrainCollider);            
        }
        else
        {
            Renderer renderer = FindRendererInChildren(hit.collider.transform);
            if (renderer != null)
            {
                DetermineMeshMaterial(renderer);
            }
            else
            {
                Debug.Log("Renderer component not found in childred of the hit object.");
            }
        }
    }

    private void DetermineTerrainMaterial(TerrainCollider terrainCollider)
    {
        // Get the UV coordinates of the hit point
        Vector3 hitPoint = hit.point;
        Vector3 terrainPosition = terrainCollider.transform.position;
        Vector3 terrainLocalPos = hitPoint - terrainPosition;

        Vector3 splatMapPosition = new Vector3(
            terrainLocalPos.x / terrainCollider.terrainData.size.x,
            0,
            terrainLocalPos.z / terrainCollider.terrainData.size.z
            );

        int x = Mathf.FloorToInt(splatMapPosition.x * terrainCollider.terrainData.alphamapWidth);
        int z = Mathf.FloorToInt(splatMapPosition.x * terrainCollider.terrainData.alphamapHeight);

        float[,,] alphaMap = terrainCollider.terrainData.GetAlphamaps(x, z, 1, 1);

        int primaryIndex = 0;
        for (int i = 1; i < alphaMap.Length; i++)
        {
            if (alphaMap[0, 0, i] > alphaMap[0, 0, primaryIndex])
            {
                primaryIndex = i;
            }
        }
        Texture texture = terrainCollider.terrainData.terrainLayers[primaryIndex].diffuseTexture;

        currentMaterial = DetermineTextureMaterial(texture);
    }



    private void DetermineMeshMaterial(Renderer renderer)
    {
        Material material = renderer.material;

        if (material != null)
        {
            Texture texture = material.mainTexture;
            if (texture != null)
            {
                Debug.Log("Texture name: " + texture.name);
                currentMaterial = DetermineTextureMaterial(texture);
            }
            else
            {
                Debug.Log("Material does not have a mainTexture.");
            }

        }
        else
        {
            Debug.Log("Renderer does not have a material.");
        }
    }

    private CURRENT_MATERIAL DetermineTextureMaterial(Texture texture)
    {
        string textureName = texture.name;
        for (int i = 1; i < textureKeywords.Length; i++)
        {
            if (textureName.Contains(textureKeywords[i]))
            {
                return textureMaterialMappings[i];
            }
        }
        return CURRENT_MATERIAL.Concrete;
    }


    private Renderer FindRendererInChildren(Transform parent)
    {
        Renderer renderer = parent.GetComponent<Renderer>();
        if (renderer != null)
            return renderer;

        for (int i = 0; i < parent.childCount; i++)
        {
            renderer = FindRendererInChildren(parent.GetChild(i));
            if (renderer != null)
                return renderer;
        }

        return null;
    }


    // Method to switch materials in FMOD
    public void SelectAndPlayFootstep()
    {
        switch (currentMaterial)

        {
            case CURRENT_MATERIAL.Wood:
                PlayFootstep(0);
                break;


            case CURRENT_MATERIAL.Concrete:
                PlayFootstep(1);
                break;

            case CURRENT_MATERIAL.Grass:
                PlayFootstep(2);
                break;

            case CURRENT_MATERIAL.Gravel:
                PlayFootstep(3);
                break;

            case CURRENT_MATERIAL.Snow:
                PlayFootstep(4);
                break;

            default:
                PlayFootstep(0);
                break;
        }
    }

    // Play FMOD footsteps
    public void PlayFootstep(int material)
    {
        Player_Footsteps = FMODUnity.RuntimeManager.CreateInstance(footstepEvent);
        Player_Footsteps.setParameterByName("Material", material);
        Player_Footsteps.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));
        Player_Footsteps.start();
        Player_Footsteps.release();
    }

    public void StopFootstep()
    {
        Player_Footsteps.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
    }
}








/*
rotationOffset = rotation * raycastOffset;
originPosition = raycastOrigin.position + rotatedOffset;
raycastDirection = raycastOrigin.TransformDirection(Vector3.forward);
rotation = Quaternion.Euler(raycastRotation);








// Raycase below to see what material layer it is
private void DetermineMaterial()
{

    RaycastHit hit;
    bool isRaycastHit = Physics.Raycast(originPosition, raycastDirection, out hit, raycastDistance, ~layerToIgnore);
    Debug.DrawRay(originPosition, raycastDirection, Color.red);


    if (isRaycastHit && hit.collider != null)
    {
        Renderer renderer = FindRendererInChildren(hit.collider.transform);
        Debug.Log("Raycast hit object: " + hit.collider.gameObject.name);


        if (hit.collider is TerrainCollider terrainCollider)
        {
            TerrainData terrainData = terrainCollider.terrainData;

            // Get the UV coordinates of the hit point
            Vector3 hitPoint = hit.point;
            Vector3 terrainPosition = terrainCollider.transform.position;
            Vector3 terrainLocalPos = hitPoint - terrainPosition;
            Vector3 normalisedPos = new Vector3(
                terrainLocalPos.x / terrainData.size.x,
                terrainLocalPos.y / terrainData.size.y,
                terrainLocalPos.z / terrainData.size.z
                );

            Texture2D texture = GetTextureIndexAtUVCoordinates(terrainData, normalisedPos);
            if (texture != null)
            {
                Debug.Log("Hit Texture Name: " + texture.name);
            }
            else
            {
                Debug.Log("No Texture found at the hit point's UV coordinates.");
            }

        }

        if (renderer != null)
        {
            Debug.Log("Renderer found in children hierarchy: " + renderer.gameObject.name);
            Material material = renderer.material;
            if (material != null)
            {
                Texture texture = material.mainTexture;
                if (texture != null)
                {
                    Debug.Log("Texture name: " + texture.name);


                    if (texture.name.Contains("Wood"))
                    {
                        currentMaterial = CURRENT_MATERIAL.Wood;
                    }

                    else if (texture.name.Contains("Floor") || texture.name.Contains("Floor tileable"))
                    {
                        currentMaterial = CURRENT_MATERIAL.Concrete;
                    }

                    else if (texture.name.Contains("Grass"))
                    {
                        currentMaterial = CURRENT_MATERIAL.Grass;
                    }

                    else if (texture.name.Contains("Gravel"))
                    {
                        currentMaterial = CURRENT_MATERIAL.Gravel;
                    }
                }
                else
                {
                    Debug.Log("Material does not have a mainTexture.");
                }

            }
            else
            {
                Debug.Log("Renderer does not have a material.");
            }
        }
        else
        {
            Debug.Log("Renderer component not found in children of the hit object.");
        }

    }
}


    private void DetermineTerrainMaterial(TerrainCollider terrainCollider, RaycastHit hit)
    {

        if (cachedTerrainData == null)
        {
            cachedTerrainData = terrainCollider.terrainData;
        }



// Get the UV coordinates of the hit point
Vector3 hitPoint = hit.point;
Vector3 terrainPosition = terrainCollider.transform.position;
Vector3 terrainLocalPos = hitPoint - terrainPosition;
Vector3 normalisedPos = new Vector3(
    terrainLocalPos.x / cachedTerrainData.size.x,
    terrainLocalPos.y / cachedTerrainData.size.y,
    terrainLocalPos.z / cachedTerrainData.size.z
    );

Texture2D texture = GetTextureIndexAtUVCoordinates(cachedTerrainData, normalisedPos);
if (texture != null)
{
    Debug.Log("Hit Texture Name: " + texture.name);

    currentMaterial = DetermineTextureMaterial(texture);
}
else
{
    Debug.Log("No Texture found at the hit point's UV coordinates.");
}
    }



    private Texture2D GetTextureIndexAtUVCoordinates(TerrainData terrainData, Vector3 uvCoordinates)
    {
        TerrainLayer[] terrainLayers = terrainData.terrainLayers;
        int splatMapWidth = terrainData.alphamapWidth;
        int splatMapHeight = terrainData.alphamapHeight;
        int numLayers = terrainData.terrainLayers.Length;
        float[,,] splatmapData = terrainData.GetAlphamaps(0, 0, splatMapWidth, splatMapHeight);

        // Convert UV Coordinates to splatmap coordinates
        int mapX = Mathf.FloorToInt(uvCoordinates.x * splatMapWidth);
        int mapZ = Mathf.FloorToInt(uvCoordinates.z * splatMapHeight);

        //Find the most dominate texture (highest value) at the specified UV coordinates
        float maxTextureStrength = -1f;
        int dominantTextureIndex = 1;
        for (int textureIndex = 0; textureIndex < numLayers; textureIndex++)
        {
            float textureStrength = splatmapData[mapZ, mapX, textureIndex];
            if (textureStrength > maxTextureStrength)
            {
                maxTextureStrength = textureStrength;
                dominantTextureIndex = textureIndex;
            }

        }

        if (dominantTextureIndex >= 0 && dominantTextureIndex < numLayers)
        {
            return terrainLayers[dominantTextureIndex].diffuseTexture;
        }

        return null;
    }


    private void CacheTerrainTextures()
    {
        Terrain[] terrains = FindObjectsOfType<Terrain>();

        foreach (Terrain terrain in terrains)
        {
            cachedTerrainData = terrain.terrainData;

            // Get the UV coordinates of the terrain center
            Vector3 center = terrain.transform.position + terrain.terrainData.size * 0.5f;
            Vector3 normalizedCenter = new Vector3(
                center.x / cachedTerrainData.size.x,
                center.y / cachedTerrainData.size.y,
                center.z / cachedTerrainData.size.z
            );

            Texture2D dominantTexture = GetTextureIndexAtUVCoordinates(cachedTerrainData, normalizedCenter);
            if (dominantTexture != null)
            {
                cachedTerrainTextures[terrain] = dominantTexture;

            }

        }

    }

*/