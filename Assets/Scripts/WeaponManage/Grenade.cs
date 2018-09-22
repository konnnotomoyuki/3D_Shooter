using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour {
    public GameObject PreEffct;

    // Use this for initialization
    void Start ()
    {
        StartCoroutine("bom");      // コルーチン開始
    }

    IEnumerator bom()
    {
        yield return new WaitForSeconds(2.5f);      // 2.5秒、処理を待機.

        GameObject Effect = Instantiate(PreEffct, transform.position, Quaternion.identity) as GameObject;
        Destroy(Effect, 1.0f);

        bomAttack();				// ボムによる攻撃処理

        Destroy(gameObject);
    }

    private void bomAttack()
    {
        Collider[] targets = Physics.OverlapSphere(transform.position, 5f);   // 自分自身を中心に、半径5f以内にいるColliderを探し、配列に格納.
        foreach (Collider obj in targets)
        {       // targets配列を順番に処理 (その時に仮名をobjとする)
            if (obj.tag == "Enemy")
            {               // タグ名がEnemyなら
                Destroy(obj.gameObject);        // そのゲームオブジェクトを消滅させる。
            }
        }
    }
}
