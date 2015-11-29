using UnityEngine;
using System.Collections;

public class UnitUtilities : MonoBehaviour
{

    public static BattleResult ResolveBattle(UnitRank attacker, UnitRank defender)
    {
        //Assume attacker fails
        BattleResult result = BattleResult.Fail;

        if (defender == UnitRank.Flag)
        {
            //Handle flag
            result = BattleResult.Success;
        }
        else if (attacker == defender)
        {
            //Handle split, same rank
            result = BattleResult.Split;
        }
        else if (attacker == UnitRank.Spy || defender == UnitRank.Spy)
        {
            if (attacker == UnitRank.Spy && defender != UnitRank.Private)
            {
                //Handle attacker spy logic
                result = BattleResult.Success;
            }
            else if (defender == UnitRank.Spy && attacker == UnitRank.Private)
            {
                result = BattleResult.Success;
            }
        }
        else if (attacker > defender)
        {
            //Handle rank compare
            result = BattleResult.Success;
        }

        return result;
    }

    public static string ReadableRank(UnitRank rank)
    {
        string readable = "Unknown";

        switch(rank)
        {
            case UnitRank.BrigadierGeneral:
                {
                    readable = "Brigadier General";
                    break;
                }
            case UnitRank.FirstLieutenant:
                {
                    readable = "First Lieutenant";
                    break;
                }
            case UnitRank.GeneralOfTheArmy:
                {
                    readable = "General Of The Army";
                    break;
                }
            case UnitRank.LieutenantColonel:
                {
                    readable = "Lieutenant Colonel";
                    break;
                }
            case UnitRank.LieutenantGeneral:
                {
                    readable = "Lieutenant General";
                    break;
                }
            case UnitRank.MajorGeneral:
                {
                    readable = "Major General";
                    break;
                }
            case UnitRank.SecondLieutenant:
                {
                    readable = "Second Lieutenant";
                    break;
                }
            default:
                {
                    readable = rank.ToString();
                    break;
                }
        }

        return readable;
    }
}
