using UnityEngine;
using System.Linq;
using System.Collections.Generic;

namespace Skill {
    public abstract class IAttackSelector {
        /// <summary>
        /// ����Ŀ��
        /// </summary>
        /// <param name="data">��������</param>
        /// <param name="skillTF">�������ڶ����Transform</param>
        /// <returns>������Ŀ��Transform������</returns>
        public Transform[] SelectTarget(SkillData data, Transform skillTF) {
            Transform[] allTargets = this._getAllTargets(data);
            return this._filteringTargets(allTargets, data, skillTF);
        }

        /// <summary>
        /// ����tag��ȡ����Ŀ��
        /// </summary>
        protected Transform[] _getAllTargets(SkillData data) {
            List<Transform> targets = new List<Transform>();
            foreach (string tag in data.attackTargetTags) {
                targets.AddRange(GameObject.FindGameObjectsWithTag(tag).Select(go => go.transform));
            }
            return targets.ToArray();
        }

        /// <summary>
        /// ɸѡĿ�꣺����Ƿ��ڷ�Χ�ڣ��Ƿ������ΪĿ��
        /// </summary>
        protected virtual Transform[] _filteringTargets(Transform[] allTargets, SkillData data, Transform skillTF) {
            return allTargets;
        }
    }
}