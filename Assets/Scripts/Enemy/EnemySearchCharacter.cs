using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySearchCharacter : MonoBehaviour {

    private void OnTriggerStay(Collider other)
    {
        // プレイヤーキャラクターを発見
        if (other.tag == "player")
        {
            // 敵キャラクターの状態を取得
            EnemyMovement.EnemyState state = GetComponentInParent<EnemyMovement>().GetState();
            // 敵キャラクターが追いかける状態でなければ追いかける設定に変更
            if (state != EnemyMovement.EnemyState.chase)
            {
                Debug.Log("プレイヤー発見");
                GetComponentInParent<EnemyMovement>().SetState("chase", other.transform);
            }
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.tag == "Player")
        {
            Debug.Log("見失う");
            GetComponentInParent<EnemyMovement>().SetState("wait");
        }
    }
}
