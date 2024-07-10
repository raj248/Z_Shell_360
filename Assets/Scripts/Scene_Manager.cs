using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Scene_Manager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void LoadSurvivalGame()
    {
        SceneManager.LoadScene("Survival");
    }
    public void LoadLevelOne()
    {
        SceneManager.LoadScene("Level!");
    }
    public void LoadAmazingParticleEffect()
    {
        SceneManager.LoadScene("AmazingParticleEffect");
    }
}
