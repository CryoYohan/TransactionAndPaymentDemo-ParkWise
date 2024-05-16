using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TransactionDemo
{
    public partial class Form1 : Form
    {
        DateTime datetime = DateTime.Now;
        SqlConnection conn;
        SqlCommand cmd;
        private string currentUser = "leo";
        public Form1()
        {
            InitializeComponent();
        }

        List<Button> buttonList = new List<Button>();
        private void button1_Click(object sender, EventArgs e)
        {
            string plate_no1 = textBox1.Text;
            setPlateNo(plate_no1);
            string vehicle_type = textBox2.Text;
            string vehicle_brand = textBox3.Text;
            int active = 1;
            string command = "INSERT INTO Vehicles (pn, vt, vb, IsActive)" +
                $"VALUES('{plate_no}', '{vehicle_type}', '{vehicle_brand}',{active})";
            exeCommands(command);
            showVehicles();
            generateButtons();
            foreach (Button buttons in buttonList)
            {
                buttons.Enabled = true;
            }
            parkingslotsPanel.Show();
            updateSlot();
            
        }
        // Generate Parking Slot Buttons
        private void generateButtons()
        {
            for (int i = 0; i < 16; i++)
            {
                Button button = new Button();
                button.SetBounds(0, 0, 99, 92);
                button.BackColor = Color.MediumSpringGreen;
                button.Cursor = Cursors.Hand;
                button.Text = $"GF{i}";
                button.Click += buttons_Click;
                flowLayoutPanel1.Controls.Add(button);
                buttonList.Add(button);
            }
        }

        private void updateSlot()
        {
            for (int i = 0; i <= buttonList.Count; i++)
            {
                searchPSlot(i);
            }
        }

        private void exeCommands(string command)
        {
            conn = new SqlConnection("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=ParkingVehicles;Integrated Security=True;Connect Timeout=30;Encrypt=False;");
            conn.Open();
            cmd = new SqlCommand(command, conn);
            cmd.ExecuteNonQuery();
            conn.Close();
        }
        private void exeCommandsTransactions(string command)
        {
            conn = new SqlConnection("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=ParkingVehicles;Integrated Security=True;Connect Timeout=30;Encrypt=False;");
            conn.Open();
            cmd = new SqlCommand(command, conn);
            cmd.ExecuteNonQuery();
            conn.Close();
        }

        private void showVehicles()
        {
            conn = new SqlConnection("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=ParkingVehicles;Integrated Security=True;Connect Timeout=30;Encrypt=False;");
            conn.Open();
            string command = "SELECT v_id as [Vehicle ID],pn as [Plate No.], vt as [Vehicle Type], vb as [Vehicle Brand] from Vehicles WHERE IsActive = 1";
            cmd = new SqlCommand(command, conn);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds, "ParkingVehicles");
            dataGridView1.DataSource = ds.Tables[0];
            conn.Close();
        }
        private void showTransactions()
        {
            conn = new SqlConnection("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=ParkingVehicles;Integrated Security=True;Connect Timeout=30;Encrypt=False;");
            conn.Open();
            string command = "SELECT p_id as [Vehicle ID], p_loc as [Parking Slot], p_date as [Parking Date/Time] FROM p_trans"; // kuwangan ni og User Logs
            cmd = new SqlCommand(command, conn);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds, "ParkingVehicles");
            dataGridView2.DataSource = ds.Tables[0];
            conn.Close();
        }
        private void showPayments()
        {
            conn = new SqlConnection("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=ParkingVehicles;Integrated Security=True;Connect Timeout=30;Encrypt=False;");
            conn.Open();
            string command = "SELECT t_id as [Vehicle ID],t_pn as [Plate No.], t_vt as [Vehicle Type], t_date as [Parking Date/Time], t_payment as [Payment] FROM payments"; // kuwangan ni og User Logs og Duration
            cmd = new SqlCommand(command, conn);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds, "ParkingVehicles");
            dataGridView3.DataSource = ds.Tables[0];
            conn.Close();
        }

        private void setPlateNo(string plate_no)
        {
            this.plate_no = plate_no;
        }
        private string getPlateNo()
        {
            return plate_no;
        }
        private void gFloorParkingBTN_Click(object sender, EventArgs e)
        {
           

        }

        private void closeParkingSlot_Click(object sender, EventArgs e)
        {
            parkingslotsPanel.Hide();

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            parkingslotsPanel.Hide();
            paymentsPanel.Hide();
            showVehicles();
            showTransactions();
        }

        private string slotname = "";
        private void setSlotname(string slotname)
        {
            this.slotname = slotname;
        }
        private string getSlotname()
        {
            return slotname;
        }

       private void buttons_Click(object sender, EventArgs e)
        {
            int counter = 0;
            Button clickedButton = (Button)sender;
            foreach(Button buttons in buttonList)
            {
                if(buttons == clickedButton)
                {
                    DialogResult dg = MessageBox.Show("Confirm Slot", "ParkWise Demo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (dg == DialogResult.Yes)
                    {
                        
                        insertParkingSlot(getVehicleID(), $"GF-{counter}", counter, datetime);
                        parkingslotsPanel.Hide();
                        showTransactions();
                        MessageBox.Show($"You have successfully parked in GF-{counter}");
                        buttons.Enabled = false;
                        buttons.BackColor = Color.DarkRed;
                        parkingslotsPanel.Hide();
                    }
                }
                else
                    counter++;
            }
           
        }
        private void insertParkingSlot(int p_id, string p_loc, int p_slot, DateTime datetime)
        {
            conn = new SqlConnection("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=ParkingVehicles;Integrated Security=True;Connect Timeout=30;Encrypt=False;");
            string command = "INSERT INTO p_trans(p_id, p_loc, p_slot, p_date)" +
                "VALUES(" + p_id + ", '" + p_loc + "'," + p_slot + ", '" + datetime + "')";
            exeCommandsTransactions(command);
        }

        private int getVehicleID()
        {
            string connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=ParkingVehicles;Integrated Security=True;Connect Timeout=30;Encrypt=False;";
            string command = $"SELECT v_id FROM Vehicles WHERE pn = '{getPlateNo()}'";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                // Open the connection
                connection.Open();

                // Create a command object with the query and connection
                using (SqlCommand cmd2 = new SqlCommand(command, connection))
                {


                    // Execute the query and get the result
                    using (SqlDataReader reader = cmd2.ExecuteReader())
                    {
                        // Check if there are any rows in the result
                        if (reader.HasRows)
                        {
                            // Read the first row
                            reader.Read();

                            // Retrieve the stored password from the reader
                            retrievedVehicleID = reader.GetInt32(0);
                            return retrievedVehicleID;


                        }
                         else
                           {
                             MessageBox.Show("Vehicle does not exist!", "Login", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return 0;
                           }
                    }
                }
            }
        }


        private void button2_Click(object sender, EventArgs e)
        {
            parkingslotsPanel.Show();
            foreach (Button buttons in buttonList)
            {
                buttons.Enabled = false;
            }
            updateSlot();
        }
        // Park-out Button
        private void button6_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                foreach (DataGridViewCell cell in dataGridView1.SelectedRows[0].Cells)
                {
                    // Check if the cell value is not null and not empty
                    if (cell.Value != null && !string.IsNullOrEmpty(cell.Value.ToString()))
                    {
                        string plateNo = dataGridView1.CurrentRow.Cells[1].Value.ToString();
                        string command = "INSERT INTO payments(t_id, t_pn, t_vt)" +
                            "SELECT v_id, pn, vt FROM Vehicles ";
                        exeCommands(command);

                        decimal payment = 40.00m;
                        string command2 = $"UPDATE payments SET t_date = '"+ datetime +"', t_payment = "+ payment +"";
                        exeCommands(command2);
                        int inactive = 0;
                        string command3 = $"UPDATE Vehicles " +
                            $"SET IsActive = {inactive} " +
                            $"WHERE pn = '{plateNo}'";
                        exeCommands(command3);

                        string extractedStringID = dataGridView1.CurrentRow.Cells[0].Value.ToString();
                        int converterdStringID = Convert.ToInt32(extractedStringID);
                      //  updateButtonColors(converterdStringID);
                        string command4 = $"UPDATE p_trans SET p_slot = {inactive} WHERE p_id = {converterdStringID}";
                        exeCommands(command4);

                        showPayments();
                        updateSlot();
                        showTransactions();
                        showVehicles();
                    }
                }

            }
            else if (dataGridView1.Rows.Count == 0)
                MessageBox.Show("No Vehicles to Park-out");
            else
                MessageBox.Show("Select Vehicles to Park-out");
        }
      /* private void updateButtonColors(int p_id)
        {
            string command = $"SELECT p_slot FROM p_trans WHERE  p_id = {p_id}";
            string connectionString = "Data Source = (localdb)\\MSSQLLocalDB; Initial Catalog = ParkingVehicles; Integrated Security = True; Connect Timeout = 30; Encrypt = False;";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                // Open the connection
                connection.Open();

                // Create a command object with the query and connection
                using (SqlCommand cmd2 = new SqlCommand(command, connection))
                {


                    // Execute the query and get the result
                    using (SqlDataReader reader = cmd2.ExecuteReader())
                    {
                        // Check if there are any rows in the result
                        if (reader.HasRows)
                        {
                            // Read the first row
                            reader.Read();

                            // Retrieve the stored password from the reader
                            int retrievedPIDSlot = reader.GetInt32(0);
                            switch(retrievedPIDSlot)
                            {
                                case 1:
                                    gf1.BackColor = Color.Lime;
                                    break;
                                case 2:
                                    gf2.BackColor = Color.Lime;
                                    break;
                                case 3:
                                    gf3.BackColor = Color.Lime;
                                    break;
                                case 4:
                                    gf4.BackColor = Color.Lime;
                                    break;
                                case 5:
                                    gf5.BackColor = Color.Lime;
                                    break;
                                case 6:
                                    gf6.BackColor = Color.Lime;
                                    break;
                            }
                            
                        }

                    }
                }
            }
        }*/
        // Show Payments
        private void button7_Click(object sender, EventArgs e)
        {
            showPayments();
            paymentsPanel.Show();
        }

        private void totalBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void button8_Click(object sender, EventArgs e)
        {
            paymentsPanel.Hide();
        }

        private int retrievedSlotNo, retrievedVehicleID;


        private void searchPSlot(int slotNo)
        {
            string command = $"SELECT p_slot FROM p_trans WHERE  p_slot = {slotNo}";
            string connectionString = "Data Source = (localdb)\\MSSQLLocalDB; Initial Catalog = ParkingVehicles; Integrated Security = True; Connect Timeout = 30; Encrypt = False;";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                // Open the connection
                connection.Open();

                // Create a command object with the query and connection
                using (SqlCommand cmd2 = new SqlCommand(command, connection))
                {


                    // Execute the query and get the result
                    using (SqlDataReader reader = cmd2.ExecuteReader())
                    {
                        // Check if there are any rows in the result
                        if (reader.HasRows)
                        {
                            // Read the first row
                            reader.Read();

                            // Retrieve the stored password from the reader
                            retrievedSlotNo = reader.GetInt32(0);
                            int counter = 0;
                            foreach(Button buttons in buttonList)
                            {
                                if (retrievedSlotNo == counter)
                                {
                                    buttons.BackColor = Color.DarkRed;
                                    buttons.Enabled = false;
                                    break;
                                }
                                else
                                    counter++;
                            }

                        }
                    }
                }
            }
        }
        private string plate_no = "";
       
        private void parkingslotsPanel_Paint(object sender, PaintEventArgs e)
        {

        }
    }

}
// 500
