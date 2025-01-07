using NueGames.NueDeck.Scripts.Utils;
using UnityEngine;
using UnityEngine.Video;

[RequireComponent(typeof(VideoPlayer))]
[RequireComponent(typeof(SceneChanger))]
public class VideoIntro : MonoBehaviour
{
    [SerializeField] VideoPlayer player;
    [SerializeField] SceneChanger sceneChanger;

    private void Awake()
    {
        player.Play();
        player.loopPointReached += OnVideoFinish;
        
    }

    private void OnVideoFinish(VideoPlayer source)
    {
        player.loopPointReached -= OnVideoFinish;
        sceneChanger.OpenMainMenuScene();
    }


    // Update is called once per frame
    void Update()
    {
        if(Input.anyKeyDown)
        {
            OnVideoFinish(player);

        }
            
    }
}
