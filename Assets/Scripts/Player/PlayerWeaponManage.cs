using UnityEngine;
using System.Collections;

public class PlayerWeaponManage : MonoBehaviour
{
    Ray shootRay = new Ray();
    RaycastHit shootHit;
    ParticleSystem gunParticles;
    LineRenderer gunLine;
    AudioSource gunAudio;
    Light gunLight;

    public GameObject Grenade_Dummy;
    public GameObject Grenade;
    public GameObject GunBarrel;
    public GameObject ReloadMove;

    public int shootableMask;
    public int damagePerShot = 20;
    private int WeaponType = 0;                // 武器タイプ
    private int WeaponNum = 2;                 // 武器の種類数
    public int CartridgeBulletNum = 30;        // 一つのカートリッジに入っている弾の数
    public int CurrentBulletNum = 30;          // 現在のカートリッジの弾の数
    public int AllBulletNum = 150;             // プレイヤーが所持している弾の総数
    public float timeBetweenBullets = 0.15f;
    public float range = 100f;
    float timer;
    float effectsDisplayTime = 0.2f;

    public bool CanShootable = true;

    void Awake()
    {
        shootableMask = LayerMask.GetMask("Shootable");
        gunParticles = GunBarrel.GetComponent<ParticleSystem>();
        gunLine = GunBarrel.GetComponent<LineRenderer>();
        gunAudio = GunBarrel.GetComponent<AudioSource>();
        gunLight = GunBarrel.GetComponent<Light>();
    }

    private void Update()
    {
        Attack();
    }

    void Attack()
    {
        timer += Time.deltaTime;

        if (Input.GetMouseButtonDown(1))
        {           // 右クリックを押すと、
            WeaponType = (WeaponType + 1) % WeaponNum;                  // 武器をチェンジ
            Debug.Log("現在の武器：" + WeaponType);
        }

        if ((AllBulletNum <= 0 && CurrentBulletNum == 0) || CanShootable == false)
        {
            return;
        }
        else if ((CurrentBulletNum == 0 && AllBulletNum != 0) || (Input.GetKey(KeyCode.R) && AllBulletNum != 0))
        {
            CanShootable = false;

            var parent = this.transform;
            Instantiate(ReloadMove, parent);
        }
        else if ((WeaponType == 0 && CanShootable == true) || (WeaponType == 1 && Grenade_Dummy == null && CanShootable == true))
        {
            if (Input.GetButton("Fire1") && timer >= timeBetweenBullets && Time.timeScale != 0)
            {
                Shoot();
            }

            if (timer >= timeBetweenBullets * effectsDisplayTime)
            {
                DisableEffects();
            }
        }
        else if (WeaponType == 1 && Grenade_Dummy != null)
        {
            Vector3 pos = transform.position + transform.TransformDirection(Vector3.forward);  // プレイヤー位置　+　プレイヤー正面にむけて１進んだ距離
            // グレネードのダミーを削除
            Destroy(Grenade_Dummy);
            // グレネード本体を作成
            GameObject bom = Instantiate(Grenade, pos, Quaternion.identity) as GameObject;

            Vector3 bom_speed = transform.TransformDirection(Vector3.forward) * 5;      // 手榴弾の移動速度。『プレイヤー正面に向けての速度ベクトル』を５。
            bom_speed += Vector3.up * 5;                                                // 手榴弾の『高さ方向の速度』を加算
            bom.GetComponent<Rigidbody>().velocity = bom_speed;                         // 手榴弾の速度を代入
            bom.GetComponent<Rigidbody>().angularVelocity = Vector3.forward * 7;	    // 手榴弾の回転速度を代入.
        }
    }


    public void DisableEffects()
    {
        gunLine.enabled = false;
        gunLight.enabled = false;
    }

    void Shoot()
    {
        timer = 0f;

        gunAudio.Play();

        gunLight.enabled = true;

        gunParticles.Stop();
        gunParticles.Play();

        gunLine.enabled = true;
        gunLine.SetPosition(0, transform.position);

        shootRay.origin = transform.position;
        shootRay.direction = transform.forward;

        if (Physics.Raycast(shootRay, out shootHit, range, shootableMask))
        {
            EnemyHealth enemyHealth = shootHit.collider.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damagePerShot, shootHit.point);
            }
            gunLine.SetPosition(1, shootHit.point);
        }
        else
        {
            gunLine.SetPosition(1, shootRay.origin + shootRay.direction * range);
        }

        CurrentBulletNum--;
    }

    //// リロード処理
    //IEnumerator Reload()
    //{
    //    if (AllBulletNum + CurrentBulletNum < CartridgeBulletNum)
    //    {
    //        if (AllBulletNum != 0)
    //        {
    //            yield return new WaitForSeconds(3.0f);      // 3秒、処理を待機

    //            // 弾丸を込められるだけ込める
    //            CurrentBulletNum = CurrentBulletNum + AllBulletNum;
    //            // AllBulletNumの方は0にする
    //            AllBulletNum = 0;
    //        }
    //    }
    //    else if (AllBulletNum + CurrentBulletNum >= CartridgeBulletNum)
    //    {
    //        yield return new WaitForSeconds(3.0f);      // 3秒、処理を待機

    //        // 何発弾を使ったかを計算
    //        int TempBulletNum = CartridgeBulletNum - CurrentBulletNum;
    //        // 使った弾分、全体の弾丸から引く
    //        AllBulletNum = AllBulletNum - TempBulletNum;
    //        // カートリッジ収納分まで弾を回復
    //        CurrentBulletNum = CartridgeBulletNum;
    //    }

    //    CanShootable = true;
    //}
}
