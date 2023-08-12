using UnityEngine;

namespace Skill {
    public class SelfSelector : IAttackSelector {
        protected override Transform[] _filteringTargets(Transform[] allTargets, SkillData data, Transform skillTF) {
            return new Transform[] { data.owner.transform };
        }
    }
}