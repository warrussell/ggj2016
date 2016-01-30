using UnityEngine;
using System.Collections;

public class test1 : MonoBehaviour {

	// Use this for initialization
	void Start () {
        InputListener.inputHeldEvent += handleInputHeld;
        GameObject star = Instantiate(Resources.Load("star")) as GameObject;
        star.transform.SetParent(this.transform);
        star.transform.position = Vector3.zero;
	}

    private void handleInputHeld(KeyCode key)
    {
        if (key == KeyCode.W)
            HologramRotate("up");
    }

    private void HologramRotate(System.String)
    {
    }
    // Update is called once per frame
    void Update () {
	
	}
}
