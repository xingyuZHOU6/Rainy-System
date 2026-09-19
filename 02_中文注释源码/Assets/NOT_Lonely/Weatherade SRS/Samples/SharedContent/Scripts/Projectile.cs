/*
 * ================================================================================
 * 【Codex 中文研读注释｜学习副本，不是原作者注释】
 * 原始路径：Assets\NOT_Lonely\Weatherade SRS\Samples\SharedContent\Scripts\Projectile.cs
 * 分类/优先级：样例交互 / C-扩展
 * 文件职责：样例发射器与投射物碰撞逻辑。
 * 数据流位置：输入 -> Projectile -> 碰撞/VFX；压痕由 SRS_Occluder 深度网格产生。
 * 主要 Unity 技术：Instantiate、Rigidbody、Collision、ParticleSystem
 * 建议关注：区分物理反馈与 GPU 痕迹。
 * 说明：下方原始执行逻辑保持不变；本副本只增加文件头和少量【中文注释】导航。
 * ================================================================================
 */

namespace NOT_Lonely
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class Projectile : MonoBehaviour
    {
        [Range(0, 1)] public float destructAngleThreshold = 0.4f;
        public float minVelocityForFX = 2;
        public ParticleSystem snowHitFX;
        public ParticleSystem destructFX;
        public LayerMask snowLayermask = ~0;
        private Collider coll;
        private MeshRenderer mr;
        private Rigidbody rb;

        private bool isHit;
        private bool isTouching;

        // Start is called before the first frame update
        void Awake()
        {
            coll = GetComponent<Collider>();
            mr = GetComponent<MeshRenderer>();
            rb = GetComponent<Rigidbody>();
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (isHit) return;

            ContactPoint contactPoint = collision.GetContact(0);
            Vector3 pos = contactPoint.point;
            Vector3 normal = contactPoint.normal;

            float dot = Vector3.Dot(normal, rb.velocity.normalized);
            if (destructAngleThreshold < dot)
            {
                ParticleSystem pSys = Instantiate(destructFX, pos, Quaternion.LookRotation(normal, Vector3.up));
                pSys.Play();
                isHit = true;

                Destroy(gameObject);
            }
            else
            {
                if ((snowLayermask.value & (1 << collision.gameObject.layer)) != 0)
                    PlayFX();

                StartCoroutine(DestroyDelayed());
            }
        }


        ParticleSystem.VelocityOverLifetimeModule velOLT;
        private void OnCollisionStay(Collision collision)
        {
            if ((snowLayermask.value & (1 << collision.gameObject.layer)) == 0) return;

            PlayFX();
        }

        private void OnCollisionExit(Collision collision)
        {
            if ((snowLayermask.value & (1 << collision.gameObject.layer)) == 0) return;

            snowHitFX.Stop();
        }

        private void PlayFX()
        {

            float speed = rb.velocity.magnitude;

            if (speed > minVelocityForFX)
            {
                velOLT = snowHitFX.velocityOverLifetime;
                velOLT.speedModifierMultiplier = speed * 0.5f;

                if (snowHitFX.isPlaying) return;

                snowHitFX.Play();
            }
            else
            {
                snowHitFX.Stop();
            }
        }

        IEnumerator DestroyDelayed()
        {
            yield return new WaitForSeconds(5);
            Destroy(gameObject);
        }
    }
}
