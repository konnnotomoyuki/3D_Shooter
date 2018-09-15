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

    public int damagePerShot = 20;
    private int WeaponType = 0;           // 武器タイプ
    private int WeaponNum = 2;			// 武器の種類数
    int shootableMask;
    public float timeBetweenBullets = 0.15f;
    public float range = 100f;
    float timer;
    float effectsDisplayTime = 0.2f;

    void Awake ()
    {
        shootableMask = LayerMask.GetMask ("Shootable");
        gunParticles = GetComponent<ParticleSystem> ();
        gunLine = GetComponent <LineRenderer> ();
        gunAudio = GetComponent<AudioSource> ();
        gunLight = GetComponent<Light> ();
    }


    void Update ()
    {
        timer += Time.deltaTime;

        if (Input.GetMouseButtonDown(1))
        {           // 右クリックを押すと、
            WeaponType = (WeaponType + 1) % WeaponNum;                  // 武器をチェンジ
            Debug.Log("現在の武器：" + WeaponType);
        }

        if (WeaponType == 0 || (WeaponType == 1 && Grenade_Dummy == null))
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
            // グレネードのダミーを削除
            Destroy(Grenade_Dummy);
            // グレネード本体を作成
            GameObject bom = Instantiate(Grenade, GameObject.Find("Player").transform);
            bom.transform.parent = null;
            Vector3 bom_speed = transform.TransformDirection(Vector3.forward) * 5;      // 手榴弾の移動速度。『プレイヤー正面に向けての速度ベクトル』を５。
            bom_speed += Vector3.up * 5;                                                // 手榴弾の『高さ方向の速度』を加算
            bom.GetComponent<Rigidbody>().velocity = bom_speed;                     // 手榴弾の速度を代入

            bom.GetComponent<Rigidbody>().angularVelocity = Vector3.forward * 7;	// 手榴弾の回転速度を代入.
        }
    }


    public void DisableEffects ()
    {
        gunLine.enabled = false;
        gunLight.enabled = false;
    }


    void Shoot ()
    {
        timer = 0f;

        gunAudio.Play ();

        gunLight.enabled = true;

        gunParticles.Stop ();
        gunParticles.Play ();

        gunLine.enabled = true;
        gunLine.SetPosition (0, transform.position);

        shootRay.origin = transform.position;
        shootRay.direction = transform.forward;

        if(Physics.Raycast (shootRay, out shootHit, range, shootableMask))
        {
            EnemyHealth enemyHealth = shootHit.collider.GetComponent <EnemyHealth> ();
            if(enemyHealth != null)
            {
                enemyHealth.TakeDamage (damagePerShot, shootHit.point);
            }
            gunLine.SetPosition (1, shootHit.point);
        }
        else
        {
            gunLine.SetPosition (1, shootRay.origin + shootRay.direction * range);
        }
    }

    void Bom()
    {
        Destroy(Grenade);
    }
}
