using System;
using System.Collections.Generic;
using System.Text;

namespace SignalRChatClient
{
    interface INode
    {
        string Id {get; set;}//the unique id of node
        string Content { get; set; }//the value of the node/data
        bool IsReady { get; set; }//is the tree ready
        int WhiteSpace { get; set; }//amount of intial white space

        List<Node> Children { get; set; }//list of children nodes

        Node Parent { get; set; }//parent object
        int Depth { get; set; }//depth in tree

        string Get(string Id, bool shouldGetBranch);
        bool AddNode(Node toAdd, string parentId); //add node to specific parent
        bool DeleteNode(string objectId);//remove a node from tree
        bool MoveNode(string objectId, string parentId); // move all children/"the branch" as needed
        List<Node> FindNode(Node n); // returns list of matches,find by object value 
        Node FindNode(string id);//returns a single node,find by unique id
    }
}
