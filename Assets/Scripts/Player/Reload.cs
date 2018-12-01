using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reload : MonoBehaviour {

    public int CartridgeBulletNum;  // 一つのカートリッジに入っている弾の数
    public int CurrentBulletNum;    // 現在のカートリッジの弾の数
    public int AllBulletNum;        // プレイヤーが所持している弾の総数
    public bool CanShootable;

    // Use this for initialization
    void Start ()
    {
        CartridgeBulletNum = FindObjectOfType<PlayerWeaponManage>().CartridgeBulletNum;
        CurrentBulletNum = FindObjectOfType<PlayerWeaponManage>().CurrentBulletNum;
        AllBulletNum = FindObjectOfType<PlayerWeaponManage>().AllBulletNum;
        CanShootable = FindObjectOfType<PlayerWeaponManage>().CanShootable;

        StartCoroutine(ReloadMove());
    }

    // リロード処理
    IEnumerator ReloadMove()
    {
        if (AllBulletNum + CurrentBulletNum < CartridgeBulletNum)
        {
            if (AllBulletNum != 0)
            {
                yield return new WaitForSeconds(3.0f);      // 3秒、処理を待機

                // 弾丸を込められるだけ込める
                FindObjectOfType<PlayerWeaponManage>().CurrentBulletNum = CurrentBulletNum + AllBulletNum;
                // AllBulletNumの方は0にする
                FindObjectOfType<PlayerWeaponManage>().AllBulletNum = 0;
            }
        }
        else if (AllBulletNum + CurrentBulletNum >= CartridgeBulletNum)
        {
            yield return new WaitForSeconds(3.0f);      // 3秒、処理を待機

            // 何発弾を使ったかを計算
            int TempBulletNum = CartridgeBulletNum - CurrentBulletNum;
            // 使った弾分、全体の弾丸から引く
            FindObjectOfType<PlayerWeaponManage>().AllBulletNum = AllBulletNum - TempBulletNum;
            // カートリッジ収納分まで弾を回復
            FindObjectOfType<PlayerWeaponManage>().CurrentBulletNum = CartridgeBulletNum;
        }

        FindObjectOfType<PlayerWeaponManage>().CanShootable = true;

        Destroy(gameObject);
    }
}
