using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace SignalRChatClient
{
    class Tree
    {
        //empty Root Node
        public Node root;

        //Holds a reference of the most recently added Node
        private Node tempNode;

        //Holds a list of all the siblings needed to be added at the same depth
        private List<Node> ToBeAdded = new List<Node>();

        //temp node value for finding node by id in node
        public Node temp;

        private string pathWay;

        //starting function with file input
        public string Start(string path)
        {
            root = new Node(0, null, null);
            root.Depth = -1;
            pathWay = path;
            bool foundFile=LoadContent();
            if(!foundFile)
            {
                return "File not found";
            }
            return null;
        }
 
        //starting function for user input
        public void StartUser()
        {
            root = new Node(0, null, null);
            root.Depth = -1;
        }

        //load content from a file
        private bool LoadContent()
        {
            StreamReader sr;
            try
            {
                sr = new StreamReader(string.Format(@"C:\workspace\{0}", pathWay));
            }
            catch(Exception e)
            {
                sr = null;
            }
            if (sr != null)
            {
                using (sr)
                {
                    //stores each line of file
                    string line;
                    int count = 0;
                    int parentCount = 0;

                    while ((line = sr.ReadLine()) != null)
                    {
                        if (line.Length > 0)
                        {
                            for (int j = 0; j <= line.Length; j++)
                            {
                                //TO:DO parsing ID, Content

                                //this is a check for depth
                                if (line[j].Equals(' '))
                                {
                                    count++;
                                }
                                else if (line[j].Equals('\t'))
                                {
                                    count += 8; //1 tab = 8 empty spaces
                                }
                                else
                                {
                                    break; //breaks out of for loop thats checking each char
                                }
                            }

                            if (parentCount == 0) // if it's count of whitespace was 0 then add set the tempNode to the root
                            {
                                tempNode = root;
                            }

                            if (count != parentCount)
                            {
                                AddNodesToTree(ToBeAdded); //add nodes in list to the tree

                                if (root.Children[0] != null)
                                {
                                    tempNode = ToBeAdded[ToBeAdded.Count - 1]; //keep a reference of the last node added
                                }
                                ToBeAdded.Clear(); //clear the list
                            }

                            line = line.TrimStart('\t');
                            AddNodeToList(count, ToBeAdded, line); // add node to the list with its info

                            parentCount = count;
                            count = 0; //reset whitespace counter
                        }
                    }//end of While

                    AddNodesToTree(ToBeAdded);//Add last of the nodes
                                              //check for IO errors and other exceptions
                    return true;
                }//end of StreamReader
            }
            else
            {
                return false;
                
            }
        }//end of LoadContent()

        //add a node to a temporary list of nodes
        private void AddNodeToList(int count, List<Node> toBeAdded, string line)
        {
            //create new node and add it to list
            ToBeAdded.Add(new Node(count, null, line));
        }

        //add all nodes of the same depth to tree
        private void AddNodesToTree(List<Node> toBeAdded)
        {
            foreach (var sib in toBeAdded)
            {
                tempNode.AddNode(sib);
            }
        }

        //write out to a file
        public void OutputToFile(Node node)
        {
            string line = "";
            using (StreamWriter sw = new StreamWriter(@"C:\workspace\test.txt", true))
            {
                line += "\n";
                //assign the appropriate amount of tabs based on depth
                for (int i = 0; i < node.Depth; i++)
                {
                    line += "\t";
                }
                //write line out to file
                sw.WriteLine(line+node.Content);
            }
            //go through each child node until all nodes have been written out
                if (node.Children.Count > 0)
                {
                    foreach (Node n in node.Children)
                    {
                        OutputToFile(n);
                    }
                }
               
        }
    }
}

