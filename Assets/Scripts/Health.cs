using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Health : MonoBehaviour
{

    [SerializeField] List<Image> lives_icons = new List<Image>();
    [SerializeField] GameObject explosion;
    [SerializeField] int max_lives = 3;
    int lives;
    SpriteRenderer sr;
    // Start is called before the first frame update
    void Start()
    {
        lives = max_lives;
        sr = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RemoveLife()
    {
        lives--;
        
        if (lives == 0)
        {
            if (gameObject.CompareTag("Player") || gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                // Reload the level instantly
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
            else
            {
                // If it's an enemy, just deactivate them
                Instantiate(explosion, transform.position, Quaternion.identity);
                gameObject.SetActive(false);
            }
        }
        if(lives_icons.Count > 0) 
        {
            lives_icons[lives].enabled = false;
        }
    }
    
    public void AddLife()
    {
        if(lives_icons.Count > 0) 
        {
            lives_icons[lives].enabled = true;
        }
        if(lives < max_lives)
        {
            lives++; 
        }


    }
}
