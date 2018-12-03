using System;
using System.Collections.Generic;
using System.Text;

namespace SignalRChatClient
{
    class Node : INode
    {
        #region Properties 
        //unique id
        public string Id
        {
            get => _Id;

            set
            {
                if (value != null)
                {
                    _Id = value;
                }
                else
                {
                    //generate guid id no preset id is specified
                    _Id = Guid.NewGuid().ToString();
                }
            }
        }

        private string _Id;

        //a nodes value
        public string Content
        {
            get => _Content;
            set { _Content = value; }
        }

        private string _Content;

        //set if the tree is ready
        public bool IsReady
        {
            get;
            set;
        }
        //list of children nodes
        public List<Node> Children
        {
            get { return _children; }
            set { _children = value; }
        }
        private List<Node> _children = new List<Node>();

        //parent node reference
        public Node Parent
        {
            get { return _parent; }
            set { _parent = value; }
        }
        private Node _parent;

        //node depth on the tree
        public int Depth
        {
            get => _depth;
            set
            {
                _depth = value;
            }
        }
        private int _depth;

        //amount of intial white space
        public int WhiteSpace
        {
            get => _wspace;
            set { _wspace = value; }
        }
        private int _wspace;
        #endregion

        //constructor for node, sets whitespace, unique id, and node value
        public Node(int _whitespace, string _id, string _content)
        {
            WhiteSpace = _whitespace;
            Id = _id;
            Content = _content;
        }

        //add a node to a set parent
        public bool AddNode(Node toAdd, string parentId)
        {
            Node parent = FindNode(parentId);

            //return whether this was successful or not
            return parent.AddNode(toAdd);
        }

        //add a node in the proper place in the tree
        public bool AddNode(Node toAdd)
        {
            //calculate depth of node
            toAdd.Depth = toAdd.WhiteSpace / 1;
            //If the depth is less than node comparing to then compare to that node's parent
            if (toAdd.Depth < this.Depth)
            {
                this.Parent.AddNode(toAdd);
            }
            //If the depth is equal to the comparing node, then add to that node's parent
            else if (toAdd.Depth == this.Depth)
            {
                this.Parent.AddNode(toAdd);
            }
            //if depth is greater than comparing node, then add to comparing node as child
            else
            {
                toAdd.Parent = this;
                this.Children.Add(toAdd);
            }

            return true;
        }

        //delete a node
        public bool DeleteNode(string objectId)
        {
            //finds the node to be deleted
            Node toBeDeleted = FindNode(objectId);

            //gets the parent of that node
            Node parent = toBeDeleted.Parent;

            //remove the child from the parent
            return parent.Children.Remove(toBeDeleted);
        }

        //Find node by object
        public List<Node> FindNode(Node n)
        {
            //creates a list of duplicates
            List<Node> matches = new List<Node>();
            //finds all nodes with matching values
            if (Children.Count > 0)
            {
                foreach (var child in Children)
                {
                    if (!child.Content.Equals(n.Content))
                    {
                        matches.AddRange(child.FindNode(n));
                    }
                    else
                    {
                        matches.Add(child);
                    }
                }
            }
            //return list
            return matches;
        }
        
        //find node by unique id
        public Node FindNode(string id)
        {
            Node parent = this;

            //go through whole tree to find specific node
            if (Children.Count > 0)
            {
                foreach (var child in Children)
                {
                    if (!child.Id.Equals(id))
                    {
                        //if it does not equal then check that node and it's children
                        parent = child.FindNode(id);
                        
                    }
                    else
                    {
                        //store equivalent node in variable in the tree class
                        parent = child;
                        //return that node
                        return parent;
                    }
                }
            }
            //checks node, even if it has no children
            else
            {
                if (this.Id.Equals(id))
                {
                    //returning root node
                    parent = this;
                    return parent;
                }

            }

            //return node value
            return parent;
        }

        //get node by itself or with branch
        public string Get(string id, bool shouldGetBranch)
        {
            //string that will be returned
            string retrievedNodes = "";
            //if branch was requested
            if (shouldGetBranch)
            {
                //finds the node based on unique id
                Node found = FindNode(id);
                //add a new line
                retrievedNodes += "\n";
                //add tabs based on depth
                for(int i =0; i<found.Depth;i++)
                {
                    retrievedNodes += " ";
                }
                //add node value
                retrievedNodes += (found.Content);

                //if there are no children, just return the node value
                if (found.Children.Count <= 0)
                    return retrievedNodes;


                //run through all children in branch
                foreach(Node n in found.Children)
                {
                    retrievedNodes += found.Get(n.Id, shouldGetBranch);
                }
            }
            //if branch was not requested
            else
            {
                //string equals node value
                retrievedNodes += "\n"+FindNode(id).Content;
            }
            //return string
            return retrievedNodes;
        }

        //move node in tree
        public bool MoveNode(string objectId, string parentId)
        {
            //node that is going to be move in tree
            Node toBeMoved = FindNode(objectId);
            //the node that the other node is going to be moved to
            Node whereToBeMoved = FindNode(parentId);

            //make sure nodes aren't null
            if (toBeMoved == null || whereToBeMoved == null)
            {
                return false;
            }

            //remove moving node from current parent
            toBeMoved.Parent.Children.Remove(toBeMoved);
            //reset parent to new parent object
            toBeMoved.Parent = whereToBeMoved;
            //move the node to be a child of new parent
            whereToBeMoved.Children.Add(toBeMoved);
            //adjust the depth to the new location in the tree
            IncreaseDepth(toBeMoved, whereToBeMoved);

            return true;
        }

        //set the depth to be 1+ the parent's depth
        private void IncreaseDepth(Node increasing,Node constant)
        {
            increasing.Depth = constant.Depth + 1;
            foreach(Node n in increasing.Children)
            {
                IncreaseDepth(n, n.Parent);
            }
        }
    }
}
