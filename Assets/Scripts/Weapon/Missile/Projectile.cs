using UnityEngine;
using System.Collections;

namespace Scripts.Weapon.Missile
{
	public class Projectile : MonoBehaviour
	{

		private bool explodeSelf;
		[Tooltip("启用其施加恒定的力，否则只在发射时施加力")] 
		public bool UseConstantForce;
		[Tooltip("炮弹速度")] 
		public float ConstantForceSpeed = 5000f;
		[Tooltip("炮弹发射后进行自我爆炸的时间")] 
		public float ExplodeAfter = 2.5f;
		private bool hasStartedExplode;

		private bool hasCollided; // 是否已经发生碰撞

		[Header("爆炸效果预制体")] 
		public Transform ExplosionPrefab;

		[Header("自定义选项")] 
		[Tooltip("发射作用力")] 
		public float Force = 2500f;
		[Tooltip("炮弹发射后的销毁时间")] 
		public float DespawnTime = 30f;

		[Header("爆炸选项")] [Tooltip("爆炸半径")] 
		public float Radius = 5f;
		[Tooltip("爆炸冲击力")] 
		public float Power = 2250f;

		[Header("火箭弹")] 
		[Tooltip("如果投射物有粒子效果则启用")]
		public bool UsesParticles;

		public ParticleSystem SmokeParticles;
		public ParticleSystem FlameParticles;
		[Tooltip("销毁延迟，保证粒子效果播放完毕")] 
		public float DestroyDelay = 5f;

		private void Start()
		{
			// 如果不是使用恒定的力 (榴弹发射器)
			if (!UseConstantForce)
			{
				// 只在一开始施加力的作用使其向前发射
				GetComponent<Rigidbody>().AddForce
					(gameObject.transform.forward * Force);
			}

			// 启动销毁计时器
			StartCoroutine(DestroyTimer());
		}

		private void FixedUpdate()
		{
			// 根据炮弹前进的方向改变其模型朝向角度
			if (GetComponent<Rigidbody>().velocity != Vector3.zero)
				GetComponent<Rigidbody>().rotation =
					Quaternion.LookRotation(GetComponent<Rigidbody>().velocity);

			// 如果使用恒定的力 (RPG)
			if (UseConstantForce == true && !hasStartedExplode)
			{
				// 一直给炮弹施加恒定的力，使其不会抛物线落下
				GetComponent<Rigidbody>().AddForce
					(gameObject.transform.forward * ConstantForceSpeed);

				// 开始爆炸
				StartCoroutine(ExplodeSelf());

				// 停止循环
				hasStartedExplode = true;
			}
		}

		// 当炮弹飞向天空或者类似情况，不会与其它物体碰撞，就会在一段时间后自己爆炸
		private IEnumerator ExplodeSelf()
		{
			// 等待设定时间后
			yield return new WaitForSeconds(ExplodeAfter);
			// 如果没有发生碰撞，而生成爆炸效果预制体
			if (!hasCollided)
			{
				Instantiate(ExplosionPrefab, transform.position, transform.rotation);
			}

			// 隐藏炮弹
			gameObject.GetComponent<MeshRenderer>().enabled = false;
			// 冻结物体不受物理引擎作用
			gameObject.GetComponent<Rigidbody>().isKinematic = true;
			// 禁用碰撞体
			gameObject.GetComponent<BoxCollider>().isTrigger = true;
			// 停止粒子效果
			if (UsesParticles == true)
			{
				FlameParticles.GetComponent<ParticleSystem>().Stop();
				SmokeParticles.GetComponent<ParticleSystem>().Stop();
			}

			// 等待粒子效果完全消失
			yield return new WaitForSeconds(DestroyDelay);
			// 销毁炮弹
			Destroy(gameObject);
		}

		// 发射炮弹后在设定时间后销毁炮弹（无发射碰撞情况）
		private IEnumerator DestroyTimer()
		{
			yield return new WaitForSeconds(DespawnTime);
			Destroy(gameObject);
		}

