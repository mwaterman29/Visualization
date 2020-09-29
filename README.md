# VisualizationProject
Visualization Project for Data Structures / Algorithms

Check it out on github pages [Here](https://mwaterman29.github.io/Visualization/Public%20Builds/WebGL_Build1/index.html)
Note that this might be laggy on chrome, but better on edge.

Download a build for PC/Mac/Linux [Here](https://downgit.github.io/#/home?url=https://github.com/mwaterman29/Visualization/tree/master/Public%20Builds/Windows_Build1)

# CONTROLS 
The controls are as following:
WASD Movement

Q / LeftShift to move down

E / Space to move up

Pressing V will turn gravity on, and will convert the spacebar into a jump button. You can toggle back and forth between "noclip" and physics enabled modes.

By pressing the \[ and \] keys, you can switch between the selected algorithms. \[ moves back, and \] moves forward. The implemented algorithms are: Branch Sums (Binary Tree), Binary Search (Binary Tree), Linear Search (Linked List), and Shift Linked List (Linked List).

Similarly, the ; and ' keys work to change the iteration mode. Algorithms will iterate, and you can select how. You can use Spacebar mode to use the spacebar between iterations. Or, you can use one of three wait modes to wait either 1, 3, or 10 seconds between steps. Lastly, there is the "instantaneous" iteration mode, with which the entire algorithm will procede as fast as possible, not waiting more than a frame between steps, and disregarding user input.

You can press Z to create a node. This node can be one of the three implemented types: A generic node, a Linked List node, or a Binary Tree node.

Once you have nodes, you can do the following:

Pressing O will organize nodes into structures. Generic Nodes will not be organized, as they have no set form. 

While looking at a node, you can:

-Press F or Middle Mouse to pick it up.

-Press X or Left-Click to edit the node value

-Press R or Right-Click to connect nodes.

-Press P to start an algorithm with this node


While holding a node, you can use the mouse wheel to move it away from you, and the same keys (F / Pick Up) to drop. You can also press Backspace or Delete to delete the node.

While editing a node, you can press X or Escape to abort editing. You can use Enter or Left-Click to submit the edit and change the node value. Escape might make the program lose focus, depending on platform. 

While looking at a node connection, you can click R to delete the connection.
