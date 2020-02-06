using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;


public class PlatformerController : MonoBehaviour
{
    private GameController gc;
    public List<GameObject> allowedLevelParts;
    private List<GameObject> spawnedLevelParts;
    public int levelPartsAtOneTime;
    public Transform world;

    private bool worldPopulated;

    public void GenerateWorld()
    {
        if(gc == null)
            gc = FindObjectOfType<GameController>();

        if (spawnedLevelParts != null)
            spawnedLevelParts.ForEach(x => Destroy(x));

        spawnedLevelParts = new List<GameObject>();

        GenerateLevel();
        RespawnPlayer(spawnedLevelParts[0].GetComponent<LevelPart>().spawnPoint.position);
    }

    void Update()
    {
        if (!worldPopulated)
            return;
        gc.PlayerUnit.MoveForward();
    }

    public void SpawnPlayer(GameObject playerObject)
    {
        if (gc == null)
            gc = FindObjectOfType<GameController>();

        GameObject go = Instantiate(playerObject, new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, 9 + gameObject.transform.position.y / 10.00f), Quaternion.identity, world);
        gc.playerObject = go;
        gc.camera.Follow = gc.playerObject.transform;
        gc.PlayerUnit = gc.playerObject.GetComponent<PlayerUnit>();
    }

    private void RespawnPlayer(Vector3 respawnAt)
    {
        gc.playerObject.transform.position = new Vector3(respawnAt.x, respawnAt.y, 9 + respawnAt.y / 10.00f);
        gc.playerObject.SetActive(true);
    }

    private void GenerateLevel()
    {
        Vector3 prevPos = new Vector3(0, 0, 10f);
        for (int i = 0; i < levelPartsAtOneTime; i++)
        {
            GameObject go = allowedLevelParts[Random.Range(0, allowedLevelParts.Count)];
            LevelPart lp = go.GetComponent<LevelPart>();
            spawnedLevelParts.Add(Instantiate(go, new Vector3(prevPos.x + lp.Length, prevPos.y, prevPos.z), Quaternion.identity, world));
            prevPos = spawnedLevelParts[spawnedLevelParts.Count - 1].transform.position;
        }
        worldPopulated = true;
    }

    public void RestartGame()
    {
        if (spawnedLevelParts == null)
            return;

        Vector3 prevPos = new Vector3(0, 0, 10f);
        for (int i = 0; i < spawnedLevelParts.Count; i++)
        {
            LevelPart lp = spawnedLevelParts[i].GetComponent<LevelPart>();
            spawnedLevelParts[i].transform.position = new Vector3(prevPos.x + lp.Length, prevPos.y, prevPos.z);
            lp.Regenerate(restart: true);
            prevPos = spawnedLevelParts[i].transform.position;
        }
        RespawnPlayer(spawnedLevelParts[0].GetComponent<LevelPart>().spawnPoint.position);
    }
}