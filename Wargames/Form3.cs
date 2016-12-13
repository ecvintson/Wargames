using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Wargames
{
    public partial class lowerBorderLine : Form
    {

        int heading = 1;
        int hasTarget = 0;
        string targetInfo = "";

        //variable for determining which target is selected
        // 1: Self
        // 2: Carrier
        // 3: Sub
        // 4: Battleship
        // 5: Freighter
        int whichTarget;

        int torp1Which;
        int torp2Which;
        int torp3Which;
        int torp4Which;
        int torp5Which;

        //variables for determining when a tube is reloaded
        int reload1;
        int reload2;
        int reload3;
        int reload4;
        int reload5;

        //determines if a torpedo is currently in the water
        bool torp1Active = false;
        bool torp2Active = false;
        bool torp3Active = false;
        bool torp4Active = false;
        bool torp5Active = false;

        //determines if each torpedo is in homing state or not
        bool isHoming1 = false;
        bool isHoming2 = false;
        bool isHoming3 = false;
        bool isHoming4 = false;
        bool isHoming5 = false;

        //speed of torpedo travel
        int torpSpeed = 1;

        //how long it takes for a tube to reload
        int timeReload = 500;

        //location of target, used to determine destination of a torpedo picture box
        Point targetLoc;

        //target information for individual torpedos (necessary to prevent change in course when a second torpedo is fired while one is already en route to target
        Point t1targetLoc;
        Point t2targetLoc;
        Point t3targetLoc;
        Point t4targetLoc;
        Point t5targetLoc;

        //variables for storing enemies targets
        Point e1targetLoc;
        Point e2targetLoc;
        Point e3targetLoc;

        //variable for determining if enemy torpeods are in the water
        bool etorp1Active = false;
        bool etorp2Active = false;
        bool etorp3Active = false;


        //variables for creating motion patterns for enemies
        // 0: stationary
        // 1: South
        // 2: East
        // 3: North
        // 4: West
        int carrierDirection = 1;
        int cargoDirection = 2;
        int battleshipDirection = 1;

        //variables for storing speeds
        int carrierSpeed = 2;
        int cargoSpeed = 1;
        int battleshipSpeed = 3;

        //lower boundary of the play field
        Rectangle lowerRec;

        //out of bounds starting point for torps
        Point torpStartLoc;
        
        //determines if the enemy is aware of the player's presence
        bool aware = false;

        //determines if the enemy has your location
        bool discovered = false;

        public lowerBorderLine()
        {
            InitializeComponent();
           
            //flash a messagebox for selecting a scenario

            //depending on scenario, enable relevant pictureboxes and set locations
            torpStartLoc = torpedoBox1.Location;
          

        }

        private void MovePictureBox(PictureBox box, int dx, int dy)
        {
            // Get Location
            Point loc = box.Location;

            // Change Location
            loc.X = loc.X + dx;
            loc.Y = loc.Y + dy;

            // Set Location
            box.Location = loc;
        }

        private void TorpedoCleanup()
        {
            //used to remove torpedos from the play area once they are no longer active


            if (torpedoBox1.Visible == false)
            {
                torpedoBox1.Location = targetLabel.Location;
            }
            if (torpedoBox2.Visible==false)
            {
                torpedoBox2.Location = targetLabel.Location;
            }
            if (torpedoBox3.Visible==false)
            {
                torpedoBox3.Location = targetLabel.Location;
            }
            if (torpedoBox4.Visible==false)
            {
                torpedoBox4.Location = targetLabel.Location;
            }
            if (torpedoBox5.Visible==false)
            {
                torpedoBox5.Location = targetLabel.Location;
            }
        }
        
        private void EnemySonar()
        {
            //used to check if the enemy detects the player


            Rectangle carrierBound = carrierPicture.Bounds;
            Rectangle cargoBound = cargoBox.Bounds;
            Rectangle battleshipBound = battleshipBox.Bounds;
            Rectangle playerBound = playerBox.Bounds;

            int playerSpeed = Convert.ToInt16(speedBox.Value);
            

            if (carrierBound.IntersectsWith(playerBound) | cargoBound.IntersectsWith(playerBound) | battleshipBound.IntersectsWith(playerBound))
            {
                if (playerSpeed > 0)
                {
                    aware = true;
                    discovered = true;
                }

            }
            

        }

        private void EnemyAttack(ref Point targetLoc, bool torpActive)
        {
            if (discovered == true)
            {
                targetLoc = playerBox.Location;

            }

            if (torpActive == false)
            {
            }
        }

        private void TurnEnemy(Point aheadPoint, PictureBox enemyBox, int direction, int speed, int turnDirection)
        {
            Rectangle enemyBoxBounds = enemyBox.Bounds;
            enemyBoxBounds.X = aheadPoint.X;
            enemyBoxBounds.Y = aheadPoint.Y;
            lowerRec = boundaryPB.Bounds;

            Rectangle carrierBound = carrierPicture.Bounds;
            Rectangle cargoBound = cargoBox.Bounds;
            Rectangle battleshipBound = battleshipBox.Bounds;
            Rectangle playerBound = playerBox.Bounds;

            bool aheadClear = true;

            //checks to make sure the box isn't about to hit N, E or W wall of play field
            if (aheadPoint.X < 0 || aheadPoint.Y < 0 || aheadPoint.X + enemyBox.Width > this.ClientRectangle.Width || aheadPoint.Y + enemyBox.Height > this.ClientRectangle.Height)
            {
                aheadClear = false;
            }

            //if box has reached lower bound, moves it off the lower bound
            if ( enemyBoxBounds.IntersectsWith(lowerRec))
            {
                aheadClear = false;
              
            }

            //if any two boxes will intersect
            if (enemyBoxBounds.IntersectsWith(carrierBound) && enemyBox != carrierPicture)
            {
                aheadClear = false;
                
            }
            if (enemyBoxBounds.IntersectsWith(cargoBound) && enemyBox != cargoBox)
            {
                aheadClear = false;
                
            }
            if (enemyBoxBounds.IntersectsWith(battleshipBound) && enemyBox != battleshipBox)
            {
                aheadClear = false;
                
            }
            

            //if room to move, passes instructions to MovePictureBox
            if (aheadClear == true)
            {
                MovePictureBox(enemyBox, (aheadPoint.X - enemyBox.Location.X), (aheadPoint.Y - enemyBox.Location.Y));
            }
                //if no room to move, changes direction appropriate to the patrol pattern of the ship
            else
            {
                if (enemyBox == carrierPicture)
                {
                   carrierDirection = carrierDirection + turnDirection;
                   
                   if (carrierDirection > 4)
                   {
                       carrierDirection = 2;
                   }
                }
                else if (enemyBox == cargoBox)
                {
                    cargoDirection = cargoDirection + turnDirection;
                    if (cargoDirection > 4)
                    {
                        cargoDirection = 2;
                    }
                }
                else if (enemyBox == battleshipBox)
                {
                    battleshipDirection = battleshipDirection + 2;
                    if (battleshipDirection > 4)
                    {
                        battleshipDirection = 1;
                    }
                }
               
            }
        }

        private void MoveEnemy(PictureBox enemyBox, int direction, int speed, int turnDirection)
        {
            //bool aheadClear = true;

            //moves active enemy pictureboxes
            //checks to see if the box is active, then moves it
            if (enemyBox.Enabled == true)
            {
                Point aheadPoint = enemyBox.Location;

                if (direction == 1)
                {
                    aheadPoint.Y = aheadPoint.Y + speed;                    
                    TurnEnemy(aheadPoint, enemyBox, direction, speed, turnDirection);
                }

                if (direction == 2)
                {
                    aheadPoint.X = aheadPoint.X + speed;                   
                    TurnEnemy(aheadPoint, enemyBox, direction, speed, turnDirection);
                }

                if (direction == 3)
                {
                    aheadPoint.Y = aheadPoint.Y - speed;
                    TurnEnemy(aheadPoint, enemyBox, direction, speed, turnDirection);
                }
                if (direction == 4)
                {
                    aheadPoint.X = aheadPoint.X - speed;
                    TurnEnemy(aheadPoint, enemyBox, direction, speed, turnDirection);
                }

            }
            if (enemyBox.Enabled == false)
            {
                enemyBox.Location = targetLabel.Location;
            }
        }

        private void HomingProcedure(bool isHoming, ref Point homingTarget, int internalWhich)
        {
            //updates target coordinates if torpedo is a homing torpedo
            //used on appropriate timers to ensure constant updating
            //checks to see if manual targeting is activated

            if (isHoming == true)
            {
                //manual targeting off, find target
                if (internalWhich == 2)
                {
                    homingTarget = carrierPicture.Location;
                }
                else if (internalWhich == 3)
                {

                }
                else if (internalWhich == 4)
                {
                    homingTarget = battleshipBox.Location;
                }
                else if (internalWhich == 5)
                {
                    homingTarget = cargoBox.Location;
                }

            }
           
        }

        private void FiringProcedure(RadioButton InternalRadioButton, ref bool internalTorpActive, PictureBox internalTorpedoBox, Timer internalTimer, ref Point internalTargetLoc, bool isHoming)
        {


           InternalRadioButton.Enabled = false;
             internalTorpActive = true;
            internalTorpedoBox.Visible = true;
            internalTorpedoBox.Location = playerBox.Location;            
            internalTimer.Enabled = true;

            //HomingProcedure(isHoming, ref internalTargetLoc);
        }

        private void TorpedoHit(PictureBox InternalTorpBox)
        {
            //checks to see if a torpedo has hit a target
            //embedded inside MoveTorpedo

            Rectangle torpRect = InternalTorpBox.Bounds;

            Rectangle lowerBound = boundaryPB.Bounds;

            Rectangle carrierBound = carrierPicture.Bounds;
            Rectangle cargoBound = cargoBox.Bounds;
            Rectangle battleshipBound = battleshipBox.Bounds;

            if (torpRect.IntersectsWith(carrierBound))
            {
                InternalTorpBox.Visible = false;
                carrierPicture.Visible = false;
                carrierPicture.Enabled = false;
            }
            else if (torpRect.IntersectsWith(cargoBound))
            {
                InternalTorpBox.Visible = false;
                cargoBox.Visible = false;
                cargoBox.Enabled = false;
            }
            else if (torpRect.IntersectsWith(battleshipBound))
            {
                InternalTorpBox.Visible = false;
                battleshipBox.Visible = false;
                battleshipBox.Enabled = false;
            }
            else if (torpRect.IntersectsWith(lowerBound))
            {
                InternalTorpBox.Visible = false;
            }

        }

        private void MoveTorpedo(int reloadCounter, PictureBox InternalTorpBox, bool internalTorpActive, RadioButton internalRadioButton, Timer internalTimer, Point internalTargetLoc)
        {


            //firing a torpedo makes the enemy aware that you are there
            aware = true;


            
            //waits 30 seconds to reload tube
            if (reloadCounter >= timeReload)
            {
                //if tube is reloaded, and torpedo1 is no longer active, stop the timer and make tube 1 available again
                if (internalTorpActive == false)
                {
                    internalRadioButton.Enabled = true;
                    reloadCounter = 0;
                    internalTimer.Enabled = false;
                    InternalTorpBox.Location = torpStartLoc;
                }
            }

            //check to make sure torpedo has not struck a target
            TorpedoHit(InternalTorpBox);

            //if previous checks fail, use tick to progress active torpedo towards a target
            if (internalTorpActive == true)
            {

                double xtorpedo = InternalTorpBox.Location.X + (InternalTorpBox.Width / 2.0);
                double ytorpedo = InternalTorpBox.Location.Y + (InternalTorpBox.Height / 2.0);
                double xtarget = internalTargetLoc.X;
                double ytarget = internalTargetLoc.Y;
                double torpdx = (xtarget - xtorpedo);
                double torpdy = (ytarget - ytorpedo);

          
                if ( Math.Abs(torpdx) + Math.Abs(torpdy) <= 2)
                {

                    //if torp reached target, remove torp
                    InternalTorpBox.Visible = false;
                    
                }
                
                else
                {
                    double distance = Math.Sqrt(torpdx * torpdx + torpdy * torpdy);
                    double ratio = torpSpeed / distance;

                    // Create new int vars because C# is finnicky
                    int dxi = (int)Math.Round(torpdx * ratio);
                    int dyi = (int)Math.Round(torpdy * ratio);

                    MovePictureBox(InternalTorpBox, dxi, dyi);
                }
            }

        }

        private void button5_Click(object sender, EventArgs e)
        {
            northButton.Enabled = false;
            eastButton.Enabled = true;
            southButton.Enabled = true;
            westButton.Enabled = true;
            heading = 1;
        
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            hasTarget = 1;
            whichTarget = 2;
            
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void rectangleShape1_Click(object sender, EventArgs e)
        {

        }

        private void playerTimer_Tick(object sender, EventArgs e)
        {
            int xval = 0;
            int yval = 0;
            int xspeed = Convert.ToInt16(speedBox.Value);
            int yspeed = Convert.ToInt16(speedBox.Value);
            int xthrough = 0;
            int ythrough = 0;

            //the if-else tree for determining speed and heading
            if (heading == 1)
            {
                xval = 0;
                yval = -1;

                xthrough = xval * xspeed;
                ythrough = yval * yspeed;


            }
            else if (heading == 2)
            {
                xval = 1;
                yval = 0;

                xthrough = xval * xspeed;
                ythrough = yval * yspeed;
            }
            else if (heading == 3)
            {
                xval = 0;
                yval = 1;

                xthrough = xval * xspeed;
                ythrough = yval * yspeed;
            }
            else if (heading == 4)
            {
                xval = -1;
                yval = 0;

                xthrough = xval * xspeed;
                ythrough = yval * yspeed;
            }

            //takes speed and heading, and passes them to the MovePictureBox function
                if (speedBox.Value > 0)
                {
                    MovePictureBox(playerBox, xthrough, ythrough);
                }

            //moves active enemy pictureboxes

                MoveEnemy(carrierPicture, carrierDirection, carrierSpeed, 1);
                MoveEnemy(cargoBox, cargoDirection, cargoSpeed, 2);
                MoveEnemy(battleshipBox, battleshipDirection, battleshipSpeed, 2);

            //Checks to see if the player is detected
            //If so, fires a torpedo from any relevant pictureboxes

                TorpedoCleanup();

             //checks to see if the enemy sonar has detected the player
                EnemySonar();

            //checks to see if a target has been selected, and make sure target is not self
            if (hasTarget == 1 && whichTarget != 1)
            {
                fireButton.Enabled = true;

                //determines which target has been selected
                if (whichTarget == 2)
                {
                    targetInfo = "This is an enemy carrier. \n It's maximum speed is 0 \n It carries no weapons \n Sink it";
                }
                else if (whichTarget == 3)
                {
                    targetInfo = "This is the placeholder for the Sub info";
                }
                else if (whichTarget == 4)
                {
                    targetInfo = "This is the placeholder for the battleship info";
                }
                else if (whichTarget == 5)
                {
                    targetInfo = "This is the placeholder for the freighter info";
                }

                //sets the info in the label
                targetLabel.Text = targetInfo;
            }
                //if a target is selected, but that target is self
            else if (hasTarget == 1 && whichTarget == 1)
            {
                fireButton.Enabled = false;
                targetLabel.Text = "You have yourself targeted.";
            }
                //if manual targeting is enabled
            else if (button1.Enabled == false)
            {
                fireButton.Enabled = true;
                targetLabel.Text = "You are manually targeting.";
            }
                //if no targets are selected, and not manually targeting
            else
            {
                fireButton.Enabled = false;
                targetLabel.Text = "You currently have no target.";
            }
        }

        private void eastButton_Click(object sender, EventArgs e)
        {
            northButton.Enabled = true;
            eastButton.Enabled = false;
            southButton.Enabled = true;
            westButton.Enabled = true;
            heading = 2;
        }

        private void southButton_Click(object sender, EventArgs e)
        {
            northButton.Enabled = true;
            eastButton.Enabled = true;
            southButton.Enabled = false;
            westButton.Enabled = true;
            heading = 3;
        }

        private void westButton_Click(object sender, EventArgs e)
        {
            northButton.Enabled = true;
            eastButton.Enabled = true;
            southButton.Enabled = true;
            westButton.Enabled = false;
            heading = 4;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            hasTarget = 1;
            whichTarget = 1;
        }

        private void fireButton_Click(object sender, EventArgs e)
        {




            if (radioButton1.Checked && radioButton1.Enabled == true)
            {

                torp1Which = whichTarget;
                if (button1.Enabled == false)
                {
                    isHoming1 = false;
                    t1targetLoc = crosshairsBox.Location;
                }
                else if (button1.Enabled == true)
                {
                    isHoming1 = true;
                }
                FiringProcedure(radioButton1, ref torp1Active, torpedoBox1, timer1, ref t1targetLoc, isHoming1);
            }

        
                else if (radioButton2.Checked && radioButton2.Enabled == true)
                {
                    torp2Which = whichTarget;
                    if (button1.Enabled == false)
                    {
                        isHoming2 = false;
                        t2targetLoc = crosshairsBox.Location;
                    }
                    else if (button1.Enabled == true)
                    {
                        isHoming2 = true;
                    }
                    FiringProcedure(radioButton2, ref torp2Active, torpedoBox2, timer2, ref t2targetLoc, isHoming2);
                }
                
                else if (radioButton3.Checked && radioButton3.Enabled == true)
                {
                    torp3Which = whichTarget;
                    if (button1.Enabled == false)
                    {
                        isHoming3 = false;
                        t3targetLoc = crosshairsBox.Location;
                    }
                    else if (button1.Enabled == true)
                    {
                        isHoming3 = true;
                    }
                    FiringProcedure(radioButton3, ref torp3Active, torpedoBox3, timer3, ref t3targetLoc, isHoming3);

                    
                }
                else if (radioButton4.Checked && radioButton4.Enabled == true)
                {
                    torp4Which = whichTarget;
                    if (button1.Enabled == false)
                    {
                        isHoming4 = false;
                        t4targetLoc = crosshairsBox.Location;
                    }
                    else if (button1.Enabled == true)
                    {
                        isHoming4 = true;
                    }
                    FiringProcedure(radioButton4, ref torp4Active, torpedoBox4, timer4, ref t4targetLoc, isHoming4);
                }
                else if (radioButton5.Checked && radioButton5.Enabled == true)
                {
                    torp5Which = whichTarget;
                    if (button1.Enabled == false)
                    {
                        isHoming5 = false;
                        t5targetLoc = crosshairsBox.Location;
                    }
                    else if (button1.Enabled == true)
                    {
                        isHoming5 = true;
                    }
                    FiringProcedure(radioButton5, ref torp5Active, torpedoBox5, timer5,ref t5targetLoc, isHoming5);
                }
        }

        
            
           

          
        

        private void timer1_Tick(object sender, EventArgs e)
        {
            HomingProcedure(isHoming1, ref t1targetLoc, torp1Which);
       

            if (torpedoBox1.Visible == false)
            {
                torp1Active = false;
            }
            reload1++;
            MoveTorpedo(reload1, torpedoBox1, torp1Active, radioButton1, timer1, t1targetLoc);
            if (radioButton1.Enabled == true)
            {
                reload1 = 0;
            }

          
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            HomingProcedure(isHoming2, ref t2targetLoc, torp2Which);
         

            if (torpedoBox2.Visible == false)
            {
                torp2Active = false;

            }
            reload2++;
            MoveTorpedo(reload2, torpedoBox2, torp2Active, radioButton2, timer2, t2targetLoc);
            if (radioButton2.Enabled == true)
            {
                reload2 = 0;
            }


        }

        private void timer3_Tick(object sender, EventArgs e)
        {
            HomingProcedure(isHoming3, ref t3targetLoc, torp3Which);

            if (torpedoBox3.Visible == false)
            {
                torp3Active = false;

            }
            reload3++;
            MoveTorpedo(reload3, torpedoBox3, torp3Active, radioButton3, timer3, t3targetLoc);
            if (radioButton3.Enabled == true)
            {
                reload3 = 0;
            }
        }

        private void timer4_Tick(object sender, EventArgs e)
        {
            HomingProcedure(isHoming4, ref t4targetLoc, torp4Which);
            if (torpedoBox4.Visible == false)
            {
                torp4Active = false;

            }
            reload4++;
            MoveTorpedo(reload4, torpedoBox4, torp4Active, radioButton4, timer4, t4targetLoc);
            if (radioButton4.Enabled == true)
            {
                reload4 = 0;
            }
        }

        private void timer5_Tick(object sender, EventArgs e)
        {
            HomingProcedure(isHoming5, ref t5targetLoc, torp5Which);
            if (torpedoBox5.Visible == false)
            {
                torp5Active = false;

            }
            reload5++;
            MoveTorpedo(reload5, torpedoBox5, torp5Active, radioButton5, timer5, t5targetLoc);
            if (radioButton5.Enabled == true)
            {
                reload5 = 0;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            button2.Enabled = true;
            button1.Enabled = false;
            crosshairsBox.Visible = true;
            hasTarget = 0;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            button1.Enabled = true;
            button2.Enabled = false;
            crosshairsBox.Visible = false;

        }


        private void Form3_Click(object sender, EventArgs e)
        {
        }

        private void Form3_MouseClick(object sender, MouseEventArgs e)
        {
            if (button1.Enabled == false)
            {
                if (e.Button == MouseButtons.Left)
                {
                    targetLoc = e.Location;
                    crosshairsBox.Location = targetLoc;

                }
            }

        }

        private void Form3_KeyDown(object sender, KeyEventArgs e)
        {
            

        }

        private void targetLabel_Click(object sender, EventArgs e)
        {

        }

        private void battleshipBox_Click(object sender, EventArgs e)
        {
            hasTarget = 1;
            whichTarget = 4;
        }

        private void pictureBox1_Click_1(object sender, EventArgs e)
        {
            hasTarget = 1;
            whichTarget = 5;
        }




    }
}
