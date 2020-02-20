using System.Collections.Generic;
using UnityEngine;


public class PlatformerController : MonoBehaviour
{
    public List<GameObject> allowedLevelParts;
    public int levelPartsAtOneTime;
    public Transform world;

    private List<GameObject> spawnedLevelParts;

    public static PlatformerController Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public void GenerateWorld()
    {
        if (spawnedLevelParts != null)
            spawnedLevelParts.ForEach(x => Destroy(x));

        spawnedLevelParts = new List<GameObject>();

        GenerateLevel();
        RespawnPlayer(spawnedLevelParts[0].GetComponent<LevelPart>().spawnPoint.position);
    }

    public void SpawnPlayer(GameObject playerObject, Vehicle vehicle)
    {
        if (playerObject == null)
            return;

        if (GameController.Instance.playerObject != null)
            Destroy(GameController.Instance.playerObject);

        GameObject go = Instantiate(playerObject, new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, 9 + gameObject.transform.position.y / 10.00f), Quaternion.identity, world);
        GameController.Instance.playerObject = go;
        GameController.Instance.camera.Follow = GameController.Instance.playerObject.transform;
        PlayerUnit pu = GameController.Instance.playerObject.GetComponent<PlayerUnit>();
        pu.ApplyStats(vehicle);
        GameController.Instance.PlayerUnit = pu;
    }

    private void RespawnPlayer(Vector3 respawnAt)
    {
        if (GameController.Instance.playerObject == null)
            return;
        PlayerUnit unit = GameController.Instance.playerObject.GetComponent<PlayerUnit>();
        GameController.Instance.playerObject.transform.position = new Vector3(respawnAt.x, respawnAt.y, 9 + respawnAt.y / 10.00f);
        GameController.Instance.playerObject.SetActive(true);
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