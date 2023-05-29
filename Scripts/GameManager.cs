using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.Serialization;

public sealed class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [SerializeField] private int[] numberEnemyToSpawn;
    [SerializeField] private GameObject player;
    [SerializeField] private int scoreIncrement = 100;
    [SerializeField] private TextMeshProUGUI scoreLabel;
    [SerializeField] private GameObject defeatScreen;
    [SerializeField] private GameObject victoryScreen;

    private int _enemyToSpawnIndex;
    private int _enemyLocked;
    private int _score;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("More than one GameManager instance !");
            return;
        }
        Instance = this;
    }

    public void IncrementScore()
    {
        _score += scoreIncrement;
        scoreLabel.text = "Score : "+_score.ToString();
    }

    public void IncrementCatLocked()
    {
        _enemyLocked++;

        if(_enemyLocked >= numberEnemyToSpawn[_enemyToSpawnIndex]){
            NextPhase();
            _enemyLocked = 0;
        }
    }

    public void DecrementCatLocked()
    {
        _enemyLocked--;
    }

    private void NextPhase()
    {
        GameObject[] catsLocked = GameObject.FindGameObjectsWithTag("Cat");
        foreach (GameObject c in catsLocked)
        {
            if (c.GetComponent<EnemyMovement>() != null)
                c.GetComponent<EnemyMovement>().TurnIntoCheese();
        }

        _enemyToSpawnIndex++;

        if (_enemyToSpawnIndex >= numberEnemyToSpawn.Length)
        {
            Victory();
            return;
        }

        EnemySpawner.Instance.SpawnEnemy(numberEnemyToSpawn[_enemyToSpawnIndex]);
    }

    private void Victory()
    {
        SoundManager.Instance.StopAudio(AudioName.SoundTrack);
        victoryScreen.SetActive(true);
        SoundManager.Instance.PlayAudio(AudioName.Victory);
        player.SetActive(false);
    }

    public void Defeat()
    {
        SoundManager.Instance.StopAudio(AudioName.SoundTrack);
        defeatScreen.SetActive(true);
        SoundManager.Instance.PlayAudio(AudioName.Failure);
        player.SetActive(false);
    }

    public void Restart()
    {
        UnPause();
        SceneManager.LoadScene(1);
    }

    public void MainMenu()
    {
        UnPause();
        SceneManager.LoadScene(0);
    }

    public void Pause()
    {
        Time.timeScale = 0f;
    }
    public void UnPause()
    {
        Time.timeScale = 1f;
    }
}
