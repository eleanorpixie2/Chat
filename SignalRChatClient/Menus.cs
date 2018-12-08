using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalRChatClient
{
    class Menus
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
        //tree object
        public Tree tree;
        UpdateMenus update;
        public Menus(MainWindow w)
        {
            tree = new Tree();
            window = w;
            update = new UpdateMenus(window, tree);
        }
       
        
        public string menuMessage= ("Please choose an option: \n1-Add a node\n2-Move a node\n3-Delete a node\n4-Get a node\n5-Write tree to file\n6-Exit");

        void StartMenu()
        {
            //menu option choice
            int choice = 0;
            while (true)
            {
                Console.WriteLine("Please choose how you want to create a tree: \n1-Read in from file\n2-Create tree from input");
                string sChoice = Console.ReadLine();
                //convert user input into an int
                try
                {
                    choice = Convert.ToInt32(sChoice);
                    //make sure number entered is a menu option
                    if (choice == 1 || choice == 2)
                    {
                        break;
                    }
                    else
                    {
                        Console.WriteLine("Please enter valid menu option");
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Please enter a valid number.");
                }
            }
            //call method based on user menu choice
            switch (choice)
            {
                case 1:
                    GetFile();
                    break;

                case 2:
                    CreateTreeUser();
                    break;
            }
        }

        //get filename for input
        void GetFile()
        {
            string notFound = "";
            while (notFound != null)
            {
                Console.WriteLine(@"Enter file name and extenision for file in c:\workspace\: ");
                string path = Console.ReadLine();
                notFound = tree.Start(path);
                if (notFound != null)
                {
                    Console.WriteLine(notFound);
                }
            }
        }

        //bool updating = false;
        int index = 0;
        //Main menu for editing tree
        public void Menu()//really the key here is that we check each time to see what part of the menu our choice is for...
        {
            if (!treeCreated)
            {
                tree.StartUser();
                treeCreated = true;
            }

            if (window.messagesList.Items.Count > 2)
            {
                string input = window.messagesList.Items[window.messagesList.Items.Count - 2].ToString();
                string name = "";
                string[] splitStrings = input.Split(':');//split up our 2 strings
                if (splitStrings.Count() >= 2)
                {
                    name = splitStrings[0];
                    input = splitStrings[1];//first one is parent

                }
                if (input.Equals(" updating") && name != window.userTextBox.Text && index != window.messagesList.Items.Count)
                {
                    index = window.messagesList.Items.Count;
                    update.Menu(window.messagesList.Items.Count - 1);
                }
            }

            tree.currentTime = DateTime.UtcNow;//overrwrite the current time every time it gets updated


            switch (myInputState)//this tells us what kind of input we got and directs us to the correct handler function for it.
            {
                case InputState.MainMenu:
                    GetMenuChoice();
                    break;
                case InputState.AddNodeGetInfo:
                    AddNodeByUser();
                    break;
                case InputState.MoveNodeGetValue:
                    MovingNode();
                    window.messagesList.Items.Add("Enter the value of the object you wish to move the other " +
                    "object to:");//new prompt for next input
                    break;
                case InputState.MoveNodeGetParent:
                    MovingNode();
                    window.messagesList.Items.Add("Done");
                    break;
                case InputState.RemoveNodeGetValue:
                    RemoveNode();
                    window.messagesList.Items.Add("Done");
                    break;
                case InputState.GetNodeGetValue:
                    GettingNode();
                    break;
                case InputState.GetNodeGetBranchOrNot:
                    window.messagesList.Items.Add("Do you want the branch with it? Y/N?");
                    GettingNode();
                    break;

            }
        }
           

        

        private void ResetToMainMenu()
        {
            myInputState = InputState.MainMenu;//go back to main menu input if this worked
        }

        

        void GetMenuChoice()
        {
            //menu choice
            int choice = 0;
            string sChoice = window.messageTextBox.Text;

            //convert user input to int
            try
            {
                choice = Convert.ToInt32(sChoice);
                //make sure choice is a valid menu option
                if (choice == 1 || choice == 2 || choice == 3 || choice == 4 || choice == 5 || choice == 6)
                {

                }
                else
                {
                    window.messagesList.Items.Add("Please enter valid menu option");
                }
            }
            catch (Exception e)
            {
                window.messagesList.Items.Add("Please enter a valid number.");
            }

            //call function based on user choice
            switch (choice)//these are going to tell the menu what function to call next. Not going to call them directly
            {
                case 1://add node               
                    myInputState = InputState.AddNodeGetInfo;
                    window.messagesList.Items.Add("Please enter the information in the form of: parent, value");
                    break;
                case 2://move node
                    myInputState = InputState.MoveNodeGetValue;
                    window.messagesList.Items.Add("Enter the value of the object you wish to move:");
                    break;
                case 3://delete a node
                    //get the value you want to delete
                    myInputState = InputState.RemoveNodeGetValue;
                    window.messagesList.Items.Add("Enter vaule you want deleted:");
                    break;
                case 4://get a node
                    myInputState = InputState.GetNodeGetValue;
                    window.messagesList.Items.Add("Enter value of Node you want to find: ");
                    break;
                case 5://write tree to file
                       //write out to text or the uwp part
                    tree.OutputToFile(tree.root);
                    Console.WriteLine("Done");
                    break;
                case 6://exit
                       //keepGoing = false;
                    break;
            }

        }


        //create a tree solely from user input
        void CreateTreeUser()
        {
            //create intial root node
            tree.StartUser();
            //temporary holding variable
            Node tempNode = null;
            string value = "";
            //press f to exit loop
            Console.WriteLine("Enter F for the node value when you are finished entering nodes.");
            while (value != "f" || value != "f")
            {
                Console.WriteLine("Enter node value: ");
                value = Console.ReadLine();
                //check if f was entered
                if (value == "f" || value == "F")
                {
                    break;
                }
                int depth = 0;
                //get depth of node
                while (true)
                {
                    Console.WriteLine("Enter the node's depth(i.e. 0,1,2,etc): ");
                    string dValue = Console.ReadLine();
                    try
                    {
                        depth = Convert.ToInt32(dValue);
                        break;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Please enter valid number: ");
                    }
                }
                //make new node
                Node n = new Node(8 * depth, null, value);
                //if there is no whitespace, then add as child to root node
                if (n.WhiteSpace == 0)
                {
                    tempNode = tree.root;
                }
                //add node to parent node
                tempNode.AddNode(n);
                //keep reference of last node added
                tempNode = n;
            }

        }

        //get a node
        void GettingNode()
        {
            if (myInputState == InputState.GetNodeGetValue)
            {
                getNodeValue = window.messageTextBox.Text;
            }

            else if (myInputState == InputState.GetNodeGetBranchOrNot)
            {
                //get whether the user just wants the node or the whole branch               
                string getbranch = window.messageTextBox.Text;

                //set bool based on user input
                if (getbranch.Equals("Y") || getbranch.Equals("y"))
                {
                    getBranch = true;
                    myInputState = InputState.MainMenu;
                }
                else if (getbranch.Equals("N") || getbranch.Equals("n"))
                {
                    getBranch = false;
                    myInputState = InputState.MainMenu;
                }
                else
                {
                    window.messagesList.Items.Add("Please enter valid option");
                }
            }

            string id = FindNodes(getNodeValue);

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
            string nodeValue = "";
            Node nodeToMoveTo;

            if (myInputState == InputState.MoveNodeGetValue)//if we need to get the value, then it gets value only
            {
                nodeValue = window.messageTextBox.Text;
                //find the node we want to move
                nodeToMove = tree.root.FindNode(FindNodes(nodeValue));
            }

            else if (myInputState == InputState.MoveNodeGetParent)//if it already knows value, we can skip it and get on with doing everything else
            {
                parentValue = window.messageTextBox.Text;//this is the last value we need, so we can move the node now

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
            window.SendInfo("updating");
            window.SendInfo(2 + "/" + parentValue+","+nodeValue);
            ResetToMainMenu();
        }

        //add a node somewhere in the tree
        void AddNodeByUser()
        {
            string parent;//these are local and will be set as soon as our string is parsed and used as params in AddNodeByUser()
            string value;
            string sParentandValue = window.messageTextBox.Text;

            try//Try to parse our input and split into 2 strings
            {
                string[] splitStrings = sParentandValue.Split(',');//split up our 2 strings
                if (splitStrings.Count() == 2)
                {
                    parent = splitStrings[0];//first one is parent
                    value = splitStrings[1];//second one is value

                    //get the parent node
                    Node nParent = tree.root.FindNode(FindNodes(parent));
                    if (nParent == null)
                    {
                        nParent = tree.root;
                    }

                    //create new node
                    Node temp = new Node(nParent.WhiteSpace + 1, null, value);
                    //add new node as child of the parent node
                    nParent.AddNode(temp, nParent.Id);

                    window.messagesList.Items.Add("Your node has been added.");//success message to let user know it worked
                    
                }
                else
                    window.messagesList.Items.Add("Incorrect syntax. Please enter the information in the form of: parent, value");
            }
            catch (Exception e)//if the parse doesn't work, print an error message and repeat the syntax of input required
            {
                window.messagesList.Items.Add("Incorrect syntax. Please enter the information in the form of: parent, value");
            }
            window.SendInfo("updating");
            window.SendInfo(1 + "/" + sParentandValue);
            ResetToMainMenu();
        }

        //delete a node from tree
        void RemoveNode()
        {
            string value = window.messageTextBox.Text;
            //remove the node from tree
            tree.root.DeleteNode(FindNodes(value));
            window.SendInfo("updating");
            window.SendInfo(3 + "/" + value);
            ResetToMainMenu();
        }

        //Finds a node and returns id value, if there are duplicates it deals with that
        string FindNodes(string value)
        {
            //temporary node variable
            Node temp = new Node(0, null, value);
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
                window.messagesList.Items.Add("Can't find value");
                return null;
            }
            return null;
        }
    }
}
