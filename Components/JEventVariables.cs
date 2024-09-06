using GameNetcodeStuff;
using JLL.API.Events;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace JLL.Components
{
    public class JEventVariables : MonoBehaviour
    {
        private int targetIndex = 0;
        [Header("Number Variables (\"number\")")]
        public List<JEventVariable<float>> numberVariables = new List<JEventVariable<float>>();

        [Header("Boolean Variables (\"bool\")")]
        public List<JEventVariable<bool>> booleanVariables = new List<JEventVariable<bool>>();

        [Header("GameObjects")]
        public bool allowDuplicates = true;

        [Header("Player Variables (\"player\")")]
        public List<JEventVariable<PlayerControllerB>> playerVariables = new List<JEventVariable<PlayerControllerB>>();
        public InteractEvent playerEvent = new InteractEvent();

        [Header("Enemy Variables (\"enemy\")")]
        public List<JEventVariable<EnemyAI>> enemyVariables = new List<JEventVariable<EnemyAI>>();
        public EnemyEvent enemyEvent = new EnemyEvent();

        [Header("GameObject Variables (\"object\")")]
        public List<JEventVariable<GameObject>> gameObjectVariables = new List<JEventVariable<GameObject>>();
        public ObjectEvent gameObjectEvent = new ObjectEvent();

        [Serializable]
        public class JEventVariable<T>
        {
            public T Variable;
            public UnityEvent<T> Event = new UnityEvent<T>();

            public void Trigger()
            {
                if (Variable == null)
                {
                    return;
                }
                Event.Invoke(Variable);
            }

            public void Set(T var)
            {
                Variable = var;
            }
        }

        public enum VariableType
        {
            Unknown = -1,

            Number,
            Boolean,
            GameObject,
            Player,
            Enemy,
        }

        private int GetIndex(VariableType id)
        {
            int count;
            switch(id)
            {
                case VariableType.Number:
                    count = numberVariables.Count; break;
                case VariableType.Boolean:
                    count = booleanVariables.Count; break;
                case VariableType.GameObject:
                    count = gameObjectVariables.Count; break;
                case VariableType.Player:
                    count = playerVariables.Count; break;
                case VariableType.Enemy:
                    count = enemyVariables.Count; break;
                default:
                    return -1;
            }

            if (count == 0 || targetIndex >= count || targetIndex < 0) return -1;
            return targetIndex;
        }

        private VariableType GetType(string identifier)
        {
            return identifier.ToLower().Replace("\"", "") switch
            {
                "number" => VariableType.Number,
                "bool" => VariableType.Boolean,
                "object" => VariableType.GameObject,
                "player" => VariableType.Player,
                "enemy" => VariableType.Enemy,
                _ => VariableType.Unknown,
            };
        }

        public void TargetIndex(int index)
        {
            targetIndex = index;
        }

        public void Trigger(string varType)
        {
            VariableType type = GetType(varType);
            int i = GetIndex(type);
            if (i >= 0)
            {
                switch (type)
                {
                    case VariableType.Number:
                        numberVariables[i].Trigger();
                        break;
                    case VariableType.Boolean:
                        booleanVariables[i].Trigger();
                        break;
                    case VariableType.GameObject:
                        gameObjectVariables[i].Trigger();
                        break;
                    case VariableType.Player:
                        playerVariables[i].Trigger();
                        break;
                    case VariableType.Enemy:
                        enemyVariables[i].Trigger();
                        break;
                }
            }
        }

        public void TriggerAll(string varType)
        {
            VariableType type = GetType(varType);
            if (type != VariableType.Unknown)
            {
                switch (type)
                {
                    case VariableType.Number:
                        for (int i = 0; i < numberVariables.Count; i++)
                        {
                            numberVariables[i].Trigger();
                        }
                        break;
                    case VariableType.Boolean:
                        for (int i = 0; i < booleanVariables.Count; i++)
                        {
                            booleanVariables[i].Trigger();
                        }
                        break;
                    case VariableType.GameObject:
                        for (int i = 0; i < gameObjectVariables.Count; i++)
                        {
                            gameObjectVariables[i].Trigger();
                            if (gameObjectVariables[i].Variable != null)
                            {
                                gameObjectEvent.Invoke(gameObjectVariables[i].Variable);
                            }
                        }
                        break;
                    case VariableType.Player:
                        for (int i = 0; i < playerVariables.Count; i++)
                        {
                            playerVariables[i].Trigger();
                            if (playerVariables[i].Variable != null)
                            {
                                playerEvent.Invoke(playerVariables[i].Variable);
                            }
                        }
                        break;
                    case VariableType.Enemy:
                        for (int i = 0; i < enemyVariables.Count; i++)
                        {
                            enemyVariables[i].Trigger();
                            if (enemyVariables[i].Variable != null)
                            {
                                enemyEvent.Invoke(enemyVariables[i].Variable);
                            }
                        }
                        break;
                }
            }
        }

        public void Remove(string varType)
        {
            VariableType type = GetType(varType);
            int i = GetIndex(type);
            if (i >= 0)
            {
                switch (type)
                {
                    case VariableType.Number:
                        numberVariables.RemoveAt(i);
                        break;
                    case VariableType.Boolean:
                        booleanVariables.RemoveAt(i);
                        break;
                    case VariableType.GameObject:
                        gameObjectVariables.RemoveAt(i);
                        break;
                    case VariableType.Player:
                        playerVariables.RemoveAt(i);
                        break;
                    case VariableType.Enemy:
                        enemyVariables.RemoveAt(i);
                        break;
                }
            }
        }

        // Note: These need to be here in order to be selected by UnityEvents in the inspector.

        // Nums
        public void SetNumber(float num)
        {
            int i = GetIndex(VariableType.Number);
            if (i >= 0)
            {
                numberVariables[i].Set(num);
            }
        }
        public void AddNumber(float num)
        {
            int i = GetIndex(VariableType.Number);
            if (i >= 0)
            {
                numberVariables[i].Set(num + numberVariables[i].Variable);
            }
        }
        public void MultiplyNumber(float num)
        {
            int i = GetIndex(VariableType.Number);
            if (i >= 0)
            {
                numberVariables[i].Set(num * numberVariables[i].Variable);
            }
        }

        // Bools
        public void SetBoolean(bool val)
        {
            int i = GetIndex(VariableType.Boolean);
            if (i >= 0)
            {
                booleanVariables[i].Set(val);
            }
        }
        public void AndBool(bool val)
        {
            int i = GetIndex(VariableType.Boolean);
            if (i >= 0)
            {
                booleanVariables[i].Set(val && booleanVariables[i].Variable);
            }
        }
        public void OrBool(bool val)
        {
            int i = GetIndex(VariableType.Boolean);
            if (i >= 0)
            {
                booleanVariables[i].Set(val || booleanVariables[i].Variable);
            }
        }
        public void NotBool(bool val)
        {
            int i = GetIndex(VariableType.Boolean);
            if (i >= 0)
            {
                booleanVariables[i].Set(val != booleanVariables[i].Variable);
            }
        }

        // Objs
        public void SetObject(GameObject obj)
        {
            int i = GetIndex(VariableType.GameObject);
            if (i >= 0)
            {
                gameObjectVariables[i].Set(obj);
            }
        }
        public void AddObject(GameObject obj)
        {
            if (!allowDuplicates)
            {
                for (int i = 0; i < gameObjectVariables.Count; i++)
                {
                    if (gameObjectVariables[i].Variable == obj)
                    {
                        return;
                    }
                }
            }
            gameObjectVariables.Add(new JEventVariable<GameObject>() { Variable = obj } );
        }

        // Players
        public void SetPlayer(PlayerControllerB player)
        {
            int i = GetIndex(VariableType.Player);
            if (i >= 0)
            {
                playerVariables[i].Set(player);
            }
        }
        public void AddPlayer(PlayerControllerB player)
        {
            if (!allowDuplicates)
            {
                for (int i = 0; i < playerVariables.Count; i++)
                {
                    if (playerVariables[i].Variable == player)
                    {
                        return;
                    }
                }
            }
            playerVariables.Add(new JEventVariable<PlayerControllerB>() { Variable = player });
        }

        // Enemies
        public void SetEnemy(EnemyAI enemy)
        {
            int i = GetIndex(VariableType.Enemy);
            if (i >= 0)
            {
                enemyVariables[i].Set(enemy);
            }
        }
        public void AddEnemy(EnemyAI enemy)
        {
            if (!allowDuplicates)
            {
                for (int i = 0; i < enemyVariables.Count; i++)
                {
                    if (enemyVariables[i].Variable == enemy)
                    {
                        return;
                    }
                }
            }
            enemyVariables.Add(new JEventVariable<EnemyAI>() { Variable = enemy });
        }
    }
}