		// 炮弹发射碰撞后，在设定时间后销毁炮弹
		private IEnumerator DestroyTimerAfterCollision()
		{
			yield return new WaitForSeconds(DestroyDelay);
			Destroy(gameObject);
		}

		// 如果炮弹和某个物体发生了碰撞
		private void OnCollisionEnter(Collision collision)
		{
			// 碰撞标志位设置为true
			hasCollided = true;

			// 隐藏炮弹
			gameObject.GetComponent<MeshRenderer>().enabled = false;
			// 冻结物体不受物理引擎作用
			gameObject.GetComponent<Rigidbody>().isKinematic = true;
			// 禁用碰撞体
			gameObject.GetComponent<BoxCollider>().isTrigger = true;

			if (UsesParticles == true)
			{
				FlameParticles.GetComponent<ParticleSystem>().Stop();
				SmokeParticles.GetComponent<ParticleSystem>().Stop();
			}

			StartCoroutine(DestroyTimerAfterCollision());

			// 在碰撞点生成爆炸效果
			Instantiate(ExplosionPrefab, collision.contacts[0].point,
				Quaternion.LookRotation(collision.contacts[0].normal));

			// 如果命中了靶子
			// if (collision.gameObject.tag == "Target" && 
			//     	collision.gameObject.GetComponent<TargetScript>().isHit == false) {
			// 	
			// 	//Spawn explosion prefab on surface
			// 	Instantiate(explosionPrefab,collision.contacts[0].point,
			// 	            Quaternion.LookRotation(collision.contacts[0].normal));
			//
			// 	//Animate the target 
			// 	collision.gameObject.transform.gameObject.GetComponent
			// 		<Animation> ().Play("target_down");
			// 	//Toggle the isHit bool on the target object
			// 	collision.gameObject.transform.gameObject.GetComponent
			// 		<TargetScript>().isHit = true;
			// }

			// 给爆炸范围内的物体施加力的作用
			Vector3 explosionPos = transform.position;
			// OverlapSphere的作用是在给定的位置和半径内返回一个碰撞器数组，
			// 这些碰撞器与球形区域相交。可以用它来实现爆炸伤害或检测附近的物体
			Collider[] colliders = Physics.OverlapSphere(explosionPos, Radius);
			foreach (Collider hit in colliders)
			{
				Rigidbody rb = hit.GetComponent<Rigidbody>();

				// 对周围的刚体施加力的作用
				if (rb != null)
					rb.AddExplosionForce(Power * 50, explosionPos, Radius, 3.0F);

				// 如果命中范围内有敌人则对其造成伤害
				if (hit.gameObject.tag.Equals("Enemy"))
				{
					var enemy = hit.gameObject.GetComponent<EnemyFeature>();
					var player = GameObject.FindWithTag("Player");
					var projectileDamage = player.GetComponent<WeaponManager>().GetCarriedWeapon().Damage;
					enemy.TakeDamage(projectileDamage);
				}

				// 如果命中了靶子
				// if (hit.GetComponent<Collider>().tag == "Target" && 
				//     	hit.GetComponent<TargetScript>().isHit == false) {
				//
				// 	//Animate the target 
				// 	hit.gameObject.GetComponent<Animation> ().Play("target_down");
				// 	//Toggle the isHit bool on the target object
				// 	hit.gameObject.GetComponent<TargetScript>().isHit = true;
				// }

				// 命中爆炸桶
				// if (hit.transform.tag == "ExplosiveBarrel") {
				// 	
				// 	//Toggle the explode bool on the explosive barrel object
				// 	hit.transform.gameObject.GetComponent<ExplosiveBarrelScript>().explode = true;
				// }

				// 命中瓦斯罐
				// if (hit.GetComponent<Collider>().tag == "GasTank") 
				// {
				// 	//If gas tank is within radius, explode it
				// 	hit.gameObject.GetComponent<GasTankScript> ().isHit = true;
				// 	hit.gameObject.GetComponent<GasTankScript> ().explosionTimer = 0.05f;
				// }
			}
		}
	}
}