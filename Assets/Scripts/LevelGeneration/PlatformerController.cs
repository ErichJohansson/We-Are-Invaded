using System.Collections.Generic;
using UnityEngine;


public class PlatformerController : MonoBehaviour
{
    public List<GameObject> allowedLevelParts;
    public int levelPartsAtOneTime;
    public Transform world;

    private GameController gc;

    private List<GameObject> spawnedLevelParts;

    public void GenerateWorld()
    {
        if (gc == null)
            gc = FindObjectOfType<GameController>();

        if (spawnedLevelParts != null)
            spawnedLevelParts.ForEach(x => Destroy(x));

        spawnedLevelParts = new List<GameObject>();

        GenerateLevel();
        RespawnPlayer(spawnedLevelParts[0].GetComponent<LevelPart>().spawnPoint.position);
    }

    public void SpawnPlayer(GameObject playerObject, Vehicle vehicle)
    {
        if (gc == null)
            gc = FindObjectOfType<GameController>();

        if (playerObject == null)
            return;

        if (gc.playerObject != null)
            Destroy(gc.playerObject);

        GameObject go = Instantiate(playerObject, new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, 9 + gameObject.transform.position.y / 10.00f), Quaternion.identity, world);
        gc.playerObject = go;
        gc.camera.Follow = gc.playerObject.transform;
        PlayerUnit pu = gc.playerObject.GetComponent<PlayerUnit>();
        pu.ApplyStats(vehicle);
        gc.PlayerUnit = pu;
    }

    private void RespawnPlayer(Vector3 respawnAt)
    {
        if (gc.playerObject == null)
            return;
        PlayerUnit unit = gc.playerObject.GetComponent<PlayerUnit>();
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
            lp.ID = i;
            spawnedLevelParts.Add(Instantiate(go, new Vector3(prevPos.x + lp.Length, prevPos.y, prevPos.z), Quaternion.identity, world));
            prevPos = spawnedLevelParts[spawnedLevelParts.Count - 1].transform.position;
        }
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

    public GameObject GetSpawnedPart(int id)
    {
        if (id < 0 || id >= spawnedLevelParts.Count)
            return null;
        return spawnedLevelParts[id];
    }
}