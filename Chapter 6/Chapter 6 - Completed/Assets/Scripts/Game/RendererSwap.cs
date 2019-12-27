using UnityEngine;
using System.Collections;

public class RendererSwap : MonoBehaviour {

    public Renderer[] renderers;

    public float frameTime = 0.2f;
    private int currentFrame;

    void Awake ()
    {
        HideAll();
    }

	void Start ()
    {
		InvokeRepeating ("NextFrame", Random.Range (0, frameTime), frameTime);
	}

    void NextFrame()
    {
        currentFrame++;
        if (currentFrame >= renderers.Length)
        {
            currentFrame = 0;
        }
        for (int i = 0; i < renderers.Length; i++)
        {
            if (i == currentFrame)
            {
                renderers[i].enabled = true;
            }
            else
            {
                renderers[i].enabled = false;
            }
        }
    }

    void HideAll()
    {
        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].enabled = false;
        }
    }
}
