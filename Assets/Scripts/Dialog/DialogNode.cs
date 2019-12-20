using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogNode
{
    // unique id
    private int id_;

    // The possible transitions from this node
    private List<DialogEdge> out_edges_;

    // When true, this represents a point in the dialog where the player must make a choice (false by default)
    public bool is_choice_ { get; set; }

    public DialogNode(int id)
    {
        id_ = id;
        out_edges_ = new List<DialogEdge>();
        is_choice_ = false;
    }

    public void AddOutgoingEdge(DialogEdge edge)
    {
        out_edges_.Add(edge);
    }

    // Removes all outgoing edges from this node (used to compress the dialog graph)
    public void RemoveEdges()
    {
        out_edges_ = new List<DialogEdge>();
    }

    public List<DialogEdge> GetOutgoingEdges()
    {
        return out_edges_;
    }

    // Nodes with an id of -1 are terminal nodes indicating the end of a conversation (they have no outgoing edges)
    public bool IsTerminal()
    {
        return id_ == -1;
    }
}
