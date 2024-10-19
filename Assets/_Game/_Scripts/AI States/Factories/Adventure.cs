using SegoLib;
using SegoLib.Random;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class Adventure : StateFactory<Adventure.State>
    {
        /*vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv*/
        #region ========================================== Subclasses ==================================================
        /*vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv*/

        public class State : HeroState
        {
            private float m_elapsed = 0f;

            private Dungeon m_dungeon;

            private DungeonFloor m_currentFloor;
            private DungeonRoom m_currentRoom;
            private bool[] m_vistedRooms = new bool[3];

            private readonly List<(int floor, int room)> m_path = new();

            private Monster m_currentMonster = null;
            private SubState m_currentSubstate;

            public State(Adventure stateFactory, Sprite sprite, Hero hero, Dungeon dungeon) : base(stateFactory, sprite, hero)
            {
                ActionString = "Adventuring.";
                m_dungeon = dungeon;
            }

            public override void DoState(float deltaSeconds)
            {
                m_elapsed += deltaSeconds;

                m_currentSubstate.DoState(deltaSeconds);
            }

            public override void EnterState()
            {
                m_currentFloor = m_dungeon[0];
                ExploreRoom(0);
            }

            public override void ExitState() { }

            private void ExploreNextRoom()
            {
                int unexploredIndex = GetIndexOfUnexploredRoom();
                if(unexploredIndex != -1 && WorldState.Instance.Rng.Bool()) {
                    ExploreRoom(unexploredIndex);
                } else {
                    ExploreDown();
                }
            }

            private void ExploreDown()
            {
                for(int i = 0; i < m_vistedRooms.Length; i++)
                    m_vistedRooms[i] = false;

                m_currentFloor = m_dungeon[m_currentFloor.depth + 1];
                ExploreRoom(m_currentRoom.index);
            }

            private void ExploreRoom(int index)
            {
                m_currentRoom = m_currentFloor[index];
                m_vistedRooms[index] = true;

                m_path.Add((m_currentFloor.depth, m_currentRoom.index));
            }

            private int GetIndexOfUnexploredRoom()
            {
                for(int i = 0; i < m_vistedRooms.Length; i++) {
                    if (!m_vistedRooms[i])
                        return i;
                }

                return -1;
            }

            private void SetState(Action<float> state)
            {

            }

            private void State_Explore(float deltaSeconds)
            {

            }

            private void State_Combat(float deltaSeconds)
            {
                m_currentMonster = m_dungeon.GetMonster(m_currentRoom.level);
            }
        }

        private abstract class SubState
        {
            public Hero Hero { get; }

            public SubState(Hero hero)
            {
                Hero = hero;
            }

            public abstract void DoState(float deltaSeconds);

            public abstract void EnterState();

            public abstract void ExitState();
        }

        private class Explore : SubState
        {
            private float m_elapsedTime;
            public Explore(Hero hero) : base(hero) { }

            public override void DoState(float deltaSeconds)
            {
                m_elapsedTime += deltaSeconds;

                if(m_elapsedTime >= 3600) {

                }
            }

            public override void EnterState() { }

            public override void ExitState() { }
        }

        /*^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/
        #endregion ===================================== End Subclasses ================================================
        /*^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/

        private readonly Dungeon m_dungeon;

        public Adventure(IDungeonConfig dungeonConfig)
        {
            m_dungeon = new Dungeon(dungeonConfig);
        }

        protected override State CreateState_Internal(Hero hero)
        {
            return new State(this, Sprite, hero, m_dungeon);
        }

        protected override Sprite GetSprite()
        {
            return SpriteLibrary.GetSprite("karate");
        }

        public override float GetPriority(Hero hero)
        {
            return hero.Brain.BasePriorities.Combat * hero.Stamina.EnergyPercent * hero.Health.Percent;
        }
    }




    public class Dungeon
    {
        public DungeonFloor this[int depth] => m_floors[depth];

        private readonly DungeonFloor[] m_floors;

        public readonly int depth;

        public IDungeonConfig Config { get; }

        private float m_sumOfMonsterDensities;

        public Dungeon(IDungeonConfig config)
        {
            Config = config;

            depth = config.Depth;

            m_floors = new DungeonFloor[depth];

            InitFloors(depth);

            foreach(MonsterDensity monsterDensity in Config.MonsterDensities) {
                m_sumOfMonsterDensities += monsterDensity.Density;
            }
        }

        private void InitFloors(int depth)
        {
            for (int i = 0; i < m_floors.Length; i++) {
                float t = (float)i / depth;

                int level = Mathf.RoundToInt(Mathf.Lerp(Config.LevelRange.Min, Config.LevelRange.Max, t));

                m_floors[i] = new DungeonFloor(i, level, Config);
            }
        }

        public Monster GetMonster(int level)
        {
            int index = WeightedRandom.Naive(Config.MonsterDensities, m_sumOfMonsterDensities, WorldState.Instance.Rng, x => x.Density);
            Monster monster = new Monster(Config.MonsterDensities[index].MonsterDef, level);
            return monster;
        }
    }

    public class DungeonRoom
    {
        public readonly int level;
        public readonly int index;

        public bool IsDangerous { get; set; }
        public bool IsTreasureRoom { get; set; }

        public DungeonRoom(int index, int level)
        {
            this.index = index;
            this.level = level;
        }
    }

    public class DungeonFloor
    {
        public DungeonRoom this[int index] => m_rooms[index];

        private readonly DungeonRoom[] m_rooms = new DungeonRoom[3];
        public readonly int depth;

        public DungeonFloor(int depth, int level, IDungeonConfig config)
        {
            this.depth = depth;
            for (int i = 0; i < m_rooms.Length; i++) {
                m_rooms[i] = new DungeonRoom(i, level);
                m_rooms[i].IsDangerous = WorldState.Instance.Rng.Bool(config.ChanceOfRoomDanger);
                m_rooms[i].IsTreasureRoom = WorldState.Instance.Rng.Bool(config.ChanceOfRoomTreasure);
            }
        }
    }

    public interface IDungeonConfig
    {
        public int Depth { get; }
        public IntRange LevelRange { get; }
        public float ChanceOfRoomTreasure { get; }
        public float ChanceOfRoomDanger { get; }
        public IReadOnlyList<MonsterDensity> MonsterDensities { get; }
    }

    public class MonsterDensity
    {
        public MonsterDef MonsterDef { get; }
        public float Density { get; }
    }
}
