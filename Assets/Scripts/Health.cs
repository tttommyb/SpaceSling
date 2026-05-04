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
    bool use_life = false;
    Vector2 life_displacement;
    float elapsed_time = 0.0f;
    float translation_time = 10.0f;

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
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
            else
            {
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

        if(lives < max_lives)
        {
            if(lives_icons.Count > 0) 
            {
                lives_icons[lives].enabled = true;
            }
            lives++;
        }



    }
}
