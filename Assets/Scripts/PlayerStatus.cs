using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatus : MonoBehaviour, IConditionHandler
{
    // actual player stats could go here (e.g. strength/hp/whatever relevant for game)

    // inventory could go here?

    // Stores all data to do with the player that isn't an obvious 'stat' e.g. have they talked to NPC X yet or got to plot point Y
    // The value is an int as in some cases a 'quantity' may need to be measured
    Dictionary<string, int> player_flags_;

    // Name of the file where flags are stored (all set to 0/default)
    private string flag_filename_ = "initialflags";

    void Start()
    {
        player_flags_ = new Dictionary<string, int>();
        PopulateFlags(flag_filename_);
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

    // Setup the flags based on the given file.
    // This could be used for loading save games as well (for future projects)
    private void PopulateFlags(string filename)
    {
        TextAsset flag_file = (TextAsset)Resources.Load(filename, typeof(TextAsset));

        string[] lines = flag_file.text.Split('\n');

        foreach(string line in lines)
        {
            string[] split_line = line.Split(' ');
            int value;
            int.TryParse(split_line[1], out value);
            player_flags_.Add(split_line[0], value);
        }
    }
}