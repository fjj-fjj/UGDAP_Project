using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Skill {
    public class DashImpact : IImpactEffect {
        public void Execute(SkillDeployer deployer) {
            foreach (var target in deployer.SkillData.attackTargets)
                this.applyEffect(target, deployer);
        }
        private async void applyEffect(Transform target, SkillDeployer deployer) {
            Rigidbody2D targetRB = target.GetComponent<Rigidbody2D>();
            if (targetRB == null)
                return;
            float originSpeed = targetRB.velocity.x;
            if (Mathf.Approximately(originSpeed, 0)) {
                Debug.Log("Target is not moving, cannot dash");
                return;
            }
            float originGravity = targetRB.gravityScale;

            targetRB.velocity = new Vector2(
                Mathf.Sign(originSpeed) * deployer.SkillData.value,
                0
            );
            targetRB.gravityScale = 0;

            await Task.Delay((int)(deployer.SkillData.durationTime * 1000f));
            targetRB.velocity = new Vector2(originSpeed, 0);
            targetRB.gravityScale = originGravity;
        }
    }
}