using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using LinqToExcel;
using F = System.Drawing;
using System.Data.SQLite;

namespace WinFormShopRecording
{
    public partial class Form1 : Form
    {
        #region Global Level Variables
        static List<Item> _AllItems = new List<Item>();
        BindingList<ItemViewModel> SelectedProductList = new BindingList<ItemViewModel>();
        int _InvoiceNumber = 0;
        string GetPrefix = @"\FY" + Convert.ToString(DateTime.Now.Year).Remove(0,2) + "-" + Convert.ToString(DateTime.Now.Year + 1).Remove(0, 2);
        #endregion

        public Form1()
        {
            InitializeComponent();
            FillAllTheItems();
            DataGridMetaData();
            GetInvoiceNumber();
            label5.Text = GetPrefix;
        }

        #region Called Once onLoad 

        private void FillAllTheItems()
        {
            if (_AllItems.Count == 0)
            {
                var a = new ExcelQueryFactory(System.IO.Path.GetFullPath("TestItemPrice.xlsx"));
                _AllItems = (from t in a.Worksheet<Item>("Sheet1")
                             select t).ToList();
            }
        }

        private void DataGridMetaData()
        {
            dataGridView1.Columns[0].DataPropertyName = "Name";
            dataGridView1.Columns[1].DataPropertyName = "Quantity";
            dataGridView1.Columns[2].DataPropertyName = "SubTotal";
            dataGridView1.AutoGenerateColumns = false;
        }

