using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalRChatClient
{
    class UpdateMenus
    {
        private enum InputState//this tells us what kind of input we're looking for. We can use it to respond to the input correctly
        {
            MainMenu,
            AddNodeGetInfo,
            MoveNodeGetValue,
            MoveNodeGetParent,
            RemoveNodeGetValue,
            GetNodeGetValue,
            GetNodeGetBranchOrNot,
        }

        private InputState myInputState = InputState.MainMenu;//should always start on the main menu
  
        private bool treeCreated = false;//this will make it so everytime the menu is called it doesn't just make a new root.
        Node nodeToMove;//this I had to make global so that MovingNode could be called more than once and get different input.
        private bool getBranch = false;//global private bool used in GettingNode();
        private string getNodeValue;//global private string used in GettingNode();

        MainWindow window;
        Tree tree;
        public UpdateMenus(MainWindow w,Tree t)
        {
            window = w;
            tree = t;
        }
        //tree object

        public string menuMessage= ("Please choose an option: \n1-Add a node\n2-Move a node\n3-Delete a node\n4-Get a node\n5-Write tree to file\n6-Exit");



        public string message;
        public string value;
        public string sMenu;
        int menu;

        //Main menu for editing tree
        public void Menu(int index)//really the key here is that we check each time to see what part of the menu our choice is for...
        {
            message = window.messagesList.Items[index].ToString();
            string[] splitStrings = message.Split('/',':');//split up our 2 strings
            if (splitStrings.Count() >= 2)
            {
                sMenu = splitStrings[1];//first one is parent
                value = splitStrings[2];//second one is value

            }
            try
            {
                menu = Convert.ToInt32(sMenu);
            }
            catch (Exception e)
            {
                return;
            }
            
            switch (menu)//this tells us what kind of input we got and directs us to the correct handler function for it.
            {
                case 1:
                    AddNodeByUser();
                    break;
                case 2:
                    MovingNode();
                    break;
                //case InputState.MoveNodeGetParent:
                //    MovingNode();
                //    window.messagesList.Items.Add("Done");
                //    break;
                case 3:
                    RemoveNode();
                    break;
                //case InputState.GetNodeGetValue:
                //    GettingNode();
                //    break;
                //case InputState.GetNodeGetBranchOrNot:
                //    window.messagesList.Items.Add("Do you want the branch with it? Y/N?");
                //    GettingNode();
                //    break;

            }

        }

        private void ResetToMainMenu()
        {
            myInputState = InputState.MainMenu;//go back to main menu input if this worked
        }

        

        void GetMenuChoice()
        {
           
            //call function based on user choice
            switch (menu)//these are going to tell the menu what function to call next. Not going to call them directly
            {
                case 1://add node               
                    myInputState = InputState.AddNodeGetInfo;
                    break;
                case 2://move node
                    myInputState = InputState.MoveNodeGetValue;
                    break;
                case 3://delete a node
                    //get the value you want to delete
                    myInputState = InputState.RemoveNodeGetValue;
                    break;
                case 4://get a node
                    myInputState = InputState.GetNodeGetValue;
                    break;
                case 5://write tree to file
                       //write out to text or the uwp part
                    tree.OutputToFile(tree.root);
                    break;
                case 6://exit
                       //keepGoing = false;
                    break;
            }

        }


      

        //get a node
        void GettingNode()
        {


           // else if (myInputState == InputState.GetNodeGetBranchOrNot)
            //{
            //    //get whether the user just wants the node or the whole branch               
            //    string getbranch = window.messageTextBox.Text;

            //    //set bool based on user input
            //    if (getbranch.Equals("Y") || getbranch.Equals("y"))
            //    {
            //        getBranch = true;
            //        myInputState = InputState.MainMenu;
            //    }
            //    else if (getbranch.Equals("N") || getbranch.Equals("n"))
            //    {
            //        getBranch = false;
            //        myInputState = InputState.MainMenu;
            //    }
            //    else
            //    {
            //        window.messagesList.Items.Add("Please enter valid option");
            //    }
            //}

            string id = FindNodes(message);

            if (id != null)
                //Display node value
                window.messagesList.Items.Add(tree.root.Get(id, true));
            else if(getNodeValue.ToLower().Equals("root"))
            {
                window.messagesList.Items.Add(tree.root.Get(tree.root.Id, true));
            }

            ResetToMainMenu();
        }

        //move a node in the tree
        void MovingNode()
        {
            string parentValue="";
            string nodeValue="";
            Node nodeToMoveTo;
            string[] splitStrings = value.Split(',');//split up our 2 strings
            if (splitStrings.Count() == 2)
            {
                parentValue = splitStrings[0];//first one is parent
                nodeValue = splitStrings[1];//second one is value


            }

            if (myInputState == InputState.MoveNodeGetValue)//if we need to get the value, then it gets value only
            {
                //find the node we want to move
                nodeToMove = tree.root.FindNode(FindNodes(nodeValue));
            }

            else if (myInputState == InputState.MoveNodeGetParent)//if it already knows value, we can skip it and get on with doing everything else
            {

                if (parentValue.Equals(tree.root.Content))
                {
                    //find the object we want to move the value to
                    nodeToMoveTo = tree.root;
                }

                else
                {
                    //find the object we want to move the value to
                    nodeToMoveTo = tree.root.FindNode(FindNodes(parentValue));
                }

                //pass the id values to the node function
                tree.root.MoveNode(nodeToMove.Id, nodeToMoveTo.Id);
                
            }

            ResetToMainMenu();
        }

        //add a node somewhere in the tree
        void AddNodeByUser()
        {
            string parent;//these are local and will be set as soon as our string is parsed and used as params in AddNodeByUser()
            string nodeValue;
            string sParentandValue = value;

            try//Try to parse our input and split into 2 strings
            {
                string[] splitStrings = sParentandValue.Split(',');//split up our 2 strings
                if (splitStrings.Count() == 2)
                {
                    parent = splitStrings[0];//first one is parent
                    nodeValue = splitStrings[1];//second one is value

                    //get the parent node
                    Node nParent = tree.root.FindNode(FindNodes(parent));
                    if (nParent == null)
                    {
                        nParent = tree.root;
                    }

                    //create new node
                    Node temp = new Node(nParent.WhiteSpace + 1, null, nodeValue);
                    //add new node as child of the parent node
                    nParent.AddNode(temp, nParent.Id);

                    
                }

            }
            catch (Exception e)//if the parse doesn't work, print an error message and repeat the syntax of input required
            {
                return;
            }

            ResetToMainMenu();
        }

        //delete a node from tree
        void RemoveNode()
        {

            //remove the node from tree
            tree.root.DeleteNode(FindNodes(value));
        }

        //Finds a node and returns id value, if there are duplicates it deals with that
        string FindNodes(string nodeValue)
        {
            //temporary node variable
            Node temp = new Node(0, null, nodeValue);
            //finds all nodes with the passed in value
            List<Node> n = tree.root.FindNode(temp);
            if (n.Count != 0)
            {
                //if there is more than one object with the same value
                if (n.Count > 1)
                {
                    //write out each object with it's unique id and children
                    foreach (Node node in n)
                    {
                         window.messagesList.Items.Add(node.Id + " " + node.Content);
                        if (node.Children.Count > 0)
                        {
                            foreach (Node child in node.Children)
                            {
                                window.messagesList.Items.Add("\t" + child.Content);
                            }
                        }
                    }

                    //get which one the user wants to edit
                    Console.WriteLine("enter id of node you want to change");
                    string id = Console.ReadLine();
                    return id;
                }
                //if there is only one object with that value, return the unique id
                else if (n.Count == 1)
                {
                    return n[0].Id;
                }
            }
            //if the list is null, then the value couldn't be found;
            else
            {
               // window.messagesList.Items.Add("Can't find value");
                return null;
            }
            return null;
        }
    }
}
