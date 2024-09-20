using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UseChest : MonoBehaviour {
    private GameObject curObject;
    public GameObject handUI;
    public GameObject objToActivate;

    private bool inReach;
    void Start() {
        curObject = this.gameObject;
        handUI.SetActive(false);
        objToActivate.SetActive(false);
    }

    void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag == "Reach") {
            inReach = true;
            handUI.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other) {
        if (other.gameObject.tag == "Reach") {
            inReach = false;
            handUI.SetActive(false);
        }
    }

    void Update() {
        if (inReach && Input.GetButtonDown("Interact")) {
            handUI.SetActive(false);
            objToActivate.SetActive(true);
            curObject.GetComponent<Animator>().SetBool("open", true);
            curObject.GetComponent<BoxCollider>().enabled = false;
        }
    }
}
