using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public enum NodeType { Invalid = -1, Shields = 0, Weapons = 1, General = 2, Repair = 3, Science = 4, Scanning = 5 }
public enum BonusAmount { SmallBonus, MediumBonus, LargeBonus }
public enum MenuType { Invalid = -1, UpgradeScreen = 1, BattleScreen = 2 }

[System.Serializable]
public class ResourceTypeList
{
    public NodeType nodeType;
    public int amount;

    public ResourceTypeList(NodeType nodeType, int amount)
    {
        this.nodeType = nodeType;
        this.amount = amount;
    }
}

[System.Serializable]
public class EquipmentBonusTypeList
{
    public NodeType nodeType;
    public NodeType bonusType;
    public int amount;

    public EquipmentBonusTypeList(NodeType nodeType, NodeType bonusType, int amount)
    {
        this.nodeType = nodeType;
        this.bonusType = bonusType;
        this.amount = amount;
    }
}

public class ResistModifier
{
    public int amount;
    public int turn;

    public ResistModifier()
    {
        amount = 0;
        turn = 0;
    }
}

public class ResourceList
{
    public int shield;
    public int weapon;
    public int general;
    public int science;
    public int repair;
    public int scanning;

    /*
    public void AddBonuses(ResourceMaxs bonuses)
    {
        if (weapon > 0)
        {
            shield += bonuses.weapon;
        }
        if (shield > 0)
        {
            shield += bonuses.shield;
        }
        if (general > 0)
        {
            shield += bonuses.general;
        }
        if (science > 0)
        {
            shield += bonuses.science;
        }
        if (repair > 0)
        {
            shield += bonuses.repair;
        }
        if (scanning > 0)
        {
            shield += bonuses.scanning;
        }
    }
    */

}

[System.Serializable]
public struct ResourceMultipliers
{
    public float commonMultiplier;
    public float weaponMultiplier;
    public float shieldMultiplier;
    public float repairMultiplier;
    public float scanningMultiplier;
    public float scienceMultiplier;
    public float generalMultiplier;
}

public class Turn
{
    int turnNumber;
    bool playerTurn;

    public delegate void TurnEnded(bool playerTurn);
    public static event TurnEnded EndturnEvent;

    private static Turn instance;

    public Turn()
    {
        turnNumber = 1;
        playerTurn = true;
        instance = this;
    }

    public Turn(bool playerTurn)
    {
        turnNumber = 1;
        this.playerTurn = playerTurn;
        instance = this;
    }

    public static void EndTurn()
    {
        if (instance.playerTurn != true)
        {
            instance.turnNumber++;
        }
        instance.playerTurn = !instance.playerTurn;
        if (EndturnEvent != null)
        {
            EndturnEvent(instance.playerTurn);
        }
    }

    public static int CurrentTurn()
    {
        return instance.turnNumber;
    }

}

public enum BonusType { none, Additive, Multiplier}

[System.Serializable]
public struct Bonus
{
    public BonusType bonusType;
    public int amount;
}

public class Helper
{

