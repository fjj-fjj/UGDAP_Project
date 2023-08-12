using System;
using UnityEngine;

namespace Skill {
    [Serializable]
    public class SkillData : ICloneable {
        public int skillId;
        public string name;
        public string description;
        public bool isPassive = false;
        public float coolTime;
        [HideInInspector]
        public float coolRemain;
        public int value;
        public float attackDistance;
        public float attackRange;
        public string[] attackTargetTags;
        [HideInInspector]
        public Transform[] attackTargets;
        [Tooltip("����Ч�������б�")]
        public string[] impactType;
        public int nextBatterId;
        public float durationTime;
        public float atkInterval;
        [HideInInspector]
        public GameObject owner;
        public string prefabName;
        [HideInInspector]
        public GameObject skillPrefab;
        public string animationName;
        public string hitFxName;
        [HideInInspector]
        public GameObject hitFxPrefab;
        public SelectorType selectorType;
        [HideInInspector]
        public bool isCasting;

        public object Clone() {
            return this.MemberwiseClone();
        }
    }
}