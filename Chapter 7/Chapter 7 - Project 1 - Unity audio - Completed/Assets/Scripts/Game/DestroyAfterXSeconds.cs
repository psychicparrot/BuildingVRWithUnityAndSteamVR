using UnityEngine;
using System.Collections;

public class DestroyAfterXSeconds : MonoBehaviour {

    public float secondsUntilDestruction = 5f;

    void Start () {
        Invoke("Hit", secondsUntilDestruction);
	}

    public void Hit()
    {
        Destroy(this.gameObject);
    }
}
