using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatus : MonoBehaviour, IConditionHandler
{
    // actual player stats could go here (e.g. strength/hp/whatever relevant for game)

    // inventory could go here?

    // Stores all data to do with the player that isn't an obvious 'stat' e.g. have they talked to NPC X yet or got to plot point Y
    // The value is an int as in some cases a 'quantity' may need to be measured
    // TODO: populate player_flags_, probably from an xml or csv file
    Dictionary<string, int> player_flags_;

    void Start()
    {
        player_flags_ = new Dictionary<string, int>();
        player_flags_.Add("DarfProgress", 0);
        player_flags_.Add("FlowerStrike", 0);
        player_flags_.Add("DarfBetrayal", 0);
        player_flags_.Add("FlowersConvinced", 0);
        player_flags_.Add("FloralDiplomacy", 0);
    }

    public bool EvaluateFlag(string name, string value)
    {
        // If value can be converted to an integer, return true if the player meets or surpasses the criteria.
        // TODO(potentially): handle cases where the value should actually be a string or maybe a float or something else
        // TODO: cases where the val is a maximum not to be surpassed
        int num_val;
        if (int.TryParse(value, out num_val))
            return player_flags_[name] >= num_val;
        else return false;
    }

    // update a specified flag TODO: account for e.g. string flag values?
    public void UpdateFlag(string name, string value)
    {
        int num_val;
        if (int.TryParse(value, out num_val)) {
            player_flags_[name] = num_val;
        }

    }

    private void PopulateFlags(string filename)
    {
        
    }
}