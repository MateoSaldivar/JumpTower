
using UnityEngine;

public class MapGenerator : MonoBehaviour {
	public GameObject[] Decorations;
	public GameObject worldBackgroundPrefab;
	public GameObject platformPrefab;
	public GameObject endPrefab;
	public Transform gridTransform;
	public static int numberOfBackgrounds = 15;

	private float yOffsetBackgrounds = 5f;
	public float yOffsetPlatforms = 2f;

	private void Start() {
		GenerateMap(gridTransform);
		SpawnPlatforms(gridTransform);
		SpawnDecor(gridTransform);
	}

	private void GenerateMap(Transform parentTransform) {
		Vector3 spawnPosition = new Vector3(0f, yOffsetBackgrounds, 0f);

		for (int i = 0; i < numberOfBackgrounds; i++) {
			Instantiate(worldBackgroundPrefab, spawnPosition, Quaternion.identity, parentTransform);
			spawnPosition.y += yOffsetBackgrounds;
		}

		Instantiate(endPrefab, spawnPosition, Quaternion.identity, parentTransform);
	}

	private void SpawnPlatforms(Transform parentTransform) {
		Vector3 spawnPosition = new Vector3(0f, -5, 0f);

		while (spawnPosition.y < yOffsetBackgrounds * numberOfBackgrounds - 1) {
			float randomX = Random.Range(-5f, 5f);
			spawnPosition = new Vector3(randomX, spawnPosition.y + yOffsetPlatforms + Random.Range(0f, 2f), 0f);
			MovePlatform plat = Instantiate(platformPrefab, spawnPosition, Quaternion.identity, parentTransform).GetComponent<MovePlatform>();
			plat.Spawn();
		}
	}

	private void SpawnDecor(Transform parentTransform) {
		for (int i = 0; i < numberOfBackgrounds; i += 1) {
			float randomX = Random.Range(-6f, 6f);
			Vector3 spawnPosition = new Vector3(randomX, yOffsetBackgrounds * i, 0f);
			Instantiate(Decorations[Random.Range(0, Decorations.Length)], spawnPosition, Quaternion.identity, parentTransform);

		}
	}

}


