using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float distance;

    [SerializeField] private Quaternion vRotation;      // カメラの垂直回転(見下ろし回転)
    [SerializeField] public  Quaternion hRotation;      // カメラの水平回転

    Vector3 offset;

    private void Start()
    {
        offset.y += 2;

        // 回転の初期化
        vRotation = Quaternion.Euler(0, 0, 0);          // 垂直回転(X軸を軸とする回転)
        hRotation = Quaternion.Euler(0, 0, 0); ;       // 水平回転(Y軸を軸とする回転)
        transform.rotation = hRotation * vRotation;     // 最終的なカメラの回転は、垂直回転してから水平回転する合成回転

        // 位置の初期化
        // player位置から距離distanceだけ手前に引いた位置を設定します
        transform.position = target.position - transform.rotation * Vector3.forward * distance;
    }

    private void LateUpdate()
    {
        // カメラの位置(transform.position)の更新
        // player位置から距離distanceだけ手前に引いた位置を設定します
        transform.position = target.position - transform.rotation * Vector3.forward * distance + offset;
    }
}
