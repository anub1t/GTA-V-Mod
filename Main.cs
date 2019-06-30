using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GTA;
using GTA.Native;
using GTA.Math;
using System.Windows.Forms;
using System.IO;
using System.Drawing;
using System.Media;
using NativeUI;

namespace fixwalk
{
    public class Main : Script
    {
        private MenuPool _menuPool;                         // MenuPool

        private Ped playerPed = Game.Player.Character;
        private Player player = Game.Player;
        bool inv = false;
        bool reset = false;
        Random ran = new Random();

        private bool checkbox = false;                      // This is used to later create a checkbox in the template
        private string extra = "Item 1";                    // This is used to later create a UI list

        public Main()
        {
           
            Tick += OnTick;
            KeyDown += OnKeyDown;
            KeyUp += OnKeyUp;
        }

        // This is where loops/things are run every frame.
        private void OnTick(object sender, EventArgs e)
        {
            _menuPool.ProcessMenus();

            if (Game.IsControlPressed(2, GTA.Control.VehicleAttack) && Game.IsControlPressed(2, GTA.Control.Jump))
            {

            }

            if(playerPed.IsGettingIntoAVehicle)
            {
                Vehicle veh = playerPed.GetVehicleIsTryingToEnter();
                if (veh.LockStatus == VehicleLockStatus.Locked)
                {
                    ///playerPed.Task.PlayAnimation("missheistfbisetup1", "unlock_exit_door");
                    playerPed.Task.EnterVehicle();
                }
            }


            if (inv)
            {
                playerPed.IsInvincible = true;
            } else
            {
                playerPed.IsInvincible = false;
            }
        }

        // When you press a key down or hold it.
        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Z && !_menuPool.IsAnyMenuOpen())
            {// our menu on/off switch
                _menuPool = new MenuPool();         // MenuPool
                var mainMenu = new UIMenu("~g~Buy", "~b~anubit");       // Main menu name, you can customize name, label and colors by editing "~<letter>~"
                _menuPool.Add(mainMenu);            // Adds the main menu

                Menu1(mainMenu);
                Menu2(mainMenu);
                Menu3(mainMenu);
                _menuPool.RefreshIndex();           // Refreshes the Index

                mainMenu.Visible = !mainMenu.Visible;       // Makes the main menu visibiel upon having the hotkey above pressed
            }
            if (e.KeyCode == Keys.NumPad9)
            {
                Function.Call(Hash.SET_PED_ALTERNATE_WALK_ANIM, playerPed, "amb@world_human_power_walker@female@base", "base", 1, 1);   //"amb@world_human_power_walker@female@base", "base" default
            }

            if (e.KeyCode == Keys.NumPad8)
            {
                Vehicle pveh = playerPed.CurrentVehicle;
                Entity[] ent = World.GetNearbyEntities(playerPed.Position, 10);           //player.GetTargetedEntity();
                //UI.ShowSubtitle(ent[0].GetType().Name);
                float currentDis = 9999;
                bool j = false;
                Ped ped = playerPed;

                for (int i = 0; i < ent.Length; i++)
                {
                    if (ent[i].GetType().Name == "Ped" && ent[i] != playerPed)
                    {
                        float newdis = World.GetDistance(playerPed.Position, ent[i].Position);
                        if (newdis < currentDis)
                        {
                            currentDis = newdis;
                            ped = (Ped)ent[i];
                            j = true;
                            UI.ShowSubtitle("found one");
                        }
                    }
                }

                if (j)
                {
                    ped.AlwaysKeepTask = true;

                    Function.Call(Hash.TASK_VEHICLE_TEMP_ACTION, ped, ped.CurrentVehicle, 114, 10000);
                    var seq = new TaskSequence();
                    seq.AddTask.CruiseWithVehicle(ped.CurrentVehicle, 20, 786603);
                    seq.AddTask.ParkVehicle(ped.CurrentVehicle, ped.Position + ped.ForwardVector * 100, ped.Heading, 100);
                    //seq.AddTask.WanderAround();
                    //ped.Task.CruiseWithVehicle(ped.CurrentVehicle, 30);
                    //ped.Task.ParkVehicle(ped.CurrentVehicle, ped.Position + ped.ForwardVector * 100, ped.Heading, 100);

                    seq.Close();
                    //ped.AlwaysKeepTask = true;
                    ped.Task.PerformSequence(seq);

                    // ped.Task.WanderAround();
                }


            }

            if (e.KeyCode == Keys.E)
            {
                //Ped ped = World.GetClosestPed(playerPed.Position, 10);
                Ped[] peds = World.GetNearbyPeds(playerPed, 10);
                int i = 0;

                while (peds[i] == playerPed)
                {
                    i++;
                }


                if (playerPed.IsInVehicle())
                {
                    peds[0].Task.EnterVehicle(playerPed.CurrentVehicle, VehicleSeat.ExtraSeat2, 5000, 2.0f, 16);
                    UI.ShowSubtitle("enter veh");
                }
            }

