namespace Skill {
    public class SimpleDeployer : SkillDeployer {
        public override void DeploySkill() {
            this._calculateTargets();
            this._impactTargets();
        }
    }
}