/*
 * ================================================================================
 * 【Codex 中文研读注释｜学习副本，不是原作者注释】
 * 原始路径：Assets\NOT_Lonely\Weatherade SRS\Samples\SharedContent\Scripts\SimpleGun.cs
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
#if ENABLE_INPUT_SYSTEM
    using UnityEngine.InputSystem;
#endif

    public class SimpleGun : MonoBehaviour
    {
        public Rigidbody projectileTemplate;
        public float impulsePower = 10;
        public float torquePower = 10;
        public float projectileSizeMul = 0.5f;

        [Range(0, 1)] public float sfxVolume = 0.5f;
        private AudioSource aSource;
        
#if ENABLE_INPUT_SYSTEM
        private InputAction fireAction;
#endif
        void Awake()
        {
            aSource = GetComponent<AudioSource>();

#if ENABLE_INPUT_SYSTEM
            fireAction = new InputAction("Fire", InputActionType.Button);
            fireAction.AddBinding("<Mouse>/leftButton");
#endif
        }
        
#if ENABLE_INPUT_SYSTEM
        void OnEnable()
        {
            fireAction.Enable();
            fireAction.performed += OnFire;
        }

        void OnDisable()
        {
            fireAction.Disable();
            fireAction.performed -= OnFire;
        }
        
        private void OnFire(InputAction.CallbackContext context)
        {
            Fire();
        }
#else
        void Update()
        {
            if (Input.GetMouseButtonDown(0))
                Fire();
        }
#endif

        private void Fire()
        {
            Rigidbody projectile = Instantiate(projectileTemplate, transform.position, transform.rotation);
            projectile.transform.localScale = Vector3.one * projectileSizeMul;
            projectile.AddForce(transform.forward * impulsePower, ForceMode.Impulse);
            Vector3 torque = Random.insideUnitSphere * torquePower;
            projectile.AddTorque(torque);

            if (aSource != null) aSource.PlayOneShot(aSource.clip, sfxVolume);
        }
    }
}
