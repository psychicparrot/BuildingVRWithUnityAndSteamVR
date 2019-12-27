using UnityEngine;
using System.Collections;

public class AutoDestroyTimed : MonoBehaviour {

    public float destroyTime;

	void Start () {
        Invoke("DestroyMe", destroyTime);
	}
	
	void DestroyMe()
    {
        Destroy(this.gameObject);
    }
}
