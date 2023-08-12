using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace Skill {
    /// <summary>
    /// ���ܹ�����
    /// </summary>
    public class CharacterSkillManager : MonoBehaviour {
        public int? LastSkillId = null;
        // �����б�
        public SkillData[] skills;
        public Animator Animator = null;

        private void Start() {
            foreach (SkillData skill in skills) {
                this._initSkill(skill);
            }
        }

        private void _initSkill(SkillData data) {
            data.skillPrefab = Resources.Load<GameObject>("Skill/" + data.prefabName);
            if (data.skillPrefab == null) {
                Debug.LogError("Skill prefab not found: " + data.prefabName);
                return;
            }
            data.owner = this.gameObject;
        }

        /// <summary>
        /// ���ݼ���ID��ȡ�������ݣ�����鼼���Ƿ����
        /// </summary>
        /// <param name="skillId">����ID</param>
        /// <returns>�������ݣ��������������ؿ�</returns>
        public SkillData PrepareSkill(int skillId) {
            SkillData data = this.skills.First(x => x.skillId == skillId);
            if (data == null) {
                Debug.LogError("Skill not found: " + skillId);
                return null;
            }

            if (data.coolRemain <= 0)
                return data;
            return null;
        }

        /// <summary>
        /// ���ɼ���
        /// </summary>
        /// <param name="data">��������</param>
        public void GenerateSkill(SkillData data) {
            if (data == null) {
                Debug.LogError("Cannot generate null skill");
                return;
            }
            data.isCasting = true;
            _onSkillEnd(data);

            // ��������
            GameObject skillGO = Instantiate(data.skillPrefab, this.transform.position, this.transform.rotation);

            // ���ݼ�������
            SkillDeployer deployer = skillGO.GetComponent<SkillDeployer>();
            deployer.SkillData = data;

            // ���ż��ܶ���
            if (data.animationName != "")
                Animator?.SetTrigger(data.animationName);

            // ִ�м���            
            deployer.DeploySkill();

            // ���ټ���
            Destroy(skillGO, data.durationTime + 0.5f);

            // ����������ȴ
            StartCoroutine(this._coolTimeDown(data));
        }

        private async void _onSkillEnd(SkillData data) {
            await Task.Delay((int)(data.durationTime * 1000f));
            data.isCasting = false;
        }

        // ������ȴ
        private IEnumerator _coolTimeDown(SkillData data) {
            for (data.coolRemain = data.coolTime; data.coolRemain > 0; data.coolRemain -= 0.02f)
                yield return new WaitForSeconds(0.02f);
        }
    }
}