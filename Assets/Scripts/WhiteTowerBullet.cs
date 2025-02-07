using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhiteTowerBullet : MonoBehaviour
{
    public ParticleSystem[] fx_whiteTower;
    public GameObject[] fx_whiteTowerPre;
    private float BulletSpeed = 50f;

    private Vector3 blackFreaksPos;
    private GameObject BlackFreaks;

    private int State = 0;

    private bool isCrushed = false;
    float _damage;
    GameObject go;

    public void InitSetting(float damage, GameObject blackFreaks, Vector3 bulletSpawnPosition)
    {
        _damage = damage;
        go = this.gameObject;
        BlackFreaks = blackFreaks;
        StartCoroutine(StartProjectile(0.5f));
        transform.position = bulletSpawnPosition;


    }


    IEnumerator Shoot()
    {
        while(true)
        {
            if (BlackFreaks == null)
                break;
            blackFreaksPos = new Vector3(BlackFreaks.transform.position.x, BlackFreaks.transform.position.y + 1f, BlackFreaks.transform.position.z);


            if ((blackFreaksPos - transform.position).magnitude < 10f)
            {
                if (isCrushed == true || BlackFreaks == null)
                {
                    break;
                }
                GameManager.Damage.OnAttacked(_damage, BlackFreaks.GetComponent<Stat>());
                fx_whiteTower[0].Pause();
                fx_whiteTower[0].Play(false);
                fx_whiteTowerPre[0].SetActive(false);


                fx_whiteTowerPre[1].SetActive(true);
                fx_whiteTower[1] = fx_whiteTowerPre[1].GetComponent<ParticleSystem>();
                fx_whiteTower[1].Play(true);

                State = 2;
                StartCoroutine(DeleteThis());
                isCrushed = true;
            }


            switch (State)
            {
                case 1:
                    fx_whiteTower[0].transform.LookAt(blackFreaksPos);
                    transform.position += (blackFreaksPos - transform.position).normalized * BulletSpeed * Time.deltaTime;
                    transform.rotation = Quaternion.identity;
                    break;
                case 2:
                    transform.position += (blackFreaksPos - transform.position).normalized * BulletSpeed * Time.deltaTime;
                    transform.rotation = Quaternion.identity;
                    break;
                default:
                    break;
            }
            yield return null;
        }
    }


    IEnumerator StartProjectile(float waitTime)
    {
        go.SetActive(true);
        yield return YieldInstructionCache.WaitForSeconds(waitTime);

        fx_whiteTowerPre[0].SetActive(true);
        fx_whiteTower[0] = fx_whiteTowerPre[0].GetComponent<ParticleSystem>();
        fx_whiteTower[0].Play(true);
        State = 1;

        StartCoroutine(Shoot());
    }


    IEnumerator DeleteThis()
    {
        yield return YieldInstructionCache.WaitForSeconds(fx_whiteTower[1].main.startLifetimeMultiplier);
        fx_whiteTowerPre[1].SetActive(false);
        State = 3;

        BulletPooling.ReturnObject(go);
    }
}