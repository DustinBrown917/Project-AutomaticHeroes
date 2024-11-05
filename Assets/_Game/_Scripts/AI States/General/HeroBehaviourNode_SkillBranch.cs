using Game.Flow;
using SegoLib.Random;
using System;
using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace Game.AI.General
{
    public class HeroBehaviourNode_SkillBranch : IHeroBehaviour.Node
    {
        [System.Serializable]
        public class SkillPort : FlowPort
        {
            [field: SerializeField] public HeroSkillDef Skill { get; private set; }
        }


        [Input] public FlowPort entry;
        [Output(dynamicPortList = true)] public SkillPort[] exits = Array.Empty<SkillPort>();
        [System.NonSerialized] private readonly Dictionary<HeroSkillDef, List<SkillPort>> m_exitPortsBySkill = new();

        protected override void Init()
        {
            base.Init();

            InitExitPortsBySkill();
        }

        public override IHeroBehaviour.INode.IState Enter(IHeroBehaviour.IState bb)
        {
            IHeroBehaviour.INode.State state = new();
            state.Conclude(GetNextNode(bb));
            return state;
        }

        public override FlowPort GetFlowPort(NodePort port)
        {
            if (port.fieldName == nameof(entry))
                return entry;

            if (port.fieldName == nameof(exits) || !port.fieldName.Contains(nameof(exits)))
                return null;

            return exits[GetIndexFromFieldName(nameof(exits), port.fieldName)];
        }

        private IHeroBehaviour.Node GetNextNode(IHeroBehaviour.IState bb)
        {
            List<HeroSkill> skills = UnityEngine.Pool.ListPool<HeroSkill>.Get();

            int sumOfSkills = 0;

            foreach(HeroSkillDef skillDef in m_exitPortsBySkill.Keys) {
                HeroSkill skill = bb.Hero.Skills[skillDef];
                sumOfSkills += skill.Level;
                skills.Add(skill);
            }

            int index = WeightedRandom.Naive(skills, sumOfSkills, bb.WorldState.Rng, x => x.Level);

            HeroSkill chosenSkill = skills[index];

            List<SkillPort> skillPorts = m_exitPortsBySkill[chosenSkill.Def];

            SkillPort chosenSkillPort = bb.WorldState.Rng.GetRandom(skillPorts);

            FlowNode chosenNode = chosenSkillPort.ConnectedPortData.Node;

            UnityEngine.Pool.ListPool<HeroSkill>.Release(skills);

            return chosenNode as IHeroBehaviour.Node;
        }


        private void InitExitPortsBySkill()
        {
            m_exitPortsBySkill.Clear();

            foreach(SkillPort skillPort in exits) {
                if(skillPort.Skill == null)
                    continue;

                if(!m_exitPortsBySkill.TryGetValue(skillPort.Skill, out List<SkillPort> skillPorts)) {
                    skillPorts = new List<SkillPort>();
                    m_exitPortsBySkill[skillPort.Skill] = skillPorts;
                }

                skillPorts.Add(skillPort);
            }
        }


        public override void OnWorldTimeUpdate(IHeroBehaviour.IState bb, IHeroBehaviour.INode.IState node_bb, float deltaSeconds) { }
    } 
}