        private void GetInvoiceNumber() {
            try
            {
                using (var reader = new System.IO.StreamReader(System.IO.Path.GetFullPath("InvoiceStore.txt")))
                {
                  var _Result = reader.ReadLine();
                  int _SavedInvoiceNumber;
                  var _test = int.TryParse(_Result.Trim(), out _SavedInvoiceNumber);
                    if (_test)
                    {
                        _InvoiceNumber = _SavedInvoiceNumber;
                        txtInvoiceID.Text = _InvoiceNumber.ToString();
                    }
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        #endregion

        #region Tab Controller Events
        private void TabController_Click(object sender, EventArgs e)
        {
            var _getSelectedTab = ((TabControl)sender).SelectedTab.Tag;
            AddItemsToTab((TabControl)sender);
        }

        private void AddItemsToTab(TabControl _tabControl) {

            List<Control> ControlCOl = new List<Control>();
            int top = 0, left = 0;
            //get the IDs for filtering 
            string _FilterId = _tabControl.SelectedTab.Tag.ToString();
            string[] _FilteredIDList = _FilterId.Split(',');
             
            foreach (var item1 in _AllItems.Where(x => _FilteredIDList.Contains(x.DisplayType)).GroupBy(x => x.Type).ToList())
            {
                string _HTMLType = item1.First().HTMLtype;

                if (_HTMLType == "B")
                {
                    foreach (var item in item1)
                    {
                        System.Windows.Forms.Button _newButtonAdd = new System.Windows.Forms.Button();
                        _newButtonAdd.Name = item.PSU;
                        _newButtonAdd.Text = item.Name;
                        _newButtonAdd.Margin = new Padding(5);
                        _newButtonAdd.Top = top;
                        _newButtonAdd.Left = left;
                        _newButtonAdd.Height = 30;
                        _newButtonAdd.Font = new Font("Microsoft Sans Serif", 9, FontStyle.Regular);
                        _newButtonAdd.Location = new F.Point(_tabControl.Location.X + left, _tabControl.Location.Y + top);
                        _newButtonAdd.Click += CommonButtonFunc;
                        ControlCOl.Add(_newButtonAdd);

                        left = left + 100;
                    }
                }
                else if (_HTMLType == "C")
                {
                    foreach (var item in item1)
                    {
                        System.Windows.Forms.CheckBox _newCheckBox = new CheckBox();
                        _newCheckBox.Name = "c" + item.PSU;
                        _newCheckBox.Text = item.Name;
                        _newCheckBox.Margin = new Padding(5);
                        _newCheckBox.Top = top;
                        _newCheckBox.Left = left;
                        _newCheckBox.Width = 170;
                        _newCheckBox.Location = new F.Point(_tabControl.Location.X + left, _tabControl.Location.Y + top);
                        _newCheckBox.Click += AddExtraComponent;
                        ControlCOl.Add(_newCheckBox);

                        System.Windows.Forms.TextBox _newTextBox = new TextBox();
                        _newTextBox.Name = "t" + item.PSU;
                        _newTextBox.Text = "0";
                        _newTextBox.Width = 40;
                        _newTextBox.Location = new F.Point(_tabControl.Location.X + left + 250, _tabControl.Location.Y + top);
                        _newTextBox.Leave += txtQuantityCheckBox_Leave;
                        _newTextBox.KeyPress += textBox1_KeyPress;
                        //_newTextBox.Click += AddExtraComponent;
                        ControlCOl.Add(_newTextBox);
                    }
                }
                else if (_HTMLType == "D")
                {
                    System.Windows.Forms.Label lbl = new Label();
                    lbl.Text = item1.FirstOrDefault().ExtraData;
                    lbl.Width = 200;
                    lbl.Location = new F.Point(_tabControl.Location.X + left, _tabControl.Location.Y + top);
                    ControlCOl.Add(lbl);

                    System.Windows.Forms.ComboBox _newComboBox = new ComboBox();
                    var _DataFormatted = item1.Select(x => new { Name = x.Description, Id = x.PSU }).ToList();
                    _DataFormatted.Add(new { Name = "----Select----", Id = "0" });
                    _newComboBox.DataSource = _DataFormatted.OrderBy(x => x.Id).ToList();
                    _newComboBox.Name = "d" + item1.FirstOrDefault().ExtraData;
                    _newComboBox.DisplayMember = "Name";
                    _newComboBox.ValueMember = "Id";
                    _newComboBox.Location = new F.Point(_tabControl.Location.X + left + 200, _tabControl.Location.Y + top);
                    _newComboBox.Update();
                    _newComboBox.SelectedValue = "0";
                    _newComboBox.Width = 260;
                    _newComboBox.SelectedValueChanged += PadValueChanged;
                    ControlCOl.Add(_newComboBox);

                    System.Windows.Forms.TextBox _newTextBox = new TextBox();
                    _newTextBox.Name = "t" + item1.FirstOrDefault().ExtraData;
                    _newTextBox.Text = "0";
                    _newTextBox.Width = 40;
                    _newTextBox.Location = new F.Point(_tabControl.Location.X + left + 480, _tabControl.Location.Y + top);
                    _newTextBox.Leave += txtSheetQuantity_Leave;
                    _newTextBox.KeyPress += textBox1_KeyPress;
                    //_newTextBox.Click += AddExtraComponent;
                    ControlCOl.Add(_newTextBox);

                }
                else if (_HTMLType == "E") {


                }

                    //ControlCOl.Add(Environment.NewLine);
                    left = 0;
                top = top + 40;
            }

            if(ControlCOl != null && ControlCOl.Count > 0)
            _tabControl.TabPages[_tabControl.SelectedTab.Name].Controls.AddRange(ControlCOl.ToArray());
        }
        #endregion

        #region Add/Delete in Grid

        private void AddProduct(string ProductCode)
        {
            var gy = ParseToItemViewModel(_AllItems.Where(x => x.PSU == ProductCode).FirstOrDefault());
            var _alreadyExist = SelectedProductList.Where(x => x.Name == gy.Name).FirstOrDefault();
            if (_alreadyExist != null)
            {
                //var _test = SelectedProductList.Where(x => x.Name == gy.Name).FirstOrDefault();
                SelectedProductList.Where(x => x.Name == gy.Name).FirstOrDefault().Quantity = (_alreadyExist.Quantity + 1);
                SelectedProductList.Where(x => x.Name == gy.Name).FirstOrDefault().SubTotal = ((_alreadyExist.Quantity) * Convert.ToInt32(gy.Price));
            }
            else
            {
                var _calc = (Convert.ToInt32(gy.Price) * gy.Quantity);
                gy.SubTotal = _calc;
                SelectedProductList.Add(gy);
            }
            dataGridView1.DataSource = null;
            dataGridView1.DataSource = SelectedProductList;
            dataGridView1.Update();
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                dataGridView1.Rows[i].Cells[3].Value = "Delete";
            }
            GetTotalAmount();
            // dataGridView1.Update();
        }

        private void AddProduct(string ProductCode, int Quantity)
        {
            var gy = ParseToItemViewModel(_AllItems.Where(x => x.PSU == ProductCode).FirstOrDefault());
            var _alreadyExist = SelectedProductList.Where(x => x.Type == gy.Type).FirstOrDefault();
            if (_alreadyExist != null)
            {
                //var _test = SelectedProductList.Where(x => x.Name == gy.Name).FirstOrDefault();
                SelectedProductList.Remove(_alreadyExist);
            }
            var _calc = (Convert.ToInt32(gy.Price) * Quantity);
            gy.Quantity = Quantity;
            gy.SubTotal = _calc;
            SelectedProductList.Add(gy);

            dataGridView1.DataSource = null;
            dataGridView1.DataSource = SelectedProductList;
            dataGridView1.Update();
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                dataGridView1.Rows[i].Cells[3].Value = "Delete";
            }
            GetTotalAmount();
            // dataGridView1.Update();
        }

        private ItemViewModel ParseToItemViewModel(Item _baseClass)
        {
            return new ItemViewModel(_baseClass);
        }

        private int GetTotalAmount()
        {
            int _discountAmount = string.IsNullOrEmpty(txtAdjustentBox.Text) ? 0 : Convert.ToInt32(txtAdjustentBox.Text);
            int _total = 0;
            foreach (var item in SelectedProductList)
            {
                _total = _total + item.SubTotal;
            }
            DisplayTotalAmount.Text = (_total - _discountAmount).ToString();
            return _total - _discountAmount;
        }

        private void DeleteItem(string NameOfItem,string TypeOfItem = null)
        {
            ItemViewModel _getItemDetails = null;
            if(!string.IsNullOrEmpty(NameOfItem))
                _getItemDetails = SelectedProductList.Where(x => x.PSU == NameOfItem).FirstOrDefault();
            if(!string.IsNullOrEmpty(TypeOfItem))
                _getItemDetails = SelectedProductList.Where(x => x.Type == TypeOfItem).FirstOrDefault();

            if (_getItemDetails != null)
            {
                SelectedProductList.Remove(_getItemDetails);
                dataGridView1.DataSource = SelectedProductList;
                dataGridView1.Update();
                GetTotalAmount();
            }
        }

        private void AddExtraComponent(object sender, EventArgs e)
        {
            var a = ((System.Windows.Forms.CheckBox)sender);
            string _nameOfProduct = a.Name.Remove(0, 1);
            bool _isCheckedOrNot = a.Checked;
            int Quantity, dummyQuantity = 1;

            if (_isCheckedOrNot)
            {
                foreach (var item in a.Parent.Controls.Find("t" + _nameOfProduct, true))
                {
                    if (item.Name == "t" + _nameOfProduct)
                    {
                        var _isparsable = Int32.TryParse(item.Text, out Quantity);
                        if (_isparsable)
                        {
                            if(Quantity > 1)
                            dummyQuantity = Quantity;
                            item.Text = dummyQuantity.ToString();
                        }
                    }
                }
                AddProduct(_nameOfProduct, dummyQuantity);
            }
            else
            {
                DeleteItem(_nameOfProduct);
                foreach (var item in a.Parent.Controls.Find("t" + _nameOfProduct, true))
                {
                    item.Text = "0";
                }
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 3)
            {
                string _ProductName = SelectedProductList[e.RowIndex].PSU; //dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString();
                DeleteItem(_ProductName);
            }
        }
        #endregion

        #region DropDown and Text Box Events
        private void PadValueChanged(object sender, EventArgs e)
        {
            var _objProduct = ((System.Windows.Forms.ComboBox)sender);
            var _itemCode = Convert.ToString(_objProduct.SelectedValue);
            var _quantity = 1;
            int Quantity;
            if (_itemCode == "0") {
                var _t = _AllItems.Where(x => x.ExtraData == _objProduct.Name.Remove(0, 1)).FirstOrDefault().Type;
                UpdateAttachedDropDownBox(_objProduct, _objProduct.Name, 0);
                DeleteItem(null,_t);
            }
            else
            {
                foreach (var item in _objProduct.Parent.Controls.Find("t" + _objProduct.Name.Remove(0, 1), true))
                {
                    if (item.Name == "t" + _objProduct.Name.Remove(0, 1))
                    {
                        var _isparsable = Int32.TryParse(item.Text, out Quantity);
                        if (_isparsable)
                        {
                            if (Quantity == 0)
                            {
                                item.Text = "1";
                                _quantity = 1;
                            }
                            else
                                _quantity = Quantity;
                        }
                    }
                }
                AddProduct(_itemCode, _quantity);
            }
        }

        private void UpdateAttachedDropDownBox(ComboBox sender, string Name, int Quantity) {
            foreach (var item in sender.Parent.Controls.Find("t" + sender.Name.Remove(0, 1), true))
            {
                item.Text = Quantity.ToString();
            }
        }

        private void txtSheetQuantity_Leave(object sender, EventArgs e)
        {
            var _objTxtbox = ((TextBox)sender);           
            var _txtquantity = _objTxtbox.Text;
            int quantity = 1, _dummyQuantity;
            bool _isparsable;
            string productID = "0";
            if (!string.IsNullOrEmpty(_txtquantity))
            {
                _isparsable = Int32.TryParse(_txtquantity, out _dummyQuantity);
                if (_isparsable)
                    quantity = _dummyQuantity;
            }

            foreach (ComboBox item in _objTxtbox.Parent.Controls.Find("d" + _objTxtbox.Name.Remove(0, 1), true))
            {
                productID = item.SelectedValue.ToString();
            }
            if(productID != "0")
            AddProduct(productID, quantity);
        }

        private void txtQuantityCheckBox_Leave(object sender, EventArgs e)
        {
            var _objTxtbox = ((TextBox)sender);
            var _txtquantity = _objTxtbox.Text;
            int quantity = 1, _dummyQuantity;
            bool _isparsable;
            string productID = "0";
            if (!string.IsNullOrEmpty(_txtquantity))
            {
                _isparsable = Int32.TryParse(_txtquantity, out _dummyQuantity);
                if (_isparsable)
                    quantity = _dummyQuantity;
            }

            foreach (CheckBox item in _objTxtbox.Parent.Controls.Find("c" + _objTxtbox.Name.Remove(0, 1), true))
            {
                productID = item.Name.Remove(0, 1);
            }
            if (productID != "0")
                AddProduct(productID, quantity);
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }
        #endregion

        #region UseLess Events
        public void CommonButtonFunc(object sender, EventArgs e)
        {
            var _ProductName = ((System.Windows.Forms.Button)sender).Name;
            AddProduct(_ProductName);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
        #endregion

        #region Final Add
        public int UpdateOrder()
        {

            int InvoiceId = 0;
            int _TotalAmount = GetTotalAmount();
            bool _isInvoiceNumbergenerated = false;
            int _RowsAffected = 0;
            DateTime _InvoiceDate = dateTimePicker1.Value;
            int _invoiceID = Convert.ToInt16(txtInvoiceID.Text);
            try
            {
                #region Insert Data into sql
                using (SQLiteConnection m_dbConnection = new SQLiteConnection("Data Source=MyDatabase.sqlite;Version=3;"))
                {
                    m_dbConnection.Open();
                    string strbuilder = @"Insert into Invoice(TotalAmount,InvoiceDateDay,InvoiceMonth,InvoiceYear,InvoiceDate,Discount,DeleteInd,ExternalInvoiceID) 
                    values(" + _TotalAmount + ","
                    + _InvoiceDate.Day + ","
                    + _InvoiceDate.Month + ","
                    + _InvoiceDate.Year + ","
                    + _TotalAmount + ",'"
                    + _InvoiceDate + "')";

                    using (SQLiteCommand _cmd1 = new SQLiteCommand(strbuilder, m_dbConnection))
                    {
                        _cmd1.Prepare();
                        _isInvoiceNumbergenerated = _cmd1.ExecuteNonQuery() > 0 ? true : false;
                    }
                    if (_isInvoiceNumbergenerated)
                    {
                        using (SQLiteCommand _cmd = new SQLiteCommand("SELECT max(Id) FROM 'Invoice'", m_dbConnection))
                        {
                            var result = _cmd.ExecuteScalar();
                            InvoiceId = Convert.ToInt32(result);
                        }
                        foreach (var item in SelectedProductList)
                        {
                            string _strCmd2 = @"Insert into InvoiceITEM(InvoiceId,ITEMNAME,Price,Quantity,ItemId,ExternalID) values(" +
                                InvoiceId + ",'" +
                                item.Name + "'," +
                                item.SubTotal + "," +
                                item.Quantity + "," +
                                item.ItemId + "," +
                                item.Type
                                + ")";
                            using (SQLiteCommand _cmd = new SQLiteCommand(_strCmd2, m_dbConnection))
                            {
                                _cmd.Prepare();
                                _RowsAffected = _RowsAffected + _cmd.ExecuteNonQuery();
                            }
                        }
                    }
                    m_dbConnection.Close();
                }
                    #endregion


                    //var doc = new PrintDocument();
                    //doc.DocumentName = "Invoice" + InvoiceId;
                    //var resultDict = new Dictionary<string, string>();
                    //resultDict["InvoiceNumber"] = InvoiceId.ToString();
                    //resultDict["InvoiceDate"] = DateTime.Now.ToShortDateString();

                    //doc.PrintPage += (sender, e) => ProvideContent(resultDict, e);
                    ////new PrintPageEventHandler(ProvideContent);
                    ////new { InvoiceNumber = InvoiceId ,InvoiceDate = DateTime.Now.ToShortDateString() }    
                    //doc.Print();

                 #region Clean Data
                    SelectedProductList.Clear();
                    InvoiceId = (InvoiceId == _invoiceID) ? UpdateInvoiceNumber(InvoiceId) : InvoiceId;                 
                    #endregion
                
            }
            catch (Exception ex)
            {

            }
            return _RowsAffected;
        }

        public int UpdateInvoiceNumber(int IID)
        {
            try
            {
                using (var writer = new System.IO.StreamWriter(System.IO.Path.GetFullPath("InvoiceStore.txt")))
                {
                    writer.WriteLine(IID + 1);
                }
                txtInvoiceID.Text = Convert.ToString(IID + 1);
            }
            catch (Exception)
            {

            }
            finally
            {
               
            }
            return IID +1;
        }
        #endregion

        private void txtAdjustentBox_Leave(object sender, EventArgs e)
        {
            GetTotalAmount();
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void txtInvoiceID_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
