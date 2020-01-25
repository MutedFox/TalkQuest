Dialog overview
 
Write a 'script' for an NPC in excel
Convert it to XML using the proper schema
Create NPC with DialogGraph component, give it the filename of that XML file
To actually talk to that NPC it needs a DialogStarter component and some way to initiate the conversation (e.g. ContextActionCcontroller)

Under the GameManager should be a child object with a DialogFileReader component
In DialogGraph in Awake(), it will provide the XML filename to the DialogFileReader
The XML is then converted into a series of DialogNodes and DialogEdges (which contain DialogLines)
The root node of the resulting graph structure is given back to the DialogGraph

When a conversation is started (e.g. by SimpleDialogStarter), it creates a DialogGraphEvaluator object
That object lasts the duration of the conversation and is given the DialogGraph of the NPC talking
The DialogGraphEvaluator controls where in the conversation graph the player is in, accepting input to advance/choose nodes
(The Evaluator has a higher input priority than regular player movement etc. so they are 'locked in' to the dialog)
At the same time as the DialogGraphEvaluator is created, a DialogBox object is also made as the UI panel that actually displays dialog
The Evaluator provides the DialogBox the relevant information for it to display
When the Evaluator reaches a choice node, it creates a DialogChoiceBox (also a UI panel) for the player to select their choice with
(note the input is still handled by the evaluator itself)
When the choice is made, the ChoiceBox is removed again
This continues until a terminal node is reached, at which point:
    - DialogGraphEvaluator object is deleted
    - DialogBox (and DialogChoiceBox if it still existed) are deleted
    - Player should regain normal control (though this depends on context)

