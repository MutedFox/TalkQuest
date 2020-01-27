using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContextActionController : MonoBehaviour
{
    // The script attached to this game object that will be run when the player presses the interact button when in range of this object
    private IContextInteractable context_action_component_;

    // Set to true when the player is in range to interact with this object
    // TODO: multiple in range objects
    private bool within_context_radius_ = false;

    // This input retriever is set to reference the player's input retriever when they are in range of the object
    private InputRetriever input_;

    // Start is called before the first frame update
    void Start()
    {
        context_action_component_ = GetComponent<IContextInteractable>();
    }

    // Update is called once per frame
    void Update()
    {
        if (within_context_radius_ && input_.GetButtonDown("Jump"))
            context_action_component_.ContextInteraction();
    }

    // When in range, enable the context interaction
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            within_context_radius_ = true;
            input_ = collision.GetComponent<InputRetriever>();
        }
    }

    // When moving out of range, disable the context interaction
    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            within_context_radius_ = false;
            input_ = null;
        }
    }
}
