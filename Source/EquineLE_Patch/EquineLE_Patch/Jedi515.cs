using System;
using RimWorld;
using Verse;
using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using GeneticRim;

namespace Jedi515
{
    [DefOf]
    public static class Jedi515_Defs
    {
        public static HediffDef Jedi515_HorsePP;

        static Jedi515_Defs()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(Jedi515_Defs));
        }
    }
    [HarmonyPatch(typeof(JobDriver_Lovin), "<MakeNewToils>b__11_4")]
    [StaticConstructorOnStartup]
    public static class EquineLE_Patch
    {
        static EquineLE_Patch()
        {
            var harmony = new Harmony(id: "Jedi515");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }

        public static Thought_Memory Insert(Thought_Memory thought_Memory, JobDriver_LayDown driver)
        {
            //TODO:
            float multiplier = 0f;
            foreach (Hediff h in driver.pawn.health.hediffSet.hediffs)
            {
                if (h.def == Jedi515_Defs.Jedi515_HorsePP)
                {

                    switch (h.Severity)
                    {
                        case 0:
                            multiplier += 0.15f;
                            break;
                        case 0.1f:
                            multiplier += 0.25f;
                            break;
                        case 0.2f:
                            multiplier += 0.5f;
                            break;
                        case 0.3f:
                            multiplier += 0.65f;
                            break;
                        case 0.4f:
                            multiplier += 0.8f;
                            break;
                        case 0.5f:
                            multiplier += 1f;
                            break;
                        case 0.6f:
                            multiplier += 1.5f;
                            break;

                    }
                    //h.parent
                }
            }
            thought_Memory.moodPowerFactor += multiplier;
            return thought_Memory;
        }

        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var code = new List<CodeInstruction>(instructions);
            var ogMemoryInsert = AccessTools.Field(typeof(ThoughtHandler), "memories");
            var instructionsToInsert = new List<CodeInstruction>();

            instructionsToInsert.Add(new CodeInstruction(OpCodes.Ldarg_0));
            instructionsToInsert.Add(new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(EquineLE_Patch), "Insert", new Type[] { typeof(Thought_Memory), typeof(JobDriver_LayDown) })));

            int insertIndex = -1;

            for (int i = 0; i < code.Count - 1; i++)
            {
                if (code[i].OperandIs(ogMemoryInsert))
                {
                    insertIndex = i+2;
                    break;
                }
            }
            if (insertIndex != -1)
            {
                code.InsertRange(insertIndex, instructionsToInsert);
            }
            return code;
        }
    }
}