            if (e.KeyCode == Keys.C)
            {
                playerPed.ClearBloodDamage();
            }

            if(e.KeyCode == Keys.N )
            {
                if(playerPed.Weapons.Current.Hash != WeaponHash.Unarmed)
                {
                    playerPed.Weapons.Remove(playerPed.Weapons.Current);
                    playerPed.Money += ran.Next(5000);
                }
            }

           
            }

        // When you press a key up or release it.
        private void OnKeyUp(object sender, KeyEventArgs e)
        {

        }


       
        // This is where Menu1 is created
        public void Menu1(UIMenu menu)
        {

            // This is where the submenus and/or buttons are created
            var sub1 = _menuPool.AddSubMenu(menu, "NativeUI Menu 1");       // the submenu name. To edit the name of it, edit "Menu 1" text.
            var sub2 = _menuPool.AddSubMenu(sub1, "NativeUI Submenu 1");
            var subbtn = new UIMenuItem("Submenu Button 1");
            sub2.AddItem(subbtn);
            sub2.OnItemSelect += (sender, item, index) =>
            {
                if (item == subbtn)
                {
                    // Add code here
                    UI.Notify("~w~This is a ~b~Notification ~w~using ~b~NativeUI.");       // Puts a notification in the lower left corner in-game
                    UI.ShowSubtitle("~w~This is a ~b~Subtitle ~w~using ~b~NativeUI.");     // Puts a subtitle on the lower part of the screen in-game
                }
            };

            // Variable, button and Item creation (this is where you add the cool buttons and checkboxes)
            var button1 = new UIMenuItem("Button 1", "Description for ~b~Button 1");         // Creates a button that displays text

            sub1.AddItem(button1);          // Adds the created item onto the menu/submenu

            button1.Enabled = true;             // Enables the variable button

            sub1.OnItemSelect += (sender, item, index) =>           // Checks if the button is selected
            {
                if (item == button1)        // If statement
                {
                    // Add code here
                    UI.Notify("~w~This is a ~b~Notification ~w~using ~b~NativeUI.");       // Puts a notification in the lower left corner in-game
                    UI.ShowSubtitle("~w~This is a ~b~Subtitle ~w~using ~b~NativeUI.");     // Puts a subtitle on the lower part of the screen in-game
                }
            };

            var checkbox1 = new UIMenuCheckboxItem("Checkbox 1", checkbox, "Description for ~b~Checkbox 1");       // Creates checkbox displaying text

            sub1.AddItem(checkbox1);        // Adds the creates item onto the menu/submenu

            checkbox1.Enabled = true;           // Enables the variable button

            sub1.OnCheckboxChange += (sender, item, checked_) =>        // Enables the variable checkbox
            {
                if (item == checkbox1)          // If statement, this checks if the item selected is the variable in the if statement.
                {
                    if (checked_ == true)      // If statement, checks if the item is checked
                    {
                        // Add code here
                        UI.Notify("~b~Checkbox1 ~w~= ~g~Checked ~w~(This is a notification)");        // Puts a notification in the lower left corner in-game
                        UI.ShowSubtitle("~b~Checkbox1 ~w~= ~g~Checked ~w~(This is a subtitle)");      // Puts a subtitle on the lower part of the screen in-game
                    }
                    else if (checked_ == false)     // If statement, checks if the item is not checked
                    {
                        // Add code here
                        UI.Notify("~b~Checkbox1 ~w~= ~r~Not Checked ~w~(This is a notification)");          // Puts a notification in the lower left corner in-game
                        UI.ShowSubtitle("~b~Checkbox1 ~w~= ~r~Not Checked ~w~(This is a subtitle)");        // Puts a subtitle on the lower part of the screen in-game
                    }
                }
            };

            var newlist = new List<dynamic>         // Creates list displaying items
        {
            "Item 0",
            "Item 1",                           // Items in the UI List
            "Item 2",                           // 
            "Item 3",                           // 
            "Item 4",                           // 
            "Item 5",                           // 
        };
            var list1 = new UIMenuListItem("UI List 1", newlist, 0);        // Creates Menu List displaying text

            sub1.AddItem(list1);            // Adds the creates item onto the menu/submenu

            sub1.OnListChange += (sender, item, index) =>           // Checks if the list item is selected
            {
                if (item == list1)          // If statement
                {
                    if (index == 0)     // This is Item 0 in the list
                    {
                        // The code from this index (0) to index 5 enables different wanted levels, and shows a subtitle
                        // which tells you which wanted level you're currently on, you are free to change any code in here
                        UI.ShowSubtitle("~w~Item = ~b~0");
                    }
                    else if (index == 1)        // This is Item 1 in the list
                    {
                        // This is the code on Item 1, you are free to change any code in here
                        UI.ShowSubtitle("~w~Item = ~b~1");
                    }

                    else if (index == 2)            // This is Item 2 in the list
                    {
                        // This is the code on Item 2, you are free to change any code in here
                        UI.ShowSubtitle("~w~Item = ~b~2");
                    }

                    else if (index == 3)                // This is Item 3 in the list
                    {
                        // This is the code on Item 3, you are free to change any code in here
                        UI.ShowSubtitle("~w~Item = ~b~3");
                    }

                    else if (index == 4)                // This is Item 4 in the list
                    {
                        // This is the code on Item 4, you are free to change any code in here
                        UI.ShowSubtitle("~w~Item = ~b~4");
                    }

                    else if (index == 5)                // This is Item 5 in the list
                    {
                        // This is the code on Item 5, you are free to change any code in here
                        UI.ShowSubtitle("~w~Item = ~b~5");
                    }
                    // This is the code on general list
                    extra = item.IndexToItem(index).ToString();         // Adds to string

                }
            };

            // This is where Menu1 closes
        }

        // This is where Menu2 opens/is created
        public void Menu2(UIMenu menu)
        {
            var submenu1 = _menuPool.AddSubMenu(menu, "NativeUI Menu 2"); // the submenu name. To edit the name of it, edit "Menu 2" text.

            for (int i = 0; i > 1; i++)     // Enables the submenu
            {

            }

            // Variable, button and Item creation (this is where you add the cool buttons and checkboxes)
            var button1 = new UIMenuItem("Button 1", "Description for ~b~Button 1");         // creates button displaying text

            submenu1.AddItem(button1);          // Adds the created item onto the menu/submenu
            button1.Enabled = true;             // Enables the variable button

            submenu1.OnItemSelect += (sender, item, index) =>           // Checks if the button is selected
            {
                if (item == button1)        // If statement
                {
                    // Add code here
                    UI.Notify("~w~This is a ~b~Notification ~w~using ~b~NativeUI.");       // Puts a notification in the lower left corner in-game
                    UI.ShowSubtitle("~w~This is a ~b~Subtitle ~w~using ~b~NativeUI.");     // Puts a subtitle on the lower part of the screen in-game
                }
            };

            var checkboxx = new UIMenuCheckboxItem("Checkbox 2", checkbox, "Description for ~b~Checkbox 2");       // Creates checkbox displaying text (that can be checked)

            submenu1.AddItem(checkboxx);        // Adds the creates item onto the menu/submenu
            checkboxx.Enabled = true;           // Enables the variable button

            submenu1.OnCheckboxChange += (sender, item, checked_) =>        // Enables the variable checkbox
            {
                if (item == checkboxx)      // If statement, checks if the checkbox is selected, then goes to check if it's checked or not
                    if (checked_ == true)      // If statement, checks if the item is checked
                    {
                        // Add code here
                        UI.Notify("~b~Checkbox1 ~w~= ~g~Checked ~w~(This is a notification)");        // Puts a notification in the lower left corner in-game
                        UI.ShowSubtitle("~b~Checkbox1 ~w~= ~g~Checked ~w~(This is a subtitle)");      // Puts a subtitle on the lower part of the screen in-game
                    }

                    else if (checked_ == false)     // If statement, checks if the item is not checked
                    {
                        // Add code here
                        UI.Notify("~b~Checkbox1 ~w~= ~r~Not Checked ~w~(This is a notification)");      // Puts a notification in the lower left corner in-game
                        UI.ShowSubtitle("~b~Checkbox1 ~w~= ~r~Not Checked ~w~(This is a subtitle)");        // Puts a subtitle on the lower part of the screen in-game
                    }
            };
        }   // This closes Menu2

        // This is where Menu3 opens/is created
        public void Menu3(UIMenu menu)
        {
            var subm1 = _menuPool.AddSubMenu(menu, "NativeUI Menu 3 (Credits)");
            for (int i = 0; i > 1; i++)
            {

            }

            var creditsx = new UIMenuItem("LcpdPoliceClan1", "Click to view credits for LcpdPoliceClan1");
            subm1.AddItem(creditsx);
            subm1.OnItemSelect += (sender, item, index) =>
            {
                if (item == creditsx)
                {
                    // Add code here
                    UI.Notify("LcpdPoliceClan1 helped create the actual template for the menu and make it work in-game");
                }
            };

            var creditsz = new UIMenuItem("Will Redeemed", "Click to view credits for Will Redeemed");
            subm1.AddItem(creditsz);
            subm1.OnItemSelect += (sender, item, index) =>
            {
                if (item == creditsz)
                {
                    // Add code here
                    UI.Notify("Will Redeemed helped make code for buttons, checkboxes etc work in the original template, and also make it work in-game");
                }
            };
        }

        // This is the Main Base, including the menu name, hotkeys etc
       
    }

}

