using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public sealed class GameManager : MonoBehaviour
{
    public static GameManager _instance;
    [SerializeField] private int[] _numberEnemyToSpawn;
    [SerializeField] private GameObject _player;
    [SerializeField] private int _scoreIncrement = 100;
    [SerializeField] private TextMeshProUGUI _scoreLabel;
    [SerializeField] private GameObject _defeatScreen;
    [SerializeField] private GameObject _victoryScreen;

    private int _enemyToSpawnIndex;
    private int _enemyLocked;
    private int _score;

    private void Awake()
    {
        if (_instance != null)
        {
            Debug.LogError("More than one GameManager instance !");
            return;
        }
        _instance = this;
    }

    public void IncrementScore()
    {
        _score += _scoreIncrement;
        _scoreLabel.text = "Score : "+_score.ToString();
    }

    public void IncrementCatLocked()
    {
        _enemyLocked++;

        if(_enemyLocked >= _numberEnemyToSpawn[_enemyToSpawnIndex]){
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

        if (_enemyToSpawnIndex >= _numberEnemyToSpawn.Length)
        {
            Victory();
            return;
        }

        EnemySpawner._instance.SpawnEnemy(_numberEnemyToSpawn[_enemyToSpawnIndex]);
    }

    private void Victory()
    {
        SoundManager._instance.StopAudio(AudioName.SoundTrack);
        _victoryScreen.SetActive(true);
        SoundManager._instance.PlayAudio(AudioName.Victory);
        Debug.Log("Victory");
        _player.SetActive(false);
    }

    public void Defeat()
    {
        SoundManager._instance.StopAudio(AudioName.SoundTrack);
        _defeatScreen.SetActive(true);
        SoundManager._instance.PlayAudio(AudioName.Failure);
        Debug.Log("Defeat");
        _player.SetActive(false);
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
