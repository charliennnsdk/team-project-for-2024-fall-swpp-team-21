using System.Collections;
using System.Collections.Generic;
using UnityEngine;

sealed public class W_PacketStream : WeaponBehaviour
{

    [SerializeField] private GameObject muzzle;
    [SerializeField] private float fluctuationRadius;

    protected override IEnumerator Attack()
    {
        while (true)
        {
            StartCoroutine(Shoot());
            yield return new WaitForSeconds(finalAttackPeriod);
        }
    }


    protected override void LevelUpEffect(int level)
    {
        switch (level)
        {
            case 1:
                // Do nothing
                break;
            case 2:
                BasicAttackPeriod *= 1 / (1 + 0.15f);
                break;
            case 3:
                BasicDamage += 5;
                break;
            case 4:
                BasicMultiProjectile += 1;
                break;
            case 5:
                BasicDamage += 5;
                break;
            case 6:
                BasicAttackPeriod *= 1 / (1 + 0.15f);
                break;
            case 7:
                BasicAttackPeriod *= 1 / (1 + 0.3f);
                break;
            case 8:
                BasicDamage += 10;
                break;
            case 9:
                BasicAttackPeriod *= 1 / (1 + 0.4f);
                BasicDamage += 10;
                break;
            default:
                break;
        }
    }

    protected override void InitExplanation()
    {
        switch (MaxLevel)
        {
            case 1:
                explanations[0] = "플레이어의 전방을 향해 빠르게 탄환을 발사합니다";
                break;
            case 2:
                explanations[1] = "공격 속도 15% 증가";
                goto case 1;
            case 3:
                explanations[2] = "기본 데미지 5 증가";
                goto case 2;
            case 4:
                explanations[3] = "투사체 1개 추가 발사";
                goto case 3;
            case 5:
                explanations[4] = "기본 데미지 5 증가";
                goto case 4;
            case 6:
                explanations[5] = "공격 속도 15% 증가";
                goto case 5;
            case 7:
                explanations[6] = "공격 속도 30% 증가";
                goto case 6;
            case 8:
                explanations[7] = "기본 데미지 10 증가";
                goto case 7;
            case 9:
                explanations[8] = "공격 속도 40% 증가, 기본 데미지 10 증가";
                goto case 8;
        }
    }

    private IEnumerator Shoot()
    {
        GameObject proj;
        for (int i = 0; i < finalMultiProjectile; i++)
        {
            Vector3 finalPosition = muzzle.transform.position + FireFluctuation();
            proj = PoolManager.instance.GetObject(projectilePool, finalPosition, muzzle.transform.rotation);
            proj.GetComponent<ProjectileBehaviour>().Initialize(finalDamage * (IsCritical() ? finalCritPoint : 100) / 100);
            yield return new WaitForSeconds(0.03f);
        }
    }

    private Vector3 FireFluctuation()
    {
        int x = Random.Range(int.MinValue, int.MaxValue);
        int y = Random.Range(int.MinValue, int.MaxValue);
        return new Vector3(x, y, 0) / int.MaxValue * fluctuationRadius;
    }

}
