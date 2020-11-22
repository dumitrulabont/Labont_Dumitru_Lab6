using AutoLotModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data;
using System.ComponentModel;

namespace Labont_Dumitru_Lab6
{
    enum ActionState
    {
        New,
        Edit,
        Delete,
        Nothing
    }
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ActionState action = ActionState.Nothing;
        AutoLotEntitiesModel ctx = new AutoLotEntitiesModel();
        CollectionViewSource customerViewSource;
        CollectionViewSource inventoryViewSource;
        Binding FirsNameBinding = new Binding();
        Binding LastNameBinding = new Binding();
        Binding ColorBinding = new Binding();
        Binding MakeBinding = new Binding();

        CollectionViewSource customerOrdersViewSource;
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            FirsNameBinding.Path = new PropertyPath("FirsName");
            LastNameBinding.Path = new PropertyPath("LastName");
            ColorBinding.Path = new PropertyPath("Color");
            MakeBinding.Path = new PropertyPath("Make");


            firsNameTextBox.SetBinding(TextBox.TextProperty, FirsNameBinding);
            lastNameTextBox.SetBinding(TextBox.TextProperty, LastNameBinding);

            colorTextBox.SetBinding(TextBox.TextProperty, ColorBinding);
            makeTextBox.SetBinding(TextBox.TextProperty, MakeBinding);
            
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            //System.Windows.Data.CollectionViewSource customerViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("customerViewSource")));
            customerViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("customerViewSource")));
            inventoryViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("inventoryViewSource")));
            customerViewSource.Source = ctx.Customer.Local;
            inventoryViewSource.Source = ctx.Inventory.Local;
            customerOrdersViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("customerOrderViewSource")));
            //customerOrdersViewSource.Source = ctx.Order.Local;
            
            ctx.Customer.Load();
            ctx.Inventory.Load();
            ctx.Order.Load();
            cmbCustomers.ItemsSource = ctx.Customer.Local;
            //cmbCustomers.DisplayMemberPath = "FirsName";
            cmbCustomers.SelectedValuePath = "CustId";

            cmbInventory.ItemsSource = ctx.Inventory.Local;
            //cmbInventory.DisplayMemberPath = "Make";
            cmbInventory.SelectedValuePath = "CarId";

            BindDataGrid();
            
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.New;
            btnNew.IsEnabled = false;
            btnEdit.IsEnabled = false;
            btnDelete.IsEnabled = false;
            btnSave.IsEnabled = true;
            btnCancel.IsEnabled = true;
            customerDataGrid.IsEnabled = false;
            btnPrevious.IsEnabled = false;
            btnNext.IsEnabled = false;
            firsNameTextBox.IsEnabled = true;
            lastNameTextBox.IsEnabled = true;
            BindingOperations.ClearBinding(firsNameTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(lastNameTextBox, TextBox.TextProperty);
            
            firsNameTextBox.Text = "";
            lastNameTextBox.Text = "";
            SetValidationBinding();
            Keyboard.Focus(firsNameTextBox);
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Edit;
            string tempFirstName = firsNameTextBox.Text.ToString();
            string tempLastName = lastNameTextBox.Text.ToString();
            
            btnNew.IsEnabled = false;
            btnEdit.IsEnabled = false;
            btnDelete.IsEnabled = false;
            btnSave.IsEnabled = true;
            btnCancel.IsEnabled = true;
            customerDataGrid.IsEnabled = false;
            btnPrevious.IsEnabled = false;
            btnNext.IsEnabled = false;
            firsNameTextBox.IsEnabled = true;
            lastNameTextBox.IsEnabled = true;
            BindingOperations.ClearBinding(firsNameTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(lastNameTextBox, TextBox.TextProperty);
            firsNameTextBox.Text = tempFirstName;
            lastNameTextBox.Text = tempLastName;
            SetValidationBinding();

            Keyboard.Focus(firsNameTextBox);
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Delete;
            string tempFirstName = firsNameTextBox.Text.ToString();
            string tempLastName = lastNameTextBox.Text.ToString();
            btnNew.IsEnabled = false; 
            btnEdit.IsEnabled = false; 
            btnDelete.IsEnabled = false;
            btnSave.IsEnabled = true;
            btnCancel.IsEnabled = true;
            customerDataGrid.IsEnabled = false;
            btnPrevious.IsEnabled = false;
            btnNext.IsEnabled = false;
            BindingOperations.ClearBinding(firsNameTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(lastNameTextBox, TextBox.TextProperty);
            firsNameTextBox.Text = tempFirstName;
            lastNameTextBox.Text = tempLastName;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Nothing;
            btnNew.IsEnabled = true;
            btnEdit.IsEnabled = true;
            btnDelete.IsEnabled = true;
            btnSave.IsEnabled = false;
            btnCancel.IsEnabled = false;
            customerDataGrid.IsEnabled = true;
            btnPrevious.IsEnabled = true;
            btnNext.IsEnabled = true;
            firsNameTextBox.IsEnabled = false;
            lastNameTextBox.IsEnabled = false;
            firsNameTextBox.SetBinding(TextBox.TextProperty, FirsNameBinding);
            lastNameTextBox.SetBinding(TextBox.TextProperty, LastNameBinding);
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            Customer customer = null;
            
            if (action == ActionState.New)
            {
                try
                {
                    customer = new Customer()
                    {
                        FirsName = firsNameTextBox.Text.Trim(),
                        LastName = lastNameTextBox.Text.Trim()
                    };
                    
                    ctx.Customer.Add(customer);
                    customerViewSource.View.Refresh();
                    ctx.SaveChanges();
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                btnNew.IsEnabled = true;
                btnEdit.IsEnabled = true;
                btnSave.IsEnabled = false;
                btnCancel.IsEnabled = false;
                btnDelete.IsEnabled = true; 
                customerDataGrid.IsEnabled = true;
                btnPrevious.IsEnabled = true;
                btnNext.IsEnabled = true;
                firsNameTextBox.IsEnabled = false;
                lastNameTextBox.IsEnabled = false;
            }
            else if(action == ActionState.Edit)
            {
                try
                {
                    customer = (Customer)customerDataGrid.SelectedItem;
                    customer.FirsName = firsNameTextBox.Text.Trim();
                    customer.LastName = lastNameTextBox.Text.Trim();
                    ctx.SaveChanges();
                }
                catch(DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                customerViewSource.View.Refresh();
                customerViewSource.View.MoveCurrentTo(customer);
                btnNew.IsEnabled = true; 
                btnEdit.IsEnabled = true; 
                btnDelete.IsEnabled = true; 
                btnSave.IsEnabled = false; 
                btnCancel.IsEnabled = false;
                customerDataGrid.IsEnabled = true; 
                btnPrevious.IsEnabled = true; 
                btnNext.IsEnabled = true;
                firsNameTextBox.IsEnabled = false;
                lastNameTextBox.IsEnabled = false;
                firsNameTextBox.SetBinding(TextBox.TextProperty, FirsNameBinding);
                lastNameTextBox.SetBinding(TextBox.TextProperty, LastNameBinding);

            }
            else if(action == ActionState.Delete)
            {
                try{
                    customer = (Customer)customerDataGrid.SelectedItem;
                    ctx.Customer.Remove(customer);
                    ctx.SaveChanges();
                }
                catch(DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                customerViewSource.View.Refresh();
                btnNew.IsEnabled = true; 
                btnEdit.IsEnabled = true; 
                btnDelete.IsEnabled = true; 
                btnSave.IsEnabled = false; 
                btnCancel.IsEnabled = false;
                customerDataGrid.IsEnabled = true;
                btnPrevious.IsEnabled = true; 
                btnNext.IsEnabled = true;
                firsNameTextBox.IsEnabled = false;
                lastNameTextBox.IsEnabled = false;
                firsNameTextBox.SetBinding(TextBox.TextProperty, FirsNameBinding);
                lastNameTextBox.SetBinding(TextBox.TextProperty, LastNameBinding);
            }
        }

        private void btnPrevious_Click(object sender, RoutedEventArgs e)
        {
            customerViewSource.View.MoveCurrentToPrevious();
           
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            customerViewSource.View.MoveCurrentToNext();
        }

        private void btnNewInv_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.New;
            btnNewInv.IsEnabled = false;
            btnEditInv.IsEnabled = false;
            btnDeleteInv.IsEnabled = false;
            btnSaveInv.IsEnabled = true;
            btnCancelInv.IsEnabled = true;
            inventoryDataGrid.IsEnabled = false;
            btnPreviousInv.IsEnabled = false;
            btnNextInv.IsEnabled = false;
            colorTextBox.IsEnabled = true;
            makeTextBox.IsEnabled = true;
            BindingOperations.ClearBinding(colorTextBox, TextBox.TextProperty);
            colorTextBox.Text = "";
            BindingOperations.ClearBinding(makeTextBox, TextBox.TextProperty);
            makeTextBox.Text = "";
            Keyboard.Focus(colorTextBox);
        }

        private void btnEditInv_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Edit;
            string tempColor = colorTextBox.Text.ToString();
            string tempMake = makeTextBox.Text.ToString();
            btnNewInv.IsEnabled = false;
            btnEditInv.IsEnabled = false;
            btnDeleteInv.IsEnabled = false; 
            btnSaveInv.IsEnabled = true; 
            btnCancelInv.IsEnabled = true;
            inventoryDataGrid.IsEnabled = false;
            btnPreviousInv.IsEnabled = false;
            btnNextInv.IsEnabled = false;
            colorTextBox.IsEnabled = true;
            makeTextBox.IsEnabled = true;

            BindingOperations.ClearBinding(colorTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(makeTextBox, TextBox.TextProperty);
            colorTextBox.Text = tempColor;
            makeTextBox.Text = tempMake;
            Keyboard.Focus(colorTextBox);
        }

        private void btnDeleteInv_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Delete;
            string tempColor = colorTextBox.Text.ToString();
            string tempMake = makeTextBox.Text.ToString();
            btnNewInv.IsEnabled = false;
            btnEditInv.IsEnabled = false;
            btnDeleteInv.IsEnabled = false;
            btnSaveInv.IsEnabled = true;
            btnCancelInv.IsEnabled = true;
            inventoryDataGrid.IsEnabled = false;
            btnPreviousInv.IsEnabled = false;
            btnNextInv.IsEnabled = false;

            BindingOperations.ClearBinding(colorTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(makeTextBox, TextBox.TextProperty);
            colorTextBox.Text = tempColor;
            makeTextBox.Text = tempMake;
        }

        private void btnCancelInv_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Nothing;
            btnNewInv.IsEnabled = true;
            btnEditInv.IsEnabled = true;
            btnDeleteInv.IsEnabled = true;
            btnEditInv.IsEnabled = true;
            btnSaveInv.IsEnabled = false;
            btnCancelInv.IsEnabled = false;
            inventoryDataGrid.IsEnabled = true;
            btnPreviousInv.IsEnabled = true;
            btnNextInv.IsEnabled = true;
            colorTextBox.IsEnabled = false;
            makeTextBox.IsEnabled = false;

            colorTextBox.SetBinding(TextBox.TextProperty, ColorBinding);
            makeTextBox.SetBinding(TextBox.TextProperty, MakeBinding);
        }

        private void btnSaveInv_Click(object sender, RoutedEventArgs e)
        {
            Inventory inventory = null;
            if(action == ActionState.New)
            {
                try
                {
                    inventory = new Inventory()
                    {
                        Color = colorTextBox.Text.Trim(),
                        Make = makeTextBox.Text.Trim()
                    };
                    ctx.Inventory.Add(inventory);
                    inventoryViewSource.View.Refresh();
                    ctx.SaveChanges();
                }
                catch(DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                btnNewInv.IsEnabled = true;
                btnEditInv.IsEnabled = true;
                btnDeleteInv.IsEnabled = true;
                btnSaveInv.IsEnabled = false;
                btnCancelInv.IsEnabled = false;
                inventoryDataGrid.IsEnabled = true;
                btnPreviousInv.IsEnabled = true;
                btnNextInv.IsEnabled = true;
                colorTextBox.IsEnabled = false;
                makeTextBox.IsEnabled = false;
            }
            else if(action == ActionState.Edit)
            {
                try
                {
                    inventory = (Inventory)inventoryDataGrid.SelectedItem;
                    inventory.Make = makeTextBox.Text.Trim();
                    inventory.Color = colorTextBox.Text.Trim();
                    ctx.SaveChanges();
                }
                catch(DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                inventoryViewSource.View.Refresh();
                inventoryViewSource.View.MoveCurrentTo(inventory);

                btnNewInv.IsEnabled = true;
                btnEditInv.IsEnabled = true;
                btnDeleteInv.IsEnabled = true;
                btnSaveInv.IsEnabled = false;
                btnCancelInv.IsEnabled = false;
                inventoryDataGrid.IsEnabled = true;
                btnPreviousInv.IsEnabled = true;
                btnNextInv.IsEnabled = true;
                colorTextBox.IsEnabled = false;
                makeTextBox.IsEnabled = false;

                colorTextBox.SetBinding(TextBox.TextProperty, ColorBinding);
                makeTextBox.SetBinding(TextBox.TextProperty, MakeBinding);
            }
            else if(action == ActionState.Delete)
            {
                try
                {
                    inventory = (Inventory)inventoryDataGrid.SelectedItem;
                    ctx.Inventory.Remove(inventory);
                    ctx.SaveChanges();
                }
                catch(DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                inventoryViewSource.View.Refresh();

                btnNewInv.IsEnabled = true;
                btnEditInv.IsEnabled = true;
                btnDeleteInv.IsEnabled = true;
                btnSaveInv.IsEnabled = false;
                btnCancelInv.IsEnabled = false;
                inventoryDataGrid.IsEnabled = true;
                btnPreviousInv.IsEnabled = true;
                btnNewInv.IsEnabled = true;
                colorTextBox.IsEnabled = false;
                makeTextBox.IsEnabled = false;
                colorTextBox.SetBinding(TextBox.TextProperty, ColorBinding);
                makeTextBox.SetBinding(TextBox.TextProperty, MakeBinding);

            }
        }

        private void btnNextInv_Click(object sender, RoutedEventArgs e)
        {
            inventoryViewSource.View.MoveCurrentToNext();
        }

        private void btnPreviousInv_Click(object sender, RoutedEventArgs e)
        {
            inventoryViewSource.View.MoveCurrentToPrevious();
        }

        private void btnNewO_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.New;
            btnNewO.IsEnabled = false;
            btnEditO.IsEnabled = false;
            btnDeleteO.IsEnabled = false;
            btnSaveO.IsEnabled = true;
            btnCancelO.IsEnabled = true;
            orderDataGrid.IsEnabled = false;
            btnPreviousO.IsEnabled = false;
            btnNextO.IsEnabled = false;
            cmbCustomers.IsEnabled = true;
            cmbInventory.IsEnabled = true;
        }

        private void btnEditO_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Edit;
            btnNewO.IsEnabled = false;
            btnEditO.IsEnabled = false;
            btnDeleteO.IsEnabled = false;
            btnSaveO.IsEnabled = true;
            btnCancelO.IsEnabled = true;
            orderDataGrid.IsEnabled = false;

            btnPreviousO.IsEnabled = false;
            btnNextO.IsEnabled = false;
            cmbCustomers.IsEnabled = true;
            cmbInventory.IsEnabled = true;
        }

        private void btnDeleteO_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Delete;
            
            btnNewO.IsEnabled = false;
            btnEditO.IsEnabled = false;
            btnDeleteO.IsEnabled = false;
            btnSaveO.IsEnabled = true;
            btnCancelO.IsEnabled = true;
            orderDataGrid.IsEnabled = false;
            btnPreviousO.IsEnabled = false;
            btnNextO.IsEnabled = false;
        }

        private void btnCancelO_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Nothing;
            btnNewO.IsEnabled = true;
            btnEditO.IsEnabled = true;
            btnDeleteO.IsEnabled = true;
            btnSaveO.IsEnabled = false;
            btnCancelO.IsEnabled = false;
            orderDataGrid.IsEnabled = true;
            btnPreviousO.IsEnabled = true;
            btnNextO.IsEnabled = true;
            cmbCustomers.IsEnabled = false;
            cmbInventory.IsEnabled = false;
        }

        private void btnSaveO_Click(object sender, RoutedEventArgs e)
        {
            Order order = null;
            if(action == ActionState.New)
            {
                try
                {
                    Customer customer = (Customer)cmbCustomers.SelectedItem;
                    Inventory inventory = (Inventory)cmbInventory.SelectedItem;
                    order = new Order()
                    {
                        CustId = customer.CustId,
                        CarId = inventory.CarId
                    };
                    ctx.Order.Add(order);
                    //customerOrdersViewSource.View.Refresh();
                    
                    ctx.SaveChanges();
                    BindDataGrid();
                }
                catch(DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }

                btnNewO.IsEnabled = true;
                btnEditO.IsEnabled = true;
                btnDeleteO.IsEnabled = true;
                btnSaveO.IsEnabled = false;
                btnCancelO.IsEnabled = false;
                orderDataGrid.IsEnabled = true;
                btnPreviousO.IsEnabled = true;
                btnNextO.IsEnabled = true;
                cmbCustomers.IsEnabled = false;
                cmbInventory.IsEnabled = false;
            }
            else if(action == ActionState.Edit)
            {
                dynamic selectedOrder = orderDataGrid.SelectedItem;
                try
                {
                    int cur_id = selectedOrder.OrderId;
                    var editedOrder = ctx.Order.FirstOrDefault(s => s.OrderId == cur_id);
                    if(editedOrder != null)
                    {
                        editedOrder.CustId = Int32.Parse(cmbCustomers.SelectedValue.ToString());
                        editedOrder.CarId = Convert.ToInt32(cmbInventory.SelectedValue.ToString());
                        ctx.SaveChanges();
                    }
                }
                catch(DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                BindDataGrid();
                //customerOrdersViewSource.View.Refresh();
                customerOrdersViewSource.View.MoveCurrentTo(selectedOrder);

                btnNewO.IsEnabled = true;
                btnEditO.IsEnabled = true;
                btnDeleteO.IsEnabled = true;
                btnSaveO.IsEnabled = false;
                btnCancelO.IsEnabled = false;
                orderDataGrid.IsEnabled = true;
                btnPreviousO.IsEnabled = true;
                btnNextO.IsEnabled = true;
                cmbCustomers.IsEnabled = false;
                cmbInventory.IsEnabled = false;
            }
            else if(action == ActionState.Delete)
            {
                try
                {
                    dynamic selectedOrder = orderDataGrid.SelectedItem;
                    int curr_id = selectedOrder.OrderId;
                    var deletedOrder = ctx.Order.FirstOrDefault(s => s.OrderId == curr_id);
                    if(deletedOrder != null)
                    {
                        ctx.Order.Remove(deletedOrder);
                        ctx.SaveChanges();
                        MessageBox.Show("Order Deleted Successfully", "Message");
                        BindDataGrid();
                    }
                }
                catch(DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                //customerOrdersViewSource.View.Refresh();

                btnNewO.IsEnabled = true;
                btnEditO.IsEnabled = true;
                btnDeleteO.IsEnabled = true;
                btnSaveO.IsEnabled = false;
                btnCancelO.IsEnabled = false;
                orderDataGrid.IsEnabled = true;
                btnPreviousO.IsEnabled = true;
                btnNextO.IsEnabled = true;
                cmbCustomers.IsEnabled = false;
                cmbInventory.IsEnabled = false;
            }
        }

        private void btnPreviousO_Click(object sender, RoutedEventArgs e)
        {
            customerOrdersViewSource.View.MoveCurrentToPrevious();
        }

        private void btnNextO_Click(object sender, RoutedEventArgs e)
        {
            customerOrdersViewSource.View.MoveCurrentToNext();
        }
        
        private void BindDataGrid()
        {
            var queryOrder = from ord in ctx.Order
                            join cust in ctx.Customer on ord.CustId equals
                            cust.CustId
                            join inv in ctx.Inventory on ord.CarId
                            equals inv.CarId
                            select new { ord.OrderId, ord.CarId,ord.CustId,cust.FirsName,cust.LastName,inv.Make,inv.Color};
           customerOrdersViewSource.Source = queryOrder.ToList();
           
        }
        private void SetValidationBinding()
        {
            Binding firstNameValidationBinding = new Binding();
            firstNameValidationBinding.Source = customerViewSource;
            firstNameValidationBinding.Path = new PropertyPath("FirsName");
            firstNameValidationBinding.NotifyOnValidationError = true;
            firstNameValidationBinding.Mode = BindingMode.TwoWay;
            firstNameValidationBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            firstNameValidationBinding.ValidationRules.Add(new StringNotEmpy());
            firsNameTextBox.SetBinding(TextBox.TextProperty, firstNameValidationBinding);

            Binding lastNameValidationBinding = new Binding();
            lastNameValidationBinding.Source = customerViewSource;
            lastNameValidationBinding.Path = new PropertyPath("LastName");
            lastNameValidationBinding.NotifyOnValidationError = true;
            lastNameValidationBinding.Mode = BindingMode.TwoWay;
            lastNameValidationBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            lastNameValidationBinding.ValidationRules.Add(new StringMinLengthValidator());
            lastNameTextBox.SetBinding(TextBox.TextProperty, lastNameValidationBinding);
        }
    }
}
