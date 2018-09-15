using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour {

	// Use this for initialization
	void Start ()
    {
        StartCoroutine("bom");      // コルーチン開始
    }

    IEnumerator bom()
    {
        yield return new WaitForSeconds(2.5f);      // 2.5秒、処理を待機.
        Destroy(gameObject);
    }
}
