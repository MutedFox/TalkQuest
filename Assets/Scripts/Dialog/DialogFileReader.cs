using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;
using System.Xml;

public class DialogFileReader : MonoBehaviour
{
    /**
     * rough overview:
     * look at each node
     * for each node make a DialogNode
     * for each node, look at its out edges, and make a DialogEdge
     * for each edge add dialoglines/conditions etc. that's simple
     * 
     * some nodes/edges will not have their destinations/out edges already existing when they are first made
     * some approaches: 'depth first' i.e. if it doesn't exist, make it right away (would have highest memory usage (keeping most of the tree in memory, etc.) but relatively efficient time-wise)
     *                  start from leaf nodes and work up (but then the leaf nodes need to be labeled, which is fine, but going up may be awkward? maybe not though)
     *                  do all nodes, then fill in edges later (or vice versa)
     *                  'breadth first'/naive: go through each node, add whatever edges possible (same for edges), and go back and fill in when new edges/nodes are made. probably least efficient.
     */
    public DialogNode GenerateGraph(string filename)
    {
        Dictionary<int, DialogNode> dialog_nodes = new Dictionary<int, DialogNode>();
        List<DialogEdge> dialog_edges = new List<DialogEdge>();

        // Keep the terminal_node outside of the list of nodes for simpler indexing
        DialogNode terminal_node = new DialogNode(-1);

        XmlDocument root_xml = new XmlDocument();
        root_xml.Load(filename);

        // Used to track which DialogNodes have been created already
        List<int> node_ids = new List<int>();

        // go through each edge and create all the necessary nodes
        foreach (XmlNode xml_node in root_xml.SelectSingleNode("graph").ChildNodes)
        {
            int start_id = -1;
            Int32.TryParse(xml_node.Attributes["start_id"].Value, out start_id);
            if (!node_ids.Contains(start_id))
            {
                dialog_nodes.Add(start_id, new DialogNode(start_id));
                node_ids.Add(start_id);
            }
        }
        // for each edge, make a DialogEdge
        foreach (XmlNode xml_edge in root_xml.SelectSingleNode("graph").ChildNodes)
        {
            int start_id = -1, dest_id = -1;
            Int32.TryParse(xml_edge.Attributes["start_id"].Value, out start_id);
            Int32.TryParse(xml_edge.Attributes["dest_id"].Value, out dest_id);

            DialogEdge dialog_edge;

            // If an edge's destination is -1, it represents the end of the conversation
            if (dest_id == -1)
                dialog_edge = new DialogEdge(terminal_node);
            else
                dialog_edge = new DialogEdge(dialog_nodes[dest_id]);

            // Add dialog line to this edge 
            DialogLine dialog_line = new DialogLine(xml_edge.SelectSingleNode("line_body").InnerXml, xml_edge.SelectSingleNode("speaker_name").InnerXml, xml_edge.SelectSingleNode("portrait").InnerXml);
            dialog_edge.dialog_line_ = dialog_line;

            // If the speaker for this edge is [CHOICE], set the node it starts from to be a choice node
            if(xml_edge.SelectSingleNode("speaker_name").InnerXml == "[CHOICE]") {
                dialog_nodes[start_id].is_choice_ = true;
            }

            // Add any conditions (flags) required to traverse this edge
            string flags_required = xml_edge.SelectSingleNode("flags_req").InnerXml;
            string flag_values_required = xml_edge.SelectSingleNode("flag_values_req").InnerXml;
            dialog_edge = SetDialogEdgeFlags(flags_required, flag_values_required, dialog_edge, true);

            // Add the flags that traversing this edge should set to the edge
            string flags_to_set = xml_edge.SelectSingleNode("flags_set").InnerXml;
            string flag_values_to_set = xml_edge.SelectSingleNode("flag_values_set").InnerXml;
            dialog_edge = SetDialogEdgeFlags(flags_to_set, flag_values_to_set, dialog_edge, false);


            // Add script (names) to the edge in a similar manner to the dialog conditions
            if (xml_edge.SelectSingleNode("script").InnerXml != "")
            {
                var scripts = xml_edge.SelectSingleNode("script").InnerXml.Split(';');
                foreach (string script in scripts)
                {
                    dialog_edge.AddScript(script);
                }
            }

            // Set the edge's priority compared to other edges from the same node
            int priority = 0;
            int.TryParse(xml_edge.SelectSingleNode("priority").InnerXml, out priority);
            dialog_edge.priority_ = priority;

            // Store this edge
            dialog_edges.Add(dialog_edge);
            dialog_nodes[start_id].AddOutgoingEdge(dialog_edge);

        }

        return dialog_nodes[0];
    }

    // Update the flags that a dialog edge is required to be set to be traversed (if is_condition is true) 
    // or that traversing the edge will set (if is_condition is false)
    private DialogEdge SetDialogEdgeFlags(string flags, string flag_values, DialogEdge dialog_edge, bool is_condition)
    {
        if (flags != "")
        {
            // Extract each individual condition from what may be a list of conditions
            var flag_list = flags.Split(';').Zip(flag_values.Split(';'), (n, v) => new { name = n, value = v });
            foreach (var flag in flag_list)
            {
                if (is_condition)
                {
                    dialog_edge.AddRequiredFlag(flag.name, flag.value);
                }
                else
                {
                    dialog_edge.AddFlagToSet(flag.name, flag.value);
                }
            }
        }
        return dialog_edge;
    }
}
