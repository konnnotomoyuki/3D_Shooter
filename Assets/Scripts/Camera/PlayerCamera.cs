﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] float RotateSpeed;
    float floYaw, floPitch;

    Transform Camera_Pos;
    Vector3 offset;
    
    private void Start()
    {
        RotateSpeed = 1;
        offset.x = 0.5f;
        offset.y = 1.5f;
        offset.z = -2.5f;

        Camera_Pos = GetComponent<Transform>();
    }

    private void Update()
    {
        //プレイヤー位置を追従する
        Camera_Pos.position = new Vector3(target.position.x, target.position.y, target.position.z) + offset;

        floYaw += Input.GetAxis("Mouse X") * RotateSpeed; //横回転入力
        floPitch -= Input.GetAxis("Mouse Y") * RotateSpeed; //縦回転入力

        floPitch = Mathf.Clamp(floPitch, -80, 60); //縦回転角度制限する

        transform.eulerAngles = new Vector3(floPitch, floYaw, 0.0f); //回転の実行
    }
}

//public class PlayerCamera : MonoBehaviour
//{
//    [SerializeField] private Transform target;
//    [SerializeField] private Rigidbody m_Rigidbody;
//    [SerializeField] private float distance;

//    [SerializeField] private Quaternion vRotation;      // カメラの垂直回転(見下ろし回転)
//    [SerializeField] public Quaternion hRotation;      // カメラの水平回転

//    Vector3 offset;

//    private void Start()
//    {
//        offset.x += 1f;
//        offset.y += 4f;
//        offset.z += 1f;
//        m_Rigidbody = GetComponent<Rigidbody>();

//        回転の初期化
//        vRotation = Quaternion.Euler(0, 0, 0);          // 垂直回転(X軸を軸とする回転)
//        hRotation = Quaternion.Euler(0, -20, 0); ;       // 水平回転(Y軸を軸とする回転)
//        transform.rotation = hRotation * vRotation;     // 最終的なカメラの回転は、垂直回転してから水平回転する合成回転

//        位置の初期化
//        player位置から距離distanceだけ手前に引いた位置を設定します
//        transform.position = target.position - transform.rotation * Vector3.forward * distance;
//    }

//    private void LateUpdate()
//    {
//        カメラの位置(transform.position)の更新
//        player位置から距離distanceだけ手前に引いた位置を設定します
//        transform.position = target.position - transform.rotation * Vector3.forward * distance + offset;
//        Debug.Log("transform.rotation" + transform.rotation);
//        Debug.Log("Vector3.forward" + Vector3.forward);
//        Debug.Log("distance" + distance);
//        Debug.Log("offset" + offset);
//    }
//}