    public static Color GetNodeColor(NodeType nodeType)
    {
        Color rw = Color.white;
        switch (nodeType)
        {
            case NodeType.Weapons:
                rw = Color.red;
                break;
            case NodeType.Shields:
                rw = Color.blue;
                break;
            case NodeType.Scanning:
                rw = new Color(255f/255f,93f/255f,0);
                break;
            case NodeType.Repair:
                rw = Color.green;
                break;
            case NodeType.Science:
                rw = new Color(255f/255f,2f/255,233f/255);
                break;
            case NodeType.General:
                rw = Color.yellow;
                break;
        }
        return rw;
    }
    /*
    public static StatDataObject ConstructStats()
    {
        StatDataObject statObject = (Resources.Load("StatDataObject") as StatDataObject);
        foreach (StatData statData in statObject.stats)
        {
            statData.currentUpgradeValue = SaveManager.GetStat(statData.index);
        }
        return statObject;
    }

    public static ResourceMaxs CunstructBonusStarts()
    {
        ResourceMaxs statBonuses = new ResourceMaxs();

        StatDataObject statObject = ConstructStats();
        foreach (StatData statData in statObject.stats)
        {
            switch (statData.index)
            {
                case 4:
                    statBonuses.weapon = statData.GetCurrentValue();
                    break;
                case 6:
                    statBonuses.shield = statData.GetCurrentValue();
                    break;
                case 8:
                    statBonuses.scanning = statData.GetCurrentValue();
                    break;
                case 10:
                    statBonuses.repair = statData.GetCurrentValue();
                    break;
                case 12:
                    statBonuses.science = statData.GetCurrentValue();
                    break;
                case 14:
                    statBonuses.general = statData.GetCurrentValue();
                    break;
            }
        }

        return statBonuses;
    }

    public static List<ResourceTypeList> ConstructBonusMatches()
    {
        List<ResourceTypeList> bonusList = new List<ResourceTypeList>();
        StatDataObject statObject = ConstructStats();
        foreach (StatData statData in statObject.stats)
        {
            switch (statData.index)
            {
                case 3:
                    bonusList.Add(new ResourceTypeList(NodeType.Weapons, statData.GetCurrentValue()));
                    break;
                case 5:
                    bonusList.Add(new ResourceTypeList(NodeType.Shields, statData.GetCurrentValue()));
                    break;
                case 7:
                    bonusList.Add(new ResourceTypeList(NodeType.Science, statData.GetCurrentValue()));
                    break;
                case 9:
                    bonusList.Add(new ResourceTypeList(NodeType.Repair, statData.GetCurrentValue()));
                    break;
                case 11:
                    bonusList.Add(new ResourceTypeList(NodeType.Science, statData.GetCurrentValue()));
                    break;
                case 13:
                    bonusList.Add(new ResourceTypeList(NodeType.General, statData.GetCurrentValue()));
                    break;
            }
        }
        return bonusList;
    }

    public static string GetBonusTypeString(NodeType nodeType)
    {
        string text = "";
        switch (nodeType)
        {
            case NodeType.Weapons:
                text = "Weapon";
                break;
            case NodeType.Shields:
                text = "Shield";
                break;
            case NodeType.Scanning:
                text = "Scanning";
                break;
            case NodeType.Repair:
                text = "Repair";
                break;
            case NodeType.Science:
                text = "Science";
                break;
            case NodeType.General:
                text = "General";
                break;
        }
        return text;
    }

    public static int ApplyBonus(Bonus bonus)
    {
        int rw = 0;
        switch (bonus.bonusType)
        {
            case BonusType.Additive:
                break;
            case BonusType.Multiplier:
                break;
        }
        return rw;
    }

    public static Target ConfigureTarget(Owner owner, Targeting preferredTarget)
    {
        var target = Target.Invalid;
        switch (owner)
        {
            case Owner.Player:
                switch (preferredTarget)
                {
                    case Targeting.Self:
                        target = Target.Player;
                        break;
                    case Targeting.Opponent:
                        target = Target.Opponent;
                        break;
                    default:
                        Debug.LogWarning(preferredTarget.ToString());
                        break;
                }
                break;
            case Owner.Opponent:
                switch (preferredTarget)
                {
                    case Targeting.Self:
                        target = Target.Opponent;
                        break;
                    case Targeting.Opponent:
                        target = Target.Player;
                        break;
                }
                break;
        }
        if (target == Target.Invalid)
        {
            Debug.LogError("Targeting Failed");
            return Target.Invalid;
        }
        return target;
    }
    */


    public static void SetButtonText(Button button, string text)
    {
        SetButtonText(button.gameObject, text);
    }

    public static void SetButtonText(GameObject button, string text)
    {
        button.GetComponentInChildren<Text>().text = text;
    }

}


